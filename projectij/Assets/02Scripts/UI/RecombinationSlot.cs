using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecombinationSlot : MonoBehaviour, IPointerClickHandler
{
    public WeaponInfo weaponInfo = null;

    // 슬롯 비우기
    public void OnPointerClick(PointerEventData eventData)
    {
        EmptySlot();
    }

    public void EmptySlot()
    {
        weaponInfo = null;
        transform.GetChild(1).GetComponent<Image>().sprite = StageManager.Instance.weaponIcons[0][0];
    }

}
