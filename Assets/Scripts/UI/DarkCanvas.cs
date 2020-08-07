using UnityEngine;

public class DarkCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        gameObject.SetActive(GameState.PresentGameState == GameStateValue.Pause);
        GameState.OnStateChange += UpdateGUI;
    }

    private void UpdateGUI(GameStateValue gameState)
    {
        gameObject.SetActive(gameState == GameStateValue.Pause);
    }
}