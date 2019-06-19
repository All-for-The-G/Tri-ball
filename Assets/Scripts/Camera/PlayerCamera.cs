using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float minZoom, maxZoom;
    [SerializeField] private float minVerticalZoom, maxVerticalZoom;
    [SerializeField] private float closePanSpeed, farPanSpeed;
    [SerializeField] private float rotationSpeed;
    
    private Transform rotation, zoom;
    private float zoomValue = 1f;
    private float rotationAngle;

    private void Awake()
    {
        rotation = transform.GetChild(0);
        zoom = rotation.transform.GetChild(0);
    }
    
    private void Update () 
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f) 
        {
            AdjustZoom(zoomDelta);
        }
        
        float rotationDelta = Input.GetAxis("Mouse X");
        if (rotationDelta != 0f && Input.GetKey(KeyCode.Mouse2)) 
        {
            AdjustRotation(rotationDelta);
        }
        
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f) 
        {
            AdjustPosition(xDelta, zDelta);
        }
    }
	
    void AdjustZoom (float delta) 
    {
        zoomValue = Mathf.Clamp01(zoomValue + delta);
        
        float distance = Mathf.Lerp(minZoom, maxZoom, zoomValue);
        zoom.localPosition = new Vector3(0f, 0f, distance);
        
        float angle = Mathf.Lerp(minVerticalZoom, maxVerticalZoom, zoomValue);
        rotation.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
    
    void AdjustPosition (float xDelta, float zDelta) 
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(farPanSpeed, closePanSpeed, zoomValue) * damping * Time.deltaTime;
        
        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = position;
    }
    
    void AdjustRotation (float delta) 
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f) 
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f) 
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
}
