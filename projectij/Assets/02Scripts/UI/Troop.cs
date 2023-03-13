using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;

public class Troop : MonoBehaviour, IPointerClickHandler
{
    public GameObject new_entry;
    public GameObject entries;
    public static GameObject warning= null;
    private GameObject tmpObject;
    private float click_time = 0;

    bool isSquad = false;
    public CharacterData targetCharacter;

    private GameObject detailObj;
    private Image detailImg;
    private TextMeshProUGUI detailDescripttion;
    List<Dictionary<string, object>> explanationCSV;
    public Dictionary<string, string> start_explanation = new Dictionary<string, string>();
    private string cur_startingweapon;
    private string cur_jobname;

    private void Awake()
    {
        if (warning == null)
        {
            warning = GameObject.Find("Troop").transform.GetChild(1).GetChild(3).gameObject;
        }
        detailObj = GameObject.Find("Detail");
        detailImg = detailObj.transform.GetChild(0).gameObject.GetComponent<Image>();
        detailDescripttion = detailObj.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        explanationCSV = CSVReader.Read("CSV/AllWeaponExplanation");
        for (int i = 0; i < explanationCSV.Count; i++)
        {
            start_explanation.Add(explanationCSV[i]["무기 이름"].ToString(), explanationCSV[i]["무기 설명"].ToString());
        }

    }

    public void setIsSquad(bool b)
    {
        isSquad = b;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // 더블클릭 확인
        if ((Time.time - click_time) < 0.5f){
            //click_time = -1;
            dbclickTroop();
        } else{
            click_time = Time.time;
        }
        
        detailImg.sprite = GameManager.gameManager_Instance.buttonSprites[(int)targetCharacter.characterName];
        cur_jobname = targetCharacter.characterName.ToString();
        cur_startingweapon = GameManager.gameManager_Instance.characterInitialStatCSV[(int)targetCharacter.characterName]["시작무기"].ToString();
        StringBuilder sb = new StringBuilder();
        sb.Append($"<size=60>{cur_jobname}</size>\n");
        sb.Append($"<size=50>{targetCharacter.name.ToString()}</size>\n\v");
        sb.Append($"<align=left><size=40>Starting Weapon: <color=red>{cur_startingweapon}</color></size>\n{start_explanation[cur_startingweapon]}</align>");
        detailDescripttion.text = sb.ToString();
    }

    public void dbclickTroop(){
        Debug.Log(isSquad);
        if (isSquad)
        {
            // squad -> all
            GameManager.gameManager_Instance.ReturnPool(targetCharacter,transform.GetSiblingIndex()/2);
            Destroy(this.gameObject.transform.parent.gameObject.transform.GetChild(transform.GetSiblingIndex()+1).gameObject);
            Destroy(this.gameObject);
            
        }
        else
        {
            // all -> squad

            if (GameManager.gameManager_Instance.squad.Count <= 2)
            {
                Debug.Log(transform.GetSiblingIndex());
                GameManager.gameManager_Instance.AddSquad(targetCharacter, transform.GetSiblingIndex());
                Destroy(this.gameObject);
            }
            else
            {
                StartCoroutine("warn");
            }
        }
    }
    
    IEnumerator warn() {
        warning.SetActive(true);
        yield return new WaitForSeconds(1f);
        warning.SetActive(false);
    }

    public void Init(CharacterData c)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = GameManager.gameManager_Instance.portSprites[(int)c.characterName];
        targetCharacter = c;
        
    }

}

