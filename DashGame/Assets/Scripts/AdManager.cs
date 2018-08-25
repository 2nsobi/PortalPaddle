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
    bool giveReward;
    bool paused = false;
    bool outOfFocus = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        game = GameManager.Instance;

        string appKey = "23409dd0a45bf3a469ebc0ce6f629cc799bc6485b135934f"; //this is the app key appodeal gives each app you make
        Appodeal.disableLocationPermissionCheck();

        Appodeal.setTesting(true);

        Appodeal.setAutoCache(Appodeal.INTERSTITIAL, true);
        Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
        Appodeal.setAutoCache(Appodeal.BANNER, true);
        
        Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO, ZPlayerPrefs.GetInt("result_gdpr_sdk") == 1);
        Appodeal.setRewardedVideoCallbacks(this);
        Appodeal.setBannerCallbacks(this);

        if (Appodeal.isLoaded(Appodeal.BANNER))
        {
            Appodeal.show(Appodeal.BANNER_BOTTOM);
        }
        else if (Appodeal.isPrecache(Appodeal.BANNER))
        {
            Appodeal.show(Appodeal.BANNER_BOTTOM);
        }

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
            giveReward = false;
            Appodeal.show(Appodeal.REWARDED_VIDEO);

            showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
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

    public void ShowRewardVideo(bool givereward = true)
    {
        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
        {
            if (givereward)
            {
                giveReward = true;
            }
            else
            {
                giveReward = false;
            }
            Appodeal.show(Appodeal.REWARDED_VIDEO);
        }
    }

    public void onRewardedVideoLoaded(bool precache) { }

    public void onRewardedVideoFailedToLoad() { }

    public void onRewardedVideoShown() { }

    public void onRewardedVideoClosed(bool finished) { }

    public void onRewardedVideoExpired() { }

    public void onRewardedVideoFinished(double amount, string name)
    {
        if (giveReward)
        {
            game.UpdateGems(500);
        }
    }

    public void onBannerLoaded(bool isPrecache)
    {
        Appodeal.show(Appodeal.BANNER_BOTTOM);
    }

    public void onBannerFailedToLoad() { }

    public void onBannerShown() { }

    public void onBannerClicked() { }  

    public void onBannerExpired() { }
}
