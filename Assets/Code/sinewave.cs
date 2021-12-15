using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sinewave : MonoBehaviour
{
    //Audio Settings
    private AudioSource audioSource;
    private const float sampleRate = 48000;
    private const float waveLengthInSeconds = 2.0f;
    private int timeIndex = 0;
    public float duration = 10;
    public float frequency = 540;
    private float fixedTimeIndex = 0;
    private float amp = 1f;
    private int startTimer;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize audio settings
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
        Time.fixedDeltaTime = 0.01f;
        startTimer = 400;
        audioSource.loop = true;
        audioSource.Play();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {

            Debug.Log("Play");
            audioSource.Play();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {

            Debug.Log("Stop");
            audioSource.Stop();
            fixedTimeIndex = 0;
        }

    }
    private void FixedUpdate()
    {

        if (audioSource.isPlaying)
        {
            if (fixedTimeIndex < duration / Time.fixedDeltaTime)
            {
                //fixedTimeIndex++;
            }
            else
            {
                //audioSource.Stop();
                fixedTimeIndex = 0;
                //audioSource.Play();
            }
        }

    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = VibrationScale(amp) * CreateSine(timeIndex, VibrationFreqeuncy(), sampleRate);
            data[i + 1] = VibrationScale(amp) * CreateSine(timeIndex, VibrationFreqeuncy(), sampleRate);
            timeIndex++;

            //if timeIndex gets too big, reset it to 0
            if (timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }

    public void setAmplitude(float amp)
    {
        this.amp = amp;
    
    }

    //Create sine function
    private float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }

    /*
     * Change vibration scale
     * Vibration scale must be 0 ~ 1
     */
    private float VibrationScale(float scale=1f)
    {
        

        //Scale error
        if (scale > 1 || scale < 0)
        {
            //Debug.Log("Scale Error");
            scale = 1;
        }
        return scale;
    }

    /*
     * Set vibration frequency
     * Vibration scale must be 0 ~ 1
     */
    private float VibrationFreqeuncy()
    {
        return frequency;
    }

    public void SetFrequency(float f) 
    {
        this.frequency = f;
    }
}
