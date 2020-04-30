using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static IInputMode InputMode => instance.inputMode;
    public static Action<GameStateValue> OnStateChange;
    public static GameStateValue PresentGameState => instance.CurrentGameState;

    public bool startWithTutorial = false;

    public GameStateValue CurrentGameState
    {
        get => currentGameState;
        set {
            OnStateChange?.Invoke(value);
            currentGameState = value; 
            inputMode = GetInputMode(value);
            stateController = GetStateController(value);
        }
    }

    public static GameState instance;
    private IInputMode inputMode;
    private GameStateValue currentGameState = GameStateValue.Normal;
    private GameStateController stateController;
    private bool freezeGame = false;
    private GameStateValue lastGameState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (startWithTutorial)
        {
            CurrentGameState = GameStateValue.New_Tutorial;
        }
        else
        {
            CurrentGameState = GameStateValue.Normal;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (freezeGame)
            {
                OnStateChange?.Invoke(lastGameState);
                CurrentGameState = lastGameState;
            }
            else
            {
                lastGameState = CurrentGameState;
                OnStateChange?.Invoke(GameStateValue.Pause);
                CurrentGameState = GameStateValue.Pause;
            }
            freezeGame = !freezeGame;
        }

        if (!freezeGame)
        {
            GameStateValue newState = stateController.UpdateStateContoller();
            if (newState != CurrentGameState)
            {
                stateController = GetStateController(newState);
                OnStateChange?.Invoke(newState);
                CurrentGameState = newState;
                inputMode = instance.GetInputMode(instance.CurrentGameState);
            }
        }
    }

    public IInputMode GetInputMode(GameStateValue state)
    {
        switch (state)
        {
            case GameStateValue.Normal:
                return new NormalInputMode();

            case GameStateValue.Freeze:
            case GameStateValue.Pause:
                return new FreezeInputMode();

            case GameStateValue.Tutorial:
            case GameStateValue.New_Tutorial:
                return GetComponent<TutorialStateController>();
        }
        return null;
    }

    public GameStateController GetStateController(GameStateValue state)
    {
        switch (state)
        {
            case GameStateValue.Normal:
            case GameStateValue.Freeze:
            case GameStateValue.Pause:
                return GetComponent<DefaultStateController>();

            case GameStateValue.Tutorial:
            case GameStateValue.New_Tutorial:
                return GetComponent<TutorialStateController>();
        }
        return null;
    }
}

public enum GameStateValue
{
    Normal,
    Freeze,
    Tutorial,
    New_Tutorial,
    Pause
}

public abstract class GameStateController : MonoBehaviour
{
    public abstract GameStateValue UpdateStateContoller();
}