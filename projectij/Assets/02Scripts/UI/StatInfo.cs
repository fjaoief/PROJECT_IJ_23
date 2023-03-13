using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class StatInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject window;

    // 스탯창 아이콘 hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        window = GameManager.gameManager_Instance.SquadWindowParent.transform.parent.GetChild(2).GetChild(1).gameObject;
        window.SetActive(true);
        window.transform.position = transform.position + new Vector3(0f, 0.5f, 0);
        window.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<Image>().sprite.name;
    }

    public void OnPointerExit (PointerEventData eventData)
     {
         window.SetActive(false);
     }

}
