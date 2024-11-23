using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // whatever object if dragged into this place
    // will become the camera's focus
    [SerializeField] 
    private Transform Focus;

    // since nothing is being initalized there is no need for a start method

    // Update is called once per frame
    void Update()
    {
        // sets the x and y camera pos to the focus pos
        transform.position = new Vector3(Focus.position.x, Focus.position.y, -10.0f);
    }
}
