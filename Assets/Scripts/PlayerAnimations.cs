using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [Range(0, 1)] public float poseLerp = 0.1f;
    public Timer tWallJumpInit;
    public Timer tWallJump;
    public Timer tjumpInit;
    public Timer tJumpAnimation;
    public Timer tFallAnimation;



    // holds ast direction
    // either 1 or -1
    float direction = 1;
    Vector3 initialPosition;

    enum EAnimation
    {
        EIdle,
        EFall,
        EGroundJump,
        EWallJump,
        ENearWall,
    }
    EAnimation currentAnimationState;


    PlayerMovement playerMovement;
    InputHolder inputHolder;
    void Start()
    {
        initialPosition = transform.localPosition;

        playerMovement = GetComponentInParent<PlayerMovement>();
        inputHolder = GetComponentInParent<InputHolder>();
    }

    #region Update
    void Update()
    {
        UpdateDirection();
        UpdateEnum();
        UpdatePose();
    }
    void UpdateDirection()
    {
        if (inputHolder.wantedMovementDirection != 0)
        {
            direction = inputHolder.wantedMovementDirection;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * inputHolder.wantedMovementDirection;
            transform.localScale = scale;
        }
    }
    void UpdatePose()
    {
        if (currentAnimationState == EAnimation.EWallJump && !tWallJump.IsReady())
            WallJumpPoseOut();
        else if (currentAnimationState == EAnimation.EFall && !tFallAnimation.IsReady())
            FallPose();
        else if (playerMovement.nearWallFront)
            WallJumpPoseIn();
        else if (currentAnimationState == EAnimation.EGroundJump && !tjumpInit.IsReady())
            FallPose();
        else if (currentAnimationState == EAnimation.EGroundJump && !tJumpAnimation.IsReady())
            JumpPose();
        else
            MovementPose();
    }
    void UpdateEnum()
    {
        if (playerMovement.wallJumped)
        {
            currentAnimationState = EAnimation.EWallJump;
            tWallJump.Restart();
            tWallJumpInit.Restart();
        }
        else if (playerMovement.touchedGround)
        {
            currentAnimationState = EAnimation.EFall;
            tFallAnimation.Restart();
        }
        else if (playerMovement.nearWallBack)
        {
            currentAnimationState = EAnimation.ENearWall;
        }
        else if (playerMovement.groundJumped || playerMovement.multiJumped)
        {
            currentAnimationState = EAnimation.EGroundJump;
            tjumpInit.Restart();
            
        }else if (inputHolder.wantsToJumpContinuous)
        {
            currentAnimationState = EAnimation.EGroundJump;
            tJumpAnimation.Restart();
        }
    }
    #endregion Update

    #region Animations
    void MovementPose()
    {
        float direction = inputHolder.wantedMovementDirection;

        float desiredRotation = -30;
        float rotationTimeScale = 15.0f;
        float rotationTimeOffset = -10*Mathf.Sin(Time.time*rotationTimeScale);

        RotationAnimationNoDir(direction*(desiredRotation + rotationTimeOffset) );
        PositionAnimation(Vector2.left * 0.25f);
        ScaleAnimation(Vector3.one);
    }

    void FallPose()
    {
        ScaleAnimation(new Vector3(2.0f, 0.25f, 1.0f));
        RotationAnimation(0);
        PositionAnimation(Vector2.zero);
    }

    void JumpPose()
    {
        ScaleAnimation(new Vector3(0.5f, 1.5f, 1.0f));
        PositionAnimation(Vector2.zero);
        RotationAnimationNoDir(-inputHolder.wantedMovementDirection*30);
    }
    void WallJumpPoseIn()
    {
        ScaleAnimation(new Vector3(0.25f, 1.5f, 1.0f));
        PositionAnimation(Vector2.right*0.5f);
        RotationAnimation(0);
    }
    void WallJumpPoseOut()
    {
        ScaleAnimation(new Vector3(2f, 0.2f, 1.0f));
        PositionAnimation(new Vector2(0, 1.5f));
        RotationAnimation(30);
    }
    #endregion Animations

    #region Fundamental
    void ScaleAnimation(Vector3 desiredScale)
    {
        desiredScale.x *= direction;
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, poseLerp);
    }
    void PositionAnimation(Vector3 positionOffset)
    {
        positionOffset.x *= direction;
        Vector3 desiredPosition = initialPosition + positionOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, poseLerp);
    }
    void RotationAnimation(float desiredAngle)
    {
        desiredAngle = Mathf.LerpAngle(transform.localEulerAngles.z, direction * desiredAngle, poseLerp);
        transform.localEulerAngles = new Vector3(0, 0, desiredAngle);
    }
    void RotationAnimationNoDir(float desiredAngle)
    {
        desiredAngle = Mathf.LerpAngle(transform.localEulerAngles.z, desiredAngle, poseLerp);
        transform.localEulerAngles = new Vector3(0, 0, desiredAngle);
    }
    #endregion Fundamental
}
