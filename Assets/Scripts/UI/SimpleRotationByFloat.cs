using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotationByFloat : MonoBehaviour
{
    public float speed = 0.1f;
    void Update()
    {
        this.transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
