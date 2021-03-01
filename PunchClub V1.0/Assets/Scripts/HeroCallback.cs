using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCallback : MonoBehaviour
{
    //1
    public Hero hero;
    //2
    public void DidChain(int chain)
    {
        hero.DidChain(chain);
    }

    public void DidJumpAttack()
    {
        hero.DidJumpAttack();
    }
}
