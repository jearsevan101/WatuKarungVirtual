using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlexibleSceneLoader : MonoBehaviour

{
    [SerializeField] private string sceneName; // isi di Inspector

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}