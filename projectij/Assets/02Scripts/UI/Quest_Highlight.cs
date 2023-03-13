using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Selectable))]
public class Quest_Highlight : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    [SerializeField]
    private GameObject SelectImg;
    [SerializeField]
    private Sprite[] level_highlight = new Sprite[2];
    GameObject explanation;
    string[] army_explnation = new string[3];
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            SelectImg.GetComponent<SpriteRenderer>().sprite = level_highlight[1];
            explanation.SetActive(true);
        }

    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
        SelectImg.GetComponent<SpriteRenderer>().sprite = level_highlight[0];
        explanation.SetActive(false);
    }

    public void image_setting(Sprite origin, Sprite selected, GameObject a,int [] flagnum, Sprite [] flag_img, string[] str)//난이도에따른 이미지 세팅(기본이미지/선택이미지/이미지 오브젝트/부대번호(3개)/부대깃이미지(3개)/부대설명(3개))
    {
        SelectImg = a;
        level_highlight[0] = origin;
        level_highlight[1] = selected;
        SelectImg.GetComponent<SpriteRenderer>().sprite = level_highlight[0];
        //출현 부대 설명및 부대깃발 임포트
        explanation = this.gameObject.transform.parent.GetChild(2).gameObject;
        for (int i =0;i<3;i++)
        {
            explanation.transform.GetChild(i).GetComponent<Image>().sprite = flag_img[i];
            explanation.transform.GetChild(i).GetComponent<Flag_Highlight>().setting(str[i], i, new GameObject[] { explanation.transform.GetChild(0).gameObject, explanation.transform.GetChild(1).gameObject, explanation.transform.GetChild(2).gameObject }, explanation.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>());
        }

        
    }
}
