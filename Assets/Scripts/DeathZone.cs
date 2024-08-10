using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    Action<int> evLoseGame;
    public int loseId = 0;
    internal void SetLoseGame(Action<int> loseGame)
    {
        evLoseGame = loseGame;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            evLoseGame?.Invoke(loseId);
        }
    }
}
