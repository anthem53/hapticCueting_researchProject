using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetRouteDropdown : MonoBehaviour
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

    public void StartRoutwDropDown()// Dropdown 格废 积己
    {
        dropdown.options.Clear();
        Debug.Log("UI dropdown!");

        dropdown.onValueChanged.AddListener(delegate { ChangeRouteUIValueChanged(dropdown); });

        int routeNum = 4;

        for (int i = 0; i < routeNum; i++)//1何磐 10鳖瘤
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = (i+1).ToString() + ".  ";
            dropdown.options.Add(option);
        }

        dropdown.value = 0;
    }

    public void ChangeRouteUIValueChanged(Dropdown change)
    {
        Debug.Log( "New Value : " + change.value);
        Debug.Log("value type : " + change.value.GetType().Name);
        route.GetComponent<Route>().setStartPosition(change.value);
        route.GetComponent<Route>().drawNthStage(change.value);
        //route.GetComponent<Route>().drawCurrentStage();

    }

    public int GetDropDownValue() {
        return dropdown.value;
    }

    public string GetCurrentDropDownText() 
    {
        Debug.Log("dropdown.options[dropdown.value].text" +  dropdown.options[dropdown.value].text);
        return dropdown.options[dropdown.value].text;
    }
}
