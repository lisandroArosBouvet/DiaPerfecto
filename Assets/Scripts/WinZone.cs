using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    Action evWinGame;
    internal void SetWinGame(Action winGame)
    {
        evWinGame = winGame;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            evWinGame?.Invoke();
        }
    }
}
