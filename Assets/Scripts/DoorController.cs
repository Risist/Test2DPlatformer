using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string requiredItem;
    [Space]
    [Range(0, 1)] public float poseLerp;
    public SpriteRenderer leftDoorPart;
    public SpriteRenderer rightDoorPart;
    public SpriteRenderer centerDoorPart;
    public BoxCollider2D doorCollider;

    Vector3 leftDesiredPosition;
    Vector3 rightDesiredPosition;

    bool isOpen;

    void Start()
    {
        leftDesiredPosition = leftDoorPart.transform.position + Vector3.down * 2.5f;
        rightDesiredPosition = rightDoorPart.transform.position + Vector3.up * 2.5f;
    }

    void Update()
    {
        if (isOpen)
            OpenPose();
    }

    void OpenPose()
    {
        leftDoorPart.transform.position = Vector3.Lerp(leftDoorPart.transform.position, leftDesiredPosition, poseLerp);
        rightDoorPart.transform.position = Vector3.Lerp(rightDoorPart.transform.position, rightDesiredPosition, poseLerp);

        centerDoorPart.transform.rotation *= Quaternion.Euler(0, 0, 1);
        Color cl = centerDoorPart.color;
        cl.a = Mathf.Lerp(cl.a, 0, poseLerp);
        centerDoorPart.color = cl;
    }

    void Open()
    {
        isOpen = true;
        doorCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var inventory = collision.GetComponent<Inventory>();
        if (!isOpen && inventory && inventory.HasItem(requiredItem))
        {
            Open();
        }
    }


}
