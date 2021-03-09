using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Enemy
{
    public RobotColor color;

    public void SetColor(RobotColor color)
    {
        this.color = color;

        switch (color)
        {
            case RobotColor.Colorless:
                baseSprite.color = Color.white;
                maxLife = 50.0f;
                normalAttack.attackDamage = 2;
                break;
            case RobotColor.Copper:
                baseSprite.color = new Color(1.0f, 0.75f, 0.62f);
                maxLife = 100.0f;
                normalAttack.attackDamage = 4;
                break;
            case RobotColor.Silver:
                baseSprite.color = Color.white;
                maxLife = 125.0f;
                normalAttack.attackDamage = 5;
                break;
            case RobotColor.Gold:
                baseSprite.color = new Color(0.91f, 0.7f, 0.0f);
                maxLife = 150.0f;
                normalAttack.attackDamage = 6;
                break;
            case RobotColor.Random:
                baseSprite.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
                maxLife = Random.Range(100, 250);
                normalAttack.attackDamage = Random.Range(4, 10);
                break;
        }
        currentLife = maxLife;
    }

    [ContextMenu("Color: Copper")]
    void SetToCopper()
    {
        SetColor(RobotColor.Copper);
    }
    
    [ContextMenu("Color: Silver")]
    void SetToSilver()
    {
        SetColor(RobotColor.Silver);
    }
    
    [ContextMenu("Color: Gold")]
    void SetToGold()
    {
        SetColor(RobotColor.Gold);
    }
    
    [ContextMenu("Color: Random")]
    void SetToRandom()
    {
        SetColor(RobotColor.Random);
    }

    protected override IEnumerator KnockdownRoutine()
    {
        isKnockedOut = true;
        baseAnim.SetTrigger("Knockdown");
        ai.enabled = false;

        actorCollider.SetColliderStance(false);
        yield return new WaitForSeconds(2.0f);
        actorCollider.SetColliderStance(true);

        baseAnim.SetTrigger("GetUp");
        ai.enabled = true;
        knockdownRoutine = null;
    }
}

public enum RobotColor
{
    Colorless = 0,
    Copper,
    Silver,
    Gold,
    Random
}
