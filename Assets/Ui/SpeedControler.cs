using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedControler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        SetTimeDefault();
        SetTimeDouble();
    }


    void SetTimeDefault()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1f;
        }
    }
    void SetTimeDouble()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2f;
        }
    }
}
