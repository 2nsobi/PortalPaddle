#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
<<<<<<< HEAD
using AppodealAds.Unity.Editor.Checkers;
=======
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa

[InitializeOnLoad]
public class AppodealSettings : ScriptableObject
{
<<<<<<< HEAD
	[MenuItem ("Appodeal/SDK Documentation")]
	public static void OpenDocumentation ()
	{
		string url = "http://www.appodeal.com/sdk/choose_framework?framework=2&full=1&platform=1";
		Application.OpenURL (url);
	}

	[MenuItem ("Appodeal/Appodeal Homepage")]
	public static void OpenAppodealHome ()
	{
		string url = "http://www.appodeal.com";
		Application.OpenURL (url);
	}

	[MenuItem ("Appodeal/Check Integration")]
	public static void testWindow ()
	{
		CheckerWindow.GetWindow ().Show ();
	}

	[MenuItem ("Appodeal/Filter Android Architectures")]
	public static void filterAndroidAcrhitectures ()
	{
		ArchitectureFilter.prepareBuild ();
	}

	[MenuItem ("Appodeal/Remove plugin")]
	public static void removeAppodealPlugin ()
	{
		RemoveHelper.removePlugin ();
=======
	[MenuItem("Appodeal/SDK Documentation")]
	public static void OpenDocumentation()
	{
		string url = "http://www.appodeal.com/sdk/choose_framework?framework=2&full=1&platform=1";
		Application.OpenURL(url);
	}
	
	[MenuItem("Appodeal/Appodeal Homepage")]
	public static void OpenAppodealHome()
	{
		string url = "http://www.appodeal.com";
		Application.OpenURL(url);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
	}
}
#endif