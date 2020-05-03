using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 1f;
    public float jumpDistance = 8f;
    public LayerMask groundLayerMask;
    public LayerMask frontLayerMask;
    public LayerMask sideLayerMask;
    public LayerMask sidePanelLayerMask;
    public float forwardSpeed = 1f;
    public float safeGapToSide = 0.2f;
    public float maxLineSwitchSpeed = 10f;
    public float laneSwitchInitSpeed = 4f;
    public float laneSwitchAcceleration = 8f;
    private float laneSwitchSpeedCurrent = 0f;

    public ChunkSpawner chunkSpawner;

    public Action OnDeath;

    private float ForwardVelocity => Mathf.Sqrt(forwardSpeed);
    private float JumpSpeedY => 4 * ForwardVelocity * jumpHeight / jumpDistance;

    private float GravityY => 8 * ForwardVelocity * ForwardVelocity * jumpHeight / (jumpDistance * jumpDistance);

    private Vector3 startPosition;
    private bool jumped = false;
    private int targetXPosition;
    private Vector3 velocity = Vector3.zero;

    private Collider[] overlapBoxArray;
    private float lastCollisionTime = -1000f;
    private float collisionInterval = 1f;

    bool IsGrounded => Physics.OverlapBoxNonAlloc(CheckPosisionGround, CheckBoxGround, overlapBoxArray, Quaternion.identity, groundLayerMask) > 0;
    bool SwitchingLanes => Mathf.Abs(transform.position.x - targetXPosition) > 0.001f;
    bool IsOnTargetLane => Mathf.Abs(transform.position.x - targetXPosition) < 0.0001;
    float PositionX => transform.position.x;
    bool ShouldGoLeft => targetXPosition - PositionX < 0;
    bool ShouldGoRight => targetXPosition - PositionX > 0;

    private Vector3 CheckPosisionGround => transform.position + Vector3.down * 0.5f * transform.localScale.y;
    private Vector3 CheckBoxGround => new Vector3(transform.localScale.x * 0.25f, transform.localScale.y * 0.1f, transform.localScale.z * 0.25f);

    private Vector3 CheckBoxSide => new Vector3(transform.localScale.x * 0.2f, transform.localScale.y * 0.3f, transform.localScale.z * 0.3f);
    private Vector3 CheckBoxFront => new Vector3(transform.localScale.x * 0.1f, transform.localScale.y * 0.1f, transform.localScale.z * 0.1f);

    private Vector3 CheckPosisionLeft => transform.position + Vector3.left * 0.15f * transform.localScale.x + Vector3.forward * 0.05f * transform.localScale.z;
    private bool LeftBoxCollision => Physics.OverlapBoxNonAlloc(CheckPosisionLeft, CheckBoxSide, overlapBoxArray, Quaternion.identity, sideLayerMask) > 0;

    private Vector3 CheckPosisionRight => transform.position + Vector3.right * 0.15f * transform.localScale.x + Vector3.forward * 0.05f * transform.localScale.z;
    private bool RightBoxCollision => Physics.OverlapBoxNonAlloc(CheckPosisionRight, CheckBoxSide, overlapBoxArray, Quaternion.identity, sideLayerMask) > 0;

    private Vector3 CheckPosisionFront => transform.position + Vector3.forward * 0.2f * transform.localScale.z;
    private bool FrontBoxCollision => Physics.OverlapBoxNonAlloc(CheckPosisionFront, CheckBoxFront, overlapBoxArray, Quaternion.identity, frontLayerMask) > 0;

    // Start is called before the first frame update
    private void Start()
    {
        targetXPosition = Mathf.RoundToInt(transform.position.x);
        overlapBoxArray = new Collider[10];
        startPosition = transform.position;
        ResetPosition();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameState.InputMode.Freeze)
        {
            if (FrontBoxCollision)
            {
                if (lastCollisionTime + collisionInterval < Time.realtimeSinceStartup)
                {
                    OnDeath?.Invoke();
                    lastCollisionTime = Time.realtimeSinceStartup;
                }
            }

            forwardSpeed = Settings.World.playerAcceleration * Mathf.Sqrt(transform.position.z + 1);  // forward acceleration

            velocity.y -= GravityY * Time.deltaTime;   // gravity
            velocity.z = ForwardVelocity;

            if (!jumped && IsGrounded && GameState.InputMode.JumpKey)
            {
                velocity.y = JumpSpeedY;
                jumped = true;
            }
            if (!IsGrounded) jumped = false;
            if (IsGrounded && velocity.y < 0) velocity.y = 0;

            if (GameState.InputMode.LeftKey)
            {
                // left     x  <
                if (targetXPosition > -1 * CurrentNumOfLanes() / 2 && (ShouldGoRight || IsOnTargetLane))
                    targetXPosition--;
            }
            if (GameState.InputMode.RightKey)
            {   // right    > x
                if (targetXPosition < CurrentNumOfLanes() / 2 && (ShouldGoLeft || IsOnTargetLane))
                    targetXPosition++;
            }

            if (IsOnTargetLane)
            {
                transform.position = new Vector3(targetXPosition, transform.position.y, transform.position.z);
                velocity.x = 0f;
                laneSwitchSpeedCurrent = 0f;
            }
            else
            {
                if (laneSwitchSpeedCurrent < laneSwitchInitSpeed)
                {
                    laneSwitchSpeedCurrent = laneSwitchInitSpeed;
                }
                float velToLine = Mathf.Min(ForwardVelocity, (Mathf.Abs(PositionX - targetXPosition) / Time.deltaTime), laneSwitchSpeedCurrent + laneSwitchAcceleration * Time.deltaTime);
                if (ShouldGoLeft)
                {
                    // go left
                    if (!LeftBoxCollision)
                    {
                        velocity.x = -velToLine;
                        laneSwitchSpeedCurrent += laneSwitchAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        velocity.x = 0;
                    }
                }
                else
                {
                    // go right
                    if (!RightBoxCollision)
                    {
                        velocity.x = +velToLine;
                        laneSwitchSpeedCurrent += laneSwitchAcceleration * Time.deltaTime;
                    }
                    else
                    {
                        velocity.x = 0;
                    }
                }
            }

            if (velocity.z > GameState.InputMode.MaxForwardSpeed)
            {
                velocity.z = GameState.InputMode.MaxForwardSpeed;
                forwardSpeed = GameState.InputMode.MaxForwardSpeed * GameState.InputMode.MaxForwardSpeed;
            }
            transform.position += velocity * Time.deltaTime;
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        targetXPosition = 0;
    }

    private int CurrentNumOfLanes()
    {
        int colNum = Physics.OverlapBoxNonAlloc(transform.position, new Vector3(20f, 10f, 1f), overlapBoxArray, Quaternion.identity, sidePanelLayerMask);
        int maxPos = int.MinValue;
        int minPos = int.MaxValue;
        for (int i = 0; i < colNum; i++)
        {
            Collider collider = overlapBoxArray[i];
            if (maxPos < collider.transform.position.x) maxPos = (int)collider.transform.position.x;
            if (minPos > collider.transform.position.x) minPos = (int)collider.transform.position.x;
        }

        return maxPos - minPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(CheckPosisionGround, CheckBoxGround);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(CheckPosisionLeft, CheckBoxSide);
        Gizmos.DrawWireCube(CheckPosisionRight, CheckBoxSide);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(CheckPosisionFront, CheckBoxFront);
    }
}