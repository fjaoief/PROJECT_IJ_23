using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Entry : MonoBehaviour, IPointerClickHandler
{
    private float click_time = 0;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 더블클릭 확인
        if ((Time.time - click_time) < 0.3f){
            click_time = -1;
            dbclickEntry();
        } else{
            click_time = Time.time;

        }
    }
    public void dbclickEntry(){
        gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(1,1,1,0);
    }
    


}
