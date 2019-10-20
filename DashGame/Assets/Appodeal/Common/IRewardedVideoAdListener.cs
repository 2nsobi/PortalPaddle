namespace AppodealAds.Unity.Common {
	public interface IRewardedVideoAdListener {
<<<<<<< HEAD
		void onRewardedVideoLoaded (bool precache);
		void onRewardedVideoFailedToLoad ();
		void onRewardedVideoShown ();
		void onRewardedVideoFinished (double amount, string name);
		void onRewardedVideoClosed (bool finished);
		void onRewardedVideoExpired ();
		void onRewardedVideoClicked ();
	}
}
=======
		void onRewardedVideoLoaded(bool precache);
		void onRewardedVideoFailedToLoad();
		void onRewardedVideoShown();
        void onRewardedVideoFinished(double amount, string name);
		void onRewardedVideoClosed(bool finished);
        void onRewardedVideoExpired();
	}
}
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
