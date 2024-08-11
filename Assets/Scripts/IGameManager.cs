using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager
{
    void LoseGame(SituationType type);
    void WinGame();
    void InitalConfiguration();

}
