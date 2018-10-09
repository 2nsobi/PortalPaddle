using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class AchievementsAndLeaderboards : MonoBehaviour
{
    public static AchievementsAndLeaderboards Instance;

    string currentPlatform;

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

        currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "apple" : "android";
    }

    private void Start()
    {
        PlayGamesClientConfiguration playGamesConfig = new PlayGamesClientConfiguration.Builder().Build();

        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.InitializeInstance(playGamesConfig);
        PlayGamesPlatform.Activate();

        GoogleSignIn();
    }

    public void GoogleSignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
            Social.localUser.Authenticate(SignInCallback);
        }
        else
        {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();
        }
    }

    public void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("(Lollygagger) Signed in!");
        }
        else
        {
            Debug.Log("(Lollygagger) Sign-in failed...");
        }
    }

    #region Achievements
    public void UnlockAchievement(string ID)
    {
        Social.ReportProgress(ID, 100, (bool success) => { if(success) PlayGamesPlatform.Instance.Events.IncrementEvent(GPGSIds.event_passed_moon, 1); });
    }

    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Cannot show Achievements, not logged in");
            GoogleSignIn();
        }
    }
    #endregion

    #region Leaderboards
    public void AddScore2LeaderBoard(string leaderboardID, long score)
    {
        Social.ReportScore(score, leaderboardID, success => { });
    }

    public void ShowLeaderboards()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.Log("Cannot show leaderboard: not authenticated");
            GoogleSignIn();
        }
    }
    #endregion
}
