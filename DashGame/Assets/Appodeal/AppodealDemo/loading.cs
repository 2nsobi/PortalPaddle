using UnityEngine;

public class loading : MonoBehaviour {

	void Start () {
<<<<<<< HEAD
		int consentInt = PlayerPrefs.GetInt ("result_gdpr", 0);
		bool consent = consentInt != 0;
		if (consent) {
#if UNITY_5_3_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.LoadScene ("AppodealDemo");
#else
			Application.LoadLevel ("AppodealDemo");
#endif
		} else {
#if UNITY_5_3_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.LoadScene ("GDPR");
#else
			Application.LoadLevel ("GDPR");
#endif
		}
	}
}
=======
        int consentInt = PlayerPrefs.GetInt("result_gdpr", 0);
        bool consent = consentInt != 0;
        if(consent) {
			#if UNITY_5_3_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.LoadScene("AppodealDemo");
			#else
			Application.LoadLevel("AppodealDemo");
			#endif
        } else {
			#if UNITY_5_3_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.LoadScene("GDPR");
			#else
			Application.LoadLevel("GDPR");
			#endif
		}
	}
}
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
