using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreTutorial : MonoBehaviour
{
    public GameObject next;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (next != null) {
            next.SetActive(true);
        }

        Destroy(gameObject);
    }
}
