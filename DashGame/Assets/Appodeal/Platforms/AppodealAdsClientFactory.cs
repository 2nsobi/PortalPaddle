<<<<<<< HEAD
﻿using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity {
	internal class AppodealAdsClientFactory {
		internal static IAppodealAdsClient GetAppodealAdsClient () {
#if UNITY_ANDROID && !UNITY_EDITOR
			return new AppodealAds.Unity.Android.AndroidAppodealClient ();
#elif UNITY_IPHONE && !UNITY_EDITOR
			return AppodealAds.Unity.iOS.AppodealAdsClient.Instance;
#else
			return new AppodealAds.Unity.Dummy.DummyClient ();
#endif
=======
﻿using UnityEngine;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity
{
	internal class AppodealAdsClientFactory
	{
		internal static IAppodealAdsClient GetAppodealAdsClient()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			return new AppodealAds.Unity.Android.AndroidAppodealClient();
			#elif UNITY_IPHONE && !UNITY_EDITOR
			return AppodealAds.Unity.iOS.AppodealAdsClient.Instance;
			#else
			return new AppodealAds.Unity.Dummy.DummyClient();
			#endif
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
		}
	}
}