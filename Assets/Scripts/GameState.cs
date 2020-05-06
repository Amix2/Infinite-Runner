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
        private set {
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
    private bool FreezeGame => CurrentGameState == GameStateValue.Pause;
    private GameStateValue lastGameState;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CurrentGameState = GameStateValue.Main_Menu;
    }

    public static void SetState(GameStateValue gameState)
    {
        instance.CurrentGameState = gameState;
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape") && currentGameState != GameStateValue.Main_Menu)
        {
            if (FreezeGame)
            {
                CurrentGameState = lastGameState;
            }
            else
            {
                lastGameState = CurrentGameState;
                CurrentGameState = GameStateValue.Pause;
            }
        }

        GameStateValue newState = stateController.UpdateStateContoller();
        if (newState != CurrentGameState)
        {
            stateController = GetStateController(newState);
            CurrentGameState = newState;
            inputMode = instance.GetInputMode(instance.CurrentGameState);
        }
    }

    public IInputMode GetInputMode(GameStateValue state)
    {
        switch (state)
        {
            case GameStateValue.Normal:
            case GameStateValue.Reset_Normal:
            case GameStateValue.Main_Menu:
                return new NormalInputMode();

            case GameStateValue.Pause:
                return new FreezeInputMode();

            case GameStateValue.Tutorial:
            case GameStateValue.Reset_Tutorial:
                return GetComponent<TutorialStateController>();
        }
        return null;
    }

    public GameStateController GetStateController(GameStateValue state)
    {
        switch (state)
        {
            case GameStateValue.Normal:
            case GameStateValue.Reset_Normal:
                return GetComponent<DefaultStateController>();

            case GameStateValue.Pause:
            case GameStateValue.Main_Menu:
                return GetComponent<PreserveStateController>();

            case GameStateValue.Tutorial:
            case GameStateValue.Reset_Tutorial:
                return GetComponent<TutorialStateController>();
        }
        return null;
    }
}

public enum GameStateValue
{
    Normal,
    Reset_Normal,
    Tutorial,
    Reset_Tutorial,
    Pause,
    Main_Menu
}

public abstract class GameStateController : MonoBehaviour
{
    public abstract GameStateValue UpdateStateContoller();
}