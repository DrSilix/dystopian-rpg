using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class ScrollingBackboardController : MonoBehaviour
{
    public float speed = 0.01f;
    private Material material;
    
    void OnEnable()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 prevOffset = material.GetTextureOffset("_MainTex");
        material.SetTextureOffset("_MainTex", prevOffset + (Vector2.right * speed * Time.deltaTime));
    }
}
