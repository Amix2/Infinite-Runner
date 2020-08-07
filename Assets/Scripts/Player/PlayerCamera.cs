using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject sideCamera;

    private void Start()
    {
        GameState.OnStateChange += OnStateChange;
    }

    private void OnStateChange(GameStateValue gameState)
    {
        if (gameState == GameStateValue.Main_Menu)
        {
            sideCamera.SetActive(true);
            mainCamera.SetActive(false);
        }
        else
        {
            mainCamera.SetActive(true);
            sideCamera.SetActive(false);
        }
    }
}