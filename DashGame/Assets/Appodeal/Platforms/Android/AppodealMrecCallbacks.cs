<<<<<<< HEAD
﻿using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android {
	public class AppodealMrecCallbacks
#if UNITY_ANDROID
		: AndroidJavaProxy {
			IMrecAdListener listener;

			internal AppodealMrecCallbacks (IMrecAdListener listener) : base ("com.appodeal.ads.MrecCallbacks") {
				this.listener = listener;
			}

			void onMrecLoaded (bool isPrecache) {
				listener.onMrecLoaded (isPrecache);
			}

			void onMrecFailedToLoad () {
				listener.onMrecFailedToLoad ();
			}

			void onMrecShown () {
				listener.onMrecShown ();
			}

			void onMrecClicked () {
				listener.onMrecClicked ();
			}

			void onMrecExpired () {
				listener.onMrecExpired ();
			}
		}
#else
	{
		public AppodealMrecCallbacks (IMrecAdListener listener) { }
	}
#endif
}
=======
﻿using UnityEngine;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Android
{
    public class AppodealMrecCallbacks
#if UNITY_ANDROID
        : AndroidJavaProxy
    {
        IMrecAdListener listener;

        internal AppodealMrecCallbacks(IMrecAdListener listener) : base("com.appodeal.ads.MrecCallbacks")
        {
            this.listener = listener;
        }

        void onMrecLoaded(bool isPrecache)
        {
            listener.onMrecLoaded(isPrecache);
        }

        void onMrecFailedToLoad()
        {
            listener.onMrecFailedToLoad();
        }

        void onMrecShown()
        {
            listener.onMrecShown();
        }

        void onMrecClicked()
        {
            listener.onMrecClicked();
        }

        void onMrecExpired(){
            listener.onMrecExpired();
        }
    }
#else
    {
        public AppodealMrecCallbacks(IMrecAdListener listener) { }
    }
#endif
}
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
