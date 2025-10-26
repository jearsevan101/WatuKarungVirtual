using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionCanvasManager : MonoBehaviour
{
    [SerializeField] Canvas UICanvasOption;
    [SerializeField] Button yesButton;

    private bool isCanvasOpen = false;

    private void Start()
    {
        UICanvasOption.gameObject.SetActive(false);

        yesButton.onClick.AddListener(() =>
        {
            ToggleCanvas();
            EventManager.BackToMainMenu();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    private void Update()
    {
        // Oculus/Quest A button = "joystick button 0"
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            ToggleCanvas();
        }
    }

    private void ToggleCanvas()
    {
        isCanvasOpen = !isCanvasOpen;
        UICanvasOption.gameObject.SetActive(isCanvasOpen);
    }
}
