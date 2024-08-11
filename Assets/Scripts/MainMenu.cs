using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string nextScene = "WakeupGame";
    public void StartGame()
    {
        SceneManager.LoadScene(nextScene);
    }
}
