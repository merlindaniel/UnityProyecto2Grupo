using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour
{
    public float timeScaleGame;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScaleGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != timeScaleGame)
        {
            Time.timeScale = timeScaleGame;
        }
    }
}
