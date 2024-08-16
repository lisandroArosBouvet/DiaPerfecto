using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GotoJobManager : MonoBehaviour, IGameManager
{
    public SoundFX soundFxManager;
    public Car car;
    public WatchCooldown watchCooldown;
    public Transform outLevelPosition; 
    public Transform startLevelPosition;
    public Button startLevelBtn;
    [Header("Dialogos")]
    private PatrolUnit[] patrols;
    const GameType NAME_GAME = GameType.GotoJob; 
    public string
        NEXT_SCENE = "SampleScene",
        LOSE_SCENE = "GotoJobGame"
        ;
    public void InitalConfiguration()
    {
        startLevelBtn.gameObject.SetActive(false);
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
        watchCooldown.SetTimeAndFinishEvent(20,()=> LoseGame(SituationType.SeTerminoElTiempo));
        foreach (var patrol in patrols)
        {
            patrol.Respawn();
        }
    }
    private void EndGame()
    {
        car.Die();
    }

    public void LoseGame(SituationType type)
    {
        soundFxManager.Fail();
        watchCooldown.StopWatch();
        EndGame();
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME,ConditionType.LoseGame, type, ()=> SceneManager.LoadScene(LOSE_SCENE));
    }

    public void WinGame()
    {
        soundFxManager.Ganaste();
        watchCooldown.StopWatch();
        EndGame();
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.WinGame, () => SceneManager.LoadScene(NEXT_SCENE));
    }

    void Start()
    {
        InitalConfiguration();
    }
}
