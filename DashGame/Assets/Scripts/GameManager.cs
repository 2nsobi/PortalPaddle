using System.Collections;
using System.Collections.Generic;
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
                leftEnd = Instantiate(pref.transform.GetChild(0).gameObject,mainParticles.transform);
                leftEnd.SetActive(false);
            }
        }
    }

    PaddleController Paddle;
    LevelGenerator LG;
    BallController ball;
    public Button pauseButton;
    public Text countdownText;
    public Animator scoreReviewAnimC;
    public Animator settingsPageAnimC; //for the settings page back button
    public Animator shopPageAnimC; //for the shop page back button
    public Button skipScoreReviewButton;
    public Button replayButton;
    bool extraBall;
    int richochetCount;
    public GameObject extraBallSprite;
    TargetController TargetController;
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
    Coroutine gameErrorTest;
    bool canContinue;
    AdManager ads;
    public Vector2 targetAspectRatio;
    float thisDeviceCameraRadius;
    bool dontMoveWalls = false;
    PaddlePrefab selectedPaddle;
    bool paddleChanged = true;
    bool ballChanged = true;
    bool updateGems = false;

    public static GameManager Instance;

    public delegate void GameDelegate();
    public static event GameDelegate GameOverConfirmed; //when the game is confirmed to be over the next page is the main menu;
    public static event GameDelegate GameStarted;
    public static event GameDelegate Revive;

    public GameObject StartPage;
    public GameObject StartPageButtons;
    public GameObject GameOverPage;
    public GameObject PauseMenu;
    public GameObject CountdownPage;
    public GameObject SettingsPage;
    public GameObject GamePage;
    public GameObject ScoreReview;
    public GameObject ShopPage;
    public Text GemsText;

    public GameObject GDPRConsentForm;

    public enum pageState { Game, StartPage, GameOver, Paused, CountdownPage, SettingsPage, ScoreReview, ShopPage };
    pageState currentPageState;

    public pageState GetCurrentPageState
    {
        get
        {
            return currentPageState;
        }
    }

    public int Link2PaddleItem(string name)
    {
        for(int i = 0; i < paddles.Length; i++)
        {
            if((name.Substring(0,name.Length-1) + "(Clone)").Equals(paddles[i].mainParticles.name))
            {
                return i;
            }
        }
        return 0;
    }

    private void Awake()
    {
        /********************************************
         DELETE THING BELOW
         **********************************************/

        //ZPlayerPrefs.DeleteAll();

        /********************************************
        DELETE THING ABOVE
        **********************************************/




        ZPlayerPrefs.Initialize("K]28y[+$SZAjM3V$", "EJw8mBv5xJ4~R@q:");

        if (ZPlayerPrefs.GetInt("result_gdpr") == 0)
        {
            GDPRConsentForm.SetActive(true);
        }
        else
        {
            GDPRConsentForm.SetActive(false);
        }

        Instance = this;

        Time.timeScale = timeScale;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ConfigureCamera(); //called here before the tap area rect is configured
   
        gems = ZPlayerPrefs.GetInt("gems");
        gems = 1000;
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
    }

    public void SetPaddle(int index)
    {
        selectedPaddle = paddles[index];
        paddleChanged = true;
    }

    /*********************************************
    * In unity "The size value for orthographic camera basically decides the Height of the camera while the Aspect 
    * Ratio of your game decides the width of the camera. When increasing the "height" size on the camera the width 
    * is also increased to keep with the current aspect."
    * 
    * Also the [aspect ratio (width/height)] * [camera size] = camera width; 
    ************************************************/
    public void ConfigureCamera()
    {
        thisDeviceCameraRadius = (Camera.main.aspect * Camera.main.orthographicSize);
        float desiredCameraWidth = (targetAspectRatio.x / targetAspectRatio.y) * Camera.main.orthographicSize;

        if (thisDeviceCameraRadius < desiredCameraWidth - 0.001f) //for some reason (targetAspectRatio.x / targetAspectRatio.y) * Camera.main.orthographicSize does not equal what is should exactly
        {
            Camera.main.orthographicSize = desiredCameraWidth / Camera.main.aspect;
            dontMoveWalls = true;
        }
    }

    public float GetDistanceDifferenceForWalls() //width of a wall is a bout 0.116524, and this gives the east wall an X pos of 3.700936 when the target aspect ratio is 9:16
    {
        if (dontMoveWalls)
        {
            return 3.700936f; // x pos of wall at aspect ratio of 3.700936
        }
        return thisDeviceCameraRadius + 0.888436f; // 0.888436 is the diff between the x pos of a wall at x pos 3.700936 and the camera width of a 9:16 aspect ratio
    }

    private void Start()
    {       
        extraBall = false;
        Paddle = PaddleController.Instance;
        TargetController = TargetController.Instance;
        LG = LevelGenerator.Instance;
        ball = BallController.Instance;
        ads = AdManager.Instance;

        selectedPaddle = paddles[ZPlayerPrefs.GetInt("paddleInUse")];
        DeactivatePaddle();

        GoToStartPage();
        gameRunning = false;
        paused = false;
    }

    private void OnEnable() //this is called after start()
    {
        Ball.PlayerMissed += PlayerMissed;
        TargetController.TargetHit += TargetHit;
        TargetController.TargetHitAndRichochet += TargetHitAndRichochet;
    }

    private void OnDisable()
    {
        Ball.PlayerMissed -= PlayerMissed;
        TargetController.TargetHit -= TargetHit;
        TargetController.TargetHitAndRichochet -= TargetHitAndRichochet;
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

    void PlayerMissed()
    {
        if (canEndGame)
        {
            Debug.Log("you " + (extraBall ? "have an extra ball" : "dont have an extra ball"));
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
                //extraBall = false;
                //extraBallSprite.SetActive(false);
                StartCoroutine(ReviveDelay());

                StopCoroutine(gameErrorTest);
                gameErrorTest = StartCoroutine(GameErrorTest());
            }
            canEndGame = false;
        }
    }

    IEnumerator ReviveDelay()
    {
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

    void TargetHit()
    {
        score++;
        scoreText.text = score.ToString();
        richochetCount = 0;

        StopCoroutine(gameErrorTest);
        gameErrorTest = StartCoroutine(GameErrorTest());
    }

    void TargetHitAndRichochet()
    {
        score += 2;
        scoreText.text = score.ToString();
        richochetCount++;
        if (richochetCount == 5)
        {
            extraBall = true;
            richochetCount = 0;
            extraBallSprite.SetActive(true);
        }

        StopCoroutine(gameErrorTest);
        gameErrorTest = StartCoroutine(GameErrorTest());
    }

    public void SetPageState(pageState page)
    {
        switch (page)
        {
            case pageState.Game:
                currentPageState = pageState.Game;
                GamePage.SetActive(true);             
                StartPage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(false);
                break;

            case pageState.StartPage:
                currentPageState = pageState.StartPage;
                GamePage.SetActive(false);
                StartPage.SetActive(true);
                StartPageButtons.SetActive(true);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(true);

                gemsOnScreen = false;
                break;

            case pageState.GameOver:
                currentPageState = pageState.GameOver;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
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
                StartPage.SetActive(false);
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
                StartPage.SetActive(false);
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
                StartPage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(true);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(false);
                GemsText.gameObject.SetActive(false);
                break;

            case pageState.ScoreReview:
                currentPageState = pageState.ScoreReview;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
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
                StartPage.SetActive(false);
                StartPageButtons.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(true);
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

    public void StartUpdatingGems()
    {
        updateGems = true;
    }

    public void StartGame()
    {
        canEndGame = true;
        extraBall = true;
        richochetCount = 0;
        scoreText.text = score.ToString();
        SetPageState(pageState.Game);
        extraBallSprite.SetActive(false);
        gameRunning = true;
        gameErrorTest = StartCoroutine(GameErrorTest());
        replayButton.interactable = true;

        Paddle.gameObject.SetActive(true);
        if (paddleChanged)
        {
            Paddle.SetPaddle(selectedPaddle);
            paddleChanged = false;
        }

        if (ballChanged)
        {
            ballChanged = false;
        }

        canContinue = true;

        GameStarted();
    }


    public void GameOver() //soft game over: can still continue after
    {
        StopCoroutine(gameErrorTest);
        gameRunning = false;
        DeactivatePaddle();
        SetPageState(pageState.GameOver);
    }

    public void EndGame() //this will actually end the game: so revive or continue
    {
        StopCoroutine(gameErrorTest);
        gameRunning = false;
        DeactivatePaddle();
        GoToScoreReview();
    }

    public void Continue()
    {
        ads.ShowRewardVideo(false);

        StartCoroutine(ReviveDelay());
        Paddle.gameObject.SetActive(true);
        gameRunning = true;
        SetPageState(pageState.Game);
        gameErrorTest = StartCoroutine(GameErrorTest());

        canContinue = false;
    }

    public void ResumeGame()
    {
        SetPageState(pageState.CountdownPage);
        pauseCoroutine = StartCoroutine(Countdown());
    }

    public void GoToSettings()
    {
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
        GameOverConfirmed();

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
        for (float i = 0.0f; i < 0.8f; i += 0.1f)//set this coroutine to be the length of the swipeIn anim
        {
            yield return new WaitForSeconds(0.1f);
            while (pauseAllCoroutines)
            {
                yield return null;
            }
        }
        gemsOnScreen = true;
        skipScoreReviewButton.interactable = false;
        replayButton.interactable = true;
    }

    public void GoToScoreReview()
    {
        t = 0.0f;
        tempGems = gems;
        newGems = (int)gems + score;
        scoreReviewGems.text = gems.ToString();
        UpdateGems(score);
        if (score > highScore)
        {
            highScore = score;
            newHighScoreImage.SetActive(true);
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
            ZPlayerPrefs.SetInt("HighScore", score);

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
        ZPlayerPrefs.SetInt("HighScore", score);

        ZPlayerPrefs.SetInt("paddleInUse", selectedPaddle.index);
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
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

    IEnumerator GameErrorTest()
    {
        for (int i = 0; i < 40; i++)//set this coroutine to be the length of the swipeIn anim
        {
            yield return new WaitForSeconds(1);
            while (pauseAllCoroutines || paused)
            {
                yield return null;
            }
        }
        Debug.LogError("Game Glitch");

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
        get { return (int) gems; }
    }
}
