﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float minZoom, maxZoom;
    [SerializeField] private float minVerticalZoom, maxVerticalZoom;
    [SerializeField] private float closePanSpeed, farPanSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int edgePanzoneStart;
    [SerializeField] private int edgePanZoneEnd;
    
    private Transform rotation, zoom;
    private float zoomValue;
    private float rotationAngle;

    private void Awake()
    {
        rotation = transform.GetChild(0);
        zoom = rotation.transform.GetChild(0);
    }
    
    private void Update () 
    {
        float zoomDelta = InputControl.GetAxis("Mouse Wheel");
        if (zoomDelta != 0f) 
        {
            AdjustZoom(zoomDelta);
        }
        
        float rotationDelta = InputControl.GetAxis("Mouse X");
        if (rotationDelta != 0f && InputControl.GetButton("Right Mouse Button"))
        {
            AdjustRotation(rotationDelta);
        }
        
        float xDelta = InputControl.GetAxis("Horizontal");
        float zDelta = InputControl.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f) 
        {
            AdjustPosition(xDelta, zDelta);
        }
        else if(!EventSystem.current.IsPointerOverGameObject())
        {
            HandleScreenEdgePan();
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

    void HandleScreenEdgePan()
    {
        float xDelta = 0f;
        float zDelta = 0f;
        float mouseX = InputControl.mousePosition.x;
        float mouseZ = InputControl.mousePosition.y;
        bool edgePanning = false;

        if (mouseX < Screen.width - edgePanzoneStart && mouseX >= Screen.width - edgePanZoneEnd)
        {
            xDelta = 1;
            edgePanning = true;
        }

        if (mouseX > edgePanzoneStart && mouseX <= edgePanZoneEnd)
        {
            xDelta = -1;
            edgePanning = true;
        }

        if (mouseZ < Screen.height - edgePanzoneStart && mouseZ >= Screen.height - edgePanZoneEnd)
        {
            zDelta = 1;
            edgePanning = true;
        }

        if (mouseZ > edgePanzoneStart && mouseZ <= edgePanZoneEnd)
        {
            zDelta = -1;
            edgePanning = true;
        }

        if (edgePanning)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }
}
