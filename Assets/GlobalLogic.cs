using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Contract
{
    public float minA;
    public float maxA;
    public float minB;
    public float maxB;
    public float minC;
    public float maxC;

    public float delay;
    public float duration;
    public float tolerance;

    public Contract(float minA, float maxA, float minB, float maxB, float minC, float maxC, float delay, float duration, float tolerance)
    {
        this.minA = minA;
        this.maxA = maxA;
        this.minB = minB;
        this.maxB = maxB;
        this.minC = minC;
        this.maxC = maxC;
        this.delay = delay;
        this.duration = duration;
        this.tolerance = tolerance;
    }
}

enum State {
    TRANSITION,
    CONTRACT,
    STALLING,
}

public class GlobalLogic : MonoBehaviour
{
    public GameObject contractDisplay;

    public Turbine A;
    public Turbine B;
    public Turbine C;

    Contract currentContract;
    int currentContractIndex;

    Contract[] contracts = new Contract[] {
        new Contract(30, 40, 0, 0, 0, 0, 30, 10, 20),
        new Contract(30, 40, 100, 150, 0, 0, 30, 25, 20),
        new Contract(30, 40, 0, 5, 0, 5, 30, 10, 20),
    };

    State state;

    // Start is called before the first frame update
    void Start()
    {
        currentContractIndex = 0;
        currentContract = contracts[0];
        state = State.TRANSITION;
    }

    bool IsContractRespected()
    {
        return (A.GetCurrentPower() >= currentContract.minA && A.GetCurrentPower() <= currentContract.maxA &&
                B.GetCurrentPower() >= currentContract.minB && B.GetCurrentPower() <= currentContract.maxB &&
                C.GetCurrentPower() >= currentContract.minC && C.GetCurrentPower() <= currentContract.maxC);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case State.TRANSITION:
                if (IsContractRespected() || currentContract.delay <= 0) {
                    state = State.CONTRACT;
                } else {
                    currentContract.delay -= Time.deltaTime;
                }
                break;
            case State.CONTRACT:
                if (!IsContractRespected()) {
                    state = State.STALLING;
                } else {
                    currentContract.duration -= Time.deltaTime;

                    if (currentContract.duration <= 0) {
                        currentContract = NextContract();
                        state = State.TRANSITION;

                        if (currentContract == null) {
                            Debug.Log("No more contracts");
                        }
                    }
                }
                break;
            case State.STALLING:
                if (IsContractRespected()) {
                    state = State.CONTRACT;
                } else {
                    currentContract.tolerance -= Time.deltaTime;
                }

                if (currentContract.tolerance <= 0) {
                    Debug.Log("Contract failed");
                }
                break;
        }

        var text = contractDisplay.GetComponent<Text>();

        text.text = "";
        text.text += "A: " + currentContract.minA + " - " + currentContract.maxA + "\n";
        text.text += "B: " + currentContract.minB + " - " + currentContract.maxB + "\n";
        text.text += "C: " + currentContract.minC + " - " + currentContract.maxC + "\n";
        text.text += "Delay: " + currentContract.delay + "\n";
        text.text += "Duration: " + currentContract.duration + "\n";
        text.text += "Tolerance: " + currentContract.tolerance;
    }

    Contract NextContract()
    {
        if (currentContractIndex > contracts.Length - 1) {
            return null;
        }

        currentContractIndex++;

        return contracts[currentContractIndex];
    }
}
