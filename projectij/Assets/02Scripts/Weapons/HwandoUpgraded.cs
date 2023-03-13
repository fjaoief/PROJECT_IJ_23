using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HwandoUpgraded : Hwando
{
    protected override void CreateExtraWeapon()
    {
        //if (weaponManager.currentWeaponInfo[Define.Weapon.별운검][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] == null) return;
        GameObject newHwando = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.별운검);
        weaponManager.UpdateWeapon(weaponInfoCopy, newHwando.GetComponent<HwandoUpgraded>());
        newHwando.GetComponent<HwandoUpgraded>().SetExtraWeapon(transform.position + new Vector3(nextWeaponDistance, 0, 0));
        newHwando.GetComponent<HwandoUpgraded>().playerTransform = playerTransform;
        nextWeaponDistance += nextWeaponDistance;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (Duration_Effect < Duration_noAttack) return; // 공격 x, 이펙트 용

        base.Trigger_stun(other);
    }
}
