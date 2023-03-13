using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        foreach (PlayerCharacters characters in StageManager.Instance.playerCharacters)
        {
            if (characters.shieldCount < weaponInfoCopy.rarity + 1)
                characters.shieldCount++;
        }
        //Debug.Log($"{playerObj.name}의 쉴드 : {playerObj.GetComponent<PlayerCharacters>().shieldCount}개");
        ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
    }

    public override void PassiveOn()
    {
        foreach (PlayerCharacters characters in StageManager.Instance.playerCharacters)
        {
            characters.characterStat[Define.CharacterStat.size] -= characters.InitialCharStat[Define.CharacterStat.size] * 0.1f;
        }
    }

    public override void PassiveOff()
    {
        foreach (PlayerCharacters characters in StageManager.Instance.playerCharacters)
        {
            characters.characterStat[Define.CharacterStat.size] += characters.InitialCharStat[Define.CharacterStat.size] * 0.1f;
        }
    }
}
