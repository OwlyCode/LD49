using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class Turbine : MonoBehaviour
{
    public Core intake;

    public GameObject powerDisplay;

    public GameObject rotor;

    public GameObject lowButton;
    public GameObject mediumButton;
    public GameObject highButton;

    public GameObject lowMagnet;
    public GameObject mediumMagnet;
    public GameObject highMagnet;

    public Light2D validationLight;

    public GameObject Help;


    public GameObject EngineHum;

    private int gearBox = 1;

    float speed = 0f;

    bool controlled = false;

    bool valid = true;

    // Start is called before the first frame update
    void Start()
    {
        EngineHum.GetComponent<AudioSource>().volume = 0;
        EngineHum.GetComponent<AudioSource>().Play();
    }

    public void SetValid(bool valid)
    {
        this.valid = valid;
    }

    // Update is called once per frame
    void Update()
    {
        Help.SetActive(controlled);

        speed = Mathf.Lerp(speed, intake.GetSteamOutput() * 1800f, Time.deltaTime * 0.5f);

        EngineHum.GetComponent<AudioSource>().volume = Mathf.Min(speed / 10f, 0.75f);
        EngineHum.GetComponent<AudioSource>().pitch = 0.5f + gearBox * speed / 50f;

        rotor.transform.Rotate(Vector3.forward, 45 * Time.deltaTime * speed);

        powerDisplay.GetComponent<Text>().text = (GetCurrentPower()).ToString("0") + " MW";

        if (Input.GetKeyDown(KeyCode.UpArrow) && gearBox < 4 && controlled)
        {
            switch(gearBox) {
                case 1:
                    gearBox = 2;
                    break;
                case 2:
                    gearBox = 4;
                    break;
                default:
                    gearBox = 1;
                    break;
            }

            GetComponent<AudioSource>().Play();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && gearBox > 1 && controlled)
        {
            switch(gearBox) {
                case 4:
                    gearBox = 2;
                    break;
                case 2:
                    gearBox = 1;
                    break;
                default:
                    gearBox = 4;
                    break;
            }

            GetComponent<AudioSource>().Play();
        }

        lowButton.GetComponent<SpriteRenderer>().color = Color.white;
        mediumButton.GetComponent<SpriteRenderer>().color = Color.white;
        highButton.GetComponent<SpriteRenderer>().color = Color.white;

        if (gearBox == 1) {
            lowButton.GetComponent<SpriteRenderer>().color = Color.green;
            lowMagnet.transform.Rotate(Vector3.forward, 200 * Time.deltaTime * speed);

        }
        if (gearBox == 2) {
            mediumButton.GetComponent<SpriteRenderer>().color = Color.green;
            mediumMagnet.transform.Rotate(Vector3.forward, 200 * Time.deltaTime * speed);
        }
        if (gearBox == 4) {
            highButton.GetComponent<SpriteRenderer>().color = Color.green;
            highMagnet.transform.Rotate(Vector3.forward, 200 * Time.deltaTime * speed);
        }

        if (valid) {
            powerDisplay.GetComponent<Text>().color = Color.green;
            validationLight.color = Color.green;
        } else {
            powerDisplay.GetComponent<Text>().color = Color.yellow;
            validationLight.color = Color.yellow;
        }
    }

    public float GetCurrentPower()
    {
        return speed * gearBox;
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
