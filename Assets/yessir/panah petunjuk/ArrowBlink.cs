using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBlink : MonoBehaviour
{
    public Renderer arrowRenderer; // drag komponen renderer ke sini
    public Color color1 = Color.cyan;   // warna awal
    public Color color2 = Color.white;  // warna tujuan
    public float speed = 2f; // kecepatan berkedip

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        arrowRenderer.material.color = Color.Lerp(color1, color2, t);
    }
}