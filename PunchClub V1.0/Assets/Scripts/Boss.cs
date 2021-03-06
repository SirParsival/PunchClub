using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy {

	protected override void Start () 
    {
        base.Start();
        canFlinch = false;
	}

    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        base.TakeDamage(value, hitVector, false);
    }

}
