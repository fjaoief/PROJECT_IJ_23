using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mace : Weapon
{
    // 캐릭터 중심으로 원형 공격. 
    // 공격을 하기 전에 먼저 동그라미로 공격 지역을 표시하고 잠시뒤에 철퇴가 떨어짐. 
    // 공격 받은 적은 잠시동안 기절
    protected override void OnEnable()
    {
        base.OnEnable();
        soundplayer.PlayOneShot(startsound);
    }
    
    private void FixedUpdate()
    {
        Duration_Effect -= Time.fixedDeltaTime;
        if (Duration_Effect <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }

    public override Quaternion SetDirection()
    {
        return Quaternion.identity;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (Duration_Effect < Duration_noAttack) return; // 공격 x, 이펙트 용
        Trigger_stun(other);
    }
}
