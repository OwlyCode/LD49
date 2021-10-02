using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour
{
    public Core intake;

    public GameObject rotor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = intake.GetSteamOutput() * 600f;

        rotor.transform.Rotate(Vector3.forward, 45 * Time.deltaTime * speed);
    }

    void FixedUpdate()
    {

    }
}
