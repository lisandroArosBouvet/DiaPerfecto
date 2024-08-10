using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    Action evLoseGame;
    internal void SetLoseGame(Action loseGame)
    {
        evLoseGame = loseGame;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            evLoseGame?.Invoke();
        }
    }
}
