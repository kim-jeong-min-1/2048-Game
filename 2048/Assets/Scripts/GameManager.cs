using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState
{
    Input,
    Wait,
    Clear,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; set; }
    public Action TurnOver;

    public GameState curGameState = GameState.Wait;

    private void Awake()
    {
        Inst = this;

        TurnOver += () => { curGameState = GameState.Input; };
    }

    public void Processing()
    {
        Action action = curGameState switch
        {
            GameState.Input => null,
            GameState.Wait => TurnOver,
            GameState.Clear => null,
            GameState.GameOver => null,
        };

        action?.Invoke();
    }
}
