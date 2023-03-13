using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class LevelUpWindow : MonoBehaviour
{
    // 무기 이름, 희귀도, 옵션
    List<(Define.Weapon, int, Dictionary<Define.CharacterStat, float>)> newWeaponList;
    KeyCode[] keyCodes = { KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4 };
    public Sprite supply_box;
    bool supply_selected;
    int supply_num;

    void OnEnable()
    {
        SetLevelUpWindow();
    }

    private void Update() 
    {
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
            {
                OnClickButton(Array.IndexOf(keyCodes, keyCode));
            }
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Refresh();
        }
    }
    public float reduceLoss = 0f;

    public void Refresh()
    {
        SetLevelUpWindow();
        float highestHP = 0;
        PlayerCharacters highestCharacter = null;
        for (int i = 0; i < 3; i++)
        {
            PlayerCharacters character = StageManager.Instance.playerCharacters[i];
            if (character.HP > highestHP)
            {
                highestHP = character.HP;
                highestCharacter = character;
            }
        }

        highestCharacter.ReduceHP(highestCharacter.characterStat[Define.CharacterStat.최대체력] / 20 * (1 - reduceLoss));
        //Debug.Log($"새로고침 대미지: {highestCharacter.maxHP / 20 * (1 - reduceLoss)}");
    }

    public void SetLevelUpWindow()
    {
        newWeaponList = new List<(Define.Weapon, int, Dictionary<Define.CharacterStat, float>)>();
        List<int> tempList = new List<int>();
        for (int i = 0; i < (int)Define.Weapon.왜검+1; i++)
        {
            tempList.Add(i);
        }

        for (int i = 0; i < 4; i++)
        {
            (Define.Weapon, int, Dictionary<Define.CharacterStat, float>) TargetWeapon;
            int num = UnityEngine.Random.Range(0, tempList.Count);
            TargetWeapon = ((Define.Weapon)tempList[num], 1, null);
            newWeaponList.Add(TargetWeapon);
            tempList.RemoveAt(num);

            Transform Parent = transform.GetChild(0).GetChild(i);

            Parent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = StageManager.Instance.weaponIcons[TargetWeapon.Item2][(int)TargetWeapon.Item1];
            string rarityStar = "";
            for (int star = 0; star < TargetWeapon.Item2; star++)
                rarityStar += "★";
            Parent.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"{TargetWeapon.Item1} {rarityStar}");

            Parent.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(StageManager.Instance.explanation[TargetWeapon.Item1]);

            if (TargetWeapon.Item3 == null) continue;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<Define.CharacterStat, float> kvp in TargetWeapon.Item3)
            {
                sb.Append($"{kvp.Key} +{kvp.Value}\n");
            }
            Parent.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().SetText(sb.ToString());
        }
        if((StageManager.Instance.all_quest & 4) == 4)
        {
            int winnging = UnityEngine.Random.Range(0, 1);
            if(winnging == 0)
            {
                supply_selected = true;
                int playce = UnityEngine.Random.Range(0, 4);
                supply_num = playce;
                Transform Parent = transform.GetChild(0).GetChild(playce);
                Parent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = supply_box;
                Parent.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText("보급상자");
                Parent.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().SetText("보급상자");
                Parent.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().SetText("보급상자");
            }
        }
    }

    public void OnClickButton(int index)
    {
        //Debug.Log($"index :{index}, count : {newWeaponList.Count}");
        //Debug.Log($"{UpgradeList[index].Item1} , {UpgradeList[index].Item2}");
        // StageManager.Instance.GetWeaponManager().UpgradeWeapon(UpgradeList[index].Item1, UpgradeList[index].Item2);
        if(supply_selected && supply_num == index)//보급상자고름
        {
            newWeaponList.Clear();
            supply_selected = false;
            StageManager.Instance.supply_get++;
            if(StageManager.Instance.supply_get == StageManager.Instance.supply_needed)//보급 다모음
            {
                StageManager.Instance.quest_clear += 4;
            }
            StageManager.Instance.setPause(1);
            gameObject.SetActive(false);
        }
        else
        {
            WeaponManager weaponManager = StageManager.Instance.GetWeaponManager();

            bool condition1 = weaponManager.charWeaponCount[0] + weaponManager.charWeaponCount[1] + weaponManager.charWeaponCount[2] < 6;
            bool condition2 = weaponManager.storageCount < 12;

            if (condition1 || condition2)
            {
                weaponManager.DistributeNewWeapon(newWeaponList[index]);
                newWeaponList.Clear();
                StageManager.Instance.setPause(1);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("무기창이 꽉 찼네..");
            }
        }
    }
}
