using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowSlotInfo : MonoBehaviour
{
    public int charIndex;
    public int iconIndex;

    private void Update() {
        
        WeaponInfo weaponInfo = StageManager.Instance.weaponIconsTrans.GetChild(charIndex).GetChild(iconIndex).GetComponent<InventorySlot>().weaponInfo;
        if (weaponInfo == null)
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "null";
        else
            //transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{weaponInfo.weapon.weaponName}\n{weaponInfo.rarity}\n{weaponInfo.weapon.masteryName}";
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{weaponInfo.weapon.weaponName}\n{weaponInfo.weapon.masteryName}";
    }
}
