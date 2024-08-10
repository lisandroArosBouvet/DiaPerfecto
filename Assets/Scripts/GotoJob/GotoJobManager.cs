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
    public DialogManager DialogManager;
    const string STAR_NAME = "Star";
    private PatrolUnit[] patrols;
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

        List<DialogData> dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Normal/Ya pudiste salir de tu casa. Ahora empieza lo dificil.", STAR_NAME),
                new DialogData("/emote:Saludo/Tendras que esquivar todos los obstaculos de la vida moderna!",STAR_NAME, ()=> startLevelBtn.gameObject.SetActive(true))
            };
        DialogManager.Show(dialogs);
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

    public void LoseGame(int loseId)
    {
        EndGame();
        List<DialogData> dialogs;
        string goScene = "GotoJob";
        switch (loseId)
        {
            case 0: //Chocaste contra un edificio.
                dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Enojado/Te la diste contra un edificio... Mira que esos no se mueven eh", STAR_NAME, ()=>SceneManager.LoadScene(goScene))
            };
                break;
            case 1: //Saliste de los limites del mapa.
                dialogs = new List<DialogData>()
            {
                new DialogData("/emote:MuyEnojado/A donde crees que vas? Te fuiste al chori...", STAR_NAME, ()=>SceneManager.LoadScene(goScene))
            };
                break;
            case 2: //Chocaste contra la valla de contencion
                dialogs = new List<DialogData>()
            {
                new DialogData("/emote:MuyEnojado/Pero si seras... como no ves esas cosas?", STAR_NAME, ()=>SceneManager.LoadScene( goScene))
            };
                break;
            case 3: // Chocaste a la ancianita
                dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Sorprendido/Pero... si va a dos por hora como...", STAR_NAME),
                new DialogData("/emote:Frustrado/Bueno, de todas formas no le quedaba mucho, las estrellas sabemos cosas.", STAR_NAME),
                new DialogData("/emote:Enojado/Pero eso no quita que halla que empezar otra vez!", STAR_NAME,()=>SceneManager.LoadScene(goScene))
            };
                break;
            case 4: //chocaste al trabajador
                dialogs = new List<DialogData>()
            {
                    new DialogData("/emote:Sorprendido/De verdad chocaste a un obrero?", STAR_NAME),
                new DialogData("/emote:Frustrado/No puedo culparte... no deberias haber salido de la cama hoy.", STAR_NAME, ()=>SceneManager.LoadScene(goScene))
            };
                break;
            default:
                dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Sorprendido/Esto no esta programado.", STAR_NAME, ()=>SceneManager.LoadScene(goScene))
            };
                break;
        }
        DialogManager.Show(dialogs);
    }

    public void WinGame()
    {
        EndGame();
        List<DialogData> dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Saludo/Grande, maquina, titan, messi!", STAR_NAME),
                new DialogData("/emote:Sarcastico/Ahora podras comenzar a trabajar en tu maravilloso trabajo...",STAR_NAME,  ()=>SceneManager.LoadScene("SampleScene"))
            };
        DialogManager.Show(dialogs);
    }

    void Start()
    {
        InitalConfiguration();
    }
}
