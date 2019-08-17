using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Ratings : MonoBehaviour
{
    public static Ratings Instance;

    public GameObject Reward1;
    public GameObject Reward2;
    public GameObject Reward3;
    public GameObject Reward4;
    public GameObject Reward5;

    GameObject RateMePage;
    GameObject DailyRewardPage;

    float timeSinceLastAsk = 0; // time since Ask4Rate() was called
    float timeOfLastAsk; //time since the rating panel popped up on screen
                         //static string currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "apple" : "android";

    string lastDateTimeString;
    System.DateTime lastDateTime;
    System.DateTime currentDate;
    int daysPlayed; //max of 5
    string dateOriginalVal;
    int daysOriginalVal;
    Coroutine tryDailyReward;
    Coroutine terminateDailyRewardPage;
    bool rewardPlayer = false;
    int timesAsked = 0; //this is pretty much equal to the amount of times a player has visited the score review screen in the main game mode
    bool been24Hours = false;

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

        DontDestroyOnLoad(gameObject);

        RateMePage = transform.GetChild(0).gameObject;
        DailyRewardPage = transform.GetChild(1).gameObject;
    }

    private void Start()
    {
        timeOfLastAsk = 0;

        lastDateTimeString = ZPlayerPrefs.GetString("lastDateTime", "notPlayed"); //saved as year,month,day,hour,minute,second
        dateOriginalVal = lastDateTimeString;

        daysPlayed = ZPlayerPrefs.GetInt("daysPlayed", 0);
        daysOriginalVal = daysPlayed;

        tryDailyReward = StartCoroutine(TryDailyRewardC());
    }

    public void Ask4Rate() //used from othergamemode manager
    {
        timeSinceLastAsk = Time.realtimeSinceStartup - timeOfLastAsk;

        if (timeSinceLastAsk / 420 >= 1)
        {
            timeOfLastAsk = Time.realtimeSinceStartup;

            RateMePage.SetActive(true);
        }
    }

    public void Ask4RateOrGetDailyReward() //used from gamemanager
    {
        timesAsked++;

        if (timesAsked == 1)
        {
            RewardPlayer();
            return;
        }

        timeSinceLastAsk = Time.realtimeSinceStartup - timeOfLastAsk;

        if (timeSinceLastAsk / 420 >= 1)
        {
            timeOfLastAsk = Time.realtimeSinceStartup;

            RateMePage.SetActive(true);
        }
    }

    public void Go2Rate()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.nnaji.PortalPaddle");
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/id1444939274");
#endif   
    }

    public void ExitRateMePage()
    {
        RateMePage.SetActive(false);
    }

    IEnumerator TryDailyRewardC()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://us-central1-simplyconnectedgames-website.cloudfunctions.net/getTime");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string[] dateFromServer = www.downloadHandler.text.Split(new char[] { '-', '/', ':' }); //date from server looks like this: 12-04-2018/21:39:59 (month-day-year/hour:min:sec)

            currentDate = new System.DateTime(int.Parse(dateFromServer[2]), int.Parse(dateFromServer[0]), int.Parse(dateFromServer[1]), int.Parse(dateFromServer[3]),
                 int.Parse(dateFromServer[4]), int.Parse(dateFromServer[5]));

            float interval = 0;
            if (lastDateTimeString != "notPlayed")
            {
                string[] lastDateTimeSplit = lastDateTimeString.Split(new char[] { ',' });

                lastDateTime = new System.DateTime(int.Parse(lastDateTimeSplit[0]), int.Parse(lastDateTimeSplit[1]), int.Parse(lastDateTimeSplit[2]), int.Parse(lastDateTimeSplit[3]),
                     int.Parse(lastDateTimeSplit[4]), int.Parse(lastDateTimeSplit[5]));

                interval = (float)currentDate.Subtract(lastDateTime).TotalSeconds;
            }

            if (daysPlayed > 4)
            {
                if (interval >= 86400)
                {
                    daysPlayed = 0;
                    rewardPlayer = true;
                }
            }
            else
            {
                if (interval >= 86400 || daysPlayed == 0)
                {
                    rewardPlayer = true;
                }
            }
        }
    }

    public void RewardPlayer()
    {
        // Gems are actually updated in TerminateDailyRewardPage coroutine for aesthetics tings
        if (rewardPlayer)
        {
            rewardPlayer = false;
            switch (daysPlayed)
            {
                case 0:
                    DailyRewardPage.SetActive(true);
                    Reward1.SetActive(true);
                    terminateDailyRewardPage = StartCoroutine(TerminateDailyRewardPage(50));
                    break;
                case 1:
                    DailyRewardPage.SetActive(true);
                    Reward2.SetActive(true);
                    terminateDailyRewardPage = StartCoroutine(TerminateDailyRewardPage(100));
                    break;
                case 2:
                    DailyRewardPage.SetActive(true);
                    Reward3.SetActive(true);
                    terminateDailyRewardPage = StartCoroutine(TerminateDailyRewardPage(150));
                    break;
                case 3:
                    DailyRewardPage.SetActive(true);
                    Reward4.SetActive(true);
                    terminateDailyRewardPage = StartCoroutine(TerminateDailyRewardPage(200));
                    break;
                case 4:
                    DailyRewardPage.SetActive(true);
                    Reward5.SetActive(true);
                    terminateDailyRewardPage = StartCoroutine(TerminateDailyRewardPage(250));
                    break;
            }
        }
    }

    IEnumerator TerminateDailyRewardPage(int coins2Add)
    {
        yield return new WaitForSeconds(4.167f);

        AudioManager.Instance.PlayUISound("unlockItem");

        GameManager.Instance.UpdateGemsVisuallyAlso(coins2Add);

        daysPlayed++;
        lastDateTimeString = (currentDate.Year + "," + currentDate.Month + "," + currentDate.Day + "," + currentDate.Hour + "," + currentDate.Minute + "," + currentDate.Second);

        yield return new WaitForSeconds(0.5833f);

        DailyRewardPage.SetActive(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (lastDateTimeString != dateOriginalVal)
                ZPlayerPrefs.SetString("lastDateTime", lastDateTimeString);

            if (daysPlayed != daysOriginalVal)
                ZPlayerPrefs.SetInt("daysPlayed", daysPlayed);
        }
    }

    //private void OnApplicationQuit()
    //{
    //    if (lastDateTimeString != dateOriginalVal)
    //        ZPlayerPrefs.SetString("lastDateTime", lastDateTimeString);

    //    if (daysPlayed != daysOriginalVal)
    //        ZPlayerPrefs.SetInt("daysPlayed", daysPlayed);
    //}
}
