using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject escapeScreen;
    public GameObject player;
    private BasicBehaviour rb;
    public GameObject winScreen;


    private void Start() {
        rb = FindObjectOfType<BasicBehaviour>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            winlose();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 0;
        }
    }

    public void winlose()
    {
        if(!deathScreen.activeSelf)
        {
            escapeScreen.SetActive(!escapeScreen.activeSelf);
            if(escapeScreen.activeSelf)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void DeathScreen()
    {
        deathScreen.SetActive(true);
        rb.enabled = false;
        Time.timeScale = 0;
    }

    public void WinScreen()
    {
        winScreen.SetActive(true);
        rb.enabled = false;
        Time.timeScale = 0;
    }
}
