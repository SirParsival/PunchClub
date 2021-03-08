using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.GetComponent<AudioSource>().Play();
    }

    public void GoToGame()
    {
        GameManager.CurrentLevel = 0;
        //3
        SceneManager.LoadScene("Game");
    }
}
