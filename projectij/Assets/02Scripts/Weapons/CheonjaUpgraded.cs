using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheonjaUpgraded : Cheonja
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (Duration_Effect < Duration_noAttack) return; // 공격 x, 이펙트 용
        base.Trigger_stun(other);
    }
}
