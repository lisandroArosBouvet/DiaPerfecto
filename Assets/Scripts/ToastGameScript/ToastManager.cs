using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Metadata;

public class ToastManager : MonoBehaviour, IGameManager
{
    public SoundFX soundFxManager;
    [SerializeField] GameObject toast;
    public Transform startPoint;   // Punto de inicio
    public Transform endPoint;     // Punto final
    public float speed = 1.0f;   // Velocidad de oscilación


    private Rigidbody2D rb;
    public GameObject[] toastBones;
    private bool isHeld = false;
    private float timeToOscilian = 0;

    public float stillThreshold = 0.04f;

    [Header("WinConditions")]
    public SpriteRenderer[] toastInMachineGraphics;
    public int toastToWin = 2;
    private int _toastInMachine = 0;
    private bool _startGame = false;
    const GameType NAME_GAME =  GameType.Toast;

    public string
        NEXT_SCENE = "GotoJobGame",
        LOSE_SCENE = "ToastGame"
        ;

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
            if (rb.velocity.magnitude < stillThreshold)
            {
                LoseGame(SituationType.SobreLaMesa);
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
        foreach (var c in toastBones)
        {
            c.SetActive(true);
            c.GetComponent<Rigidbody2D>().velocity = rb.velocity;
        }
        //rb.velocity = CalculateReleaseVelocity(); // Mantener la inercia en el momento de la liberación
    }

    public void LoseGame(SituationType loseType)
    {
        soundFxManager.Fail();
        if (_startGame == false) return;
        _startGame = false;
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.LoseGame,loseType, () => SceneManager.LoadScene(LOSE_SCENE));
    }

    public void WinGame()
    {
        if (_startGame == false)
            return;
        _startGame = false;
        if (_toastInMachine < toastInMachineGraphics.Length)
            toastInMachineGraphics[_toastInMachine].enabled = true;
        _toastInMachine++;
        if (_toastInMachine >= toastToWin)
        {
            rb.MovePosition(endPoint.position);
            soundFxManager.Ganaste();
            ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.WinGame, () => SceneManager.LoadScene(NEXT_SCENE));
        }
        else
        {
            soundFxManager.Tostadora();
            ResetGame();
            _startGame = true;
        }
    }

    public void ResetGame()
    {
        rb.velocity = Vector2.zero;
        rb.MovePosition(startPoint.position);
        rb.gravityScale = 0;
        timeToOscilian = 0;
        rb.rotation = 0;
        rb.angularVelocity = 0;
        isHeld = true;
        foreach (var c in toastBones)
        {
            c.SetActive(false);
        }
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
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME,ConditionType.Initial,()=> _startGame = true);

    }
}
