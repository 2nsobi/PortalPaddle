using UnityEngine;
using UnityEditor;

public class UnityEditorUtilities
{
    [MenuItem("Edit/Delete All PlayerPrefs")]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
