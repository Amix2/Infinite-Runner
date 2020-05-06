using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(GameState.PresentGameState == GameStateValue.Pause);
        GameState.OnStateChange += UpdateGUI;
    }

    void UpdateGUI(GameStateValue gameState)
    {
        gameObject.SetActive(gameState == GameStateValue.Pause);
    }

    public void OnContinueButton()
    {
        GameState.SetState(GameStateValue.Normal);
    }

    public void OnRestartWithTutorial()
    {
        player.GetComponent<PlayerController>().ResetPosition();
        GameState.SetState(GameStateValue.Reset_Tutorial);
    }

    public void OnRestart()
    {
        player.GetComponent<PlayerController>().ResetPosition();
        GameState.SetState(GameStateValue.Reset_Normal);
    }

    public void OnExit()
    {
        player.GetComponent<PlayerController>().ResetPosition();
        GameState.SetState(GameStateValue.Main_Menu);
    }
}
