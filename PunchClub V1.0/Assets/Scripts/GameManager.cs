﻿//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //1
    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;
    //2
    void Start()
    {
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);
    }
    //3
    
    void Update() {
        if (cameraFollows)
        {
            cameraBounds.SetXPosition(actor.transform.position.x);
        }
    }
}