using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCount : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    int scoreSubtract = 0;
    int scoreValue = 0;
    int Score => (scoreValue - scoreSubtract);
    int topScore = 0;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.OnDeath += HandleDeath;
        textMesh.text = "Score: " + scoreValue;
        GameState.OnStateChange += OnStateChange;
    }

    void HandleDeath()
    {
        scoreSubtract += Score / 2;
    }

    private void Update()
    {
        scoreValue = (int) playerController.transform.position.z;
        topScore = topScore < Score ? Score : topScore;
        textMesh.text = "Score: " + Score + "\n" + "(" + topScore + ")";
    }

    private void OnStateChange(GameStateValue gameState)
    {
        if(gameState == GameStateValue.Reset_Normal || gameState == GameStateValue.Reset_Tutorial)
        {
            scoreSubtract = 0;
            scoreValue = 0;
            topScore = 0;
        }
    }
}
