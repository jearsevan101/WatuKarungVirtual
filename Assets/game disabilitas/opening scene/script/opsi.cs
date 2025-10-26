using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class opsi : MonoBehaviour

{
    // Fungsi umum untuk pindah scene
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi khusus jika mau keluar aplikasi
    public void QuitApp()
    {
        Application.Quit();
    }
}
