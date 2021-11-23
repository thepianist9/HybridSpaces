using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public ProgressBar Pb;

    public int Valuer = 60;


    private void Start()
    {
        LoadLevel();
    }
    

    public void LoadLevel()
    {
        StartCoroutine(LoadAsynchronously("WelcomeScene"));
    }

    IEnumerator LoadAsynchronously(String SceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            Pb.BarValue = operation.progress*100;
            yield return null;
        }
    }
}
