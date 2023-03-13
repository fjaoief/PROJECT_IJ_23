using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimePowderUpgraded : LimePowder
{
    protected override void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if (time >= attackInterval)
        {
            time = 0;
            StartCoroutine(Attack());
        }
    }
}
