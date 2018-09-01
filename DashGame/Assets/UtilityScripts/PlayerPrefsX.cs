using System.Collections;
using UnityEngine;

public static class PlayerPrefsX {

    //uses ZPlayerPrefs class

    public static void SetBool( string key, bool value)
    {
        ZPlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(string key)
    {
        return ZPlayerPrefs.GetInt(key) == 1;
    }
}
