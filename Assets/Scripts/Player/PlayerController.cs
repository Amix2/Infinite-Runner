using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 1f;
    public float jumpDistance = 8f;
    public float laneSwitchSpeed = 1f;
    public LayerMask groundLayerMask;
    public LayerMask frontLayerMask;
    public LayerMask sideLayerMask;
    public LayerMask sidePanelLayerMask;
    public float forwardSpeed = 1f;
    public float safeGapToSide = 0.2f;
    public float maxLineSwitchSpeed = 10f;

    public ChunkSpawner chunkSpawner;

    public Action OnDeath;

    private float ForwardVelocity => Mathf.Sqrt(forwardSpeed);
    private float JumpSpeedY => 4 * ForwardVelocity * jumpHeight / jumpDistance;
    private float GravityY => 8 * ForwardVelocity * ForwardVelocity * jumpHeight / (jumpDistance * jumpDistance);

    private bool jumped = false;
    private int targetXPosition;
    private bool crouch = false;
    private Vector3 velocity = Vector3.zero;


    private Collider[] overlapBoxArray;
    private Chunk lastDeathChunk = null;


    bool IsGrounded => Physics.OverlapBoxNonAlloc(CheckPosisionGround, CheckBoxGround, overlapBoxArray, Quaternion.identity, groundLayerMask) > 0;
    bool SwitchingLanes => Mathf.Abs(transform.position.x - targetXPosition) > 0.01f;
    bool IsOnTargetLane => Mathf.Abs(transform.position.x - targetXPosition) < 0.01;
    float PositionX => transform.position.x;
    bool ShouldGoLeft => targetXPosition - PositionX < 0;
    bool ShouldGoRight => targetXPosition - PositionX > 0;

    private Vector3 CheckPosisionGround => transform.position + Vector3.down * 0.5f * transform.localScale.y;
    private Vector3 CheckBoxGround => new Vector3(transform.localScale.x * 0.25f, transform.localScale.y * 0.1f, transform.localScale.z * 0.25f);

    private Vector3 CheckBoxSide => new Vector3(transform.localScale.x * 0.2f, transform.localScale.y * 0.3f, transform.localScale.z * 0.3f);
    private Vector3 CheckBoxFront => new Vector3(transform.localScale.x * 0.2f, transform.localScale.y * 0.2f, transform.localScale.z * 0.1f);

    private Vector3 CheckPosisionLeft => transform.position + Vector3.left * 0.15f * transform.localScale.x;
    private bool LeftBoxCollision => Physics.OverlapBoxNonAlloc(CheckPosisionLeft, CheckBoxSide, overlapBoxArray, Quaternion.identity, sideLayerMask) > 0;

    private Vector3 CheckPosisionRight => transform.position + Vector3.right * 0.15f * transform.localScale.x;
    private bool RightBoxCollision => Physics.OverlapBoxNonAlloc(CheckPosisionRight, CheckBoxSide, overlapBoxArray, Quaternion.identity, sideLayerMask) > 0;

    private Vector3 CheckPosisionFront => transform.position + Vector3.forward * 0.2f * transform.localScale.z;
    private bool FrontBoxCollision => Physics.OverlapBoxNonAlloc(CheckPosisionFront, CheckBoxFront, overlapBoxArray, Quaternion.identity, frontLayerMask) > 0;

    // Start is called before the first frame update
    private void Start()
    {
        targetXPosition = Mathf.RoundToInt(transform.position.x);
        overlapBoxArray = new Collider[10];
        lastDeathChunk = chunkSpawner.FirstChunk;
    }

    // Update is called once per frame
    private void Update()
    {

        if(FrontBoxCollision)
        {
            if(lastDeathChunk != chunkSpawner.FirstChunk)
            {
                OnDeath?.Invoke();
                lastDeathChunk = chunkSpawner.FirstChunk;
            }
        }

        forwardSpeed += Settings.World.playerAcceleration * Time.deltaTime;  // forward acceleration
        velocity.y -= GravityY * Time.deltaTime;   // gravity
        velocity.z = ForwardVelocity;

        if (!jumped && IsGrounded && Input.GetKey(KeyCode.Space))
        {
            velocity.y = JumpSpeedY;
            jumped = true;
        }
        if (!IsGrounded) jumped = false;
        if (IsGrounded && velocity.y < 0) velocity.y = 0;


        HandleInputToChangeLanes();

        ApplyChangeLanes();


        //HandleCrouch();

        transform.position += velocity * Time.deltaTime;
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Vector3 scale = transform.localScale;
            if (crouch)
            {
                scale.y = 1f;
                crouch = false;
            } else
            {
                scale.y = 0.5f;
                crouch = true;
            }
            transform.localScale = scale;
        }
    }

    private void ApplyChangeLanes()
    {
        //print(laneSwitchSpeed * Mathf.Sqrt(forwardVelocity));
        if (IsOnTargetLane)
        {
            transform.position = new Vector3(targetXPosition, transform.position.y, transform.position.z);
            velocity.x = 0f;
        }
        else
        {
            float velToLine = Mathf.Min(laneSwitchSpeed * ForwardVelocity, (Mathf.Abs(PositionX - targetXPosition) / Time.deltaTime), maxLineSwitchSpeed);
            if (ShouldGoLeft)
            {
                // go left
                if(!LeftBoxCollision)
                {
                    velocity.x = -velToLine;
                } else
                {
                    if (overlapBoxArray[0].gameObject.layer == LayerMask.NameToLayer("SidePanel"))
                    {
                        Physics.BoxCast(CheckPosisionLeft, CheckBoxSide, Vector3.left, out RaycastHit hitInfo);
                        velocity.x = hitInfo.distance;
                    }
                    else
                    {
                        velocity.x = 0;
                    }
                }
            }
            else
            {
                // go right
                if(!RightBoxCollision)
                {
                    velocity.x = +velToLine;
                }
                else
                {
                    if(overlapBoxArray[0].gameObject.layer == LayerMask.NameToLayer("SidePanel")) {
                        Physics.BoxCast(CheckPosisionRight, CheckBoxSide, Vector3.right, out RaycastHit hitInfo);
                        velocity.x = -hitInfo.distance;
                    } else
                    {
                        velocity.x = 0;
                    }
                }
            }
        }
    }

    private void HandleInputToChangeLanes()
    {
        if ((Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S)) || Input.GetKeyDown(KeyCode.A))
        {
            // left     x  <
            if (targetXPosition > -1*CurrentNumOfLanes()/2 && (ShouldGoRight || IsOnTargetLane))
            {
                targetXPosition--;
            }
        }
        if ((Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) || Input.GetKeyDown(KeyCode.D))
        {   // right    > x
            if (targetXPosition < CurrentNumOfLanes()/2 && (ShouldGoLeft || IsOnTargetLane))
                targetXPosition++;
        }
    }

    private int CurrentNumOfLanes()
    {
        int colNum = Physics.OverlapBoxNonAlloc(transform.position, new Vector3(20f, 10f, 1f), overlapBoxArray, Quaternion.identity, sidePanelLayerMask);
        int maxPos = int.MinValue;
        int minPos = int.MaxValue;
        for(int i=0; i< colNum; i++)
        {
            Collider collider = overlapBoxArray[i];
            if (maxPos < collider.transform.position.x) maxPos = (int) collider.transform.position.x;
            if (minPos > collider.transform.position.x) minPos = (int) collider.transform.position.x;
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