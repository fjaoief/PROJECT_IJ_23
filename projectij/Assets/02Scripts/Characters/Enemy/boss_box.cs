using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_box : MonoBehaviour
{
    public void GetDistroy()
    {
        Debug.Log("보스상자 부숴짐");
        WeaponManager weaponManager = StageManager.Instance.GetWeaponManager();

        bool condition1 = weaponManager.charWeaponCount[0] + weaponManager.charWeaponCount[1] + weaponManager.charWeaponCount[2] < 6;
        bool condition2 = weaponManager.storageCount < 12;

        if (condition1 || condition2)
        {
            List<int> tempList = new List<int>();
            for (int i = 0; i < (int)Define.Weapon.왜검 + 1; i++)
                tempList.Add(i);
            int num = Random.Range(0, tempList.Count);
            (Define.Weapon, int, Dictionary<Define.CharacterStat, float>) TargetWeapon;
            TargetWeapon = ((Define.Weapon)tempList[num], 2, null);
            weaponManager.DistributeNewWeapon(TargetWeapon);
        }
        else
        {
            Debug.Log("무기창이 꽉 찼네..");
        }
        Destroy(this.gameObject);
    }
}
