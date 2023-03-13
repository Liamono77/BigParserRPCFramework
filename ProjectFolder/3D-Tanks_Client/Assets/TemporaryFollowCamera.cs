using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryFollowCamera : MonoBehaviour
{
    public float lerpValue = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TankClientUniversal.instance.myNetSync != null)
        {
            transform.position = Vector3.Lerp(transform.position, TankClientUniversal.instance.myNetSync.transform.position, Time.deltaTime * lerpValue);
        }
    }
}
