using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour
{
    public float offset;
    public float minVisibleX;
    public float maxVisibleX;
    private float minValue;
    private float maxValue;
    public float cameraHalfWidth;
    //3
    public Camera activeCamera;
    //4
    public Transform cameraRoot;
    public Transform leftBounds;
    public Transform rightBounds;
    //5
    void Start()
    {
       activeCamera = Camera.main;

        cameraHalfWidth = Mathf.Abs(activeCamera.ScreenToWorldPoint(
            new Vector3(0, 0, 0)).x - activeCamera.ScreenToWorldPoint(
                new Vector3(Screen.width, 0, 0)).x) * 0.5f;
        minValue = minVisibleX + cameraHalfWidth;
        maxValue = maxVisibleX - cameraHalfWidth;

        Vector3 position;
        position = leftBounds.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth;
        leftBounds.transform.localPosition = position;

        position = rightBounds.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth;
        rightBounds.transform.localPosition = position;
    }
    //8
    public void SetXPosition(float x)
    {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x + offset, minValue, maxValue);
        cameraRoot.position = trans;
    }

    public void CalculateOffset(float actorPosition)
    {
        offset = cameraRoot.position.x - actorPosition;
        SetXPosition(actorPosition);
        StartCoroutine(EaseOffset());
    }

    private IEnumerator EaseOffset()
    {
        while (offset != 0)
        {
            offset = Mathf.Lerp(offset, 0, 0.1f);
            if (Mathf.Abs(offset) < 0.05f)
            {
                offset = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
