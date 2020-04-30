using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(GameState.PresentGameState == GameStateValue.Pause || GameState.PresentGameState == GameStateValue.Freeze);
        GameState.OnStateChange += UpdateGUI;
    }

    void UpdateGUI(GameStateValue gameState)
    {
        gameObject.SetActive(gameState == GameStateValue.Pause || gameState == GameStateValue.Freeze);
    }
}
