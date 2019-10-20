<<<<<<< HEAD
﻿using Appodeal.Unity.Editor.iOS;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AppodealPostProcess : MonoBehaviour {

	[PostProcessBuild (100)]
	public static void OnPostProcessBuild (BuildTarget target, string path) {
		if (target.ToString () == "iOS" || target.ToString () == "iPhone") {
			iOSPostprocessUtils.PrepareProject (path);
		}
	}
=======
﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Appodeal.Unity.Editor.iOS;

public class AppodealPostProcess : MonoBehaviour {

	[PostProcessBuild(100)]
	public static void OnPostProcessBuild (BuildTarget target, string path) {		
		if (target.ToString () == "iOS" || target.ToString () == "iPhone") {
            iOSPostprocessUtils.PrepareProject (path);
            iOSPostprocessUtils.UpdatePlist(path);
		}
	}

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
}