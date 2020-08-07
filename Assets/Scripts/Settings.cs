using UnityEngine;

public class Settings : MonoBehaviour
{
    public PlayerSettings player;

    public static PlayerSettings Player
    {
        get { return instance.player; }
    }

    public WorldSettings world;

    public static WorldSettings World
    {
        get { return instance.world; }
    }

    private static Settings instance;

    private void Awake()
    {
        instance = this;
    }
}

[System.Serializable]
public class PlayerSettings
{
}

[System.Serializable]
public class WorldSettings
{
    public int minDistanceInFront = 10;
    public float playerAcceleration = 1f;
    public float maxPlayerVelocity;
}