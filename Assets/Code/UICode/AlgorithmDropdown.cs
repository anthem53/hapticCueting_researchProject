using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlgorithmDropdown : MonoBehaviour
{
    public Dropdown dropdown;
    public GameObject route;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startAlgorithmDropdown() 
    {
        dropdown.options.Clear();
        Debug.Log("UI dropdown!");

        int calcNum = 4;

        dropdown.onValueChanged.AddListener(delegate { ChangeAlgorithmUIValueChanged(dropdown); });

        for (int i = 0; i < calcNum; i++)//1부터 10까지
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = (i + 1).ToString() + ".  " + "CalcScore" + (i+1).ToString();
            dropdown.options.Add(option);
        }

        dropdown.value = 0;

    }

    public void ChangeAlgorithmUIValueChanged(Dropdown change)
    {
        Debug.Log("ChangeAlgorithm : " + change.value);
        Debug.Log("ChangeAlgorithm : " + change.value.GetType().Name);
    }

    public int GetDropDownValue()
    {
        return dropdown.value;
    }

}
