using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanWand : Weapon
{
    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        StartCoroutine(SetWeaponCoroutine());
    }

    IEnumerator SetWeaponCoroutine()
    {
        for (int i = 0; i < weaponInfoCopy.rarity; i++)
        {
            StageManager.Instance.LevelUpWindow.SetActive(true);
            StageManager.Instance.setPause(0);
            yield return new WaitForSeconds(0.01f);
        }

        ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
    }

    public override void PassiveOn()
    {
        foreach (PlayerCharacters characters in StageManager.Instance.playerCharacters)
        {
            characters.ExpBuff += 0.05f;
        }
    }

    public override void PassiveOff()
    {
        foreach (PlayerCharacters characters in StageManager.Instance.playerCharacters)
        {
            characters.ExpBuff -= 0.05f;
        }
    }
}
