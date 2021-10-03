using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
    const float failureMaxDelay = 5.0f;

    float failureDelay = 5.0f;

    public GameObject contractDisplay;

    public Turbine TurbineA;
    public Turbine TurbineB;
    public Turbine TurbineC;

    public Pump PumpA;
    public Pump PumpB;
    public Pump PumpC;

    public Core CoreA;
    public Core CoreB;
    public Core CoreC;

    public GameObject MasterAlarm;

    Contract currentContract;
    int currentContractIndex;

    Contract[] contracts = new Contract[] {
        // Tutorial
        new Contract(32, 40, 0, 0, 0, 0, delay: 300f, duration: 30f, tolerance: 30f),
        new Contract(68, 76, 0, 0, 0, 0, delay: 45f, duration: 30f, tolerance: 20f),

        // Second starts
        new Contract(68, 76, 68, 76, 0, 0, delay: 30f, duration: 30f, tolerance: 20f),
        new Contract(32, 40, 140, 148, 0, 0, delay: 30f, duration: 30f, tolerance: 20f),

        // Third starts
        new Contract(32, 40, 140, 148, 32, 40, delay: 30f, duration: 30f, tolerance: 20f),

        // Great revert
        new Contract(32, 40, 68, 76, 140, 148, delay: 30f, duration: 30f, tolerance: 20f),
        new Contract(140, 148, 68, 76, 32, 40, delay: 30f, duration: 30f, tolerance: 20f),

        // Back to one engine
        new Contract(0, 1, 140, 148, 0, 1, delay: 20f, duration: 30f, tolerance: 20f),

        // Pump tricks
        new Contract(0, 1, 10, 20, 0, 1, delay: 20f, duration: 15f, tolerance: 10f),

        // Stop
        new Contract(0, 1, 0, 1, 0, 1, delay: 15f, duration: 5f, tolerance: 15f),
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
        return (TurbineA.GetCurrentPower() >= currentContract.minA && TurbineA.GetCurrentPower() <= currentContract.maxA &&
                TurbineB.GetCurrentPower() >= currentContract.minB && TurbineB.GetCurrentPower() <= currentContract.maxB &&
                TurbineC.GetCurrentPower() >= currentContract.minC && TurbineC.GetCurrentPower() <= currentContract.maxC);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFailing()) {
            failureDelay -= Time.deltaTime;
            MasterAlarm.SetActive(true);
            if (failureDelay <= 0) {
                SceneManager.LoadScene("Scenes/Meltdown");
            }
        } else {
            MasterAlarm.SetActive(false);
            failureDelay = failureMaxDelay;
        }

        TurbineA.SetValid(TurbineA.GetCurrentPower() >= currentContract.minA && TurbineA.GetCurrentPower() <= currentContract.maxA);
        TurbineB.SetValid(TurbineB.GetCurrentPower() >= currentContract.minB && TurbineB.GetCurrentPower() <= currentContract.maxB);
        TurbineC.SetValid(TurbineC.GetCurrentPower() >= currentContract.minC && TurbineC.GetCurrentPower() <= currentContract.maxC);

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
                            SceneManager.LoadScene("Scenes/Win");

                            return;
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
                    SceneManager.LoadScene("Scenes/Blackout");
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
        if (currentContractIndex >= contracts.Length - 1) {
            return null;
        }

        currentContractIndex++;

        return contracts[currentContractIndex];
    }

    public bool IsFailing()
    {
        return PumpA.IsFailing() || PumpB.IsFailing() || PumpC.IsFailing() || CoreA.IsFailing() || CoreB.IsFailing() || CoreC.IsFailing();
    }
}
