using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialStateController : GameStateController, IInputMode
{
    public ChunkSpawner chunkSpawner;
    public GameObject tutorialChunk;
    public GameObject player;

    public GameObject spaceToCont;
    public GameObject blackCanvas;
    public GameObject welcomeMsg;
    public GameObject sceneInfo;
    public GameObject laneSwitchInfo;
    public GameObject avoidWallsInfo;
    public GameObject singleJumpInfo;
    public GameObject multipleJumpInfo;
    public GameObject endTutorial;

    private NormalInputMode normalInput = new NormalInputMode();
    private bool freezeCharacter = false;
    private bool blockLaneSwitch = true;
    private bool blockJump = true;

    public bool JumpKey => freezeCharacter || blockJump ? false : normalInput.JumpKey;

    public bool LeftKey => freezeCharacter || blockLaneSwitch ? false : normalInput.LeftKey;

    public bool RightKey => freezeCharacter || blockLaneSwitch ? false : normalInput.RightKey;

    public bool Freeze => freezeCharacter;

    public float MaxForwardSpeed => freezeCharacter ? 0f : normalInput.MaxForwardSpeed;

    private float DistanceFromStart => player.transform.position.z;
    private float[] stopArray = { 2, 5, 5, 1, 23, 15, 35 };

    private int stateIndex = 0;
    private float nextStop;

    public override GameStateValue UpdateStateContoller()
    {
        if (stateIndex == 0 && DistanceFromStart > nextStop)
        {   // welcome
            welcomeMsg.SetActive(true);
            if (WaitToUnfreeze())
            {
                welcomeMsg.SetActive(false);
            }
        }

        if (stateIndex == 1 && DistanceFromStart > nextStop)
        {   // welcome
            sceneInfo.SetActive(true);
            if (WaitToUnfreeze())
            {
                sceneInfo.SetActive(false);
            }
        }

        if (stateIndex == 2 && DistanceFromStart > nextStop)
        {   // welcome
            laneSwitchInfo.SetActive(true);
            if (WaitToUnfreeze())
            {
                laneSwitchInfo.SetActive(false);
                blockLaneSwitch = false;
            }
        }

        if (stateIndex == 3 && DistanceFromStart > nextStop)
        {   // welcome
            avoidWallsInfo.SetActive(true);
            if (WaitToUnfreeze())
            {
                avoidWallsInfo.SetActive(false);
            }
        }

        if (stateIndex == 4 && DistanceFromStart > nextStop)
        {   // welcome
            singleJumpInfo.SetActive(true);
            if (WaitToUnfreeze())
            {
                blockJump = false;
                singleJumpInfo.SetActive(false);
            }
        }

        if (stateIndex == 5 && DistanceFromStart > nextStop)
        {   // welcome
            multipleJumpInfo.SetActive(true);
            if (WaitToUnfreeze())
            {
                multipleJumpInfo.SetActive(false);
            }
        }

        if (stateIndex == 6 && DistanceFromStart > nextStop)
        {   // welcome
            endTutorial.SetActive(true);
            if (WaitToUnfreeze())
            {
                endTutorial.SetActive(false);
            }
        }

        if (stateIndex == stopArray.Length) return GameStateValue.Normal;
        return GameStateValue.Tutorial;
    }

    public void ResetTutorial()
    {
        DisableAllPanels();
        freezeCharacter = false;
        blockLaneSwitch = true;
        blockJump = true;

        stateIndex = 0;
        nextStop = stopArray[stateIndex];
        chunkSpawner.ClearChunks();
        chunkSpawner.SetNextChunk(tutorialChunk);
        player.transform.position = new Vector3(0, 1.5f, 0);
    }

    private void Awake()
    {
        GameState.OnStateChange += OnStateChange;
    }

    private void OnStateChange(GameStateValue gameState)
    {
        if (gameState == GameStateValue.Reset_Tutorial)
        {
            ResetTutorial();
        }
        if (gameState == GameStateValue.Tutorial)
        {
            player.GetComponent<PlayerController>().OnDeath += OnDeath;
        }
        if (gameState != GameStateValue.Reset_Tutorial && gameState != GameStateValue.Tutorial)
        {
            DisableAllPanels();
            player.GetComponent<PlayerController>().OnDeath -= OnDeath;
        }
    }

    private void DisableAllPanels()
    {
        spaceToCont.SetActive(false);
        blackCanvas.SetActive(false);
        welcomeMsg.SetActive(false);
        sceneInfo.SetActive(false);
        laneSwitchInfo.SetActive(false);
        avoidWallsInfo.SetActive(false);
        singleJumpInfo.SetActive(false);
        multipleJumpInfo.SetActive(false);
        endTutorial.SetActive(false);
    }

    private void OnDeath()
    {
        float spawnPos = 0;
        for(int i=0; i< stateIndex; i++)
        {
            spawnPos += stopArray[i];
        }
        nextStop -= stopArray[stateIndex];
        stateIndex--;
        player.GetComponent<PlayerController>().ResetPosition();
        player.transform.position = new Vector3(0, 0.65f, spawnPos);
    }

    public void OnClickContinue()
    {
        endTutorial.SetActive(false);
        GameState.SetState(GameStateValue.Normal);
    }

    public void OnClickRepeat()
    {
        GameState.SetState(GameStateValue.Tutorial);
    }

    private bool WaitToUnfreeze()
    {
        freezeCharacter = true;
        if (Input.GetKey(KeyCode.Return))
        {
            spaceToCont.SetActive(false);
            blackCanvas.SetActive(false);
            freezeCharacter = false;
            stateIndex++;
            if (stateIndex < stopArray.Length) nextStop += stopArray[stateIndex];
            else nextStop = float.MaxValue;
            return true;
        }
        else
        {
            blackCanvas.SetActive(true);
            spaceToCont.SetActive(true);
        }
        return false;
    }
}