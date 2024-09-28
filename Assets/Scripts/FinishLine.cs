using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public Button nextLvl;
    public Button restartLvl;

    public void Start()
    {
        nextLvl.gameObject.SetActive(false);
        restartLvl.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            print("finished!!");
            other.GetComponentInParent<PlayerMovement>().enabled = false;
            nextLvl.gameObject.SetActive(true);
            restartLvl.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void restartLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
