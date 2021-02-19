using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour
{
    //2
    public float minVisibleX;
    public float maxVisibleX;
    private float minValue;
    private float maxValue;
    public float cameraHalfWidth;
    //3
    private Camera activeCamera;
    //4
    public Transform cameraRoot;
    //5
    void Start()
    {
        //6
       activeCamera = Camera.main;

        //7
        cameraHalfWidth =
        Mathf.Abs(activeCamera.ScreenToWorldPoint(
        new Vector3(0, 0, 0)).x -
        activeCamera.ScreenToWorldPoint(
        new Vector3(Screen.width, 0, 0)).x) * 0.5f;
        minValue = minVisibleX + cameraHalfWidth;
        maxValue = maxVisibleX - cameraHalfWidth;
    }
    //8
    public void SetXPosition(float x)
    {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x, minValue, maxValue);
        cameraRoot.position = trans;
    }
}
