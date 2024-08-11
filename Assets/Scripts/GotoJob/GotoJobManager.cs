using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GotoJobManager : MonoBehaviour, IGameManager
{
    public Car car;
    public Transform outLevelPosition; 
    public Transform startLevelPosition;
    public Button startLevelBtn;
    [Header("Dialogos")]
    private PatrolUnit[] patrols;
    const GameType NAME_GAME = GameType.GotoJob;
    public void InitalConfiguration()
    {
        car.enabled = false;
        car.transform.position = outLevelPosition.position;
        patrols = FindObjectsByType<PatrolUnit>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        var deathZones = FindObjectsByType<DeathZone>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        foreach (var zone in deathZones)
        {
            zone.SetLoseGame(LoseGame);
        }
        FindAnyObjectByType<WinZone>().SetWinGame(WinGame);

        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.Initial, () => startLevelBtn.gameObject.SetActive(true));
    }

    public void StartGame()
    {
        car.transform.position = startLevelPosition.position;
        car.enabled = true;
        foreach (var patrol in patrols)
        {
            patrol.Respawn();
        }
    }
    private void EndGame()
    {
        Destroy(car.gameObject);
    }

    public void LoseGame(SituationType type)
    {
        EndGame();
        string goScene = "GotoJob";
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME,ConditionType.LoseGame, type, ()=> SceneManager.LoadScene(goScene));
    }

    public void WinGame()
    {
        EndGame();
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.WinGame, () => SceneManager.LoadScene("SampleScene"));
    }

    void Start()
    {
        InitalConfiguration();
    }
}
