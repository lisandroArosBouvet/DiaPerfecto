using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToastGame : MonoBehaviour, IGameManager
{
    [SerializeField] GameObject toast;
    public Transform startPoint;   // Punto de inicio
    public Transform endPoint;     // Punto final
    public float speed = 1.0f;   // Velocidad de oscilación

    private Rigidbody2D rb;
    private bool isHeld = false;
    private float timeToOscilian = 0;

    public float stillThreshold = 0.01f;

    [Header("WinConditions")]
    public SpriteRenderer[] toastInMachineGraphics;
    public int toastToWin = 2;
    private int _toastInMachine = 0;
    private bool _startGame = false;
    [Header("Dialogos")]
    public DialogManager DialogManager;
    string starName = "Star";

    void Start()
    {
        InitalConfiguration();
    }

    void Update()
    {
        if (_startGame == false)
            return;
        if (isHeld)
        {
            timeToOscilian += Time.deltaTime;
            rb.velocity = new Vector2(Mathf.Sin(timeToOscilian) *speed, 0);
        }else
        {
            if(rb.velocity.magnitude < stillThreshold)
            {
                LoseGame();
            }
        }

        // Detectar si el botón del ratón está siendo presionado o soltado
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseBall();
        }
    }

    void ReleaseBall()
    {
        isHeld = false;
        rb.gravityScale = 1;
        //rb.velocity = CalculateReleaseVelocity(); // Mantener la inercia en el momento de la liberación
    }

    public void LoseGame()
    {
        _startGame = false;
        List<DialogData> dialogs = new List<DialogData>()
        {
            new DialogData("/emote:Enojado/¡¡QUE!! ¡No podes! ¡El dia perfecto ahora esta lleno de bacterias!", starName),
            new DialogData("/emote:MuyEnojado/¿¡Que tan dificil es poner un pan en una tostadora!?", starName, ()=>SceneManager.LoadScene("SampleScene")),
        };
        DialogManager.Show(dialogs);
    }

    public void WinGame()
    {
        if(_toastInMachine < toastInMachineGraphics.Length)
            toastInMachineGraphics[_toastInMachine].enabled = true;
        _toastInMachine++;
        if (_toastInMachine >= toastToWin)
        {
            _startGame = false;
            rb.MovePosition(endPoint.position);
            List<DialogData> dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Sorprendido/¿Lo lograste?", starName),
                new DialogData("/emote:Sarcastico/Si, lo lograste, supongo, ningun desafio para tus pulgares opuestos supongo.", starName),
                new DialogData("/emote:Saludo/¡Continuemos con tus flipantes aventuras!", starName,  ()=>SceneManager.LoadScene("SampleScene"))
            };
            DialogManager.Show(dialogs);
        }
        else
            ResetGame();
    }

    public void ResetGame()
    {
        rb.velocity = Vector2.zero;
        rb.MovePosition(startPoint.position);
        isHeld = true;
        rb.gravityScale = 0;
        timeToOscilian = 0;
        rb.rotation = 0;
        rb.angularVelocity = 0;
    }

    public void InitalConfiguration()
    {
        rb = toast.GetComponent<Rigidbody2D>();
        ResetGame();
        FindAnyObjectByType<DeathZone>().SetLoseGame(LoseGame);
        FindAnyObjectByType<WinZone>().SetWinGame(WinGame);

        foreach (var graphic in toastInMachineGraphics)
        {
            graphic.enabled = false;
        }
        List<DialogData> dialogs = new List<DialogData>()
            {
                new DialogData("/emote:Saludo/Hola! Soy Strellin! Y juntos lograremos que tengas un dia perfecto.", starName),
                new DialogData("/emote:Normal/El desayuno es la comida mas importante del dia! No la arruines con salmonella", starName),
                new DialogData("/emote:Feliz/Solo pon el pan dentro de la tostadora! /emote:Frustrado/ No es dificil... ¿Verdad?", starName,  ()=> _startGame = true)
            };
        DialogManager.Show(dialogs);
    }
}
