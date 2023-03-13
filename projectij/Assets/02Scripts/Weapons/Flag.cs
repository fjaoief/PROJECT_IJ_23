using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : Weapon
{
    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        if (weaponManager.flagCount < weaponInfoCopy.rarity)
            weaponManager.flagCount++;

        ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
    }

    public override void PassiveOn()
    {
        LevelUpWindow levelUpWindow = StageManager.Instance.LevelUpWindow.GetComponent<LevelUpWindow>();

        levelUpWindow.reduceLoss += 0.3f;
    }

    public override void PassiveOff()
    {
        LevelUpWindow levelUpWindow = StageManager.Instance.LevelUpWindow.GetComponent<LevelUpWindow>();

        levelUpWindow.reduceLoss -= 0.3f;
    }
}
