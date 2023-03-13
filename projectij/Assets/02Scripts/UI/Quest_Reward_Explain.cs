using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Quest_Reward_Explain : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    List<Dictionary<string, object>> explaination;
    GameObject tab;

    private void Start()
    {
        explaination = CSVReader.Read("CSV/Quest_reward_content");
        tab = transform.parent.GetChild(5).gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Dictionary<string, object> a = explaination.Find(x => x["reward"].ToString() == this.GetComponent<Text>().text);
        tab.transform.GetChild(0).GetComponent<Text>().text = a["reward_explanation"].ToString();
        tab.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tab.SetActive(false);
    }

}
