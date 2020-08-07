using TMPro;
using UnityEngine;

public class DeathCount : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private int count = 0;

    // Start is called before the first frame update
    private void Start()
    {
        GameState.OnStateChange += OnStateChange;
        textMesh = GetComponent<TextMeshProUGUI>();
        GameObject.Find("Player").GetComponent<PlayerController>().OnDeath += UpdateDeathCount;
        textMesh.text = "Deaths: " + count;
    }

    private void UpdateDeathCount()
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