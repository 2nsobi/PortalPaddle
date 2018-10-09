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
        public SnapScrollRectController.ShopItem shopItem;

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
    BallController ball;
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
    static string currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "apple" : "android";
    bool updateHS = false;

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
    public Text GemsText;

    public GameObject GDPRConsentForm;

    public enum pageState { Game, StartPage, GameOver, Paused, CountdownPage, SettingsPage, ScoreReview, ShopPage};
    pageState currentPageState;

    public int Link2PaddleItem(string name)
    {
        for (int i = 0; i < paddles.Length; i++)
        {
            if ((name.Substring(0, name.Length - 1) + "(Clone)").Equals(paddles[i].mainParticles.name))
            {
                return i;
            }
        }
        return 0;
    }

    private void Awake()
    {
        ZPlayerPrefs.Initialize("K]28y[+$SZAjM3V$", "EJw8mBv5xJ4~R@q:");

        //if (ZPlayerPrefs.GetInt("result_gdpr") == 0)
        //{
        //    GDPRConsentForm.SetActive(true);
        //}
        //else
        //{
        //    GDPRConsentForm.SetActive(false);
        //}





        /********************************************
         DELETE THING BELOW
         **********************************************/

        //ZPlayerPrefs.DeleteAll();

        /********************************************
        DELETE THING ABOVE
        **********************************************/





        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        print(firstPlayEver);
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
        ball = BallController.Instance;
        ads = AdManager.Instance;
        sceneChanger = SceneChanger.Instance;
        audioManager = AudioManager.Instance;
        rankings = AchievementsAndLeaderboards.Instance;
        rate = Ratings.Instance;

        selectedPaddle = paddles[ZPlayerPrefs.GetInt("paddleInUse")];
        Paddle.SetPaddle(selectedPaddle);
        DeactivatePaddle();

        GoToStartPage();
        gameRunning = false;
        paused = false;

        noSound = PlayerPrefsX.GetBool("noSound");
        if (!noSound)
        {
            StartCoroutine(FadeInVolume());
            audioManager.PlayLvlSound("ambientLab");
        }
    }

    IEnumerator FadeInVolume() //fade in the games master volume
    {
        AudioListener.volume = 0;

        float targetTime = 1;
        float elaspedTime = 0;

        while (elaspedTime < targetTime)
        {
            elaspedTime += Time.deltaTime;

            AudioListener.volume = Mathf.Lerp(0, 1, elaspedTime / targetTime);
            yield return null;
        }
    }

    IEnumerator FadeOutVolume() //fade out games master volume
    {
        float targetTime = 0.28f;
        float elaspedTime = 0;

        while (elaspedTime < targetTime)
        {
            elaspedTime += Time.deltaTime;

            AudioListener.volume = Mathf.Lerp(1, 0, elaspedTime / targetTime);
            yield return null;
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

        if (updateHS)
        {
            rankings.AddScore2LeaderBoard(GPGSIds.leaderboard_high_scores, highScore);
        }
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
            while (pauseAllCoroutines || paused)
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
                InfoPage.SetActive(false);
                ScoresPage.SetActive(false);
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
        richochetCount = 0;

        ads.ShowRewardVideo(false);

        StartCoroutine(ReviveDelay());
        Paddle.gameObject.SetActive(true);
        gameRunning = true;
        SetPageState(pageState.Game);

        canContinue = false;
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
        LG.ComeBackFromShop();
    }

    public void PauseGame()
    {
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
        ball.Fade2Black();
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
        print(FirstPlayEver);
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
            pauseAllCoroutines = true;

            ZPlayerPrefs.SetInt("gems", (int)gems);
            ZPlayerPrefs.SetInt("HighScore", highScore);

            ZPlayerPrefs.SetInt("paddleInUse", selectedPaddle.index);
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationQuit()
    {
        ZPlayerPrefs.SetInt("gems", (int)gems);
        ZPlayerPrefs.SetInt("HighScore", highScore);

        ZPlayerPrefs.SetInt("paddleInUse", selectedPaddle.index);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
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
        ball.SetTutorialToggle();
    }

    public void ExitInfo()
    {
        InfoPage.SetActive(false);
    }

    public void Go2ScoresPage()
    {
        audioManager.PlayUISound("computerSelect1");

        ScoresPage.SetActive(true);
    }

    public void ExitScoresPage()
    {
        ScoresPage.SetActive(false);
    }

    public void Go2Rate()
    {
        if (currentPlatform == "android")
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.nnaji.Portal.Paddle");
        }
    }

    public bool FirstPlayEver
    {
        get
        {
            print(firstPlayEver);
            return (firstPlayEver == 0) ? true : false;
        }
    }
}
