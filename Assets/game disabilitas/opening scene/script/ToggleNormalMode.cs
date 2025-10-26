using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleNormalMode : MonoBehaviour
{
    public GameObject iconNormal;        // tombol awal
    public GameObject iconNormalClick;   // ikon ketika dipilih
    public GameObject iconSelanjutnya;   // tombol selanjutnya

    private bool isSelected = false; // status toggle

    public void ToggleNormal()
    {
        if (!isSelected)
        {
            // Jika belum dipilih → aktifkan NormalClick + Selanjutnya
            iconNormal.SetActive(false);
            iconNormalClick.SetActive(true);
            iconSelanjutnya.SetActive(true);
            isSelected = true;
        }
        else
        {
            // Jika ditekan lagi → kembalikan ke awal
            iconNormal.SetActive(true);
            iconNormalClick.SetActive(false);
            iconSelanjutnya.SetActive(false);
            isSelected = false;
        }
    }
}