using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    private GameObject ResultPopUP;

    [SerializeField]
    private GameObject Clear;

    [SerializeField]
    private GameObject Fail;
    private void Awake()
    {
        Inst = this;
    }

    public void Processing()
    {
        TurnOver = curGameState switch
        {
            GameState.Input => GameSystem.Inst.SpawnBlock,
            GameState.Wait => null,
            GameState.Clear => Result,
            GameState.GameOver => Result,
        };

        TurnOver?.Invoke();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Ingame");
    }

    void Result()
    {
        ResultPopUP.SetActive(true);

        if (curGameState == GameState.Clear)
        {
            Clear.SetActive(true);
        }
        else
        {
            Fail.SetActive(true);
        }
    }
}
