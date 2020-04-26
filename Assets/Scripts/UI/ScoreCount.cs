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
}
