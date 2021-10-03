using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    public string getATarget()
    {
        if (this.maxA == 1) {
            return "OFF";
        }

        return this.minA + "-" + this.maxA + "MW";
    }

    public string getBTarget()
    {
        if (this.maxB == 1) {
            return "OFF";
        }

        return this.minB + "-" + this.maxB + "MW";
    }

    public string getCTarget()
    {
        if (this.maxC == 1) {
            return "OFF";
        }

        return this.minC + "-" + this.maxC + "MW";
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
        A.SetValid(A.GetCurrentPower() >= currentContract.minA && A.GetCurrentPower() <= currentContract.maxA);
        B.SetValid(B.GetCurrentPower() >= currentContract.minB && B.GetCurrentPower() <= currentContract.maxB);
        C.SetValid(C.GetCurrentPower() >= currentContract.minC && C.GetCurrentPower() <= currentContract.maxC);

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

        text.text = "STATUS    | A         | B         | C         | TIME   \n";
        text.text += "------------------------------------------------------\n";

        if (state == State.TRANSITION) {
            text.text += "<color=yellow>"+ "REACH".PadRight(10) + "</color>| ";
            text.text += currentContract.getATarget().PadRight(10) + "| ";
            text.text += currentContract.getBTarget().PadRight(10) + "| ";
            text.text += currentContract.getCTarget().PadRight(10) + "| ";

            TimeSpan time = TimeSpan.FromSeconds(currentContract.delay);
            text.text += time.ToString(@"m\:ss") + "s\n";
        }

        if (state == State.CONTRACT) {
            text.text += "MAINTAIN".PadRight(10) + "| ";
            text.text += currentContract.getATarget().PadRight(10) + "| ";
            text.text += currentContract.getBTarget().PadRight(10) + "| ";
            text.text += currentContract.getCTarget().PadRight(10) + "| ";

            TimeSpan time = TimeSpan.FromSeconds(currentContract.duration);
            text.text += time.ToString(@"m\:ss") + "s\n";
        }

        if (state == State.STALLING) {
            text.text += "<color=red>"+ "!STALLING!".PadRight(10) + "</color>| ";
            text.text += currentContract.getATarget().PadRight(10) + "| ";
            text.text += currentContract.getBTarget().PadRight(10) + "| ";
            text.text += currentContract.getCTarget().PadRight(10) + "| ";

            TimeSpan time = TimeSpan.FromSeconds(currentContract.tolerance);
            if (currentContract.tolerance < 10) {
                text.text += "<color=red>" + time.ToString(@"m\:ss") + "s</color>\n";
            } else {
                text.text += time.ToString(@"m\:ss") + "s\n";
            }
        }

        if (currentContractIndex < contracts.Length - 1) {
            var nextContract = contracts[currentContractIndex + 1];

            text.text += "<color=#555555ff>" + "NEXT".PadRight(10) + "| ";
            text.text += nextContract.getATarget().PadRight(10) + "| ";
            text.text += nextContract.getBTarget().PadRight(10) + "| ";
            text.text += nextContract.getCTarget().PadRight(10) + "| ";

            TimeSpan time = TimeSpan.FromSeconds(nextContract.duration);
            text.text += time.ToString(@"m\:ss") + "s</color>\n";
        }
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
