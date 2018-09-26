using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OtherGameModesManager : MonoBehaviour
{

    public static OtherGameModesManager Instance;

    public GameObject StartPage;
    public GameObject PauseMenu;
    public GameObject CountdownPage;
    public GameObject GamePage;
    public GameObject ScoreReview;
    public Button pauseButton;
    public Text countdownText;
    public Button PlusOneButton;
    public Button DeadeyeButton;
    public Button ClairvoyanceButton;
    public RectTransform pauseButtonRect;
    public Text scoreText;
    public Text highScoreText;
    public Text gameOverScore; //score when you loose for that run
    public GameObject newHighScoreImage;
    public Button skipScoreReviewButton;
    public Button replayButton;
    public Button GoBack2ModeSelectButton;
    public Animator scoreReviewAnimC;
    public Text PlusOneHighScore;
    public Text DeadeyeHighScore;
    public Text ClairvoyanceHighScore;
    public Text UltraHighScore; // just the sum of all the other high scores

    GameManager game;
    ObstacleSpawner obSpawner;
    PaddleController Paddle;
    BallController ballC;
    SceneChanger sceneChanger;
    TargetController targetC;
    AdManager ads;

    Coroutine disableReplayButtonC;
    Coroutine pauseCoroutine;
    Text scoreReviewGems;

    bool gemsOnScreen = false;
    bool gameModeRunning = false;
    bool pauseAllCoroutines = false;
    bool paused = false;
    bool firstStart = true;
    int PlusOneHS;
    int DeadeyeHS;
    int ClairvoyanceHS;
    int UltraScore;
    int score = 0;
    float t = 0;
    float tempGems = 0;
    float gems = 0;
    int newGems;
    int activeHighScore;

    public delegate void OtherGameModesManagerDelegate();
    public static event OtherGameModesManagerDelegate StartPlusOne;
    public static event OtherGameModesManagerDelegate StartDeadeye;
    public static event OtherGameModesManagerDelegate StartClairvoyance;
    public static event OtherGameModesManagerDelegate GameModeStarted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetPageState(pageState.StartPage);

        PlusOneHS = ZPlayerPrefs.GetInt("PlusOneHighScore");
        PlusOneHighScore.text = PlusOneHS.ToString();
        DeadeyeHS = ZPlayerPrefs.GetInt("DeadeyeHighScore");
        DeadeyeHighScore.text = DeadeyeHS.ToString();
        ClairvoyanceHS = ZPlayerPrefs.GetInt("ClairvoyanceHighScore");
        ClairvoyanceHighScore.text = ClairvoyanceHS.ToString();

        UltraScore = PlusOneHS + DeadeyeHS + ClairvoyanceHS;
        UltraHighScore.text = UltraScore.ToString();

        gems = ZPlayerPrefs.GetInt("gems");

        scoreReviewGems = ScoreReview.transform.Find("gems").GetComponent<Text>();
        scoreReviewGems.text = gems.ToString();
    }

    private void Start()
    {
        obSpawner = ObstacleSpawner.Instance;
        game = GameManager.Instance;
        ballC = BallController.Instance;
        Paddle = PaddleController.Instance;
        sceneChanger = SceneChanger.Instance;
        targetC = TargetController.Instance;
        ads = AdManager.Instance;

        Paddle.SetPauseButtonRect(pauseButtonRect);
        DeactivatePaddle();

        SetGameMode(gameMode.None);
    }

    public void Scored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void Missed()
    {
        DeactivatePaddle();
        GoToScoreReview();
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

    public void GoToScoreReview()
    {
        t = 0.0f;
        tempGems = gems;
        newGems = (int)gems + score;
        scoreReviewGems.text = gems.ToString();
        gems += score;
        if (score > ActiveHighScore())
        {
            ActiveHighScore(score);
            newHighScoreImage.SetActive(true);
        }
        else
        {
            newHighScoreImage.SetActive(false);
        }
        gameOverScore.text = score.ToString();
        highScoreText.text = ActiveHighScore().ToString();

        skipScoreReviewButton.interactable = true;
        disableReplayButtonC = StartCoroutine(DisableReplayButon());
        SetPageState(pageState.ScoreReview);
    }

    IEnumerator DisableReplayButon()
    {
        replayButton.interactable = false;
        GoBack2ModeSelectButton.interactable = false;

        yield return new WaitForSeconds(0.8f);//set this coroutine to be the length of the swipeIn anim
        while (pauseAllCoroutines)
        {
            yield return null;
        }

        gemsOnScreen = true;
        skipScoreReviewButton.interactable = false;
        replayButton.interactable = true;
        GoBack2ModeSelectButton.interactable = true;
    }

    public void skipScoreReviewAnim()
    {
        skipScoreReviewButton.interactable = false;
        StopCoroutine(disableReplayButtonC);
        replayButton.interactable = true;
        GoBack2ModeSelectButton.interactable = true;
        scoreReviewAnimC.SetTrigger("skipAnim");
        gemsOnScreen = true;
    }

    public int ActiveHighScore(int newHS = 0)
    {
        switch (activeHighScore)
        {
            case 1:
                if (newHS > 0)
                {
                    PlusOneHS = newHS;
                    PlusOneHighScore.text = PlusOneHS.ToString();
                    UpdateUltraScore();
                }

                return PlusOneHS;

            case 2:
                if (newHS > 0)
                {
                    DeadeyeHS = newHS;
                    DeadeyeHighScore.text = DeadeyeHS.ToString();
                    UpdateUltraScore();
                }

                return DeadeyeHS;

            case 3:
                if (newHS > 0)
                {
                    ClairvoyanceHS = newHS;
                    ClairvoyanceHighScore.text = ClairvoyanceHS.ToString();
                    UpdateUltraScore();
                }

                return ClairvoyanceHS;
        }

        return 0;
    }

    public enum pageState { Game, StartPage, Paused, CountdownPage, ScoreReview };
    pageState currentPageState;

    public enum gameMode { PlusOne, Deadeye, Clairvoyance, None }
    gameMode currentGameMode;

    public void SetPageState(pageState page)
    {
        switch (page)
        {
            case pageState.Game:
                currentPageState = pageState.Game;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                ScoreReview.SetActive(false);
                break;

            case pageState.StartPage:
                currentPageState = pageState.StartPage;
                GamePage.SetActive(false);
                StartPage.SetActive(true);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                ScoreReview.SetActive(false);

                gemsOnScreen = false;

                break;


            case pageState.Paused:
                currentPageState = pageState.Paused;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                PauseMenu.SetActive(true);
                CountdownPage.SetActive(false);
                ScoreReview.SetActive(false);

                pauseButton.gameObject.SetActive(false);

                break;

            case pageState.CountdownPage:
                currentPageState = pageState.CountdownPage;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(true);
                ScoreReview.SetActive(false);

                pauseButton.gameObject.SetActive(true);

                break;


            case pageState.ScoreReview:
                currentPageState = pageState.ScoreReview;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                ScoreReview.SetActive(true);
                break;
        }
    }

    public void SetGameMode(gameMode gameMode)
    {
        switch (gameMode)
        {
            case gameMode.PlusOne:
                currentGameMode = gameMode.PlusOne;

                obSpawner.SetGameMode(gameMode.PlusOne);
                ballC.SetGameMode(gameMode.PlusOne);

                activeHighScore = 1;
                break;

            case gameMode.Deadeye:
                currentGameMode = gameMode.Deadeye;

                obSpawner.SetGameMode(gameMode.Deadeye);
                ballC.SetGameMode(gameMode.Deadeye);

                activeHighScore = 2;
                break;

            case gameMode.Clairvoyance:
                currentGameMode = gameMode.Clairvoyance;

                obSpawner.SetGameMode(gameMode.Clairvoyance);
                ballC.SetGameMode(gameMode.Clairvoyance);

                activeHighScore = 3;
                break;

            case gameMode.None:
                currentGameMode = gameMode.None;

                obSpawner.SetGameMode(gameMode.None);
                ballC.SetGameMode(gameMode.None);

                targetC.SetTargetColor(Color.yellow);
                break;
        }
    }

    public void Go2PlusOne()
    {
        ballC.Fade2GameMode(pageState.Game, gameMode.PlusOne);
        SetGameModeSelectButtons(false);
    }

    public void Go2Deadeye()
    {
        ballC.Fade2GameMode(pageState.Game, gameMode.Deadeye);
        SetGameModeSelectButtons(false);
    }

    public void Go2Clairavoyance()
    {
        ballC.Fade2GameMode(pageState.Game, gameMode.Clairvoyance);
        SetGameModeSelectButtons(false);
    }

    public void GoBack2ModeSelect()
    {
        GoBack2ModeSelectButton.interactable = false;

        score = 0;
        scoreText.text = score.ToString();

        ads.ShowInterstitialOrNonSkipAd();

        ballC.Fade2GameMode(pageState.StartPage, gameMode.None);
        SetGameModeSelectButtons(true);
    }

    public void Replay()
    {
        replayButton.interactable = false;

        ads.ShowInterstitialOrNonSkipAd();

        firstStart = true;
        scoreReviewAnimC.SetTrigger("swipeOut");
        ballC.Fade2GameMode(pageState.Game, currentGameMode, true);

        score = 0;
        scoreText.text = score.ToString();
    }

    public void StartGameMode()
    {
        if (currentGameMode != gameMode.None)
        {
            Time.timeScale = 0;
            SetPageState(pageState.CountdownPage);
            pauseCoroutine = StartCoroutine(Countdown());
        }

        firstStart = true;

        replayButton.interactable = true;
        GoBack2ModeSelectButton.interactable = true;
    }

    public void PauseGame()
    {
        SetPageState(pageState.Paused);
        Time.timeScale = 0;
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
        }
        DeactivatePaddle();
        paused = true;
    }

    public void ResumeGame()
    {
        SetPageState(pageState.CountdownPage);
        pauseCoroutine = StartCoroutine(Countdown());
    }

    public void SetGameModeSelectButtons(bool enable)
    {
        if (enable)
        {
            PlusOneButton.interactable = true;
            DeadeyeButton.interactable = true;
            ClairvoyanceButton.interactable = true;
        }
        else
        {
            PlusOneButton.interactable = false;
            DeadeyeButton.interactable = false;
            ClairvoyanceButton.interactable = false;
        }
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
        ActivatePaddle();
        Time.timeScale = 1;
        gameModeRunning = true;
        if (firstStart)
        {
            firstStart = false;

            ActivatePaddle();
            GameModeStarted();
        }
    }

    public void GoBackHome()
    {
        sceneChanger.Fade2Scene(0);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            ZPlayerPrefs.SetInt("PlusOneHighScore", PlusOneHS);
            ZPlayerPrefs.SetInt("DeadeyeHighScore", DeadeyeHS);
            ZPlayerPrefs.SetInt("ClairvoyanceHighScore", ClairvoyanceHS);
            ZPlayerPrefs.SetInt("gems", (int)gems);

            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ZPlayerPrefs.SetInt("PlusOneHighScore", PlusOneHS);
            ZPlayerPrefs.SetInt("DeadeyeHighScore", DeadeyeHS);
            ZPlayerPrefs.SetInt("ClairvoyanceHighScore", ClairvoyanceHS);
            ZPlayerPrefs.SetInt("gems", (int)gems);

            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    public void DeactivatePaddle()
    {
        Paddle.DeactivatePaddle(); // deactivatePaddle also sets othergamerunning bool to false;
        Paddle.gameObject.SetActive(false);
    }

    public void ActivatePaddle()
    {
        Paddle.gameObject.SetActive(true);
        Paddle.IsOtherGameModeRunning = true;
    }

    private void OnDisable()
    {
        ZPlayerPrefs.SetInt("PlusOneHighScore", PlusOneHS);
        ZPlayerPrefs.SetInt("DeadeyeHighScore", DeadeyeHS);
        ZPlayerPrefs.SetInt("ClairvoyanceHighScore", ClairvoyanceHS);
        ZPlayerPrefs.SetInt("gems", (int)gems);
    }

    void UpdateUltraScore()
    {
        UltraScore = PlusOneHS + DeadeyeHS + ClairvoyanceHS;
        UltraHighScore.text = UltraScore.ToString();
    }
}
