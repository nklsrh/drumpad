using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LevelEditorWindow : EditorWindow
{
    private GameLevelData currentLevelData;
    private string savePath = "Assets/Resources/Levels/";
    private string fileName = "NewLevel.json";
    private AudioSource editorAudioSource; // AudioSource for playing clips

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        if (editorAudioSource == null)
        {
            GameObject audioSourceObject = new GameObject("EditorAudioSource");
            audioSourceObject.hideFlags = HideFlags.HideAndDontSave;
            editorAudioSource = audioSourceObject.AddComponent<AudioSource>();
        }
    }

    private void OnDisable()
    {
        if (editorAudioSource != null)
        {
            DestroyImmediate(editorAudioSource.gameObject);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);

        // Song ID
        currentLevelData.songID = EditorGUILayout.TextField("Song ID", currentLevelData.songID);

        // Starting Point
        currentLevelData.startingPoint = EditorGUILayout.FloatField("Starting Point", currentLevelData.startingPoint);

        // Clips List
        if (currentLevelData.clips == null)
            currentLevelData.clips = new List<GameClipData>();

        EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);
        for (int i = 0; i < currentLevelData.clips.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            var c = currentLevelData.clips[i];
            c.startingPoint = (i>0? currentLevelData.clips[i - 1].startingPoint + currentLevelData.clips[i-1].duration : currentLevelData.startingPoint);
            EditorGUILayout.LabelField("Starting Point: " +  c.startingPoint + "s");
            c.duration = EditorGUILayout.FloatField("Duration", currentLevelData.clips[i].duration);
            currentLevelData.clips[i] = c;

            if (GUILayout.Button("Play Audio"))
            {
                PlayClip(i);
            }

            if (GUILayout.Button("Remove"))
            {
                currentLevelData.clips.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Clip"))
        {
            currentLevelData.clips.Add(new GameClipData());
        }

        // Save and Load
        EditorGUILayout.Space();
        fileName = EditorGUILayout.TextField("File Name", fileName);

        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }
    }

    private void SaveLevel()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string json = JsonUtility.ToJson(currentLevelData, true);
        File.WriteAllText(Path.Combine(savePath, fileName), json);
        AssetDatabase.Refresh();
        Debug.Log("Level Saved!");
    }

    private void LoadLevel()
    {
        string filePath = Path.Combine(savePath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            currentLevelData = JsonUtility.FromJson<GameLevelData>(json);
            Debug.Log("Level Loaded!");
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }private double stopTime = 0; // To track when to stop playback

private void PlayClip(int clipIndex)
{
    if (string.IsNullOrEmpty(currentLevelData.songID))
    {
        Debug.LogError("Song ID is not set!");
        return;
    }

    AudioClip audioClip = Resources.Load<AudioClip>("songs/"+currentLevelData.songID);
    if (audioClip == null)
    {
        Debug.LogError($"Audio clip not found in Resources: {currentLevelData.songID}");
        return;
    }

    GameClipData clipData = currentLevelData.clips[clipIndex];
    float startPoint = clipData.startingPoint;
    float duration = clipData.duration;

    if (startPoint + duration > audioClip.length)
    {
        Debug.LogWarning("Clip duration exceeds audio clip length. Adjusting playback duration.");
        duration = audioClip.length - startPoint;
    }

    editorAudioSource.clip = audioClip;
    editorAudioSource.time = startPoint;
    editorAudioSource.Play();
    Debug.Log($"Playing clip {clipIndex}: Starting at {startPoint}, Duration {duration}");

    // Schedule the stop time
    stopTime = EditorApplication.timeSinceStartup + duration;

    // Subscribe to EditorApplication.update for stopping playback
    EditorApplication.update += StopPlaybackAfterDuration;
}

private void StopPlaybackAfterDuration()
{
    // Check if the stop time has been reached
    if (EditorApplication.timeSinceStartup >= stopTime)
    {
        if (editorAudioSource.isPlaying)
        {
            editorAudioSource.Stop();
            Debug.Log("Playback stopped.");
        }

        // Unsubscribe from EditorApplication.update
        EditorApplication.update -= StopPlaybackAfterDuration;
    }
}

}
