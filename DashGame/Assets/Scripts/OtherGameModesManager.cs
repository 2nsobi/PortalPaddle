using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherGameModesManager : MonoBehaviour {

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

    GameManager game;
    ObstacleSpawner obSpawner;
    PaddleController Paddle;
    BallController ballC;

    bool gemsOnScreen = false;
    Coroutine pauseCoroutine;
    bool GameModeRunning = false;
    bool pauseAllCoroutines = false;
    bool paused = false;

    public delegate void OtherGameModesManagerDelegate();
    public static event OtherGameModesManagerDelegate StartPlusOne;
    public static event OtherGameModesManagerDelegate StartDeadeye;
    public static event OtherGameModesManagerDelegate StartClairvoyance;

    private void Awake()
    {
        Instance = this;

        SetPageState(pageState.StartPage);
    }

    private void Start()
    {
        obSpawner = ObstacleSpawner.Instance;
        Paddle = PaddleController.Instance;
        game = GameManager.Instance;
        ballC = BallController.Instance;
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
                break;

            case gameMode.Deadeye:
                currentGameMode = gameMode.Deadeye;

                obSpawner.SetGameMode(gameMode.Deadeye);
                break;

            case gameMode.Clairvoyance:
                currentGameMode = gameMode.Clairvoyance;

                obSpawner.SetGameMode(gameMode.Clairvoyance);
                break;

            case gameMode.None:
                currentGameMode = gameMode.None;

                obSpawner.SetGameMode(gameMode.None);
                break;
        }
    }

    public void Go2PlusOne()
    {
        SetGameMode(gameMode.PlusOne);
        ballC.Fade2GameMode();
        SetGameModeSelectButtons(false);
    }

    public void Go2Deadeye()
    {
        SetGameMode(gameMode.Deadeye);
        ballC.Fade2GameMode();
        SetGameModeSelectButtons(false);
    }

    public void Go2Clairavoyance()
    {
        SetGameMode(gameMode.Clairvoyance);
        ballC.Fade2GameMode();
        SetGameModeSelectButtons(false);
    }

    public void GoBack2ModeSelect()
    {
        SetGameMode(gameMode.None);
        ballC.Fade2GameMode();
        SetGameModeSelectButtons(true);
    }

    public void StartOtherGameMode()
    {
        Time.timeScale = 0;
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
        Paddle.gameObject.SetActive(true);
        Time.timeScale = 1;
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

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    public void DeactivatePaddle()
    {
        Paddle.DeactivatePaddle();
        Paddle.gameObject.SetActive(false);
    }

}
