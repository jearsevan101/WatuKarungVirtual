using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPCgamepasir : MonoBehaviour
{
    [Header("Dialog UI")]
    public GameObject panelDialog;
    public TMP_Text textDialog;
    public Button btnYes;
    public Button btnNo;

    [Header("Instruksi UI")]
    public GameObject panelInstruksi;
    public TMP_Text textInstruksi;
    public TMP_Text textTimer;

    [Header("Instruksi Settings")]
    [TextArea]
    public string[] instruksiList = new string[]
    {
        "Silahkan gerakan kaki kiri anda ke arah luar",
        "Silahkan gerakan kaki kanan anda ke arah luar",
        "Silahkan gerakan kedua kaki anda ke depan dan ke belakang",
        "Silahkan gerakan kedua kaki anda ke kiri dan ke kanan",
        "Silahkan gerakan kedua kaki anda dengan gerakan memutar",
        "Silahkan gerakan kaki anda sesuka hati anda"
    };

    public float durasiInstruksi = 10f;

    private bool playerNearby = false;

    void Start()
    {
        panelDialog.SetActive(false);
        panelInstruksi.SetActive(false);

        btnYes.onClick.AddListener(OnYesClicked);
        btnNo.onClick.AddListener(OnNoClicked);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            ShowDialog();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            panelDialog.SetActive(false);
        }
    }

    void ShowDialog()
    {
        panelDialog.SetActive(true);
        textDialog.text = "Hai, apakah kamu ingin merasakan bagaimana sentuhan emas pasir?";
    }

    void OnYesClicked()
    {
        panelDialog.SetActive(false);
        StartCoroutine(StartTraining());
    }

    void OnNoClicked()
    {
        panelDialog.SetActive(false);
        Debug.Log("Player menolak, game tidak dimulai.");
    }

    IEnumerator StartTraining()
    {
        panelInstruksi.SetActive(true);

        for (int i = 0; i < instruksiList.Length; i++)
        {
            // Countdown sebelum instruksi
            for (int c = 3; c > 0; c--)
            {
                textInstruksi.text = "Mulai dalam: " + c;
                textTimer.text = "";
                yield return new WaitForSeconds(1f);
            }

            // Tampilkan instruksi dengan timer mundur
            float waktu = durasiInstruksi;
            while (waktu > 0)
            {
                textInstruksi.text = instruksiList[i];
                textTimer.text = "Sisa waktu: " + Mathf.CeilToInt(waktu).ToString() + " dtk";
                waktu -= Time.deltaTime;
                yield return null;
            }
        }

        // Selesai semua instruksi
        textInstruksi.text = "Latihan selesai, terima kasih!";
        textTimer.text = "";
        yield return new WaitForSeconds(3f);

        panelInstruksi.SetActive(false);
    }
}