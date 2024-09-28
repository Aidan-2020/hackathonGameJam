using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StopWatch : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text totalText;
    public float time;

    public bool isRunning = false;

    public GameObject gun;

    public int maxPlants;

    // Update is called once per frame
    void Update()
    {
        if(isRunning == true)
        {
            time += Time.deltaTime;
            text.text = time.ToString();
        }
    }

    public void showScore()
    {
        scoreText.text = gun.GetComponent<Gun>().score.ToString();
        totalText.text = maxPlants.ToString();
    }

}
