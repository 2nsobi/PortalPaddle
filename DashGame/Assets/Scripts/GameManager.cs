using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PaddleController Paddle;
    LevelGenerator LG;
    EnemyBehavior ball;
    public Button pauseButton;
    public Text countdownText;
    public Animator scoreReviewAnimC;
    public Animator settingsPageAnimC;
    public Button skipScoreReviewButton;
    public Button replayButton;
    bool extraBall;
    int richochetCount;
    public GameObject extraBallSprite;
    TargetController TargetController;
    public Text scoreText;
    private int score;
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
    Text scoreReviewGems, startPageGems;
    bool gemsOnScreen = false;
    float gems;
    int newGems;
    float t = 0.0f;
    bool canEndGame = true;

    public static GameManager Instance;

    public delegate void GameDelegate();
    public static event GameDelegate GameOverConfirmed; //when the game is confirmed to be over the next page is the main menu;
    public static event GameDelegate GameStarted;
    public static event GameDelegate Revive;

    public GameObject StartPage;
    public GameObject GameOverPage;
    public GameObject PauseMenu;
    public GameObject CountdownPage;
    public GameObject SettingsPage;
    public GameObject GamePage;
    public GameObject ScoreReview;

    public enum pageState { Game, StartPage, GameOver, Paused, CountdownPage, SettingsPage, ScoreReview };
    pageState currentPageState;

    public pageState GetCurrentPageState
    {
        get
        {
            return currentPageState;
        }
    }

    private void Awake()
    {

        PlayerPrefs.DeleteAll();



        Instance = this;
        Time.timeScale = timeScale;
    }

    private void Start()
    {
        extraBall = false;
        Paddle = PaddleController.Instance;
        TargetController = TargetController.Instance;
        LG = LevelGenerator.Instance;
        ball = EnemyBehavior.Instance;
        Paddle.gameObject.SetActive(false);

        gems = PlayerPrefs.GetInt("gems");
        scoreReviewGems = ScoreReview.transform.Find("gems").GetComponent<Text>();
        scoreReviewGems.text = gems.ToString();
        startPageGems = StartPage.transform.Find("gems").GetComponent<Text>();
        startPageGems.text = gems.ToString();

        GoToStartPage();
        gameRunning = false;
        paused = false;
    }

    private void OnEnable()
    {
        EnemyBehavior.PlayerMissed += PlayerMissed;
        TargetController.TargetHit += TargetHit;
        TargetController.TargetHitAndRichochet += TargetHitAndRichochet;
    }

    private void OnDisable()
    {
        EnemyBehavior.PlayerMissed -= PlayerMissed;
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
        Time.timeScale = timeScale;
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

    void PlayerMissed()
    {
        if (canEndGame)
        {
            Debug.Log("you " + (extraBall ? "have an extra ball" : "dont have an extra ball"));
            if (!extraBall)
            {
                GameOver();
            }
            else
            {
                extraBall = false;
                extraBallSprite.SetActive(false);
                StartCoroutine(ReviveDelay());

                StopCoroutine("GameErrorTest");
                StartCoroutine("GameErrorTest");
            }
            canEndGame = false;
        }
    }

    IEnumerator ReviveDelay()
    {
        yield return new WaitForSeconds(0.6f);
        while (pauseAllCoroutines)
        {
            yield return null;
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

        StopCoroutine("GameErrorTest");
        StartCoroutine("GameErrorTest");
    }

    void TargetHitAndRichochet()
    {
        score += 2;
        scoreText.text = score.ToString();
        richochetCount++;
        if (richochetCount == 1)
        {
            extraBall = true;
            richochetCount = 0;
            extraBallSprite.SetActive(true);
        }

        StopCoroutine("GameErrorTest");
        StartCoroutine("GameErrorTest");
    }

    public void SetPageState(pageState page)
    {
        switch (page)
        {
            case pageState.Game:
                currentPageState = pageState.Game;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                break;

            case pageState.StartPage:
                currentPageState = pageState.StartPage;
                GamePage.SetActive(false);
                StartPage.SetActive(true);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);

                gemsOnScreen = false;
                break;

            case pageState.GameOver:
                currentPageState = pageState.GameOver;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
                GameOverPage.SetActive(true);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                break;

            case pageState.Paused:
                currentPageState = pageState.Paused;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(true);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);

                pauseButton.gameObject.SetActive(false);

                break;

            case pageState.CountdownPage:
                currentPageState = pageState.CountdownPage;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(true);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);

                pauseButton.gameObject.SetActive(true);

                break;

            case pageState.SettingsPage:
                currentPageState = pageState.SettingsPage;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(true);
                ScoreReview.SetActive(false);
                break;

            case pageState.ScoreReview:
                currentPageState = pageState.ScoreReview;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(true);
                break;

        }
    }

    private void Update()
    {
        if (gemsOnScreen)
        {
            t += 0.1f * Time.deltaTime;
            gems = Mathf.Lerp(gems, newGems, t);
            if(gems == newGems)
            {
                gemsOnScreen = false;
            }
            scoreReviewGems.text = Mathf.RoundToInt(gems).ToString();
        }
    }

    public void StartGame()
    {
        canEndGame = true;
        extraBall = false;
        richochetCount = 0;
        score = 0;
        scoreText.text = score.ToString();
        SetPageState(pageState.Game);
        Paddle.gameObject.SetActive(true);
        extraBallSprite.SetActive(false);
        gameRunning = true;
        StartCoroutine("GameErrorTest");

        replayButton.interactable = true;

        GameStarted();
    }

    public void GameOver()
    {
        StopCoroutine("GameErrorTest");
        gameRunning = false;
        Paddle.gameObject.SetActive(false);
        SetPageState(pageState.GameOver);
    }

    public void ResumeGame()
    {
        SetPageState(pageState.CountdownPage);
        pauseCoroutine = StartCoroutine(Countdown());
        Paddle.gameObject.SetActive(true);
    }

    public void GoToSettings()
    {
        SetPageState(pageState.SettingsPage);
        LG.GoToSettingsPage();
    }

    public void ComeBackFromSettings()
    {
        settingsPageAnimC.SetTrigger("leave");
        LG.ComeBackFromSettingsPage();
    }

    public void PauseGame()
    {
        SetPageState(pageState.Paused);
        Time.timeScale = 0;
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine); // whenever trying to stop a coroutine make sure you start and stop it with its string name. This way all coroutines running with that name stop instead of the specific one that was started.
        }
        Paddle.gameObject.SetActive(false);
        paused = true;
    }

    public void GoToStartPage()
    {
        replayButton.interactable = false;
        if (LG.PlayedOnce)
        {
            StartCoroutine(EndGame());
        }
        else
        {
            SetPageState(pageState.StartPage);
        }
        GameOverConfirmed();
    }

    IEnumerator EndGame()//sent to enemybehavior
    {
        scoreReviewAnimC.SetTrigger("swipeOut");
        yield return new WaitForSeconds(0.24f);//set this float to be the length of the swipeOut anim
        ball.Fade2Black();
    }

    IEnumerator DisableReplayButon()
    {
        replayButton.interactable = false;
        yield return new WaitForSeconds(0.8f); //set this float to be the length of the swipeIn anim
        gemsOnScreen = true;
        skipScoreReviewButton.interactable = false;
        replayButton.interactable = true;
    }

    public void GoToScoreReview()
    {
        t = 0.0f;
        gems = PlayerPrefs.GetInt("gems");
        newGems = (int) gems + score;
        scoreReviewGems.text = gems.ToString();
        PlayerPrefs.SetInt("gems", newGems);
        startPageGems.text = newGems.ToString();
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            newHighScoreImage.SetActive(true);
        }
        else
        {
            newHighScoreImage.SetActive(false);
        }
        gameOverScore.text = score.ToString();
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = highScore.ToString();

        skipScoreReviewButton.interactable = true;
        disableReplayButtonC = StartCoroutine(DisableReplayButon());
        SetPageState(pageState.ScoreReview);
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
        yield return new WaitForSeconds(40);
        while (paused)
        {
            yield return null;
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
}
