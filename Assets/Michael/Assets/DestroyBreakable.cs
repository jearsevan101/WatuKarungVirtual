using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBreakable : MonoBehaviour
{
    private int collisionCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            collisionCount += 1;
        }
    }
}
