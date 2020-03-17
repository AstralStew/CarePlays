using UnityEngine;
using UnityEditor;

public class DeletePlayerPrefs : EditorWindow {

    [MenuItem("Paperticket/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPrefs() {

        // Remove all keys and values from PlayerPrefs
        PlayerPrefs.DeleteAll();

    }

}