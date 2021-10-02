using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turbine : MonoBehaviour
{
    public Core intake;

    public GameObject powerDisplay;

    public GameObject rotor;

    private int gearBox = 1;

    bool controlled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = intake.GetSteamOutput() * 1800f;

        rotor.transform.Rotate(Vector3.forward, 45 * Time.deltaTime * speed);

        powerDisplay.GetComponent<Text>().text = (speed * gearBox).ToString("0") + " MW";

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
        }
    }

    void FixedUpdate()
    {

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
