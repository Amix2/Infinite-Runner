using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathCount : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameState.OnStateChange += OnStateChange;
        textMesh = GetComponent<TextMeshProUGUI>();
        GameObject.Find("Player").GetComponent<PlayerController>().OnDeath += UpdateDeathCount;
        textMesh.text = "Deaths: " + count;
    }

    void UpdateDeathCount()
    {
        count++;
        textMesh.text = "Deaths: " + count;
    }

    private void OnStateChange(GameStateValue gameState)
    {
        if (gameState == GameStateValue.Reset_Normal || gameState == GameStateValue.Reset_Tutorial)
        {
            count = 0;
            textMesh.text = "Deaths: " + count;
        }
    }
}
