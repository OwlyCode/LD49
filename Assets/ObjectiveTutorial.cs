using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTutorial : MonoBehaviour
{
    public GameObject next;

    public GameObject text1;
    public GameObject text2;

    public GlobalLogic GlobalLogic;

    void Update()
    {
        if (GlobalLogic.IsContractRespected()) {
            text1.SetActive(false);
            text2.SetActive(true);
        }

        if (GlobalLogic.GetCurrentContractIndex() > 0) {
            next.SetActive(true);
            Destroy(gameObject);
        }
    }
}
