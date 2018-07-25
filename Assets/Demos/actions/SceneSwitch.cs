using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField]
    private string[] scenes;

    [SerializeField]
    private string[] subSupportScenes;

    [SerializeField]
    private int sceneIndex;
    private Rect windowRect;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadScene(sceneIndex);
        windowRect = new Rect(Screen.width * 0.8f, 0, Screen.width * 0.2f, Screen.height * 0.2f);
    }

    void OnGUI()
    {

        windowRect = GUILayout.Window(0, windowRect, WindowFunc, new GUIContent("场景跳转"));
    }

    private void WindowFunc(int id)
    {
        if (GUILayout.Button("Last Scene"))
        {
            LoadScene(sceneIndex - 1);
        }
        if (GUILayout.Button("Next Scene"))
        {
            LoadScene(sceneIndex + 1);
        }
    }
    private void LoadScene(int index)
    {
        if (index >= 0 && index < scenes.Length)
        {
            StartCoroutine(DelayLoadInternal(index));
        }
    }
    IEnumerator DelayLoadInternal(int index)
    {
        if(index != sceneIndex)
        {
            var operate = SceneManager.UnloadSceneAsync(scenes[sceneIndex]);
            yield return operate;
        }
       
        sceneIndex = index;
        SceneManager.LoadScene(scenes[sceneIndex]);
        for (int i = 0; i < subSupportScenes.Length; i++)
        {
            SceneManager.LoadScene(subSupportScenes[i], LoadSceneMode.Additive);
        }
    }
}
