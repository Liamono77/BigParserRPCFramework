using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A simple script to make input reactions happen.
//Replace this later with something that uses New Input System
public class InputModule_1 : MonoBehaviour
{
    public TankInputUpdator tankInputUpdator;
    // Update is called once per frame
    void Update()
    {

        tankInputUpdator.currentInputData.moveDirection.x = Input.GetAxis("Horizontal1");
        tankInputUpdator.currentInputData.moveDirection.y = Input.GetAxis("Vertical1");
        tankInputUpdator.currentInputData.fire = Input.GetButton("Fire1");
    }
}
