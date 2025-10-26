
using UnityEngine;
using UnityEngine.SceneManagement;


public class modescene : MonoBehaviour

{
    // Pindah ke Scene Utama
    public void GoToModePermainan()
    {
        SceneManager.LoadScene("ModePermainan");
    }

    // Pindah ke Scene Home
    public void GoToHome()
    {
        SceneManager.LoadScene("Home");
    }
}