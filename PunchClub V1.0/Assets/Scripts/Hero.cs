using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{

    //1
    public Animator baseAnim;
    public Rigidbody body;
    //public SpriteRenderer shadowSprite;
    //2
    public float speed = 2;
    public float walkSpeed = 2;
    //3
    Vector3 currentDir;
    bool isFacingLeft;
    protected Vector3 frontVector;

    void Update()
    {
        //1
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //2
        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();
        //3
        if ((v == 0 && h == 0))
        {
            Stop();
        }
        else if ((v != 0 || h != 0))
        {
            Walk();
        }
    }

    //1
    public void Stop()
    {
        speed = 0;
        baseAnim.SetFloat("Speed", speed);
    }
    //2
    public void Walk()
    {
        speed = walkSpeed;
        baseAnim.SetFloat("Speed", speed);
    }

    //1
    void FixedUpdate()
    {
        Vector3 moveVector = currentDir * speed;
        body.MovePosition(transform.position + moveVector *
       Time.fixedDeltaTime);
        baseAnim.SetFloat("Speed", moveVector.magnitude);
        //2
        if (moveVector != Vector3.zero)
        {
            if (moveVector.x != 0)
            {
                isFacingLeft = moveVector.x < 0;
            }
            FlipSprite(isFacingLeft);
        }
    }
    //3
    public void FlipSprite(bool isFacingLeft)
    {
        if (isFacingLeft)
        {
            frontVector = new Vector3(-1, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            frontVector = new Vector3(1, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
