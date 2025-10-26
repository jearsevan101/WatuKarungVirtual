using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // kalau pakai TextMeshPro

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;
    public float fakeLoadingDuration = 5f; // atur lama loading (detik)
    public string nextSceneName = "GameScene"; // ganti dengan nama scene VR kamu

    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        float elapsed = 0f;

        while (elapsed < fakeLoadingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fakeLoadingDuration);

            progressBar.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        // Setelah selesai loading, masuk ke GameScene
        SceneManager.LoadScene(nextSceneName);
    }
}
