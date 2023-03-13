using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GakgoongUpgraded : Gakgoong
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "Enemy")
        {
            PenetrationCount--;
            // 불장판 생성
            if (weaponInfoCopy.isUpgradedByFlag || weaponManager.currentWeaponInfo[Define.Weapon.각궁_불화살연계][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
            {
                Weapon newField = weaponManager.SpawnWeapon(Define.Weapon.각궁_불화살연계, weaponInfoCopy);
                newField.SetWeapon(weaponInfoCopy, playerTransform, transform.position);
            }
            
        }
    }
}
