using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager
{
    void LoseGame(int loseId);
    void WinGame();
    void InitalConfiguration();

}
