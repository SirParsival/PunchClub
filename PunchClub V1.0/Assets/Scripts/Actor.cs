using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Actor : MonoBehaviour
{

    public AttackData normalAttack;

    //public float attackDamage = 10;
    public float maxLife = 100.0f;
    public float currentLife = 100.0f;
    public bool isAlive = true;
    public SpriteRenderer baseSprite;
    public Animator baseAnim;
    public Rigidbody body;

    public float speed = 2;
    protected Vector3 frontVector;
    public bool isGrounded;

    protected Coroutine knockdownRoutine;
    public bool isKnockedOut;

    public GameObject hitSparkPrefab;

    public LifeBar lifeBar;
    public Sprite actorThumbnail;

    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioSource audioSource;

    protected ActorCollider actorCollider;

    public GameObject hitValuePrefab;

    protected bool canFlinch = true;

    protected virtual void Start()
    {
        currentLife = maxLife;
        isAlive = true;
        baseAnim.SetBool("IsAlive", isAlive);

        actorCollider = GetComponent<ActorCollider>();
        actorCollider.SetColliderStance(true);
    }

    public virtual void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        FlipSprite(hitVector.x > 0);
        currentLife -= value;
        if (isAlive && currentLife <= 0)
        {
            Die();
        }
        else if (knockdown)
        {
            if (knockdownRoutine == null)
            {
                Vector3 pushbackVector = (hitVector + Vector3.up * 0.75f).normalized;
                body.AddForce(pushbackVector * 250);
                knockdownRoutine = StartCoroutine(KnockdownRoutine());
            }
        }
        else if (canFlinch)
        {
            baseAnim.SetTrigger("IsHurt");
        }

        lifeBar.EnableLifeBar(true);
        lifeBar.SetProgress(currentLife / maxLife);
        Color color = baseSprite.color;
        if (currentLife < 0)
        {
            color.a = 0.75f;
        }
        lifeBar.SetThumbnail(actorThumbnail, color);
    }

    public bool CanBeHit()
    {
        return isAlive && !isKnockedOut;
    }

    public virtual void Update()
    {
        //Vector3 shadowSpritePosition = shadowSprite.transform.position;
        //shadowSpritePosition.y = 0;
        //shadowSprite.transform.position = shadowSpritePosition;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = true;
            baseAnim.SetBool("IsGrounded", isGrounded);
            DidLand();
        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Floor")
        {
            isGrounded = false;
            baseAnim.SetBool("IsGrounded", isGrounded);
        }
    }

    protected virtual void DidLand()
    {
    }

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

    public virtual void Attack()
    {
        baseAnim.SetTrigger("Attack");
    }

    public virtual void DidHitObject(Collider collider, Vector3 hitPoint, Vector3 hitVector)
    {
        Actor actor = collider.GetComponent<Actor>();
        if (actor != null && actor.CanBeHit() && collider.tag != gameObject.tag)
        {
            if (collider.attachedRigidbody != null)
            {
                HitActor(actor, hitPoint, hitVector);
            }
        }
    }
    
    protected virtual void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        actor.EvaluateAttackData(normalAttack, hitVector, hitPoint);
        PlaySFX(hitClip);
    }

    public virtual void FaceTarget(Vector3 targetPoint)
    {
        FlipSprite(transform.position.x - targetPoint.x > 0);
    }

    protected virtual void Die()
    {
        if (knockdownRoutine != null)
        {
            StopCoroutine(knockdownRoutine);
        }

        isAlive = false;
        baseAnim.SetBool("IsAlive", isAlive);
        StartCoroutine(DeathFlicker());

        PlaySFX(deathClip);

        actorCollider.SetColliderStance(false);
    }

    public virtual bool CanWalk()
    {
        return true;
    }

    protected virtual void SetOpacity(float value)
    {
        Color color = baseSprite.color;
        color.a = value;
        baseSprite.color = color;
    }

    private IEnumerator DeathFlicker()
    {
        int i = 5;
        while (i > 0)
        {
            SetOpacity(0.5f);
            yield return new WaitForSeconds(0.1f);
            SetOpacity(1.0f);
            yield return new WaitForSeconds(0.1f);
            i--;
        }
    }

    public virtual void EvaluateAttackData(AttackData data, Vector3 hitVector, Vector3 hitPoint)
    {
        body.AddForce(data.force * hitVector);
        TakeDamage(data.attackDamage, hitVector, data.knockdown);
        ShowHitEffects(data.attackDamage, hitPoint);
    }

    public void DidGetUp()
    {
        isKnockedOut = false;
    }

    protected virtual IEnumerator KnockdownRoutine()
    {
        isKnockedOut = true;
        baseAnim.SetTrigger("Knockdown");

        actorCollider.SetColliderStance(false);
        yield return new WaitForSeconds(1.0f);
        actorCollider.SetColliderStance(true);

        baseAnim.SetTrigger("GetUp");
        knockdownRoutine = null;
    }

    protected void ShowHitEffects(float value, Vector3 position)
    {
        GameObject sparkObj = Instantiate(hitSparkPrefab);
        sparkObj.transform.position = position;

        GameObject obj = Instantiate(hitValuePrefab);
        obj.GetComponent<Text>().text = value.ToString();
        obj.GetComponent<DestroyTimer>().EnableTimer(1.0f);

        GameObject canvas = GameObject.FindGameObjectWithTag("WorldCanvas");
        obj.transform.SetParent(canvas.transform);
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.transform.position = position;
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

[System.Serializable]
public class AttackData
{
    public float attackDamage = 10;
    public float force = 50;
    public bool knockdown = false;
}