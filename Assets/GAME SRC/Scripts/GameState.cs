using System;
using UnityEngine.Events;

[Serializable]
public class GameState
{
    public UnityEvent OnLost, InGame;
    public State State { get; private set; }

    public  GameState() 
    {
        OnLost = new UnityEvent();
        InGame = new UnityEvent();
        State = State.Pause;
        OnLost.AddListener(PauseGame);
        InGame.AddListener(ContinueGame);
    }

    private void PauseGame() 
    {
        State = State.Pause;
    }
    private void ContinueGame() 
    {
        State = State.InGame;
    }
}
public enum State 
{
    Pause,
    InGame
}