using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    const float MAX_HEAT = 400f;

    public GameObject Control;
    public GameObject Graphite;
    public GameObject Thermometer;
    public GameObject Help;

    public Pump intake;

    float rod = 0f;
    bool controlled = false;
    float controlSpeed = 0.05f;
    float steamOutput = 0f;

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

        Help.SetActive(controlled);
    }

    void FixedUpdate()
    {
        if (controlled) {
            rod += Input.GetAxis("Vertical") * controlSpeed;
        }

        rod = Mathf.Clamp(rod, 0f, 1f);

        heatExcess += rod;

        steamOutput = intake.Consume(Mathf.Clamp(heatExcess, 0f, 200f)/100f * Time.fixedDeltaTime);

        heatExcess -= 30f * steamOutput;

        heatExcess = Mathf.Clamp(heatExcess, 0f, MAX_HEAT);

        Thermometer.transform.localPosition = new Vector3(0f, Mathf.Lerp(-0.47f, 0.47f, heatExcess/MAX_HEAT), -1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        controlled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        controlled = false;
    }

    public float GetSteamOutput()
    {
        return steamOutput;
    }
}
