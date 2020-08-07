using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GameState.OnStateChange += OnStateChange;
    }

    private void Update()
    {
    }

    private void OnStateChange(GameStateValue gameState)
    {
        if (gameState == GameStateValue.Main_Menu)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnStartButton()
    {
        GameState.SetState(GameStateValue.Reset_Normal);
    }

    public void OnStartTutorialButton()
    {
        GameState.SetState(GameStateValue.Reset_Tutorial);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}