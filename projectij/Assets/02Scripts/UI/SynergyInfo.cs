using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SynergyInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject window;
    public Define.Mastery masteryName;

    List<Dictionary<string, object>> SynergyExplanationCSV;
    private void Start() 
    {
        window = StageManager.Instance.SynergyInfoWindow;
        SynergyExplanationCSV = CSVReader.Read("CSV/SynergyExplanation");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        window.SetActive(true);
        window.transform.position = transform.position;
        string text = $"<b><size=30>{masteryName}</size></b>\n{SynergyExplanationCSV[(int)masteryName]["Explanation"].ToString()}";
        window.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }
    public void OnPointerExit (PointerEventData eventData)
     {
         window.SetActive(false);
     }
}
