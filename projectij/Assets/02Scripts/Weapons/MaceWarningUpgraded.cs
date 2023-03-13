using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceWarningUpgraded : MaceWarning
{
    protected override void FixedUpdate()
    {
        Duration_Effect -= Time.fixedDeltaTime;
        if (Duration_Effect <= 0 && !attacked)
        {
            if (weaponInfoCopy.isUpgradedByFlag || weaponManager.currentWeaponInfo[Define.Weapon.은입사철퇴연계][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
            {
                attacked = true;
                StartCoroutine(MaceAttack());
            }
            else
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);

        }
    }

    IEnumerator MaceAttack()
    {
        Vector2 pos = transform.position;

        for (int i = 0; i < 2; i++)
        {
            Weapon newMace = weaponManager.SpawnWeapon(Define.Weapon.은입사철퇴연계, weaponInfoCopy);
            newMace.SetWeapon(weaponInfoCopy, playerTransform, pos);
            yield return new WaitForSeconds(0.3f);
        }

        ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
    }
}
