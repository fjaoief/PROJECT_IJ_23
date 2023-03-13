using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialInfo
{
    public float posx;
    public float posy;
    public string content;
}

public class Tutorial : MonoBehaviour
{
    private int cur;
    private int all;
    private List<Dictionary<string, object>> tutorialCSV;
    private List<TutorialInfo> allTutorialInfo = new List<TutorialInfo>();
    public Sprite end_sprite;

    public GameObject dialogObj;
    private TextMeshProUGUI ment;
    private RectTransform dialog_pos;

    // Start is called before the first frame update
    void Awake()
    {
        cur = 0;
        tutorialCSV = CSVReader.Read("CSV/AllTutorialInfo");
        all = tutorialCSV.Count - 1;
        for (int i = 0; i <= all ; i++)
        {
            TutorialInfo add = new TutorialInfo();
            add.posx = float.Parse(tutorialCSV[i]["Pos X"].ToString());
            add.posy = float.Parse(tutorialCSV[i]["Pos Y"].ToString());
            add.content = tutorialCSV[i]["내용"].ToString().Replace("n", "\n");
            allTutorialInfo.Add(add);
        }
        ment = dialogObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        Debug.Log(ment);
        dialog_pos = dialogObj.GetComponent<RectTransform>();

        dialog_pos.anchoredPosition = new Vector2(allTutorialInfo[0].posx, allTutorialInfo[0].posy);
        ment.text = allTutorialInfo[0].content;
    }

    private void Start() {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("Button"));
    }
    public void NextBtn()
    {
        if (cur == all)
        {
            SceneManager.LoadScene("StartScene");
        } else{
            cur += 1;
            TutorialText(cur);
        }        
    }

    private void TutorialText(int current)
    {
        dialog_pos.anchoredPosition = new Vector2(allTutorialInfo[current].posx, allTutorialInfo[current].posy);
        ment.text = allTutorialInfo[current].content;

        if (current == all)
        {
            dialogObj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = end_sprite;
        }
    }
}
