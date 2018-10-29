using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class AchievementsAndLeaderboards : MonoBehaviour
{
    public static AchievementsAndLeaderboards Instance;

    int currentPlatform;

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

        currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? 2 : 1; // 2 is for apple and 1 is for android 
    }

    private void Start()
    {
        if (currentPlatform == 1)
        {
            PlayGamesClientConfiguration playGamesConfig = new PlayGamesClientConfiguration.Builder().Build();

            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.InitializeInstance(playGamesConfig);
            PlayGamesPlatform.Activate();

            PlayGamesPlatform.Instance.Authenticate(FirstStartSignInCallback, false);
        }
    }

    public void GoogleSignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
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

    public void FirstStartSignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("(Lollygagger) Signed in!");
        }
        else
        {
            Debug.Log("(Lollygagger) Sign-in failed...trying again");
            GoogleSignIn();
        }
    }

    #region Achievements
    public void UnlockAchievement(string ID)
    {
        if (Social.localUser.authenticated)
        {
            if (currentPlatform == 1)
            {
                PlayGamesPlatform.Instance.ReportProgress(ID, 100.0f, (bool success) =>
                {
                    if (success && ID == GPGSIds.achievement_lunar_king)
                    {
                        PlayGamesPlatform.Instance.Events.IncrementEvent(GPGSIds.event_passed_moon, 1);
                    }
                });
            }
        }
    }

    public void ShowAchievements()
    {
        if (currentPlatform == 1)
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
    }
    #endregion

    #region Leaderboards
    public void AddScore2LeaderBoard(string leaderboardID, long score)
    {
        if (Social.localUser.authenticated)
        {
            if (currentPlatform == 1)
            {
                // Note: make sure to add 'using GooglePlayGames'
                PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, (bool success) =>
                {
                    Debug.Log(leaderboardID + ", (Lollygagger) Leaderboard update success: " + success);
                });
            }
        }
    }

    public void ShowLeaderboards()
    {
        if (currentPlatform == 1)
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
    }
    #endregion
}
