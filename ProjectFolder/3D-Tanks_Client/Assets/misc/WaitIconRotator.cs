using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitIconRotator : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 vect;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(vect, speed * Time.deltaTime);
    }
}
