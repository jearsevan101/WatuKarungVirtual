using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypointFloating : MonoBehaviour

{
    [Header("Rotasi (sumbu Z)")]
    public float rotationSpeed = 50f; // derajat per detik

    [Header("Naik Turun (sumbu Y)")]
    public float floatAmplitude = 0.5f; // tinggi naik-turun
    public float floatFrequency = 1f;   // cepat naik-turun

    private Vector3 startPos;

    void Start()
    {
        // Simpan posisi awal
        startPos = transform.position;
    }

    void Update()
    {
        // --- Rotasi pada sumbu Z ---
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // --- Naik turun pada sumbu Y ---
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}