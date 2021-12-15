using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class intro : MonoBehaviour
{
    public static int type;
    public InputField inputName;
    public static string name;
    // Start is called before the first frame update
    void Start()
    {
        type = 0;
        name = "";
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("inputName.text : " + inputName.text);
        name = inputName.text;
    }
    private void Awake() 
    {
        DontDestroyOnLoad(this);
    }

    public void PracticeTest()
    {
        type = 0;
        SceneManager.LoadScene("SampleScene");
    }

    public void StartTest() 
    {
        type = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public string getName() 
    {  
        return name;
    }
    public int getTestType() 
    {
        return type;
    }
}
