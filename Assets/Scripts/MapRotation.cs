using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotation : MonoBehaviour
{
    private Vector3 previousMousePosition;
    public float rotationSpeed = 0.5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - previousMousePosition;

            float rotationX = -mouseDelta.y * rotationSpeed;
            float rotationY = mouseDelta.x * rotationSpeed;

            Camera.main.transform.RotateAround(transform.position, Vector3.up, rotationY);
            Camera.main.transform.RotateAround(transform.position, Camera.main.transform.right, rotationX);

            previousMousePosition = currentMousePosition;
        }
    }
}