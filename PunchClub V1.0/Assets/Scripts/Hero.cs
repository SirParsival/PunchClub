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
    bool isHurtAnim;


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

    public AttackData runAttack;
    public float runAttackForce = 1.8f;

    //public bool isGrounded;

    Vector3 currentDir;
    bool isFacingLeft;
    //protected Vector3 frontVector;

    public AttackData normalAttack2;
    public AttackData normalAttack3;
    
    float chainComboTimer;
    public float chainComboLimit = 0.3f;
    const int maxCombo = 3;

    public float hurtTolerance;
    public float hurtLimit = 20;
    public float recoveryRate = 5;

    bool isPickingUpAnim;
    bool weaponDropPressed = false;
    public bool hasWeapon;
    
    public bool canJump = true;
    
    public SpriteRenderer powerupSprite;
    public Powerup nearbyPowerup;
    public Powerup currentPowerup;
    public GameObject powerupRoot;

    public AudioClip hit2Clip;

    public override void Update()
    {
        base.Update();

        if (!isAlive)
        {
            return;
        }

        isAttackingAnim = 
            baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1") ||
            baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack2") ||
            baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack3") ||
            baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack") ||
            baseAnim.GetCurrentAnimatorStateInfo(0).IsName("run_attack");

        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") ||
        baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");

        isHurtAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("hurt");

        isPickingUpAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("pickup");

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
        
        if (!isAttackingAnim)
        {
            if (chainComboTimer > 0)
            {
                chainComboTimer -= Time.deltaTime;
                if (chainComboTimer < 0)
                {
                    chainComboTimer = 0;
                    currentAttackChain = 0;
                    evaluatedAttackChain = 0;
                    baseAnim.SetInteger("CurrentChain", currentAttackChain);
                    baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                }
            }
            if (v == 0 && h == 0)
            {
                Stop();
                isMoving = false;
            }
            else if (!isMoving && (v != 0 || h != 0))
            {
                isMoving = true;
                float dotProduct = Vector3.Dot(currentDir, lastWalkVector);
               
                if (canRun && Time.time < lastWalk + tapAgainToRunTime && dotProduct > 0)
                {
                    Run();
                }
                else
                {
                    Walk();
                    if (h != 0)
                    {
                        lastWalkVector = currentDir;
                        lastWalk = Time.time;
                    }
                }
            }
        }

        if (jump && hasWeapon)
        {
            weaponDropPressed = true;
            DropWeapon();
        }

        if (weaponDropPressed && !jump)
        {
            weaponDropPressed = false;
        }

        if (canJump && jump && !isKnockedOut && !isJumpLandAnim && !isAttackingAnim && !isPickingUpAnim && !weaponDropPressed && 
            (isGrounded || (isJumpingAnim && Time.time < lastJumpTime + jumpDuration)))
        {
            Jump(currentDir);
        }

        if (attack && Time.time >= lastAttackTime + attackLimit && isGrounded && !isPickingUpAnim)
        {
            if (nearbyPowerup != null && nearbyPowerup.CanEquip())
            {
                lastAttackTime = Time.time;
                Stop();
                PickupWeapon(nearbyPowerup);
            }
        }

        if (attack && Time.time >= lastAttackTime + attackLimit && !isKnockedOut && !isPickingUpAnim)
        {
            lastAttackTime = Time.time;
            Attack();
        }

        if (hurtTolerance < hurtLimit)
        {
            hurtTolerance += Time.deltaTime * recoveryRate;
            hurtTolerance = Mathf.Clamp(hurtTolerance, 0, hurtLimit);
        }
    }

    protected override void Start()
    {
        base.Start();
        lifeBar = GameObject.FindGameObjectWithTag("HeroLifeBar").GetComponent<LifeBar>();
        lifeBar.SetProgress(currentLife / maxLife);
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

    public override void Attack()
    {
        if (currentAttackChain <= maxCombo)
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
                if (isRunning)
                {
                    body.AddForce((Vector3.up + (frontVector * 5)) * runAttackForce, ForceMode.Impulse);

                    currentAttackChain = 1;
                    evaluatedAttackChain = 0;
                    baseAnim.SetInteger("CurrentChain", currentAttackChain);
                    baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                }
                else
                {
                    if (currentAttackChain == 0 || chainComboTimer == 0)
                    {
                        currentAttackChain = 1;
                        evaluatedAttackChain = 0;
                    }
                    baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                    baseAnim.SetInteger("CurrentChain", currentAttackChain);
                }
            }
        }
    }

    protected override void DidLand()
    {
        base.DidLand();
        Walk();
    }

    public void DidChain(int chain)
    {
        evaluatedAttackChain = chain;
        baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }

        if (!isAutoPiloting)
        {
            Vector3 moveVector = currentDir * speed;

            if (isGrounded && !isAttackingAnim && !isJumpLandAnim && !isKnockedOut && !isHurtAnim)
            {
                body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
                baseAnim.SetFloat("Speed", moveVector.magnitude);
            }
            if (moveVector != Vector3.zero && isGrounded && !isKnockedOut && !isAttackingAnim)
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
        chainComboTimer = chainComboLimit;
    }

    protected override void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
        {
            AttackData attackData = hasWeapon ? currentPowerup.attackData1 : normalAttack;
            AnalyzeNormalAttack(attackData, 2, actor, hitPoint, hitVector);
            PlaySFX(hitClip);
            if (hasWeapon)
            {
                currentPowerup.Use();
            }
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack2"))
        {
            AttackData attackData = hasWeapon ? currentPowerup.attackData2 : normalAttack2;
            AnalyzeNormalAttack(attackData, 3, actor, hitPoint, hitVector);
            PlaySFX(hitClip);
            if (hasWeapon)
            {
                currentPowerup.Use();
            }
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack3"))
        {
            AttackData attackData = hasWeapon ? currentPowerup.attackData3 : normalAttack3;
            AnalyzeNormalAttack(attackData, 1, actor, hitPoint, hitVector);
            PlaySFX(hit2Clip);
            if (hasWeapon)
            {
                currentPowerup.Use();
            }
            //{
            //    AnalyzeNormalAttack(normalAttack3, 1, actor, hitPoint, hitVector);
            //}
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack"))
        {
            AnalyzeSpecialAttack(jumpAttack, actor, hitPoint, hitVector);
            PlaySFX(hit2Clip);
        }
        else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("run_attack"))
        {
            AnalyzeSpecialAttack(runAttack, actor, hitPoint, hitVector);
            PlaySFX(hit2Clip);
        }
    }

    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        hurtTolerance -= value;
        if (hurtTolerance <= 0 || !isGrounded)
        {
            hurtTolerance = hurtLimit;
            knockdown = true;
        }
        if (hasWeapon)
        {
            DropWeapon();
        }
        base.TakeDamage(value, hitVector, knockdown);
    }

    public override bool CanWalk()
    {
        return (isGrounded && !isAttackingAnim && !isJumpLandAnim && !isKnockedOut && !isHurtAnim);
    }

    protected override IEnumerator KnockdownRoutine()
    {
        body.useGravity = true;
        return base.KnockdownRoutine();
    }

    private void AnalyzeNormalAttack(AttackData attackData, int attackChain, Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        actor.EvaluateAttackData(attackData, hitVector, hitPoint);
        currentAttackChain = attackChain;
        chainComboTimer = chainComboLimit;
    }

    public void PickupWeapon(Powerup powerup)
    {
        baseAnim.SetTrigger("PickupPowerup");
    }

    public void DidPickupWeapon()
    {
        if (nearbyPowerup != null && nearbyPowerup.CanEquip())
        {
            Powerup powerup = nearbyPowerup;
            hasWeapon = true;
            currentPowerup = powerup;
            nearbyPowerup = null;
            powerupRoot = currentPowerup.rootObject;
            powerup.user = this;

            currentPowerup.body.velocity = Vector3.zero;
            powerupRoot.SetActive(false);
            Walk();

            powerupSprite.enabled = true;
            canRun = false;
            canJump = false;
        }
    }

    public void DropWeapon()
    {
        powerupRoot.SetActive(true);
        powerupRoot.transform.position = transform.position + Vector3.up;
        currentPowerup.body.AddForce(Vector3.up * 100);
        
        powerupRoot = null;
        currentPowerup.user = null;
        currentPowerup = null;
        nearbyPowerup = null;
        
        powerupSprite.enabled = false;
        canRun = true;
        hasWeapon = false;
        canJump = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Powerup"))
        {
            Powerup powerup = collider.gameObject.GetComponent<Powerup>();
            if (powerup != null)
            {
                nearbyPowerup = powerup;
            }
        }
    }
    
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Powerup"))
        {
            Powerup powerup = collider.gameObject.GetComponent<Powerup>();
            if (powerup == nearbyPowerup)
            {
                nearbyPowerup = null;
            }
        }
    }

    public override void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector)
    {
        Container containerObject = collider.GetComponent<Container>();
        if (containerObject != null)
        {
            containerObject.Hit(hitPoint);
            if (containerObject.CanBeOpened() && collider.tag != gameObject.tag)
            {
                containerObject.Open(hitPoint);
                PlaySFX(hitClip);
            }
        }
        else
        {
            base.DidHitObject(collider, hitPoint, hitVector);
        }
    }
}
