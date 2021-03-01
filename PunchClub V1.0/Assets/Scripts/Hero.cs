using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hero : Actor 
{

    //1
    //public Animator baseAnim;
    //public Rigidbody body;
    ////public SpriteRenderer shadowSprite;
    ////2
    //public float speed = 2;
    public bool canJumpAttack = true;
    private int currentAttackChain = 1;
    public int evaluatedAttackChain = 0;
    public AttackData jumpAttack;

    public Walker walker;
    public bool isAutoPiloting;
    public bool controllable = true;

    bool isAttackingAnim;
    float lastAttackTime;
    float attackLimit = 0.14f;

    public float walkSpeed = 2;
    public float runSpeed = 5;

    bool isRunning;
    bool isMoving;
    float lastWalk;
    public bool canRun = true;
    float tapAgainToRunTime = 0.2f;
    Vector3 lastWalkVector;

    bool isJumpLandAnim;
    bool isJumpingAnim;

    public InputHandler input;

    public float jumpForce = 1750;
    private float jumpDuration = 0.2f;
    private float lastJumpTime;

    //public bool isGrounded;

    //3
    Vector3 currentDir;
    bool isFacingLeft;
    //protected Vector3 frontVector;

    public override void Update()
    {
        base.Update();

        if (!isAlive)
        {
            return;
        }

        isAttackingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1");

        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");

        if (isAutoPiloting)
        {
            return;
        }

        float h = input.GetHorizontalAxis();
        float v = input.GetVerticalAxis();

        bool jump = input.GetJumpButtonDown();

        bool attack = input.GetAttackButtonDown();

        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();
        //1
        if (!isAttackingAnim)
        {
            if ((v == 0 && h == 0))
            {
                Stop();
                isMoving = false;
            }
            else if (!isMoving && (v != 0 || h != 0))
            {
                //2
                isMoving = true;
                float dotProduct = Vector3.Dot(currentDir, lastWalkVector);
                //3
                if (canRun && Time.time < lastWalk + tapAgainToRunTime && dotProduct > 0)
                {
                    Run();
                }
                else
                {
                    Walk();
                    //4
                    if (h != 0)
                    {
                        lastWalkVector = currentDir;
                        lastWalk = Time.time;
                    }
                }
            }
        }
        if (jump && !isJumpLandAnim && !isAttackingAnim && (isGrounded || (isJumpingAnim && Time.time < lastJumpTime + jumpDuration)))
        {
            Jump(currentDir);
        }

        if (attack && Time.time >= lastAttackTime + attackLimit)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    public void Stop()
    {
        speed = 0;
        baseAnim.SetFloat("Speed", speed);
        isRunning = false;
        baseAnim.SetBool("IsRunning", isRunning);
    }

    public void Walk()
    {
        speed = walkSpeed;
        baseAnim.SetFloat("Speed", speed);
        isRunning = false;
        baseAnim.SetBool("IsRunning", isRunning);
    }

    public void Run()
    {
        speed = runSpeed;
        isRunning = true;
        baseAnim.SetBool("IsRunning", isRunning);
        baseAnim.SetFloat("Speed", speed);
    }

    void Jump(Vector3 direction)
    {
        //1
        if (!isJumpingAnim)
        {
            baseAnim.SetTrigger("Jump");
            lastJumpTime = Time.time;
            //2
            Vector3 horizontalVector = new Vector3(direction.x, 0, direction.z) *
           speed * 40;
            body.AddForce(horizontalVector, ForceMode.Force);
        }
        //3
        Vector3 verticalVector = Vector3.up * jumpForce * Time.deltaTime;
        body.AddForce(verticalVector, ForceMode.Force);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.name == "Floor")
    //    {
    //        isGrounded = true;
    //        baseAnim.SetBool("IsGrounded", isGrounded);
    //        DidLand();
    //    }
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.name == "Floor")
    //    {
    //        isGrounded = false;
    //        baseAnim.SetBool("IsGrounded", isGrounded);
    //    }
    //}
    public override void Attack()
    {
        if (!isGrounded)
        {
            if (isJumpingAnim && canJumpAttack)
            {
                canJumpAttack = false;

                currentAttackChain = 1;
                evaluatedAttackChain = 0;
                baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                baseAnim.SetInteger("CurrentChain", currentAttackChain);

                body.velocity = Vector3.zero;
                body.useGravity = false;
            }
        }
        else
        {
            currentAttackChain = 1;
            evaluatedAttackChain = 0;
            baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
            baseAnim.SetInteger("CurrentChain", currentAttackChain);
        }
    }

    protected override void DidLand()
    {
        base.DidLand();
        Walk();
    }

    public void DidChain(int chain)
    {
        baseAnim.SetInteger("EvaluatedChain", 1);
    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }

        if (!isAutoPiloting)
        {
            Vector3 moveVector = currentDir * speed;

            if (isGrounded && !isAttackingAnim)
            {
                body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
                baseAnim.SetFloat("Speed", moveVector.magnitude);
            }
            if (moveVector != Vector3.zero)
            {
                if (moveVector.x != 0)
                {
                    isFacingLeft = moveVector.x < 0;
                }
                FlipSprite(isFacingLeft);
            }
        }
    }

    public void AnimateTo(Vector3 position, bool shouldRun, Action callback)
    {
        if (shouldRun)
        {
            Run();
        }
        else
        {
            Walk();
        }
        walker.MoveTo(position, callback);
    }

    public void UseAutopilot(bool useAutopilot)
    {
        isAutoPiloting = useAutopilot;
        walker.enabled = useAutopilot;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.collider.name == "Floor")
        {
            canJumpAttack = true;
        }
    }

    public void DidJumpAttack()
    {
        body.useGravity = true;
    }

    private void AnalyzeSpecialAttack(AttackData attackData, Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        actor.EvaluateAttackData(attackData, hitVector, hitPoint);
    }

    protected override void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
        {
            base.HitActor(actor, hitPoint, hitVector);
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack"))
        {
            AnalyzeSpecialAttack(jumpAttack, actor, hitPoint, hitVector);
        }
    }
}
