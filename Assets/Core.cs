using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Core : MonoBehaviour
{
    const float MAX_HEAT = 400f;

    public GameObject Control;
    public GameObject Graphite;
    public GameObject Thermometer;
    public GameObject Help;
    public GameObject Alarm;
    public GameObject Glow;

    public Pump intake;

    float rod = 0f;
    bool controlled = false;
    float controlSpeed = 0.005f;
    float steamOutput = 0f;

    public float heatExcess = 0f;

    float deviation = 1f;
    float deviationDuration = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsFailing()
    {
        return heatExcess >= MAX_HEAT;
    }

    // Update is called once per frame
    void Update()
    {
        deviationDuration -= Time.deltaTime;

        if (deviationDuration < 0f) {
            deviationDuration = 10f;
            deviation = Random.Range(1f, 1.5f);
        }

        Control.transform.rotation = Quaternion.Euler(0, 0, rod * 900);

        Graphite.transform.localPosition = new Vector3(0f, rod * 6.39f, -0.1f);

        Help.SetActive(controlled);
        Alarm.SetActive(heatExcess > MAX_HEAT * 0.75f);
    }

    void FixedUpdate()
    {
        if (controlled) {
            rod += Input.GetAxis("Vertical") * controlSpeed;
        }

        rod = Mathf.Clamp(rod, 0f, 1f);

        Glow.GetComponent<Light2D>().intensity = 2f * rod;

        heatExcess += rod * deviation;

        steamOutput = intake.Consume(Mathf.Clamp(heatExcess, 0f, 100f)/100f * Time.fixedDeltaTime);

        heatExcess -= 30f * steamOutput;

        heatExcess = Mathf.Clamp(heatExcess, 0f, MAX_HEAT);

        Thermometer.transform.localPosition = new Vector3(0.171f, Mathf.Lerp(-1.33f, 1.33f, heatExcess/MAX_HEAT), -1f);
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
