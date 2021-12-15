using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Vector3 initPlaceAtRound;
    // Start is called before the first frame update
    void Start()
    {
        initPlaceAtRound = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNewPlace() 
    {
        float XValue = Random.Range(-20.0f, 30.0f);
        float YValue = Random.Range(-10.0f, 10.0f);

        transform.position = new Vector3(XValue,YValue,0.0f);
    }

    public void setInitPlaceAtRound() 
    {
        initPlaceAtRound = transform.position; 
    }
}
