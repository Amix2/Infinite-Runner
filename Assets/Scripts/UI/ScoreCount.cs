using TMPro;
using UnityEngine;

public class ScoreCount : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private int scoreSubtract = 0;
    private int scoreValue = 0;
    int Score => (scoreValue - scoreSubtract);
    private int topScore = 0;
    private PlayerController playerController;

    // Start is called before the first frame update
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.OnDeath += HandleDeath;
        textMesh.text = "Score: " + scoreValue;
        GameState.OnStateChange += OnStateChange;
    }

    private void HandleDeath()
    {
        scoreSubtract += Score / 2;
    }

    private void Update()
    {
        scoreValue = (int)playerController.transform.position.z;
        topScore = topScore < Score ? Score : topScore;
        textMesh.text = "Score: " + Score + "\n" + "(" + topScore + ")";
    }

    private void OnStateChange(GameStateValue gameState)
    {
        if (gameState == GameStateValue.Reset_Normal || gameState == GameStateValue.Reset_Tutorial)
        {
            scoreSubtract = 0;
            scoreValue = 0;
            topScore = 0;
        }
    }
}