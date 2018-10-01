using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSteering : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;

    private void Update()
    {
        Vector3 movement = new Vector3();
        if(Input.GetKey(KeyCode.W))
        {
            movement.z = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement.z = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement.x  = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement.x = -1;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            movement *= 2.5f;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0.0f, -rotationSpeed * Time.deltaTime, 0.0f, Space.Self);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f, Space.Self);
        }

        transform.Translate(movement * movementSpeed * Time.deltaTime, Space.Self);
    }
}