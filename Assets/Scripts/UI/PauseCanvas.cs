using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
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
        GameState.instance.CurrentGameState = GameStateValue.Normal;
    }
}
