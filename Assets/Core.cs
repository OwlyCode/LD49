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
    float soundCooldown = 0f;

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
            var oldRod = rod;
            rod += Input.GetAxis("Vertical") * controlSpeed;
            rod = Mathf.Clamp(rod, 0f, 1f);


            if (Input.GetAxis("Vertical") != 0f) {
                if (soundCooldown < 0f && oldRod != rod) {
                    GetComponent<AudioSource>().Play();
                    soundCooldown = 0.1f;
                }

                soundCooldown -= Time.fixedDeltaTime;
            }
        }

        var glowLight = Glow.GetComponent<Light2D>();

        if (heatExcess > MAX_HEAT * 0.75f) {
            glowLight.intensity = Mathf.Max(2f * rod, 2f * (heatExcess - 0.75f * MAX_HEAT)/(MAX_HEAT * 0.25f));
            glowLight.color = Color.Lerp(new Color32(0x00, 0x73, 0xFF, 0xFF), new Color32(0xFF, 0xC6, 0x00, 0xFF), (heatExcess/MAX_HEAT - 0.75f) / 0.25f);
        } else {
            glowLight.intensity = 2f * rod;
            glowLight.color = new Color32(0x00, 0x73, 0xFF, 0xFF);
        }



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
