using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    Action<SituationType> evLoseGame;
    public SituationType loseType = 0;
    internal void SetLoseGame(Action<SituationType> loseGame)
    {
        evLoseGame = loseGame;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            evLoseGame?.Invoke(loseType);
        }
    }
}
