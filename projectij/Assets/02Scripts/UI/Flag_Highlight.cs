using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Selectable))]
public class Flag_Highlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    string flag_explanation;
    int order;
    GameObject[] flag_obj = new GameObject[3];
    TextMeshProUGUI text;
    // Start is called before the first frame update
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
        {
            text.text = flag_explanation;
            for(int i =0;i<3;i++)
            {
                if (i == order)
                    flag_obj[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                else
                    flag_obj[i].GetComponent<Image>().color = new Color(1, 1, 1, 100 / 255f);
            }
        }
            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        for(int i =0;i<3;i++)
            flag_obj[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        text.text = "";
    }

    public void setting(string str, int num, GameObject[] flags, TextMeshProUGUI txt)
    {
        flag_explanation = str;
        order = num;
        flag_obj = flags;
        text = txt;
    }
}
