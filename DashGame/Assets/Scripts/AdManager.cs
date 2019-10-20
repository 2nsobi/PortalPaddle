using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System.Collections;

<<<<<<< HEAD
public class AdManager : MonoBehaviour
=======
public class AdManager : MonoBehaviour, IRewardedVideoAdListener
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
{
    [HideInInspector]
    public bool initialized = false;

    public static AdManager Instance;
    int attempts2ShowInterstitial = 0;
    bool canShowRewardVid = false;
    bool canShowInterstitial = false;
    Coroutine showRewardVidDelay;
    Coroutine showInterstitialDelay;
    bool giveReward = false;
    bool revive = false;
    int activeRewardAmount = 0;
    bool noAds = false;
    bool vidFinished = false;

#if UNITY_ANDROID
    string appKey = "23409dd0a45bf3a469ebc0ce6f629cc799bc6485b135934f"; // the app key from appodeal for the android app version
#endif
#if UNITY_IOS
    string appKey = "7772ed0a2fc4d93e847e97f1e73256434d8caa2a16a7c40a"; // the app key from appodeal for the IOS app version
#endif

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
    }

    //private void Start()
    //{
    //    //Appodeal.setTesting(true);
    //    //Appodeal.setLogLevel(Appodeal.LogLevel.Debug);
    //}

    //Ads are initialized from the GDPRConsent script
    public void InitializeAds(bool GDPRCompliance)
    {
        // print("Personalized ads are " + (GDPRCompliance ? "enabled" : "disabled"));
        noAds = PlayerPrefsX.GetBool("noAds");

        if (Application.platform == RuntimePlatform.Android)
        {
            Appodeal.disableLocationPermissionCheck();

            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int SDKLvl = version.GetStatic<int>("SDK_INT");

                if (SDKLvl > 18)
                {
                    Appodeal.disableWriteExternalStoragePermissionCheck();
                }
            }
        }

        if (!noAds)
        {
            Appodeal.setAutoCache(Appodeal.INTERSTITIAL, true);
            Appodeal.setAutoCache(Appodeal.BANNER, true);
            Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);

            Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, GDPRCompliance);
<<<<<<< HEAD
=======
            Appodeal.setRewardedVideoCallbacks(this);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

            Appodeal.show(Appodeal.BANNER_BOTTOM);
            showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
            showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());
        }
        else
        {
            Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
            Appodeal.initialize(appKey, Appodeal.REWARDED_VIDEO, GDPRCompliance);
<<<<<<< HEAD
=======
            Appodeal.setRewardedVideoCallbacks(this);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        }

        initialized = true;
    }

    IEnumerator CanShowRewardVidDelay()
    {
        canShowRewardVid = false;
        yield return new WaitForSecondsRealtime(600);
        canShowRewardVid = true;
    }

    IEnumerator CanShowInterstitialDelay()
    {
        canShowInterstitial = false;
        yield return new WaitForSecondsRealtime(75);
        canShowInterstitial = true;
    }

    public void ShowInterstitialOrNonSkipAd()
    {
        if (!noAds)
        {
            if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO) && canShowRewardVid)
            {
                Appodeal.show(Appodeal.REWARDED_VIDEO);

                showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());

                StopCoroutine(showInterstitialDelay);
                showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());
            }
            else
            {
                attempts2ShowInterstitial++;

                if ((Appodeal.isLoaded(Appodeal.INTERSTITIAL) && attempts2ShowInterstitial >= 3) || (Appodeal.isLoaded(Appodeal.INTERSTITIAL) && canShowInterstitial))
                {
                    Appodeal.show(Appodeal.INTERSTITIAL);

                    StopCoroutine(showInterstitialDelay);
                    showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());

                    attempts2ShowInterstitial = 0;
                }
            }
        }
    }

<<<<<<< HEAD
=======
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Appodeal.onResume();
        }
    }

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
    public void ShowRewardVideo(bool givereward = true)
    {
        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
        {
            if (givereward)
            {
                Appodeal.show(Appodeal.REWARDED_VIDEO, "reward1"); //reward1 is name of your placemnt on appodeal
            }
            else
            {
                Appodeal.show(Appodeal.REWARDED_VIDEO, "reviveReward"); //reviveReward is name of your placemnt on appodeal
            }
        }
    }

    private void Update()
    {
        if (vidFinished)
        {
            vidFinished = false;

            if (giveReward)
            {
                GameManager.Instance.UpdateGems(activeRewardAmount);

                giveReward = false;
                activeRewardAmount = 0;
            }
            if (revive)
            {
                revive = false;
                GameManager.Instance.Continue();
            }
        }
    }

    public void onRewardedVideoLoaded(bool precache) { }

    public void onRewardedVideoFailedToLoad() { }

    public void onRewardedVideoShown() { }

    public void onRewardedVideoClosed(bool finished)
    {
        vidFinished = true; //must use this method with update because the appodeal API runs on a different thread than the main thread
    }

    public void onRewardedVideoExpired() { }

    public void onRewardedVideoFinished(double amount, string name) // "name" argument is not the name of the placement on appodeal but instead the name of the currency for the reward
    {
        if (name == "reward1")
        {
            giveReward = true;
            activeRewardAmount = (int)amount;
        }
        else if (name == "reviveReward")
        {
            revive = true;
        }
    }

    public void RemoveAds()
    {
        noAds = true;

        PlayerPrefsX.SetBool("noAds", true);

        Appodeal.hide(Appodeal.BANNER);
    }
}