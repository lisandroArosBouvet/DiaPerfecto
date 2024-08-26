using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string nextScene = "WakeupGame";
    private void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(nextScene);
    }
}
