using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class FinishLine : MonoBehaviour
{
    public Button nextLvl;
    public Button restartLvl;
    public Button mainMenu;

    public GameObject nextLevelGO;

    public GameObject stopWatch;

    public GameObject scoreStuff;

    public void Start()
    {
        nextLvl.gameObject.SetActive(false);
        restartLvl.gameObject.SetActive(false);
        scoreStuff.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            scoreStuff.gameObject.SetActive(true);
            other.GetComponentInParent<PlayerMovement>().enabled = false;
            nextLvl.gameObject.SetActive(true);
            restartLvl.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            EventSystem.current.SetSelectedGameObject(nextLevelGO);
            stopWatch.GetComponent<StopWatch>().isRunning = false;
            stopWatch.GetComponent<StopWatch>().showScore();
        }
    }

    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void nextLevel()
    {
        SceneManager.LoadScene("realSecondLevel");
    }

    public void mainMenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
