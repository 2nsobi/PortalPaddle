using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public PaddleController Paddle;
    public Button pauseButton;
    public Text countdownText;
    bool extraBall;
    int richochetCount;
    public GameObject extraBallSprite;
    Coroutine pauseCoroutine = null;
    Coroutine transitionCoroutine = null;
    TargetController TargetController;
    public Text scoreText;
    private int score;

    public static GameManager Instance;

    public delegate void GameDelegate();
    public static event GameDelegate GameOverConfirmed; //when the game is confirmed to be over the next page is the main menu;
    public static event GameDelegate GamePaused;
    public static event GameDelegate GameResumed;
    public static event GameDelegate GameStarted;
    public static event GameDelegate MoveToNextLvl;
    public static event GameDelegate GoToSettingsPage;
    public static event GameDelegate ComeBackFromSettingsPage;

    bool gameOver, gamePaused;

    public GameObject StartPage;
    public GameObject GameOverPage;
    public GameObject PauseMenu;
    public GameObject CountdownPage;
    public GameObject SettingsPage;
    public GameObject GamePage;

    public enum pageState {Game, StartPage, GameOver,Paused,CountdownPage,SettingsPage};
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
        Instance = this;
    }

    private void Start()
    {
        extraBall = false;
        Paddle = PaddleController.Instance;
        TargetController = TargetController.Instance;
        Paddle.gameObject.SetActive(false);
        GoToStartPage();
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
        for(int i = 3; i > 0 ; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }
        CountdownPage.SetActive(false);
        Time.timeScale = 1;
    }

    void PlayerMissed()
    {
        if (!extraBall)
        {
            GameOver();
        }
        else
        {
            extraBall = false;
            extraBallSprite.SetActive(false);
            MoveToNextLvl();
        }
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
    }

    void SetPageState(pageState page)
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

                break;

            case pageState.StartPage:
                currentPageState = pageState.StartPage;
                GamePage.SetActive(false);
                StartPage.SetActive(true);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                break;

            case pageState.GameOver:
                currentPageState = pageState.GameOver;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
                GameOverPage.SetActive(true);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                break;

            case pageState.Paused:
                currentPageState = pageState.Paused;
                GamePage.SetActive(true);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(true);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);

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
                break;
        }
    }

    public void StartGame()
    {
        extraBall = false;
        richochetCount = 0;
        score = 0;
        scoreText.text = score.ToString();
        SetPageState(pageState.Game);
        Paddle.gameObject.SetActive(true);
        extraBallSprite.SetActive(false);
        GameStarted();
    }

    public void GameOver()
    {
        Paddle.gameObject.SetActive(false);
        SetPageState(pageState.GameOver);
    }

    public void ResumeGame()
    {
        SetPageState(pageState.CountdownPage);
        StartCoroutine("Countdown");
        Paddle.gameObject.SetActive(true);
    }

    public void GoToSettings()
    {
        SetPageState(pageState.SettingsPage);
        GoToSettingsPage();
    }

    public void PauseGame()
    {
        SetPageState(pageState.Paused);
        Time.timeScale = 0;
        StopCoroutine("Countdown"); // whenever trying to stop a coroutine make sure you start and stop it with its string name. This way all coroutines running with that name stop instead of the specific one that was started.
        Paddle.gameObject.SetActive(false);
    }

    public void GoToStartPage()
    {
        SetPageState(pageState.StartPage);
        GameOverConfirmed();
    }

    public void ComeBackFromSettings()
    {
        SetPageState(pageState.StartPage);
        ComeBackFromSettingsPage();
    }

    public int GetScore
    {
        get
        {
            return score;
        }
    }
}
