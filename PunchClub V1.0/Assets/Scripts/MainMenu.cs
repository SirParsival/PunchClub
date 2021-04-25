using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Coroutine loadingRoutine;

    void Start()
    {
        Application.targetFrameRate = 60;
        //AudioManager.Instance.GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Qitt!");
            Application.Quit();
        }
    }

    public void GoToGame()
    {
        if (loadingRoutine == null)
        {
            loadingRoutine = StartCoroutine(LoadGameScene(2.0f));
        }
    }

    public void QuitGame()
    {
        Debug.Log("Qitt!");
        Application.Quit();
    }

    private IEnumerator LoadGameScene(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        GameManager.CurrentLevel = 0;
        SceneManager.LoadScene("Game");
    }
}
