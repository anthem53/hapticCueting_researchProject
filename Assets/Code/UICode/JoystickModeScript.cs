using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickModeScript : MonoBehaviour
{
    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onToggleValueKeyboard(bool boolean) 
    {
        player.GetComponent<Player>().setKeyboardMode();
        Debug.Log("Keyboard bool :" + boolean);
    }
    public void onToggleValueArduino(bool boolean)
    {
        player.GetComponent<Player>().setArduinoMode();
        Debug.Log("Arduino bool :" + boolean);
    }

}
