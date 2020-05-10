
using UnityEngine;

public interface IInputMode
{
    bool JumpKey { get; }
    bool LeftKey { get; }
    bool RightKey { get; }
    bool Freeze { get; }
    float MaxForwardSpeed { get; }
}

public class NormalInputMode : IInputMode
{
    public bool JumpKey => Input.GetKey(KeyCode.Space);

    public bool LeftKey => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

    public bool RightKey => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

    public float MaxForwardSpeed => 50f;

    public bool Freeze => false;
}

public class FreezeInputMode : IInputMode
{
    public bool JumpKey => false;

    public bool LeftKey => false;

    public bool RightKey => false;

    public bool Freeze => true;

    public float MaxForwardSpeed => 0;
}