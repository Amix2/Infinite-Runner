using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpHeight = 1f;
    public float laneSwitchSpeed = 1f;
    public LayerMask layerMask;

    private float jumpSpeed;
    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;
    private bool jumped = false;
    private int targetXPosition;

    bool IsGrounded => Physics.OverlapBox(GrundCheckPosision, GrundCheckBox, Quaternion.identity, layerMask).Length > 0;
    bool SwitchingLanes => Mathf.Abs(transform.position.x - targetXPosition) > 0.01f;
    bool IsOnTargetLane => Mathf.Abs(transform.position.x - targetXPosition) < 0.01;
    float PositionX => transform.position.x;
    bool ShouldGoLeft => targetXPosition - PositionX < 0;
    bool ShouldGoRight => targetXPosition - PositionX > 0;

    private Vector3 GrundCheckPosision => transform.position + Vector3.down * 0.5f * 0.95f * transform.localScale.y;
    private Vector3 GrundCheckBox => new Vector3(transform.localScale.x * 0.5f, transform.localScale.y * 0.1f, transform.localScale.z * 0.5f);

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        jumpSpeed = Mathf.Sqrt(2 * jumpHeight * 10);
        targetXPosition = Mathf.RoundToInt(transform.position.x);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 newVelocity = rigidbody.velocity;
        newVelocity = HandleJump(newVelocity);

        HandleInputToChangeLanes();

        newVelocity = ApplyChangeLanes(newVelocity);

        newVelocity.z = 8f;

        rigidbody.velocity = newVelocity;
    }

    private Vector3 ApplyChangeLanes(Vector3 newVelocity)
    {
        if (IsOnTargetLane)
        {
            transform.position = new Vector3(targetXPosition, transform.position.y, transform.position.z);
            newVelocity.x = 0f;
        }
        else
        {
            if (ShouldGoLeft)
            {
                // go left
                newVelocity.x = -laneSwitchSpeed;
            }
            else
            {
                // go right
                newVelocity.x = +laneSwitchSpeed;
            }
        }

        return newVelocity;
    }

    private void HandleInputToChangeLanes()
    {
        if (Input.GetKey(KeyCode.A))
        {
            // left     x  <
            if (ShouldGoRight || IsOnTargetLane)
            {
                targetXPosition--;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {   // right    > x
            if (ShouldGoLeft || IsOnTargetLane)
                targetXPosition++;
        }
    }

    private Vector3 HandleJump(Vector3 newVelocity)
    {
        if (!jumped && IsGrounded && Input.GetKey(KeyCode.Space))
        {
            newVelocity.y = jumpSpeed;
            jumped = true;
        }
        if (!IsGrounded) jumped = false;
        return newVelocity;
    }

    private void OnDrawGizmos()
    {
        //collider = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position + Vector3.down * 0.45f * transform.localScale.y, new Vector3(transform.localScale.x * 0.5f, transform.localScale.y * 0.2f, transform.localScale.z * 0.5f));
    }
}