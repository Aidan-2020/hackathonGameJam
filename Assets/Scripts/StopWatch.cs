using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StopWatch : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public float time;

    public bool isRunning = false;

    // Update is called once per frame
    void Update()
    {
        if(isRunning == true)
        {
            time += Time.deltaTime;
            text.text = time.ToString();
        }
    }
}
