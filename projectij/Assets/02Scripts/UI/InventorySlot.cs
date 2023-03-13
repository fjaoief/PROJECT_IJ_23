using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IDropHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    int charIndex;
    [SerializeField]
    int iconIndex;

    public WeaponInfo weaponInfo = null;

    GameObject window = null;

    Image coolTimeImage;

    private void Start() {
        window = StageManager.Instance.weaponInfoWindow;
        if (charIndex > -1)
            coolTimeImage = transform.GetChild(2).GetComponent<Image>();
    }
    
    public void SetStartingWeaponUI()
    {
        if (iconIndex == 0)
        {
            if (weaponInfo == null)
                transform.GetChild(3).gameObject.SetActive(false);
            else
            {
                if (weaponInfo.isStartWeapon)
                    transform.GetChild(3).gameObject.SetActive(true);
                else
                    transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate() 
    {
        if (charIndex > -1)
        {
            if (weaponInfo != null)
                coolTimeImage.fillAmount = weaponInfo.coolTime / weaponInfo.weapon.CoolTime;
            else
                coolTimeImage.fillAmount = 0;
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetWeaponInfoWindow();
    }

    public void SetWeaponInfoWindow()
    {
        if (weaponInfo != null)
        {
            window.SetActive(true);
            window.transform.position = transform.position + new Vector3(0, 1f, 0);
            window.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            
            SetText(weaponInfo);
        }
        else
        {
            window.SetActive(false);
        }
    }

    public void SetText(WeaponInfo info)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"<b><size=45>{info.weapon.weaponName}</size></b>\n");
        sb.Append($"{StageManager.Instance.explanation[info.weapon.weaponName]}\n");
        if (info.option != null)
        {
            foreach (KeyValuePair<Define.CharacterStat, float> kvp in info.option)
            {
                sb.Append($"{kvp.Key}: +{kvp.Value}\n");
            }
        }
        window.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sb.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (weaponInfo != null)
        {
            window.SetActive(false);
        }
    }

    // 무기 재조합용
    public void OnPointerClick(PointerEventData eventData)
    {
        if (weaponInfo == null) return;
        if (weaponInfo.isStartWeapon) return;
        GameObject recombinationIconObj = StageManager.Instance.recombinationIconObj;
        WeaponInfo firstSlotWeaponInfo = recombinationIconObj.transform.GetChild(0).GetComponent<RecombinationSlot>().weaponInfo;
        WeaponInfo secondSlotWeaponInfo = recombinationIconObj.transform.GetChild(1).GetComponent<RecombinationSlot>().weaponInfo;
        int index = -1;
        if (firstSlotWeaponInfo == null)
            index = 0;
        else if (secondSlotWeaponInfo == null)
            index = 1;

        if (index == -1) return; // 자리 없음
        // 같은 무기 다시 클릭
        if (recombinationIconObj.transform.GetChild(1 - index).GetComponent<RecombinationSlot>().weaponInfo != null)
        {
            if (recombinationIconObj.transform.GetChild(1 - index).GetComponent<RecombinationSlot>().weaponInfo == weaponInfo)
            {
                return;
            }
        }

        // 무기 정보 세팅
        recombinationIconObj.transform.GetChild(index).GetComponent<RecombinationSlot>().weaponInfo = weaponInfo;
        // StageManager.Instance.CopyWeaponInfo_Icon(weaponInfo, recombinationIconObj.transform.GetChild(index).GetComponent<RecombinationSlot>().weaponInfo);
        // 스프라이트 세팅
        recombinationIconObj.transform.GetChild(index).GetChild(1).GetComponent<Image>().sprite = transform.GetChild(1).GetComponent<Image>().sprite;
    }

    // 무기 스왑용
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log($"OnBeginDrag {charIndex}의 {iconIndex}슬롯");
        SetClickedInfo(0);
    }

    public void SetClickedInfo(int index)
    {
        if (index == 0)
        {
            StageManager.Instance.firstClickedInfo = (charIndex, iconIndex, weaponInfo);
            if (weaponInfo != null)
            {
                transform.GetChild(0).GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
        }
        else if (index == 1)
            StageManager.Instance.secondClickedInfo = (charIndex, iconIndex, weaponInfo);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log($"OnDrag {charIndex}의 {iconIndex}슬롯");
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log($"OnEndDrag {charIndex}의 {iconIndex}슬롯");
        StageManager.Instance.SwapSlot();
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log($"OnDrop {charIndex}의 {iconIndex}슬롯");
        SetClickedInfo(1);
    }

}
