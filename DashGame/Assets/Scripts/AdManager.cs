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
    string appKey;
    bool bannerActive = false;
    int activeRewardAmount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        game = GameManager.Instance;

        appKey = "23409dd0a45bf3a469ebc0ce6f629cc799bc6485b135934f"; //this is the app key appodeal gives each app you make
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

        Appodeal.setTesting(true);
        Appodeal.setLogLevel(Appodeal.LogLevel.Debug);

        Appodeal.setAutoCache(Appodeal.INTERSTITIAL, true);
        Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
        Appodeal.setAutoCache(Appodeal.BANNER, true);

        if (ZPlayerPrefs.GetInt("result_gdpr") != 0)
        {
            Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, ZPlayerPrefs.GetInt("result_gdpr_sdk") == 1);
            Appodeal.setRewardedVideoCallbacks(this);
            Appodeal.setBannerCallbacks(this);

            Appodeal.show(Appodeal.BANNER_BOTTOM);

            showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
            showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());
        }
    }

    public void InitializeAds()
    {
        Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, ZPlayerPrefs.GetInt("result_gdpr_sdk") == 1);
        Appodeal.setRewardedVideoCallbacks(this);
        Appodeal.setBannerCallbacks(this);

        Appodeal.show(Appodeal.BANNER_BOTTOM);

        showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
        showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());
    }

    IEnumerator CanShowRewardVidDelay()
    {
        canShowRewardVid = false;
        yield return new WaitForSecondsRealtime(420);
        canShowRewardVid = true;
    }

    IEnumerator CanShowInterstitialDelay()
    {
        canShowInterstitial = false;
        yield return new WaitForSecondsRealtime(90);
        canShowInterstitial = true;
    }

    public void ShowInterstitialOrNonSkipAd()
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

    void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
            if (giveReward)
            {
                game.UpdateGems(activeRewardAmount);
                giveReward = false;
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
                Appodeal.show(Appodeal.REWARDED_VIDEO, "noReward"); //reward1 is name of your placemnt on appodeal
            }
        }
    }

    public void onRewardedVideoLoaded(bool precache) { }

    public void onRewardedVideoFailedToLoad() { }

    public void onRewardedVideoShown() { }

    public void onRewardedVideoClosed(bool finished) { }

    public void onRewardedVideoExpired() { }

    public void onRewardedVideoFinished(double amount, string name)
    {
        if(amount > 0)
        {
            giveReward = true;
            activeRewardAmount = (int) amount;
        }
    }

    public void onBannerLoaded(bool isPrecache)
    {
        if (!bannerActive)
        {
            Appodeal.show(Appodeal.BANNER_BOTTOM);
        }
    }

    public void onBannerFailedToLoad() { }

    public void onBannerShown()
    {
        bannerActive = true;
    }

    public void onBannerClicked() { }

    public void onBannerExpired()
    {
        bannerActive = false;
    }
}
