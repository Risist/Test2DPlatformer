using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyboard : MonoBehaviour
{
    InputHolder inputHolder;

    void Start()
    {
        inputHolder = GetComponentInParent<InputHolder>();
        if (!inputHolder)
            Debug.LogError("Input holder not found!");
    }

    void Update()
    {
        // record player input
        inputHolder.wantedMovementDirection = Input.GetAxisRaw("Horizontal");

        if (inputHolder.canRecordJump)
        {
            if (Input.GetKey(KeyCode.W))
                inputHolder.wantsToJumpContinuous = true;
            if (Input.GetKeyDown(KeyCode.W))
                inputHolder.wantsToJumpDiscrete = true;
        }
        else
        {
            inputHolder.wantsToJumpContinuous = false;
            inputHolder.wantsToJumpDiscrete = false;
        }
    }
}
