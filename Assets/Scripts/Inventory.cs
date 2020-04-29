using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Item animation")]
    [Range(0, 1)] public float poseLerp = 0.1f;
    [Range(0, 1)] public float fadeLerp = 0.1f;
    public Timer tDestroyItem;
    public Vector3 itemAnimationOffset;
    

    /// only several items, list with linear search is enough
    List<string> itemList = new List<string>();

    Item animatedItem;
    private void Update()
    {
        if (animatedItem)
        {
            animatedItem.transform.position = Vector3.Lerp(animatedItem.transform.position, transform.position + itemAnimationOffset, poseLerp);
            animatedItem.Fade(fadeLerp);

            animatedItem.transform.rotation *= Quaternion.Euler(0, 0, 3);

            if (tDestroyItem.IsReady())
                Destroy(animatedItem.gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        var item = collision.gameObject.GetComponent<Item>();
        if (item)
        {
            AcquireItem(item.type, item);
        }
    }

    void AcquireItem(string type, Item obj)
    {
        itemList.Add(type);


        // prepare animation;
        animatedItem = obj;

        // turn off collisions
        var colliders = obj.GetComponentsInChildren<Collider2D>();
        foreach (var it in colliders)
            it.enabled = false;

        tDestroyItem.Restart();
    }

    public bool HasItem(string type)
    {
        foreach (var it in itemList)
            if (it == type)
                return true;

        return false;
    }


}
