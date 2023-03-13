using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyeonjaUpgraded : Hyeonja
{
    bool attacked = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        attacked = false;
    }

    protected override void FixedUpdate()
    {
        if (attacked) return;

        Duration -= Time.fixedDeltaTime;
        if (Duration <= 0)
        {
            if (weaponInfoCopy.isUpgradedByFlag || weaponManager.currentWeaponInfo[Define.Weapon.현자총통_비격진천뢰연계][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
            {
                attacked = true;
                Weapon Explosion = weaponManager.SpawnWeapon(Define.Weapon.현자총통_비격진천뢰연계, weaponInfoCopy);
                Explosion.SetWeapon(weaponInfoCopy, playerTransform, transform.position);
            }
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            transform.position += Time.fixedDeltaTime * Speed * transform.up;
        }
    }

    protected override void CreateExtraWeapon(Vector3 pos, float angle)
    {
        HyeonjaUpgraded newHyeonja = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.현자총통_비격진천뢰).GetComponent<HyeonjaUpgraded>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newHyeonja);

        newHyeonja.SetExtraWeapon(pos, angle + transform.rotation.eulerAngles.z);
        newHyeonja.playerTransform = playerTransform;
        newHyeonja.weaponInfoCopy.rarity = weaponInfoCopy.rarity;
    }

}
