using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputHolder))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    #region Movement
    public float movementSpeed = 10;


    // Mostly to get feeling of better responsibility of player character
    // By this parameter designer can define how fast character will start its movement
    [Tooltip("Decreases velocity when grounded")]
    [Range(0, 1)] public float groundedVelocityFallof = 0.9f;

    #endregion Movement


    [Header("Jump")]
    #region Jump
    public float jumpSpeed = 10;
    [Range(0, 1)] public float jumpControlLevel = 0.0f;
    public int maxMultiJump = 2;
    public RangedFloat multijumpTimeWindow;
    
    // Mostly to get feeling of better responsibility of player character
    // By this parameter designer can define how fast character will start its movement
    [Tooltip("Decreases velocity when grounded")]
    [Range(0, 1)] public float flyingVelocityFallof = 0.9f;

    [Tooltip("Layer mask of objects treated as ground\n" +
             "Player can jump of them")]
    public LayerMask groundMask;
    public float groundedMaxDistance = 1.0f;
    public float groundRaySpacing = 0.5f;
    #endregion Jump

    [Header("Wall Jump")]
    #region Wall jump
    public float walljumpRaycastLength = 1.0f;
    public float walljumpRaycastLengthFront = 1.0f;
    public LayerMask wallMask;
    public float wallJumpSpeed;

    #endregion Wall jump
    new Rigidbody2D rigidbody;
    InputHolder inputHolder;
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inputHolder = GetComponent<InputHolder>();
    }



    // raycast results
    public bool grounded      { get; private set; }
    public bool nearWallBack  { get; private set; }
    public bool nearWallFront { get; private set; }

    // animation clues
    public bool touchedGround{ get; private set; }
    public bool groundJumped { get; private set; }
    public bool wallJumped   { get; private set; }
    public bool multiJumped  { get; private set; }

    MinimalTimer multiJumpTimer = new MinimalTimer();
    // how many jumps has happened from last ground touch
    int currentMultiJump;


    void Update()
    {
        inputHolder.canRecordJump = grounded || !multiJumpTimer.IsReady(multijumpTimeWindow.max);
    }

    

    private void FixedUpdate()
    {
        UpdateGrounded();
        UpdateWallJump();
        UpdateDrag();
        UpdateMovement();
    }
    void UpdateGrounded()
    {
        // throw raycast below and set up grounded based on findings
        var hit = Physics2D.Raycast(transform.position, Vector2.down, groundedMaxDistance, groundMask);
        bool found = hit.collider != null;

        hit = Physics2D.Raycast(transform.position + Vector3.right* groundRaySpacing, Vector2.down, groundedMaxDistance, groundMask);
        found |= hit.collider != null;

        hit = Physics2D.Raycast(transform.position + Vector3.left * groundRaySpacing, Vector2.down, groundedMaxDistance, groundMask);
        found |= hit.collider != null;

        touchedGround = found && !grounded;
        grounded = found;

        // visualize ray
        Debug.DrawRay(transform.position, Vector2.down * groundedMaxDistance, grounded ? Color.green : Color.red);
        Debug.DrawRay(transform.position + Vector3.left * groundRaySpacing, Vector2.down * groundedMaxDistance, grounded ? Color.green : Color.red);
        Debug.DrawRay(transform.position + Vector3.right * groundRaySpacing, Vector2.down * groundedMaxDistance, grounded ? Color.green : Color.red);
    }
    void UpdateDrag()
    {
        Vector2 v = rigidbody.velocity;
        v.x *= grounded ? groundedVelocityFallof : flyingVelocityFallof;
        rigidbody.velocity = v;
    }
    void UpdateMovement()
    {
        // if we're on the ground multiJumps should be available once again
        if (grounded)
        {
            currentMultiJump = 0;
        }


        // we can jump if we're on the ground
        // or specified time passed from last jump and we're not out of multijumps yet
        bool canMultijump = currentMultiJump < maxMultiJump && multiJumpTimer.IsReady(multijumpTimeWindow.min);
        bool canWallJump = nearWallBack && multiJumpTimer.IsReady(multijumpTimeWindow.min);


        groundJumped = grounded && inputHolder.wantsToJumpContinuous;
        multiJumped = !groundJumped && canMultijump && inputHolder.wantsToJumpDiscrete;
        wallJumped = canWallJump && inputHolder.wantsToJumpContinuous;

        bool jumped = (grounded || canWallJump ) && inputHolder.wantsToJumpContinuous;
        jumped |= canMultijump && inputHolder.wantsToJumpDiscrete;

        if ( jumped )
        {
            // consume input
            inputHolder.wantsToJumpDiscrete = false;
            inputHolder.wantsToJumpContinuous = false;

            // update state of multiJump
            ++currentMultiJump;
            multiJumpTimer.Restart();

            // instant impulse without taking mass into consideration
            // Box2D does not have the same force modes as PhysicsX so it's work around
            Vector2 v = rigidbody.velocity;
            // sometimes gravity was too strong after several wall jumps
            // this will reduce gravity effect while jumping
            v.y *= 0.875f;
            rigidbody.velocity = v + Vector2.up * jumpSpeed;
        }


        // calculate x movement
        Vector2 move = Vector2.zero;
        move.x = inputHolder.wantedMovementDirection * movementSpeed;
        if (!grounded) move.x *= jumpControlLevel;

        // wall jump
        if (nearWallBack && jumped && !grounded)
        {
            move.x += inputHolder.wantedMovementDirection *wallJumpSpeed;
        }

        // apply final force
        rigidbody.AddForce(move, ForceMode2D.Force);
    }
    void UpdateWallJump()
    {
        if (inputHolder.wantedMovementDirection == 0)
        {
            // should not wall jump if does not move
            // neither raycast is required
            nearWallBack = false;
            nearWallFront = false;
            return;
        }

        // throw raycast backwards and set up nearWallBack based on findings
        var hit = Physics2D.Raycast(transform.position, Vector2.left* inputHolder.wantedMovementDirection, walljumpRaycastLength, wallMask);
        nearWallBack = hit.collider != null;

        // visualize ray
        Debug.DrawRay(transform.position, Vector2.left * inputHolder.wantedMovementDirection * walljumpRaycastLength, nearWallBack ? Color.green : Color.red);

        // throw raycast in front and set up nearWallFront based on findings
        hit = Physics2D.Raycast(transform.position, Vector2.right * inputHolder.wantedMovementDirection, walljumpRaycastLength, wallMask);
        nearWallFront = hit.collider != null;

        // visualize ray
        Debug.DrawRay(transform.position, Vector2.right * inputHolder.wantedMovementDirection * walljumpRaycastLengthFront, nearWallFront ? Color.green : Color.red);
    }
}
