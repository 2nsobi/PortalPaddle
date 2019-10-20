<<<<<<< HEAD
using System.Collections;
using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android {
	public class AppodealNonSkippableVideoCallbacks
#if UNITY_ANDROID
		: AndroidJavaProxy {
			INonSkippableVideoAdListener listener;

			internal AppodealNonSkippableVideoCallbacks (INonSkippableVideoAdListener listener) : base ("com.appodeal.ads.NonSkippableVideoCallbacks") {
				this.listener = listener;
			}

			void onNonSkippableVideoLoaded (bool isPrecache) {
				listener.onNonSkippableVideoLoaded (isPrecache);
			}

			void onNonSkippableVideoFailedToLoad () {
				listener.onNonSkippableVideoFailedToLoad ();
			}

			void onNonSkippableVideoShown () {
				listener.onNonSkippableVideoShown ();
			}

			void onNonSkippableVideoFinished () {
				listener.onNonSkippableVideoFinished ();
			}

			void onNonSkippableVideoClosed (bool finished) {
				listener.onNonSkippableVideoClosed (finished);
			}

			void onNonSkippableVideoExpired () {
				listener.onNonSkippableVideoExpired ();
			}
		}
#else
	{
		public AppodealNonSkippableVideoCallbacks (INonSkippableVideoAdListener listener) { }
	}
#endif
}
=======
using UnityEngine;
using System.Collections;
using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Android 
{
	public class AppodealNonSkippableVideoCallbacks 
#if UNITY_ANDROID
		: AndroidJavaProxy
	{
		INonSkippableVideoAdListener listener;
		
		internal AppodealNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener) : base("com.appodeal.ads.NonSkippableVideoCallbacks") {
			this.listener = listener;
		}
		
        void onNonSkippableVideoLoaded(bool isPrecache) {
            listener.onNonSkippableVideoLoaded(isPrecache);
		}
		
		void onNonSkippableVideoFailedToLoad() {
			listener.onNonSkippableVideoFailedToLoad();
		}
		
		void onNonSkippableVideoShown() {
			listener.onNonSkippableVideoShown();
		}
		
		void onNonSkippableVideoFinished() {
			listener.onNonSkippableVideoFinished();
		}
		
		void onNonSkippableVideoClosed(bool finished) {
			listener.onNonSkippableVideoClosed(finished);
		}

        void onNonSkippableVideoExpired(){
            listener.onNonSkippableVideoExpired();
        }
	}
#else
	{
		public AppodealNonSkippableVideoCallbacks(INonSkippableVideoAdListener listener) { }
	}
#endif
}


>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
