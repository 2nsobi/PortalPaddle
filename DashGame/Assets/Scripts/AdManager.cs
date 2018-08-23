using UnityEngine;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;
using GoogleMobileAds.Api;
using System.Collections;

public class AdManager : MonoBehaviour//, IRewardedVideoAdListener
{
    public static AdManager Instance;
    BannerView bannerView;
    string AndroidAdUnitId = "ca-app-pub-8748712167919890/3853099194"; // test ads ID is "ca-app-pub-3940256099942544/6300978111"
    int attempts2ShowInterstitial = 0;
    bool canShowRewardVid = false;
    bool canShowInterstitial = true;
    Coroutine showRewardVidDelay;
    Coroutine showInterstitialDelay;
    GameManager game;
    bool giveReward;

    private void Awake()
    {
        Instance = this;

        //Appodeal.setRewardedVideoCallbacks(this);
    }

    private void Start()
    {
        game = GameManager.Instance;

        /*******************************************************************************************************************************
         ADMOB
         ******************************************************************************************************************************/
#if UNITY_ANDROID
        string adUnitId = AndroidAdUnitId;
#elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_AD_BANNER_ID_HERE";
#else
            string adUnitId = "unexpected_platform";
#endif

        MobileAds.Initialize(adUnitId);

        this.RequestBanner();

        ///*******************************************************************************************************************************
        // APPODEAL
        // ******************************************************************************************************************************/

        //string appKey = "0981009c5520cd8c7c2152ffa0efb067798bf54f47fbdb7e"; //this is the app key appodeal gives each app you make
        //Appodeal.disableLocationPermissionCheck();
        //Appodeal.setTesting(true);
        //Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO);

        //showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
    }

    // ADMOB methods ---------------------------------------------------------------------------------
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = AndroidAdUnitId;
#elif UNITY_IPHONE
            string adUnitId = "INSERT_IOS_AD_BANNER_ID_HERE";
#else
            string adUnitId = "unexpected_platform";
#endif

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // for ad testing on test device
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice("D644E73D41BC75D3CCFAC969A9755E98") //  <---------- test device id
            .Build();

        //// Create an empty ad request.
        //AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    ////APPODEAL methods ------------------------------------------------------------------------------------

    //IEnumerator CanShowRewardVidDelay()
    //{
    //    canShowRewardVid = false;
    //    yield return new WaitForSecondsRealtime(420);
    //    canShowRewardVid = true;
    //}

    //IEnumerator CanShowInterstitialDelay()
    //{
    //    canShowInterstitial = false;
    //    yield return new WaitForSecondsRealtime(90);
    //    canShowInterstitial = true;
    //}

    //public void ShowInterstitialOrNonSkipAd()
    //{
    //    if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO) && canShowRewardVid)
    //    {
    //        giveReward = false;
    //        Appodeal.show(Appodeal.REWARDED_VIDEO);

    //        StopCoroutine(showRewardVidDelay);
    //        showRewardVidDelay = StartCoroutine(CanShowRewardVidDelay());
    //    }
    //    else
    //    {
    //        attempts2ShowInterstitial++;

    //        if ((Appodeal.isLoaded(Appodeal.INTERSTITIAL) && attempts2ShowInterstitial >= 3) || (Appodeal.isLoaded(Appodeal.INTERSTITIAL) && canShowInterstitial))
    //        {
    //            Appodeal.show(Appodeal.INTERSTITIAL);

    //            if(showInterstitialDelay != null)
    //            {
    //                StopCoroutine(showInterstitialDelay);
    //            }
    //            showInterstitialDelay = StartCoroutine(CanShowInterstitialDelay());
    //        }
    //    }
    //}

    //public void ShowRewardVideo(bool givereward = true)
    //{
    //    if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
    //    {
    //        if (giveReward)
    //        {
    //            giveReward = true;
    //        }
    //        else
    //        {
    //            giveReward = false;
    //        }
    //        Appodeal.show(Appodeal.REWARDED_VIDEO);
    //    }
    //}

    //public void onRewardedVideoLoaded() { }

    //public void onRewardedVideoFailedToLoad() { }

    //public void onRewardedVideoShown() { }

    //public void onRewardedVideoClosed(bool finished) { }

    //public void onRewardedVideoFinished(int amount, string name)
    //{
    //    if (giveReward)
    //    {
    //        game.UpdateGems(50);
    //    }
    //}
}
