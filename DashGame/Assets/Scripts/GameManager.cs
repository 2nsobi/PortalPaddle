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
                leftEnd = Instantiate(pref.transform.GetChild(0).gameObject);
                leftEnd.SetActive(false);
            }
        }
    }

    public class BallPrefab
    {
        public GameObject prefab; // the children of the ball prefab should be ordered as follows: ballsprite, collisioneffect, falleffect, firstimpact, trail
        public GameObject ballSprite;
        public GameObject collisionEffect;
        public GameObject fallEffect;
        public GameObject firstImpact;
        public GameObject boostEffect;
        public Animator animator;
        public Color32 startColor;
        public Color32 boostColor;
        public int index;

        public BallPrefab(PrefabWithColors pref)
        {
            prefab = Instantiate(pref.prefab);
            prefab.SetActive(false);
            prefab.transform.localPosition = Vector2.zero;

            ballSprite = prefab.transform.GetChild(0).gameObject;
            collisionEffect = prefab.transform.GetChild(1).gameObject;
            fallEffect = prefab.transform.GetChild(2).gameObject;
            firstImpact = prefab.transform.GetChild(3).gameObject;
            try
            {
                boostEffect = prefab.transform.Find("BoostEffect").gameObject;
            }
            catch (System.NullReferenceException)
            {
            }

            startColor = pref.startColor;
            boostColor = pref.boostColor;
            animator = prefab.GetComponent<Animator>();
        }
    }

    [System.Serializable]
    public class PrefabWithColors
    {
        public GameObject prefab;
        public Color32 startColor; // start color should usually be this slightly grayish white : EAEAEA
        public Color32 boostColor;
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
    public GameObject[] paddlePrefabs;
    PaddlePrefab[] paddles;
    PaddlePrefab paddleInUse;
    public PrefabWithColors[] ballPrefabs;
    BallPrefab[] balls;
    BallPrefab ballInUse;

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
    public GameObject ShopPage;

    public enum pageState { Game, StartPage, GameOver, Paused, CountdownPage, SettingsPage, ScoreReview, ShopPage};
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
        /********************************************
         DELETE THIS
         **********************************************/

        PlayerPrefs.DeleteAll();

        /********************************************
        DELETE THIS
        **********************************************/


        Instance = this;
        Time.timeScale = timeScale;
    }

    private void Start()
    {
        extraBall = false;
        Paddle = PaddleController.Instance;
        TargetController = TargetController.Instance;
        LG = LevelGenerator.Instance;
        ball = BallController.Instance;

        gems = PlayerPrefs.GetInt("gems");
        scoreReviewGems = ScoreReview.transform.Find("gems").GetComponent<Text>();
        scoreReviewGems.text = gems.ToString();
        startPageGems = StartPage.transform.Find("gems").GetComponent<Text>();
        startPageGems.text = gems.ToString();

        paddles = new PaddlePrefab[paddlePrefabs.Length];
        for (int i = 0; i < paddlePrefabs.Length; i++)
        {
            paddles[i] = new PaddlePrefab(paddlePrefabs[i]);
            paddles[i].index = i;
        }
        paddleInUse = paddles[1];
        Paddle.SetPaddle(paddleInUse);
        DeactivatePaddle();

        balls = new BallPrefab[ballPrefabs.Length];
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            balls[i] = new BallPrefab(ballPrefabs[i]);
            balls[i].index = i;
        }
        ballInUse = balls[1];
        ball.SetBall(ballInUse);

        GoToStartPage();
        gameRunning = false;
        paused = false;
    }

    private void OnEnable() //this is called after start()
    {
        BallController.PlayerMissed += PlayerMissed;
        TargetController.TargetHit += TargetHit;
        TargetController.TargetHitAndRichochet += TargetHitAndRichochet;
    }

    private void OnDisable()
    {
        BallController.PlayerMissed -= PlayerMissed;
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
        for (float i = 0.0f; i < 0.6f; i += 0.1f)
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
                ShopPage.SetActive(false);
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
                ShopPage.SetActive(false);

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
                ShopPage.SetActive(false);
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
                ShopPage.SetActive(false);

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
                ShopPage.SetActive(false);

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
                ShopPage.SetActive(false);
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
                ShopPage.SetActive(false);
                break;

            case pageState.ShopPage:
                currentPageState = pageState.ShopPage;
                GamePage.SetActive(false);
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                PauseMenu.SetActive(false);
                CountdownPage.SetActive(false);
                SettingsPage.SetActive(false);
                ScoreReview.SetActive(false);
                ShopPage.SetActive(true);

                LG.shop.SetActive(true);
                break;
        }
    }

    private void Update()
    {
        if (gemsOnScreen)
        {
            t += 0.1f * Time.deltaTime;
            gems = Mathf.Lerp(gems, newGems, t);
            if (gems == newGems)
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
        extraBallSprite.SetActive(false);
        gameRunning = true;
        StartCoroutine("GameErrorTest");
        replayButton.interactable = true;
        Paddle.gameObject.SetActive(true);

        GameStarted();
    }


    public void GameOver()
    {
        StopCoroutine("GameErrorTest");
        gameRunning = false;
        DeactivatePaddle();
        SetPageState(pageState.GameOver);
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
        LG.GoToSettingsPage();
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
        gems = PlayerPrefs.GetInt("gems");
        newGems = (int)gems + score;
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

    public PaddlePrefab PaddleInUse
    {
        get
        {
            return paddleInUse;
        }
    }

    public void DeactivatePaddle()
    {
        Paddle.DeactivatePaddle();
        Paddle.gameObject.SetActive(false);
    }
}
