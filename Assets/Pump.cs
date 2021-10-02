using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pump : MonoBehaviour
{
    float flow = 0f;
    float pressure = 0f;

    float controlSpeed = 0.05f;

    bool controlled = false;

    public GameObject Control;
    public GameObject Help;
    public GameObject Gauge;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (controlled) {
            flow += Input.GetAxis("Vertical") * controlSpeed;
        }

        flow = Mathf.Clamp(flow, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        Control.transform.rotation = Quaternion.Euler(0, 0, 90 - flow * 90);
        Gauge.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(35, -35, pressure/10));

        pressure += flow * Time.deltaTime;

        Help.SetActive(controlled);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        controlled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        controlled = false;
    }
}
