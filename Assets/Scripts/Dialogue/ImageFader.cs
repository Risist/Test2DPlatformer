using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    [Range(0, 1)] public float fadeLerpFactor = 0.05f;

    protected Image[] images;
    float[] initialImageAlphas;

    protected Text[] texts;
    float[] initialTextAlphas;

    public void UpdateTargets()
    {
        images = GetComponentsInChildren<Image>(true);
        int n = images.Length;
        float[] v = new float[n];
        int i = 0;
        for (; i < initialImageAlphas.Length; ++i)
        {
            v[i] = initialImageAlphas[i];
        }
        for (; i < n; ++i)
        {
            v[i] = images[i].color.a;

            // set initial color to transparent
            Color cl = images[i].color;
            cl.a = 0;
            images[i].color = cl;
        }
        initialImageAlphas = v;

        // initialize texts
        texts = GetComponentsInChildren<Text>(true);
        n = texts.Length;
        v = new float[n];
        for (i = 0; i < initialTextAlphas.Length; ++i)
        {
            v[i] = initialTextAlphas[i];
        }
        for (; i < n; ++i)
        {
            v[i] = texts[i].color.a;

            // set initial color to transparent
            Color cl = texts[i].color;
            cl.a = 0;
            texts[i].color = cl;
        }
        initialTextAlphas = v;
    }

    protected void Start()
    {
        // initialize images
        images = GetComponentsInChildren<Image>(true);
        int n = images.Length;
        initialImageAlphas = new float[n];
        for (int i = 0; i < n; ++i)
        {
            initialImageAlphas[i] = images[i].color.a;

            // set initial color to transparent
            Color cl = images[i].color;
            cl.a = 0;
            images[i].color = cl;
        }

        // initialize texts
        texts = GetComponentsInChildren<Text>(true);
        n = texts.Length;
        initialTextAlphas = new float[n];
        for (int i = 0; i < n; ++i)
        {
            initialTextAlphas[i] = texts[i].color.a;

            // set initial color to transparent
            Color cl = texts[i].color;
            cl.a = 0;
            texts[i].color = cl;
        }
    }

    public void Hide()
    {
        int n = images.Length;
        for (int i = 0; i < n; ++i)
        {
            Color cl = images[i].color;
            cl.a = Mathf.Lerp(cl.a, 0, fadeLerpFactor);
            images[i].color = cl;
        }

        n = texts.Length;
        for (int i = 0; i < n; ++i)
        {
            Color cl = texts[i].color;
            cl.a = Mathf.Lerp(cl.a, 0, fadeLerpFactor);
            texts[i].color = cl;
        }
    }
    public void Show()
    {
        int n = images.Length;
        for (int i = 0; i < n; ++i)
        {
            Color cl = images[i].color;
            cl.a = Mathf.Lerp(cl.a, initialImageAlphas[i], fadeLerpFactor);
            images[i].color = cl;
        }

        n = texts.Length;
        for (int i = 0; i < n; ++i)
        {
            Color cl = texts[i].color;
            cl.a = Mathf.Lerp(cl.a, initialTextAlphas[i], fadeLerpFactor);
            texts[i].color = cl;
        }
    }
}
