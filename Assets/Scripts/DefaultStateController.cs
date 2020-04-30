using UnityEngine;
using System.Collections;

public class DefaultStateController : GameStateController
{
    public override GameStateValue UpdateStateContoller()
    {
        return GameStateValue.Normal;
    }
}