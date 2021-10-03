using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pump : MonoBehaviour
{
    const float MAX_PRESSURE = 5f;

    float flow = 0f;
    public float pressure = 0f;

    float controlSpeed = 0.025f;

    bool controlled = false;

    public GameObject Control;
    public GameObject Help;
    public GameObject Gauge;
    public GameObject Alarm;

    public ParticleSystem[] leaks;

    float deviation = 1f;
    float deviationDuration = 30f;
    float soundCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsFailing()
    {
        return pressure >= MAX_PRESSURE;
    }

    public float Consume(float amount)
    {
        if (amount > pressure) {
            amount = pressure;
        }

        pressure -= amount;

        return amount;
    }

    void FixedUpdate()
    {
        if (controlled) {
            var oldFlow = flow;
            flow += Input.GetAxis("Vertical") * controlSpeed;
            flow = Mathf.Clamp(flow, 0f, 1f);

            if (Input.GetAxis("Vertical") != 0f) {
                if (soundCooldown < 0f && oldFlow != flow) {
                    GetComponent<AudioSource>().Play();
                    soundCooldown = 0.1f;
                }

                soundCooldown -= Time.fixedDeltaTime;
            }
        }

        pressure += flow * deviation * Time.fixedDeltaTime;
        pressure = Mathf.Clamp(pressure, 0f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        deviationDuration -= Time.deltaTime;

        if (deviationDuration < 0f) {
            deviationDuration = 30f;
            deviation = Random.Range(1f, 1.1f);
        }


        if (pressure > MAX_PRESSURE * 0.85f) {
            foreach (var leak in leaks) {
                leak.enableEmission = true;
            }
        } else {
            foreach (var leak in leaks) {
                leak.enableEmission = false;
            }
        }

        Control.transform.rotation = Quaternion.Euler(0, 0, 90 - flow * 90);
        Gauge.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(90, -90, pressure/MAX_PRESSURE));

        Help.SetActive(controlled);
        Alarm.SetActive(pressure > MAX_PRESSURE * 0.6f);
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
