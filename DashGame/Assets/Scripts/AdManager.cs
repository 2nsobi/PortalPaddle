using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System.Collections;

public class AdManager : MonoBehaviour, IRewardedVideoAdListener, IBannerAdListener
{
    public static AdManager Instance;
    int attempts2ShowInterstitial = 0;
    bool canShowRewardVid = false;
    bool canShowInterstitial = false;
    Coroutine showRewardVidDelay;
    Coroutine showInterstitialDelay;
    GameManager game;
    bool giveReward = false;
    bool revive = false;
    bool justEndGame = false;
    string appKey = "23409dd0a45bf3a469ebc0ce6f629cc799bc6485b135934f"; //this is the app key appodeal gives each app you make
    int activeRewardAmount = 0;
    bool noAds = false;
    bool initialized = false;

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

    private void Start()
    {
        game = GameManager.Instance;

        Appodeal.disableLocationPermissionCheck();

        if (Application.platform == RuntimePlatform.Android)
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                int SDKLvl = version.GetStatic<int>("SDK_INT");

                if (SDKLvl > 18)
                {
                    Appodeal.disableWriteExternalStoragePermissionCheck();
                }
            }
        }

        //Appodeal.setTesting(true);
        //Appodeal.setLogLevel(Appodeal.LogLevel.Debug);

        noAds = PlayerPrefsX.GetBool("noAds");
    }

    //Ads are initialized from the GDPRConsent script
    public void InitializeAds(bool GDPRCompliance)
    {
        if (!initialized)
        {
            print("Personalized ads are " + (GDPRCompliance ? "enabled" : "disabled"));
            if (!noAds)
            {
                Appodeal.setAutoCache(Appodeal.INTERSTITIAL, true);
                Appodeal.setAutoCache(Appodeal.BANNER, true);
                Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);

                Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, GDPRCompliance);
                Appodeal.setRewardedVideoCallbacks(this);
                Appodeal.setBannerCallbacks(this);

                Appodeal.show(Appodeal.BANNER_BOTTOM);
                showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
                showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());
            }
            else
            {
                Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
                Appodeal.initialize(appKey, Appodeal.REWARDED_VIDEO, GDPRCompliance);
                Appodeal.setRewardedVideoCallbacks(this);
            }

            initialized = true;
        }
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

                if ((Appodeal.isPrecache(Appodeal.INTERSTITIAL) && attempts2ShowInterstitial >= 3) || (Appodeal.isPrecache(Appodeal.INTERSTITIAL) && canShowInterstitial))
                {
                    Appodeal.show(Appodeal.INTERSTITIAL);

                    StopCoroutine(showInterstitialDelay);
                    showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());

                    attempts2ShowInterstitial = 0;
                }
                else if ((Appodeal.isLoaded(Appodeal.INTERSTITIAL) && attempts2ShowInterstitial >= 3) || (Appodeal.isLoaded(Appodeal.INTERSTITIAL) && canShowInterstitial))
                {
                    Appodeal.show(Appodeal.INTERSTITIAL);

                    StopCoroutine(showInterstitialDelay);
                    showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());

                    attempts2ShowInterstitial = 0;
                }
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Appodeal.onResume();
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (giveReward)
            {
                game.UpdateGems(activeRewardAmount);

                giveReward = false;
                activeRewardAmount = 0;
            }
            if (revive)
            {
                revive = false;
                game.Continue();
            }
            if (justEndGame)
            {
                justEndGame = false;
                game.GoToScoreReview();
            }
        }
    }

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

    public void onRewardedVideoLoaded(bool precache) { }

    public void onRewardedVideoFailedToLoad()
    {
        if (game.IsGameRunning)
        {
            justEndGame = true;
        }
    }

    public void onRewardedVideoShown() { }

    public void onRewardedVideoClosed(bool finished) { }

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

    public void onBannerLoaded(bool isPrecache) { }

    public void onBannerFailedToLoad() { }

    public void onBannerShown() { }

    public void onBannerClicked() { }

    public void onBannerExpired() { }

    public void RemoveAds()
    {
        noAds = true;

        PlayerPrefsX.SetBool("noAds", true);

        Appodeal.hide(Appodeal.BANNER);
    }
}