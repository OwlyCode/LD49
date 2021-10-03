using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [Range(1,20000)]  //Creates a slider in the inspector
    public float frequency1;

    [Range(1,20000)]  //Creates a slider in the inspector
    public float frequency2;

    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;

    AudioSource audioSource;
    int timeIndex = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!audioSource.isPlaying)
            {
                timeIndex = 0;  //resets timer before playing sound
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i = 0; i < data.Length; i+= channels)
        {
            data[i] = CreateSine(timeIndex, frequency1, sampleRate);
            data[i] = Triangle(frequency1, frequency2, sampleRate, 0f, timeIndex);

            timeIndex++;

            //if timeIndex gets too big, reset it to 0
            if(timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }

    float Triangle(float minLevel, float maxLevel, float period, float phase, float t) {
            float pos = Mathf.Repeat(t - phase, period) / period;

            if (pos < .5f) {
                return Mathf.Lerp(minLevel, maxLevel, pos * 2f);
            }

            return Mathf.Lerp(maxLevel, minLevel, (pos - .5f) * 2f);
    }
}
