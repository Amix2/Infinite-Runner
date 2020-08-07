using UnityEngine;

public class MainUI : MonoBehaviour
{
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}