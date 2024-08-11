using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToastManager : MonoBehaviour, IGameManager
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
    const string NAME_GAME = "Toast";
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
                LoseGame(0);
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

    public void LoseGame(SituationType loseType)
    {
        if(_startGame == false) return;
        _startGame = false;
        ExcelReaderManager.GetInstance().EnterDialogue(NAME_GAME, ConditionType.LoseGame, () => SceneManager.LoadScene("SampleScene"));
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
            ExcelReaderManager.GetInstance().EnterDialogue(NAME_GAME, ConditionType.WinGame, () => SceneManager.LoadScene("SampleScene"));
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
        ExcelReaderManager.GetInstance().EnterDialogue(NAME_GAME,ConditionType.Initial,()=> _startGame = true);
    }
}
