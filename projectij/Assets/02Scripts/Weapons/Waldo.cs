using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waldo : Weapon
{
    // 캐릭터 중심으로 사각지대가 있는 원호 모양의 공격
    // 나중에 스프라이트 피벗 / 콜라이더 조정 (사각지대)

    float rotation = 0f;
    bool flip = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        rotation = 0f;
    }

    private void FixedUpdate() {
        Duration -= Time.fixedDeltaTime;
        if (Duration <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            if (flip)
                rotation -= Time.fixedDeltaTime * Speed;
            else
                rotation += Time.fixedDeltaTime * Speed;

            transform.rotation = Quaternion.Euler(0, 0, rotation);
            transform.position = playerTransform.position;
        }
    }

    public override Quaternion SetDirection()
    {
        return Quaternion.Euler(0, 0, 0);
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        flip = playerTrans.GetComponent<PlayerCharacters>().spriteRenderer.flipX;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
