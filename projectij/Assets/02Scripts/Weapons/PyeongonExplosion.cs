using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyeongonExplosion : Weapon
{

    private void FixedUpdate() {
        Duration_Effect -= Time.fixedDeltaTime;
        if (Duration_Effect <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (Duration_Effect < Duration_noAttack) return; // 공격 x, 이펙트 용
        base.OnTriggerEnter2D(other);
    }
}
