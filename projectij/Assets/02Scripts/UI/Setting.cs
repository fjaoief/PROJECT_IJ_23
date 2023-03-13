using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Setting : MonoBehaviour, IPointerClickHandler
{
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.pointerCurrentRaycast.gameObject == this.gameObject)
        {
            gameObject.SetActive(false);
        }

    }

    private void OnEnable() {
        EventSystem.current.SetSelectedGameObject(transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
    }

    private void OnDisable() {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Button4"));
    }
}
