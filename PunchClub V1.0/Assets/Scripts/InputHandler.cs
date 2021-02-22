using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    bool attack;
    float horizontal;
    float vertical;
    bool jump;
    float lastJumpTime;
    bool isJumping;
    public float maxJumpDuration = 0.2f;
    //1
    public float GetVerticalAxis()
    {
        return vertical;
    }
    public float GetHorizontalAxis()
    {
        return horizontal;
    }
    public bool GetJumpButtonDown()
    {
        return jump;
    }

    void Update()
    {
        //2
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        //3
        if (!jump && !isJumping && Input.GetButton("Jump"))
        {
            jump = true;
            lastJumpTime = Time.time;
            isJumping = true;
        }
        else if (!Input.GetButton("Jump"))
        {
            //4
            jump = false;
            isJumping = false;
        }
        //5
        if (jump && Time.time > lastJumpTime + maxJumpDuration)
        {
            jump = false;
        }
    }

    public bool GetAttackButtonDown()
    {
        return attack;
    }
}
