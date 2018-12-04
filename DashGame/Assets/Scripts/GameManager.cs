using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public class PaddlePrefab
    {
        public GameObject mainParticles; // always make the first child of this prefab for the right paddle end and the second for the left end
        public GameObject rightEnd;
        public GameObject leftEnd;
        public int index;

        public PaddlePrefab(GameObject pref)
        {
            mainParticles = Instantiate(pref);
            mainParticles.SetActive(false);

            rightEnd = mainParticles.transform.GetChild(0).gameObject;
            if (pref.transform.childCount > 1)
            {
                leftEnd = pref.transform.GetChild(1).gameObject;
            }
            else
            {
                leftEnd = Instantiate(pref.transform.GetChild(0).gameObject, mainParticles.transform);
                leftEnd.SetActive(false);
            }
        }
    }

    PaddleController Paddle;
    LevelGenerator LG;
    BallController ballC;
    SceneChanger sceneChanger;
    AudioManager audioManager;
    AchievementsAndLeaderboards rankings;
    Ratings rate;
    public Button pauseButton;
    public Text countdownText;
    public Animator scoreReviewAnimC;
    public Animator settingsPageAnimC; //for the settings page back button
    public Animator shopPageAnimC; //for the shop page back button
    public Button skipScoreReviewButton;
    public Button replayButton;
    public GameObject GameModeButton;
    public GameObject tutorial;
    Animator GameModeButtonAnimC;
    bool extraBall = false;
    int richochetCount;
    public GameObject extraBallSprite;
    TargetController target;
    public Text scoreText;
    private int score = 0;
    bool gameRunning;
    float timeScale = 1;
    bool paused;
    public Text highScoreText;
    public Text gameOverScore; //score when you loose for that run
    public GameObject newHighScoreImage;
    int highScore;
    Coroutine disableReplayButtonC;
    Coroutine pauseCoroutine;
    bool pauseAllCoroutines = false;
    Text scoreReviewGems;
    bool gemsOnScreen = false;
    float gems, tempGems;
    int newGems;
    float t = 0.0f;
    bool canEndGame = true;
    public GameObject[] paddlePrefabs;
    public PaddlePrefab[] paddles;
    bool canContinue;
    AdManager ads;
    PaddlePrefab selectedPaddle;
    bool paddleChanged = false;
    bool noSound = false;
    ScrollRect infoScrollRect;
    int firstPlayEver; // indicated first time playing since download, a value of 0 means its the first time playing
    //static string currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "apple" : "android";
    bool updateHS = false;
    AudioListener audioListener;
    float volB4Pause;
    static CryptoRandom rng = new CryptoRandom();
    Coroutine fadeInVolume;

    public static GameManager Instance;

    public delegate void GameDelegate();
    public static event GameDelegate GameStarted;
    public static event GameDelegate Revive;
    public static event GameDelegate PlusOneStarted;
    public static event GameDelegate DeadeyeStarted;
    public static event GameDelegate ClairvoyanceStarted;

    public GameObject StartPageButtons;
    public GameObject GameOverPage;
    public GameObject PauseMenu;
    public GameObject CountdownPage;
    public GameObject SettingsPage;
    public GameObject GamePage;
    public GameObject ScoreReview;
    public GameObject ShopPage;
    public GameObject InfoPage;
    public GameObject ScoresPage;
    public GameObject RateMePage;
    public GameObject AudioCredits;
    public Text GemsText;
    //public GameObject AudioCreditsButton; for if the audiocredits button was on the home screen instead of in the settings page

    public enum pageState { Game, StartPage, GameOver, Paused, CountdownPage, SettingsPage, ScoreReview, ShopPage };
    pageState currentPageState;

    public int Link2PaddleItem(string name)
    {
        for (int i = 0; i < paddles.Length; i++)
        {
            if ((name + "(Clone)").Equals(paddles[i].mainParticles.name))
            {
                return i;
            }
        }
        return 0;
    }

    private static string CreateRandomPassword(int passwordLength)
    {
        string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
        char[] chars = new char[passwordLength];

        for (int i = 0; i < passwordLength; i++)
        {
            chars[i] = allowedChars[rng.Next(0, allowedChars.Length)];
        }

        return new string(chars);
    }

    private void Awake()
    {
        Instance = this;

        audioListener = gameObject.GetComponent<AudioListener>();

        AudioListener.volume = 0;

        string rAFARRfwej82qwe = PlayerPrefs.GetString("rAFARRfwej82qwe"); //password for Zplayerprefs initialization
        string asfmn2348HKOA823 = PlayerPrefs.GetString("asfmn2348HKOA823"); //salt for Zplayerprefs initialization

        if(rAFARRfwej82qwe.Length == 0) //default value for a playerprefs string is "" (a string with length of 0)
        {
            rAFARRfwej82qwe = CreateRandomPassword(17);
            asfmn2348HKOA823 = CreateRandomPassword(17);

            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetString("rAFARRfwej82qwe", rAFARRfwej82qwe);
            PlayerPrefs.SetString("asfmn2348HKOA823", asfmn2348HKOA823);
        }

        ZPlayerPrefs.Initialize(rAFARRfwej82qwe, asfmn2348HKOA823); //ZPlayerPrefs does not work until it is initialized


        GameModeButtonAnimC = GameModeButton.GetComponent<Animator>();
        GameModeButton.SetActive(true);

        Time.timeScale = timeScale;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        gems = ZPlayerPrefs.GetInt("gems");
        highScore = ZPlayerPrefs.GetInt("HighScore");

        scoreReviewGems = ScoreReview.transform.Find("gems").GetComponent<Text>();
        scoreReviewGems.text = gems.ToString();

        GemsText.text = gems.ToString();

        paddles = new PaddlePrefab[paddlePrefabs.Length];
        for (int i = 0; i < paddlePrefabs.Length; i++)
        {
            paddles[i] = new PaddlePrefab(paddlePrefabs[i]);
            paddles[i].index = i;
        }

        extraBallSprite.SetActive(false);

        tutorial.SetActive(false);

        infoScrollRect = InfoPage.GetComponentInChildren<ScrollRect>();

        firstPlayEver = ZPlayerPrefs.GetInt("firstPlayEver");
    }

    public void SetPaddle(int index)
    {
        selectedPaddle = paddles[index];
        paddleChanged = true;
    }

    private void Start()
    {
        extraBall = false;
        Paddle = PaddleController.Instance;
        target = TargetController.Instance;
        LG = LevelGenerator.Instance;
        ballC = BallController.Instance;
        ads = AdManager.Instance;
        sceneChanger = SceneChanger.Instance;
        audioManager = AudioManager.Instance;
        rankings = AchievementsAndLeaderboards.Instance;
        rate = Ratings.Instance;

        audioManager.GetAudioFiltersFromCamera();

        selectedPaddle = paddles[ZPlayerPrefs.GetInt("paddleInUse")];
        paddleChanged = true;
        Paddle.DestroyOldPaddleParticles(); //prevents duplication when changing scenes
        Paddle.SetPaddle(selectedPaddle);
        DeactivatePaddle();

        GoToStartPage();
        gameRunning = false;
        paused = false;

        noSound = PlayerPrefsX.GetBool("noSound");
        audioManager.PlayLvlSound("ambientLab");
        audioListener.enabled = true;
        if (!noSound)
        {
            fadeInVolume = StartCoroutine(FadeInVolume());
        }
    }

    IEnumerator FadeInVolume() //fade in the games master volume
    {
        float targetTime = 1;
        float elaspedTime = 0;

        while (AudioListener.volume != 1)
        {
            elaspedTime += Time.deltaTime;

            AudioListener.volume = Mathf.Lerp(0, 1, elaspedTime / targetTime);
            yield return null;
        }
    }

    IEnumerator FadeInVolumeFromPause() //fade in the games master volume
    {
        AudioListener.pause = false;

        float targetTime = 0.2f;
        float elaspedTime = 0;

        while (AudioListener.volume != 1)
        {
            elaspedTime += Time.unscaledDeltaTime;

            AudioListener.volume = Mathf.Lerp(0, 1, elaspedTime / targetTime);
            yield return null;
        }
    }

    IEnumerator FadeOutVolume() //fade out games master volume
    {
        if (fadeInVolume != null) StopCoroutine(fadeInVolume);

        float targetTime = 0.28f;
        float elaspedTime = 0;

        while (AudioListener.volume != 0)
        {
            elaspedTime += Time.unscaledDeltaTime;

            AudioListener.volume = Mathf.Lerp(1, 0, elaspedTime / targetTime);
            yield return null;
        }

        if (paused)
        {
            AudioListener.pause = true;
        }
    }

    private void OnEnable() //this is called after start()
    {
        Ball.PlayerMissed += PlayerMissed;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDoneAndRichochet;
    }

    private void OnDisable()
    {
        Ball.PlayerMissed -= PlayerMissed;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDoneAndRichochet;

        ZPlayerPrefs.SetInt("gems", (int)gems);
        ZPlayerPrefs.SetInt("HighScore", highScore);

        ZPlayerPrefs.SetInt("paddleInUse", selectedPaddle.index);
        ZPlayerPrefs.SetInt("firstPlayEver", firstPlayEver);
    }

    void PlayerMissed()
    {
        if (canEndGame)
        {
            if (!extraBall)
            {
                if (canContinue)
                {
                    GameOver();
                }
                else
                {
                    EndGame();
                }
            }
            else
            {
                extraBall = false;
                extraBallSprite.SetActive(false);
                richochetCount = 0;

                StartCoroutine(ReviveDelay());
            }
            canEndGame = false;
        }
    }

    void ShowGameModeButton(bool show)
    {
        if (show)
        {
            GameModeButtonAnimC.SetTrigger("fadeIn");
            GameModeButtonAnimC.ResetTrigger("fadeOut");
        }
        else
        {
            GameModeButtonAnimC.SetTrigger("fadeOut");
            GameModeButtonAnimC.ResetTrigger("fadeIn");
        }
    }

    IEnumerator ReviveDelay()
    {
        audioManager.PlayMiscSound("revive");
        for (float i = 0.0f; i < 1.8f; i += 0.1f)  //make sure this delay is longer than the length of the ball shrink anim which is 1.3 seconds
        {
            yield return new WaitForSeconds(0.1f);
            while (paused)
            {
                yield return null;
            }
        }
        Revive();
        canEndGame = true;
    }

    public bool hasExtraBall
    {
        get
        {
            return extraBall;
        }
    }

    void AbsorbDone()
    {
        score++;
        scoreText.text = score.ToString();
        richochetCount = 0;
    }

    void AbsorbDoneAndRichochet()
    {
        score += 2;
        scoreText.text = score.ToString();
        richochetCount++;
        if (richochetCount == 2)
        {
            extraBall = true;
            richochetCount = 0;
            extraBallSprite.SetActive(true);
        }
    }

    //do not add any menus or pages here that have scripts attached to them with doNotDestoyOnLoad()
    public void SetPageState(pageState page)
    {
        switch (page)
        {
            case pageState.Game:
                currentPageState = pageState.Game;
                GamePage.SetActive(true);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                //AudioCreditsButton.SetActive(false);
                GemsText.gameObject.SetActive(false);
                break;

            case pageState.StartPage:
                currentPageState = pageState.StartPage;
                GamePage.SetActive(false);
                StartPageButtons.SetActive(true);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                //AudioCreditsButton.SetActive(true);
                GemsText.gameObject.SetActive(true);

                ShowGameModeButton(true);
                gemsOnScreen = false;
                break;

            case pageState.GameOver:
                currentPageState = pageState.GameOver;
                GamePage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(true);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(false);
                break;

            case pageState.Paused:
                currentPageState = pageState.Paused;
                GamePage.SetActive(true);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(true);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(false);

                pauseButton.gameObject.SetActive(false);

                break;

            case pageState.CountdownPage:
                currentPageState = pageState.CountdownPage;
                GamePage.SetActive(true);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(true);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(false);

                pauseButton.gameObject.SetActive(true);

                break;

            case pageState.SettingsPage:
                currentPageState = pageState.SettingsPage;
                GamePage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(true);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
               //AudioCreditsButton.SetActive(false);
                GemsText.gameObject.SetActive(false);

                LG.settingsPage.SetActive(true);
                break;

            case pageState.ScoreReview:
                currentPageState = pageState.ScoreReview;
                GamePage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(true);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(false);
                break;

            case pageState.ShopPage:
                currentPageState = pageState.ShopPage;
                GamePage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(true);
                //AudioCreditsButton.SetActive(false);
                GemsText.gameObject.SetActive(true);

                LG.shop.SetActive(true);
                break;
        }
    }

    private void Update()
    {
        if (gemsOnScreen)
        {
            t += 0.1f * Time.deltaTime;
            tempGems = Mathf.Lerp(tempGems, newGems, t);
            if (tempGems == newGems)
            {
                gemsOnScreen = false;
            }
            scoreReviewGems.text = Mathf.RoundToInt(tempGems).ToString();
        }
    }

    public void StartGame()
    {
        audioManager.PlayUISound("play");

        ShowGameModeButton(false);
        canEndGame = true;
        extraBall = false;
        richochetCount = 0;
        scoreText.text = score.ToString();
        SetPageState(pageState.Game);
        extraBallSprite.SetActive(false);
        gameRunning = true;
        replayButton.interactable = true;

        ActivatePaddle();

        canContinue = true;

        LG.TurnOffLab();

        StartCoroutine(StartGameDelay());
    }

    IEnumerator StartGameDelay() //used so that the play button sound is more distinct
    {
        yield return new WaitForSeconds(0.55f);
        GameStarted();
    }

    public void ActivatePaddle()
    {
        Paddle.gameObject.SetActive(true);
        if (paddleChanged)
        {
            Paddle.SetPaddle(selectedPaddle);
            paddleChanged = false;
        }
    }

    public void GameOver() //soft game over: can still continue after
    {
        gameRunning = false;
        DeactivatePaddle();
        SetPageState(pageState.GameOver);
    }

    public void EndGame() //this will actually end the game: no revive or continue
    {
        gameRunning = false;
        DeactivatePaddle();
        GoToScoreReview();
    }

    public void Continue()
    {
        StartCoroutine(ReviveDelay());
        Paddle.gameObject.SetActive(true);
        gameRunning = true;
        SetPageState(pageState.Game);

        canContinue = false;
    }

    public void RequestContinue()
    {
        richochetCount = 0;
        ads.ShowRewardVideo(false);
    }

    public void ResumeGame()
    {
        SetPageState(pageState.CountdownPage);
        pauseCoroutine = StartCoroutine(Countdown());
    }

    public void GoToSettings()
    {
        audioManager.PlayUISound("computerSelect1");

        ShowGameModeButton(false);
        SetPageState(pageState.SettingsPage);
        LG.GoToSettingsPage();
    }

    public void ComeBackFromSettings()
    {
        settingsPageAnimC.SetTrigger("leave");
        shopPageAnimC.SetTrigger("leave");
        LG.ComeBackFromSettingsPage();
    }

    public void GoToShop()
    {
        audioManager.PlayUISound("computerSelect1");

        ShowGameModeButton(false);
        SetPageState(pageState.ShopPage);
        LG.GoToShop();
    }

    public void ComeBackFromShop()
    {
        settingsPageAnimC.SetTrigger("leave");
        shopPageAnimC.SetTrigger("leave");

        ShopController.Instance.StopComingSoonShake();
        LG.ComeBackFromShop();
    }

    public void PauseGame()
    {
        if(!noSound)
            StartCoroutine(FadeOutVolume());

        SetPageState(pageState.Paused);
        Time.timeScale = 0;
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine); // whenever trying to stop a coroutine make sure you start and stop it with its string name. This way all coroutines running with that name stop instead of the specific one that was started.
        }
        DeactivatePaddle();
        paused = true;
    }

    public void GoToStartPage()
    {
        replayButton.interactable = false;
        if (LG.PlayedOnce)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            SetPageState(pageState.StartPage);
        }
        score = 0;

        if (LG.PlayedOnce)
        {
            ads.ShowInterstitialOrNonSkipAd();
        }
    }

    IEnumerator FadeOut()//sent to enemybehavior
    {
        scoreReviewAnimC.SetTrigger("swipeOut");
        for (float i = 0.0f; i < 0.24f; i += 0.1f)//set this coroutine to be the length of the swipeOut anim
        {
            yield return new WaitForSeconds(0.1f);
            while (pauseAllCoroutines)
            {
                yield return null;
            }
        }
        ballC.Fade2Black();
    }

    IEnumerator DisableReplayButon()
    {
        replayButton.interactable = false;

        yield return new WaitForSeconds(0.8f);//set this coroutine to be the length of the swipeIn anim
        while (pauseAllCoroutines)
        {
            yield return null;
        }

        gemsOnScreen = true;
        skipScoreReviewButton.interactable = false;
        replayButton.interactable = true;
    }

    public void GoToScoreReview()
    {
        rate.Ask4Rate();

        t = 0.0f;
        tempGems = gems;
        newGems = (int)gems + score;
        scoreReviewGems.text = gems.ToString();
        UpdateGems(score);
        if (score > highScore)
        {
            highScore = score;
            newHighScoreImage.SetActive(true);
            updateHS = true;
        }
        else
        {
            newHighScoreImage.SetActive(false);
        }
        gameOverScore.text = score.ToString();
        highScoreText.text = highScore.ToString();

        skipScoreReviewButton.interactable = true;
        disableReplayButtonC = StartCoroutine(DisableReplayButon());
        SetPageState(pageState.ScoreReview);

        firstPlayEver = 1;
    }

    public void UpdateGems(int gems2Add, bool subtract = false) //updates the gem text on each page that has it
    {
        if (!subtract)
        {
            gems += gems2Add;
            GemsText.text = gems.ToString();
        }
        else
        {
            gems -= gems2Add;
            GemsText.text = gems.ToString();
        }
    }

    //only updating Zplayerprefs when player is not using app to prevent lag while in use
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            volB4Pause = AudioListener.volume;
            AudioListener.volume = 0;
            pauseAllCoroutines = true;

            ZPlayerPrefs.SetInt("gems", (int)gems);
            ZPlayerPrefs.SetInt("HighScore", highScore);

            ZPlayerPrefs.SetInt("paddleInUse", selectedPaddle.index);

            if (updateHS)
            {
                rankings.AddScore2LeaderBoard(AchievementsAndLeaderboards.leaderBoards.highScores, highScore);
            }
        }
        else
        {
            if (!noSound)
            {
                AudioListener.volume = volB4Pause;
            }
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationQuit()
    {
        ZPlayerPrefs.SetInt("gems", (int)gems);
        ZPlayerPrefs.SetInt("HighScore", highScore);

        ZPlayerPrefs.SetInt("paddleInUse", selectedPaddle.index);

        if (updateHS)
        {
            rankings.AddScore2LeaderBoard(AchievementsAndLeaderboards.leaderBoards.highScores, highScore);
        }
    }

    public int GetScore
    {
        get
        {
            return score;
        }
    }

    public bool IsGameRunning
    {
        get
        {
            return gameRunning;
        }
    }

    public float TimeScale
    {
        get
        {
            return timeScale;
        }
    }

    public void skipScoreReviewAnim()
    {
        skipScoreReviewButton.interactable = false;
        StopCoroutine(disableReplayButtonC);
        replayButton.interactable = true;
        scoreReviewAnimC.SetTrigger("skipAnim");
        gemsOnScreen = true;
    }

    public bool Paused
    {
        get
        {
            return paused;
        }
    }

    public void DeactivatePaddle()
    {
        Paddle.DeactivatePaddle();
        Paddle.gameObject.SetActive(false);
    }

    public int Gems
    {
        get { return (int)gems; }
    }

    IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();

            audioManager.PlayMiscSound("countdownPing");

            yield return new WaitForSecondsRealtime(1);
            while (pauseAllCoroutines)
            {
                yield return null;
            }
        }
        CountdownPage.SetActive(false);
        paused = false;
        Paddle.gameObject.SetActive(true);
        Time.timeScale = timeScale;

        if (!noSound)
            StartCoroutine(FadeInVolumeFromPause());
    }

    void Go2GameModesMenu()
    {
        if (!noSound)
        {
            audioManager.PlayUISound("computerSelect2");

            StartCoroutine(FadeOutVolume());
        }
        sceneChanger.Fade2Scene(1);
    }

    public void EnableTutorial()
    {
        tutorial.SetActive(true);
    }

    public void Go2Info()
    {
        audioManager.PlayUISound("computerSelect1");

        InfoPage.SetActive(true);

        infoScrollRect.verticalNormalizedPosition = 1;
        ballC.SetTutorialToggle();
    }

    public void ExitInfo()
    {
        audioManager.PlayUISound("switchPageLouder");
        InfoPage.SetActive(false);
    }

    public void Go2ScoresPage()
    {
        audioManager.PlayUISound("computerSelect1");

        ScoresPage.SetActive(true);
    }

    public void ExitScoresPage()
    {
        audioManager.PlayUISound("switchPageLouder");
        ScoresPage.SetActive(false);
    }

    public void Go2AudioCredits() 
    {
        audioManager.PlayUISound("switchPageLouder");
        AudioCredits.SetActive(true);
        //AudioCreditsButton.SetActive(false);
    }

    public void ExitAudioCredits()
    {
        audioManager.PlayUISound("switchPage"); //do not need the "switchPageLouder" sound because this button is on the settings page where the ambient sounds are not as loud
        AudioCredits.SetActive(false);
        //AudioCreditsButton.SetActive(true);
    }

    public void Go2Rate()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.nnaji.PortalPaddle");
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1444939274");
#endif    
    }

    public bool FirstPlayEver
    {
        get
        {
            return (firstPlayEver == 0) ? true : false;
        }
    }

    public void LowEarthOrbitLink()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=dgCnYsDTiXU");
    }

    public void ShowLeaderboards()
    {
        rankings.ShowLeaderboards();
    }

    public void ShowAchievements()
    {
        rankings.ShowAchievements();
    }

    public void SetNoSound(bool noSound)
    {
        this.noSound = noSound;
    }
}
