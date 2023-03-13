using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class Tomb : MonoBehaviour
{
    public CharacterData targetCharacter;

    private GameObject detailObj;
    private Image detailImg;
    private TextMeshProUGUI detailName;
    private TextMeshProUGUI detailDescription;

    private void Awake()
    {
        detailObj = GameObject.Find("SelectedOne");
        detailImg = detailObj.transform.GetChild(0).gameObject.GetComponent<Image>();
        detailName = detailObj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        detailDescription = detailObj.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void morClick()
    {       
        detailImg.sprite = GameManager.gameManager_Instance.portSprites[(int)targetCharacter.characterName];
        detailName.text = targetCharacter.name.ToString();
        StringBuilder sb = new StringBuilder();
        if (targetCharacter.questRewardStats.Count > 0){
            foreach(Define.QuestRewardStat quest in targetCharacter.questRewardStats)
                sb.Append($"{quest.ToString()}");
        }
        detailDescription.text = sb.ToString();
    }

    public void Init(CharacterData c)
    {
        targetCharacter = c;
    }
}
