using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string type;
    SpriteRenderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Fade(float lerpFactor)
    {
        foreach (var it in renderers)
        {
            Color cl = it.color;
            cl.a = Mathf.Lerp(cl.a, 0, lerpFactor);
            it.color = cl;
        }
    }

}
