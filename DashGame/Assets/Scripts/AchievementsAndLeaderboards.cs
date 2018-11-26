using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.SocialPlatforms;
#endif

public class AchievementsAndLeaderboards : MonoBehaviour
{
    public static AchievementsAndLeaderboards Instance;

    public enum achievements { interstellar, lunarKing };
    public enum leaderBoards { highScores, ultraHighScores };

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

        DontDestroyOnLoad(this.gameObject);

        currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? 2 : 1; // 2 is for apple and 1 is for android 
    }

    private void Start()
    {
#if UNITY_ANDROID
        if (currentPlatform == 1)
        {
            PlayGamesClientConfiguration playGamesConfig = new PlayGamesClientConfiguration.Builder().Build();

            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.InitializeInstance(playGamesConfig);
            PlayGamesPlatform.Activate();

            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
#endif

#if UNITY_IOS
        if (currentPlatform == 2)
        {
            // Authenticate and register a ProcessAuthentication callback
            // This call needs to be made before we can proceed to other calls in the Social API
            Social.localUser.Authenticate(ProcessAuthentication);
        }
#endif
    }

#if UNITY_IOS
    // This function gets called when Authenticate completes
    // Note that if the operation is successful, Social.localUser will contain data from the server. 
    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            Debug.Log("Authenticated, checking achievements");

            // Request loaded achievements, and register a callback for processing them
            Social.LoadAchievements(ProcessLoadedAchievements);
        }
        else
            Debug.Log("Failed to authenticate");
    }
#endif

#if UNITY_IOS
    // This function gets called when the LoadAchievement call completes
    void ProcessLoadedAchievements(IAchievement[] achievements)
    {
        if (achievements.Length == 0)
            Debug.Log("Error: no achievements found");
        else
            Debug.Log("Got " + achievements.Length + " achievements");

        // You can also call into the functions like this
        Social.ReportProgress("Achievement01", 100.0, result =>
        {
            if (result)
                Debug.Log("Successfully reported achievement progress");
            else
                Debug.Log("Failed to report achievement");
        });
    }
#endif

#if UNITY_ANDROID
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
#endif

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
    public void UnlockAchievement(achievements achievement)
    {
        string ID = GetAchievementID(achievement);

        if (Social.localUser.authenticated)
        {
#if UNITY_ANDROID
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
#endif

#if UNITY_IOS
            if(currentPlatform == 2)
            {
                Social.ReportProgress(ID, 100, (bool success) => { Debug.Log(ID + " achievement " + (success ? "unlocked successfully." : "failed to unlock")); });
            }
#endif
        }
    }

    string GetAchievementID(achievements achievement)
    {
        string ID = "";

        if (currentPlatform == 1) // for android achievements
        {
            switch (achievement)
            {
                case achievements.interstellar:
                    ID = GPGSIds.achievement_interstellar;
                    break;
                case achievements.lunarKing:
                    ID = GPGSIds.achievement_lunar_king;
                    break;
            }
        }
        else // for iphone achievements
        {
            switch (achievement)
            {
                case achievements.interstellar:
                    ID = "";
                    break;
                case achievements.lunarKing:
                    ID = "";
                    break;
            }
        }

        return ID;
    }

    public void ShowAchievements()
    {
#if UNITY_ANDROID
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
#endif

#if UNITY_IOS
        if(currentPlatform == 2)
        {
            if (Social.localUser.authenticated)
            {
                Social.ShowAchievementsUI();
            }
            else
            {
                Debug.Log("Cannot show Achievements, not logged in");
                Social.localUser.Authenticate(ProcessAuthentication);
            }
        }
#endif

    }
    #endregion

    #region Leaderboards
    public void AddScore2LeaderBoard(leaderBoards leaderBoard, long score)
    {
        string leaderboardID = GetLeaderBoardID(leaderBoard);

        if (Social.localUser.authenticated)
        {
#if UNITY_ANDROID
            if (currentPlatform == 1)
            {
                // Note: make sure to add 'using GooglePlayGames'
                PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, (bool success) =>
                {
                    Debug.Log(leaderboardID + ", (Lollygagger) Leaderboard update success: " + success);
                });
            }
#endif

#if UNITY_IOS
            if (currentPlatform == 2)
            {
                Social.ReportScore(score,leaderboardID, (bool success) =>
                {
                    Debug.Log(leaderboardID + ", (Lollygagger) Leaderboard update success: " + success);
                });
            }
#endif
        }
    }

    string GetLeaderBoardID(leaderBoards leaderBoard)
    {
        string ID = "";

        if (currentPlatform == 1)   //for android
        {
            switch (leaderBoard)
            {
                case leaderBoards.highScores:
                    ID = GPGSIds.leaderboard_high_scores;
                    break;
                case leaderBoards.ultraHighScores:
                    ID = GPGSIds.leaderboard_ultra_high_scores;
                    break;
            }
        }
        else   //for IOS
        {
            switch (leaderBoard)
            {
                case leaderBoards.highScores:
                    ID = "";
                    break;
                case leaderBoards.ultraHighScores:
                    ID = "";
                    break;
            }
        }

        return ID;
    }

    public void ShowLeaderboards()
    {
#if UNITY_ANDROID
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
#endif

#if UNITY_IOS
        if(currentPlatform == 2)
        {
            if (Social.localUser.authenticated)
            {
                Social.ShowLeaderboardUI();
            }
            else
            {
                Debug.Log("Cannot show LeaderBoards, not logged in");
                Social.localUser.Authenticate(ProcessAuthentication);
            }
        }
#endif
    }
    #endregion
}
