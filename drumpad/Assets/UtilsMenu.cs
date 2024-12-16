using UnityEditor;
using UnityEngine;

public static class UtilsMenu
{
    [MenuItem("Tools/Clear All PlayerPrefs")]
    public static void ClearAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog(
            "Clear All PlayerPrefs",
            "Are you sure you want to clear all PlayerPrefs? This action cannot be undone.",
            "Yes",
            "No"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All PlayerPrefs have been cleared.");
        }
    }
}
