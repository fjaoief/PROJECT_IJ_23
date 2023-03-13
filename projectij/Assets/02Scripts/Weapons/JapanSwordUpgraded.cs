using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JapanSwordUpgraded : JapanSword
{
    protected override void FixedUpdate()
    {
        if (isFirstWeapon)
        {
            if (StageManager.Instance.inputDir != (0, 0))
            {
                lastDir = StageManager.Instance.inputDir;

                if (weaponManager.currentWeaponInfo[Define.Weapon.야태도][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
                    weaponManager.currentWeaponInfo[Define.Weapon.야태도][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex].coolTime += Time.fixedDeltaTime;
            }

            if (attacked)
            {
                if (count <= 0)
                {
                    ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
                }
            }
        }
        else
        {
            Duration_Effect -= Time.fixedDeltaTime;
            transform.position += Time.fixedDeltaTime * Speed * transform.right;
            if (Duration_Effect <= 0)
            {
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
            }
        }
    }

    protected override void CreateExtraWeapon(Transform playerTransform, (float, float) dir)
    {
        JapanSwordUpgraded newSword = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.야태도).GetComponent<JapanSwordUpgraded>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newSword);
        newSword.SetExtraWeapon(playerTransform, dir);
        newSword.playerTransform = playerTransform;
        count--;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Trigger_stun(other);
    }
}
