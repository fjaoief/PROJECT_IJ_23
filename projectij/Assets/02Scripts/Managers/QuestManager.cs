using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour, IPointerClickHandler
{
    public int limit_quest_rank = 4;
    public GameObject[] Quest = new GameObject [5];//게임선택창 버튼
    [SerializeField]
    private GameObject[] SelectImg = new GameObject[4];//게임선택창 이미지오브젝트
    [SerializeField]
    private Sprite[] level_img = new Sprite[8];//게임 선택창 나이도별 일반과 선택이미지
    Sprite[] FlagSprite = new Sprite[11];//적군 부대기 이미지
    Sprite[] selected_flag = new Sprite[3];//부대기 이미지 3개 추출저장용
    string[] army_explanation;//적군 부대 설명(9칸)
    string[] army_expl_select;//적군 부대 설명 추출저장용(3칸)

    public GameObject Troop_UI;
    List<Dictionary<string, object>> QuestCSV;
    List<Dictionary<string, object>> Quest_reward_CSV;
    List<Dictionary<string, object>> rank1,rank2,rank3,rank4,rank5;
    public int selected_num;

    int[] flag_num = new int[3];//생성하는 적 부대마크 식별번호(한 게임당 세종류)


    private void Awake()
    {
        rank1 = new List<Dictionary<string, object>>();
        rank2 = new List<Dictionary<string, object>>();
        rank3 = new List<Dictionary<string, object>>();
        rank4 = new List<Dictionary<string, object>>();
        rank5 = new List<Dictionary<string, object>>();

        QuestCSV = CSVReader.Read("CSV/Quest_List");
        Quest_reward_CSV = GameManager.gameManager_Instance.quest_reward_specificationt_CSV;
        for(int i =0;i<Quest_reward_CSV.Count;i++)
        {
            switch((int)Quest_reward_CSV[i]["출현 난이도"])
            {
                case 1:
                    rank1.Add(Quest_reward_CSV[i]);
                    break;
                case 2:
                    rank2.Add(Quest_reward_CSV[i]);
                    break;
                case 3:
                    rank3.Add(Quest_reward_CSV[i]);
                    break;
                case 4:
                    rank4.Add(Quest_reward_CSV[i]);
                    break;
                case 5:
                    rank5.Add(Quest_reward_CSV[i]);
                    break;
            }
        }
        //부대기 이미지 가져오기
        FlagSprite = Resources.LoadAll<Sprite>("flag");
        army_explanation = new string[] { "초소카베 부대는 이동속도가 빠르지만 공격력이 낮습니다.",
            "가토 부대는 ",
            "고니시 부대는 ",
            "구로다 부대는 ",
            "쿠바야카와 부대는 ",
            "오토모 부대는 ",
            "시나즈 부대는 ",
            "소 부대는 ",
            "우키다 부대는 "};
    }

    private void OnEnable()
    {
        /*if (GameManager.gameManager_Instance.quest_selected == false)
        {
            GameManager.gameManager_Instance.quest_selected = true;
            List<int> list = new List<int> { };
            for (int j = 0; j < QuestCSV.Count; j++)
            {
                list.Add(j);
            }

            for (int i = 0; i < 4;)
            {

                //중복 제거과정
                int num = Random.Range(0, list.Count);
                int rand = list[num];
                GameManager.data quest_data = new GameManager.data();
                if ((int)QuestCSV[rand]["difficulty"] <= limit_quest_rank)//가능한 퀘스트 난이도
                {
                    Quest[i].transform.GetChild(0).GetComponent<Text>().text = QuestCSV[rand]["difficulty"].ToString();
                    //맵에 생성될 부대3개 선정
                    List<int> army_list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    for (int k = 0; k < 3; k++)
                    {
                        int army_rand = Random.Range(0, army_list.Count);
                        quest_data.d[k] = army_list[army_rand];
                        army_list.RemoveAt(army_rand);
                    }
                    Quest[i].transform.GetChild(1).GetComponent<Text>().text = ((Define.army_name)quest_data.d[0] - 1).ToString() + ", " + ((Define.army_name)quest_data.d[1] - 1).ToString() + ", " + ((Define.army_name)quest_data.d[2] - 1).ToString();
                    //퀘스트 선택  임시로 생존만 뜨게 변경
                    Quest[i].transform.GetChild(2).GetComponent<Text>().text = QuestCSV[0]["quest_content"].ToString();//퀘스트 종류 선택
                    quest_data.a = (int)QuestCSV[rand]["difficulty"];
                    quest_data.c = QuestCSV[0]["quest_content"].ToString();
                    switch ((int)QuestCSV[rand]["difficulty"])
                    {
                        case 1:
                            Quest[i].transform.GetChild(3).GetComponent<Text>().text = rank1[Random.Range(0, rank1.Count)]["이름"].ToString();
                            quest_data.b = (Define.QuestRewardStat)System.Enum.Parse(typeof(Define.QuestRewardStat), Quest[i].transform.GetChild(3).GetComponent<Text>().text);
                            break;
                        case 2:
                            Quest[i].transform.GetChild(3).GetComponent<Text>().text = rank2[Random.Range(0, rank2.Count)]["이름"].ToString();
                            quest_data.b = (Define.QuestRewardStat)System.Enum.Parse(typeof(Define.QuestRewardStat), Quest[i].transform.GetChild(3).GetComponent<Text>().text);
                            break;
                        case 3:
                            Quest[i].transform.GetChild(3).GetComponent<Text>().text = rank3[Random.Range(0, rank3.Count)]["이름"].ToString();
                            quest_data.b = (Define.QuestRewardStat)System.Enum.Parse(typeof(Define.QuestRewardStat), Quest[i].transform.GetChild(3).GetComponent<Text>().text);
                            break;
                        case 4:
                            Quest[i].transform.GetChild(3).GetComponent<Text>().text = rank4[Random.Range(0, rank4.Count)]["이름"].ToString();
                            quest_data.b = (Define.QuestRewardStat)System.Enum.Parse(typeof(Define.QuestRewardStat), Quest[i].transform.GetChild(3).GetComponent<Text>().text);
                            break;
                        case 5:
                            Quest[i].transform.GetChild(3).GetComponent<Text>().text = rank5[Random.Range(0, rank5.Count)]["이름"].ToString();
                            quest_data.b = (Define.QuestRewardStat)System.Enum.Parse(typeof(Define.QuestRewardStat), Quest[i].transform.GetChild(3).GetComponent<Text>().text);
                            break;
                    }
                    GameManager.gameManager_Instance.quest_list.Add(quest_data);
                    list.RemoveAt(num);
                    i++;
                }
            }
        }*/
        if (GameManager.gameManager_Instance.quest_selected == false)
        {
            GameManager.gameManager_Instance.quest_selected = true;
            List<int> list = new List<int> { };
            for (int j = 0; j < QuestCSV.Count; j++)
            {
                list.Add(j);
            }
            for (int i = 0; i < 4;)
            {
                //중복 제거과정
                int num = Random.Range(0, list.Count);
                int rand = list[num];
                GameManager.data quest_data = new GameManager.data();
                if ((int)QuestCSV[rand]["difficulty"] <= limit_quest_rank)//가능한 퀘스트 난이도
                {
                    //맵에 생성될 부대3개 선정
                    List<int> army_list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    for (int k = 0; k < 3; k++)
                    {
                        int army_rand = Random.Range(0, army_list.Count);
                        quest_data.d[k] = army_list[army_rand];
                        selected_flag[k] = FlagSprite[army_list[army_rand] - 1];
                        
                        army_list.RemoveAt(army_rand);
                    }
                    army_expl_select = new string[] {army_explanation[quest_data.d[0]-1], army_explanation[quest_data.d[1]-1], army_explanation[quest_data.d[2] - 1] };
                    //퀘스트 선택  임시로 생존만 뜨게 변경
                    quest_data.a = (int)QuestCSV[rand]["difficulty"];
                    quest_data.c = QuestCSV[0]["quest_content"].ToString();
                    Quest[i].GetComponent<Quest_Highlight>().image_setting(level_img[2 * quest_data.a - 2], level_img[2 * quest_data.a - 1], SelectImg[i], quest_data.d, selected_flag, army_expl_select);//이미지 세팅
                    GameManager.gameManager_Instance.quest_list.Add(quest_data);
                    list.RemoveAt(num);
                    i++;
                }
            }
        }
    }

    private void OnDisable() {
        if (GameObject.Find("Button0") != null)
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Button0"));
    }

    public void OnPointerClick(PointerEventData eventData)//퀘스트창 밖 클릭시 퀘스트창 닫기
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.pointerCurrentRaycast.gameObject == this.gameObject)
        {
            gameObject.SetActive(false);
        }

    }

    public void Questselect()
    {
        GameObject click = EventSystem.current.currentSelectedGameObject;
        Debug.Log(click.name);
    }
}
