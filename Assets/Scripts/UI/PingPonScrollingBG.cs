using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPonScrollingBG : MonoBehaviour
{
    public float speed;
    public float xEdge;
    public int frameUpdate;
    private Material material;

    private int currFrame;

    void OnEnable()
    {
        material = GetComponent<SpriteRenderer>().material;
        currFrame = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currFrame++;
        if (currFrame < frameUpdate) return;
        Vector2 prevOffset = material.GetTextureOffset("_MainTex");
        if (Mathf.Abs(prevOffset.x) > xEdge) speed = -speed;
        material.SetTextureOffset("_MainTex", prevOffset + (Vector2.right * speed * Time.deltaTime));
        currFrame = 0;
    }
}
