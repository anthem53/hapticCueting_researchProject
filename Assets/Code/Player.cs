using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.IO.Ports;
using System.Linq;

using System;



public class Player : MonoBehaviour
{

    enum joystick {
        KEYBOARD,
        ARDUINO
    }

    joystick joystickMode;
    public Vector3 initPlaceAtRound;
    public GameObject UI_dropdown;

    SerialPort sp;
    bool serialStart;

    float xAccel;
    float yAccel;

    public GameObject Route;

    bool isFailOpenArduino;

    // Start is called before the first frame update
    void Start()
    {
        //joystickMode = joystick.KEYBOARD;
        joystickMode = joystick.ARDUINO;
        initPlaceAtRound = transform.position;
        isFailOpenArduino = false;
        sp = new SerialPort("COM4", 9600);  //set Serial port

        Debug.Log("*** Before open *** ");
        try { 
            sp.Open();
            sp.ReadTimeout = 100;

        }
        catch (System.IO.IOException error)
        {
            Debug.Log("*** Fail to connect arduino *** ");
            isFailOpenArduino = true;
        }

    }


    // Update is called once per frame
    void Update()
    {
        bool isWait = Route.GetComponent<Route>().isWait;
        if (isWait == false)
        {
            if (joystickMode == joystick.KEYBOARD)
            {
                getJoystickCoord();
                PlayerMove();
            }
            else /* joystickMode == joystick.ARDUINO*/
            {
                float[] joystickXY = getJoystickCoord();
                float[] xyAccel = getAccelCoord(joystickXY);
                moveObject(xyAccel);

            }
        }
        else 
        {
            getJoystickCoord();
            xAccel = 0;
            yAccel = 0;
            Debug.Log("Wait to move");
        }

    }


    public void setKeyboardMode() 
    {
        joystickMode = joystick.KEYBOARD;
    }

    public void setArduinoMode()
    {
        joystickMode = joystick.ARDUINO;
    }

    void PlayerMove() {
        float accel = 7 * Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            xAccel = 0.0f;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) 
        {
            yAccel = 0.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            //transform.position += new Vector3(-accel, 0, 0);
            xAccel = -accel;
        }

        if (Input.GetKey(KeyCode.D))
        {
            //transform.position += new Vector3(accel, 0, 0);
            xAccel = accel;
        }

        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += new Vector3(0, accel, 0);
            yAccel = accel;
        }

        if (Input.GetKey(KeyCode.S))
        {
            //transform.position += new Vector3(0, -accel, 0);
            yAccel = -accel;
        }
        transform.position += new Vector3(xAccel, yAccel, 0);

    }


    float[] getJoystickCoord()
    {
        if (isFailOpenArduino == true)
            return new float[2];
        string test = sp.ReadLine();
        //Debug.Log(test);
        string[] p = test.Split(' ');


        int i = 0;
        float[] xy = new float[2];
        foreach (string s in p)
        {
            xy[i] = float.Parse(s);
            //Debug.Log(i + " : " + xy[i]);
            i++;
        }

        return xy;
    }

    float[] getAccelCoord(float[] joystickXY)
    {
        foreach (int i in Enumerable.Range(0, joystickXY.Length))
        {
            joystickXY[i] -= 512f;

        }

        return joystickXY;

    }

    void moveObject(float[] xyAccel)
    {
        int x = 0, y = 1 , weight = 7000;
        transform.position += new Vector3(xyAccel[x] / weight, xyAccel[y] / weight, 0.00f);
    }

}
