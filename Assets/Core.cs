using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public GameObject Control;
    public GameObject Graphite;

    public Pump intake;

    float rod = 0f;
    bool controlled = false;
    float controlSpeed = 0.05f;

    public float heatExcess = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Control.transform.rotation = Quaternion.Euler(0, 0, rod * 900);

        Graphite.transform.localPosition = new Vector3(0f, rod, -0.1f);

        //Help.SetActive(controlled);
    }

    void FixedUpdate()
    {
        if (controlled) {
            rod += Input.GetAxis("Vertical") * controlSpeed;
        }

        rod = Mathf.Clamp(rod, 0f, 1f);

        heatExcess += rod;

        heatExcess -= 30f * intake.Consume(Mathf.Clamp(heatExcess, 0f, 200f)/100f * Time.fixedDeltaTime);

        heatExcess = Mathf.Clamp(heatExcess, 0f, 1000f);
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
