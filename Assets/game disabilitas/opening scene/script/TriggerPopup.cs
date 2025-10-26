using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerPopup : MonoBehaviour
{
    public GameObject popupUI; // Drag & drop prefab UI kamu di inspector
    public Transform playerCamera; // Biasanya Camera.main.transform
    private bool isPlayerInside = false;

    void Update()
    {
        if (isPlayerInside && popupUI.activeSelf)
        {
            // Bikin popup menghadap ke kamera (hologram feel)
            popupUI.transform.LookAt(playerCamera);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            popupUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            popupUI.SetActive(false);
        }
    }
}