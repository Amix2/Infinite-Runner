public class PreserveStateController : GameStateController
{
    public override GameStateValue UpdateStateContoller()
    {
        return GameState.PresentGameState;
    }
}