using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyeongon : Weapon
{
    protected float rotation = 0f;

    protected override void OnEnable()
    {
        base.OnEnable();
        rotation = 0f;
    }   

    // 캐릭터 중심으로 빙글빙글 돌다가 폭발
    protected virtual void FixedUpdate() {
        Duration -= Time.fixedDeltaTime;
        transform.position = playerTransform.position;
        if (Duration >= 0) // 빙글빙글
        {
            rotation -= Time.fixedDeltaTime * Speed;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
        else // 지속시간 끝
        {
            if (weaponManager.currentWeaponInfo[Define.Weapon.편곤연계][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
            {
                Weapon newPyeongon = weaponManager.SpawnWeapon(Define.Weapon.편곤연계, weaponInfoCopy);
                newPyeongon.SetWeapon(weaponInfoCopy, playerTransform, transform.GetChild(0).transform.position);
            }
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }

    public override Quaternion SetDirection()
    {
        return Quaternion.Euler(0, 0, 0);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
