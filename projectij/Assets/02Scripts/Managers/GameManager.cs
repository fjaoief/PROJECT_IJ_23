using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager_Instance=null;
    // battleScene에 넘길 list<character> 추가
    // 컨테이너는 생각해봐야함
    [SerializeField]
    public List<CharacterData> characterPool = new List<CharacterData>();
    [SerializeField]
    public List<CharacterData> squad = new List<CharacterData>();
    [SerializeField]
    public List<CharacterData> tomb = new List<CharacterData>();
    [SerializeField]
    public GameObject SquadWindowParent = null;
    [SerializeField]
    Transform StatWindow;
    [SerializeField]
    Transform morWindow;
    [SerializeField]
    GameObject CharacterPoolParent = null;
    [SerializeField]
    GameObject buttonPrefab = null;
    [SerializeField]
    GameObject emptyPrefab = null;
    [SerializeField]
    GameObject statPrefab = null;
    [SerializeField]
    GameObject morPrefab = null;
    [SerializeField]
    public List<Sprite> buttonSprites= new List<Sprite>();
    [SerializeField]
    public List<Sprite> portSprites= new List<Sprite>();
    public Sprite blank;
    public Texture2D cursor;

    public Text gold_text;
    //StageManager로 전달하는/받는 변수
    public List<data> quest_list = new List<data>();
    public class data
    {
        public int a { get; set; }//난이도
        public Define.QuestRewardStat b { get; set; }//퀘스트 보상이름
        public string c { get; set; }//퀘스트 내용

        public int[] d = new int[3];

    }
    public int gold = 1000;
    public int difficulty;
    public Define.QuestRewardStat quest_reward;
    public string quest_type;
    public int[] flag_num = new int[3];

    bool Tutorial = false;
    public bool quest_selected = false; 

    // 캐릭터 초기 스탯
    public List<Dictionary<string, object>> characterInitialStatCSV;
    //퀘스트 보상 세부 스탯
    public List<Dictionary<string, object>> quest_reward_specificationt_CSV;
    Dictionary <string, string> translate = new Dictionary<string, string>()
    {
        {"환도", "swordman"},
        {"승자총통", "seung"},
        {"깃발", "flag"},
        {"화차", "hwacha"},
        {"창", "spearman"},
        {"신장대", "shaman"},
        {"돌팔매", "stonesling"},
        {"월도", "monk"},
        {"각궁", "bow"},
        {"방패", "shieldman"},
        {"철퇴", "mace"},
        {"천자총통", "cheonja"},
        {"편곤", "flail"},
        {"왜검", "samurai"}
    };
    public Dictionary<Define.CharacterStat, float>[] totalInitialCharStat = new Dictionary<Define.CharacterStat, float>[14];
    private int[] stats = new int[7] {-1, 9, 2, 3, 4, 7, 8};

    

    private void Awake()
    {
        if (gameManager_Instance == null || FindObjectOfType<GameManager>()==null)
        {
            gameManager_Instance = this;
            if (GameObject.Find("Troop") != null)
            {
                Transform CommonParent = GameObject.Find("Troop").transform;
                SquadWindowParent = CommonParent.GetChild(1).GetChild(1).gameObject;
                StatWindow = CommonParent.GetChild(1).GetChild(2).GetChild(0);
                CharacterPoolParent = CommonParent.GetChild(4).GetChild(0).GetChild(0).gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        // 여기서 기존 데이터 파일 불러오기
        string squadFilePath = Application.dataPath + "/squad.bin";
        string characterPoolFilePath = Application.dataPath + "/characterPool.bin";
        string tombFilePath = Application.dataPath + "/tomb.bin";
        List<CharacterData> tempSquad = SaveSystem.Load<List<CharacterData>>(squadFilePath);
        List<CharacterData> tempCharPool = SaveSystem.Load<List<CharacterData>>(characterPoolFilePath);
        List<CharacterData> tempTomb = SaveSystem.Load<List<CharacterData>>(tombFilePath);

        if (tempSquad != null && tempCharPool !=null)
        {
            //Debug.Log(squad.Count + "" + characterPool.Count);
            //Debug.Log(tomb.Count);

            tomb = tempTomb;
            characterPool = tempCharPool;
            squad = tempSquad;
        }
        else
        {
            SaveSystem.Save<List<CharacterData>>(squad, squadFilePath);
            SaveSystem.Save<List<CharacterData>>(characterPool, characterPoolFilePath);
            SaveSystem.Save<List<CharacterData>>(tomb, tombFilePath);
        }

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);

        characterInitialStatCSV = CSVReader.Read("CSV/AllCharacterInitialStat");
        for (int j=0;j<characterInitialStatCSV.Count;j++){
            Dictionary<Define.CharacterStat, float> initialCharStat = new Dictionary<Define.CharacterStat, float>();
            for (int i = 0; i < (int)Define.CharacterStat.end; i++)
            {
                initialCharStat.Add((Define.CharacterStat)i, 1);
            }
                    
            Define.CharacterName characterName = (Define.CharacterName)Enum.Parse(typeof(Define.CharacterName), translate[characterInitialStatCSV[j]["시작무기"].ToString()]);
            initialCharStat[Define.CharacterStat.데미지] += float.Parse(characterInitialStatCSV[j]["Damage"].ToString());
            initialCharStat[Define.CharacterStat.공격크기] += float.Parse(characterInitialStatCSV[j]["Size"].ToString());
            initialCharStat[Define.CharacterStat.쿨타임감소] = float.Parse(characterInitialStatCSV[j]["CoolTime"].ToString());
            initialCharStat[Define.CharacterStat.추가투사체확률] = float.Parse(characterInitialStatCSV[j]["ProjectilePercentage"].ToString());
            initialCharStat[Define.CharacterStat.투사체속도] += float.Parse(characterInitialStatCSV[j]["Speed"].ToString());
            initialCharStat[Define.CharacterStat.관통횟수] = float.Parse(characterInitialStatCSV[j]["PenetrationCount"].ToString());
            initialCharStat[Define.CharacterStat.넉백] += float.Parse(characterInitialStatCSV[j]["Force"].ToString());
            initialCharStat[Define.CharacterStat.방어도] = float.Parse(characterInitialStatCSV[j]["Defense"].ToString());
            initialCharStat[Define.CharacterStat.최대체력] = float.Parse(characterInitialStatCSV[j]["HP"].ToString());
            initialCharStat[Define.CharacterStat.이동속도] = float.Parse(characterInitialStatCSV[j]["MoveSpeed"].ToString());
            initialCharStat[Define.CharacterStat.size] = float.Parse(characterInitialStatCSV[j]["CharacterDistance"].ToString());
            initialCharStat[Define.CharacterStat.clashDamage] = 5;
            totalInitialCharStat[(int)characterName] = initialCharStat;

            //Debug.Log(characterInitialStatCSV[j]["시작무기"].ToString());    
        }
        quest_reward_specificationt_CSV = CSVReader.Read("CSV/quest_reward_specification");
    }


    void Start()
    {
        SceneManager.sceneLoaded += FindGoldTxt;
        gold_text.text = "G : " + gold;
    }

    private void FindGoldTxt(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScene"){
            gold_text = GameObject.Find("Gold_Text").GetComponent<Text>();
            gold_text.text = "G : " + gold;
        }
    }

    public bool AddSquad(CharacterData c, int characterPoolIndex)
    {
        // 부대에 넣을 때
        if(squad.Count>=3)
        {
            return false;
        }
        else
        {
            GameObject go = Instantiate(buttonPrefab, SquadWindowParent.transform);
            go.GetComponent<Troop>().Init(c);
            go.GetComponent<Troop>().setIsSquad(true);
            for(int i = c.quest_pos; i<c.questRewardStats.Count;i++)
            {
                for (int k = 0; k < quest_reward_specificationt_CSV.Count; k++)
                {
                    if(Equals(quest_reward_specificationt_CSV[k]["이름"].ToString(), c.questRewardStats[i].ToString()))
                    {
                        Debug.Log(quest_reward_specificationt_CSV[k]["이름"]+ "  " + k);
                        c.quest_stat[Define.CharacterStat.데미지] += float.Parse(quest_reward_specificationt_CSV[k]["공격력"].ToString());
                        c.quest_stat[Define.CharacterStat.공격크기] += float.Parse(quest_reward_specificationt_CSV[k]["공격 크기"].ToString());
                        c.quest_stat[Define.CharacterStat.쿨타임감소] += float.Parse(quest_reward_specificationt_CSV[k]["쿨타임 감소"].ToString());
                        c.quest_stat[Define.CharacterStat.추가투사체확률] += float.Parse(quest_reward_specificationt_CSV[k]["추가 공격 확률"].ToString());
                        c.quest_stat[Define.CharacterStat.투사체속도] += float.Parse(quest_reward_specificationt_CSV[k]["투사체 속도"].ToString());
                        c.quest_stat[Define.CharacterStat.관통횟수] += float.Parse(quest_reward_specificationt_CSV[k]["투사체 추가 관통"].ToString());
                        c.quest_stat[Define.CharacterStat.넉백] += float.Parse(quest_reward_specificationt_CSV[k]["넉백"].ToString());
                        c.quest_stat[Define.CharacterStat.방어도] += float.Parse(quest_reward_specificationt_CSV[k]["방어력"].ToString());
                        c.quest_stat[Define.CharacterStat.최대체력] += float.Parse(quest_reward_specificationt_CSV[k]["hp"].ToString());
                        c.quest_stat[Define.CharacterStat.이동속도] += float.Parse(quest_reward_specificationt_CSV[k]["이동속도"].ToString());
                        c.additional_gold += int.Parse(quest_reward_specificationt_CSV[k]["파견비용 추가"].ToString());
                        break;
                    }
                }
                c.quest_pos++;
            }
            squad.Add(c);
            characterPool.RemoveAt(characterPoolIndex);
            Instantiate(emptyPrefab, SquadWindowParent.transform);
            //스탯
            GameObject stat = Instantiate(statPrefab, StatWindow.transform);
            for (int j = 0; j < 7; j++)
            {
                if (j==0){
                    stat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{(totalInitialCharStat[(int)c.characterName][Define.CharacterStat.이동속도]+ c.quest_stat[Define.CharacterStat.이동속도]):0.##}";
                }
                else{
                    stat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{(totalInitialCharStat[(int)c.characterName][(Define.CharacterStat)stats[j]]+ c.quest_stat[(Define.CharacterStat)stats[j]]):0.##}";
                }
            }
        }
        SaveStatus();
        return true;
    }

    public void ReturnPool(CharacterData c, int squadIndex)
    {
        // 부대에서 클릭해서 다시 뺄 때        
        GameObject go = Instantiate(buttonPrefab, CharacterPoolParent.transform);
        go.GetComponent<Troop>().Init(c);
        squad.RemoveAt(squadIndex);
        characterPool.Add(c); 

        //스탯 표시 제거
        Destroy(StatWindow.GetChild(squadIndex+1).gameObject);
        
        SaveStatus();
    }

    public void SendToTomb(CharacterData c)
    {
        squad.Remove(c);
        tomb.Add(c);
    }

    public void InitAllUIsinScene()
    {
        if(SquadWindowParent == null || CharacterPoolParent == null)
        {
            Transform CommonParent = GameObject.Find("Troop").transform;
            SquadWindowParent = CommonParent.GetChild(1).GetChild(1).gameObject;
            StatWindow = CommonParent.GetChild(1).GetChild(2).GetChild(0);
            CharacterPoolParent = CommonParent.GetChild(4).GetChild(0).GetChild(0).gameObject;
        }
        InitSquadWindow();
    }

    public void InitSquadWindow()
    {
        foreach(CharacterData c in characterPool)
        {
            GameObject newGO = Instantiate(buttonPrefab, CharacterPoolParent.transform);
            newGO.transform.GetChild(0).GetComponent<Image>().sprite = portSprites[(int)c.characterName];
            newGO.GetComponent<Troop>().targetCharacter = c;
            // newGO.GetComponent<Button>().onClick.AddListener(
            //     () => {
            //         if(AddSquad(c))
            //             Destroy(newGO);
            //         });
        }

        foreach(CharacterData c in squad)
        {
            GameObject newGO = Instantiate(buttonPrefab, SquadWindowParent.transform);
            newGO.transform.GetChild(0).GetComponent<Image>().sprite = portSprites[(int)c.characterName];
            newGO.GetComponent<Troop>().setIsSquad(true);
            newGO.GetComponent<Troop>().targetCharacter = c;
            newGO.GetComponent<Button>().onClick.AddListener(
                () => {
                    // newGO.GetComponent<Troop>().OnPointerClick();
                    // ReturnPool(c, transform.GetSiblingIndex());
                    // Destroy(newGO.transform.parent.gameObject.transform.GetChild(newGO.transform.GetSiblingIndex()+1).gameObject);
                    // Destroy(newGO);
                    });
            Instantiate(emptyPrefab, SquadWindowParent.transform);

            GameObject newStat = Instantiate(statPrefab, StatWindow.transform);
            for (int j = 0; j < 7; j++)
            {
                if (j==0){
                    newStat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{(totalInitialCharStat[(int)c.characterName][Define.CharacterStat.이동속도]+c.quest_stat[Define.CharacterStat.이동속도]):0.##}";
                }
                else{
                    newStat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{(totalInitialCharStat[(int)c.characterName][(Define.CharacterStat)stats[j]]+ c.quest_stat[(Define.CharacterStat)stats[j]]):0.##}";
                }
            }            
        }
    }

    public void InitTombWindow()
    {
        if (morWindow == null)
            morWindow = GameObject.Find("Tomb").transform.GetChild(2).GetChild(0).GetChild(0);

        foreach(CharacterData c in tomb)
        {
            GameObject newGO = Instantiate(morPrefab, morWindow);
            newGO.transform.GetChild(0).GetComponent<Image>().sprite = portSprites[(int)c.characterName];
            Debug.Log(c.characterName);
            newGO.GetComponent<Tomb>().Init(c);
        }
    }

    public void SaveStatus()
    {
        SaveSystem.Save<List<CharacterData>>(squad, Application.dataPath + "/squad.bin");
        SaveSystem.Save<List<CharacterData>>(characterPool, Application.dataPath + "/characterPool.bin");
        SaveSystem.Save<List<CharacterData>>(tomb, Application.dataPath + "/tomb.bin");
    }

    public void AddRandomCharacterToPool()
    {
        int totalCharacterAvailable = (int)Define.CharacterName.end;
        int randomIndex = UnityEngine.Random.Range(0, totalCharacterAvailable);
        GameObject go = Instantiate(buttonPrefab, CharacterPoolParent.transform);
        CharacterData tmp = new CharacterData(randomIndex);
        for (int i = 0; i < (int)Define.CharacterStat.end; i++)
        {
            tmp.quest_stat.Add((Define.CharacterStat)i, 0);
        }
        go.GetComponent<Troop>().Init(tmp);
        characterPool.Add(tmp);
        SaveStatus();
    }

    public void Reroll_character()
    {
        //현재 보유중인 캐릭에따라서 리롤 달라짐
        //3명은 전체 리롤, 그외는 빈자리에 새로운 캐릭 추가
        switch (squad.Count)
        {
            case 0:
                reroll_newone(3 - squad.Count);
                break;
            case 1:
                reroll_support(squad.Count);
                reroll_newone(3 - squad.Count);
                break;
            case 2:
                reroll_support(squad.Count);
                reroll_newone(3 - squad.Count);
                break;
            case 3:
                reroll_support(squad.Count);
                break;
        }
        SaveStatus();
    }

    void reroll_support(int a)
    {
        int totalCharacterAvailable = (int)Define.CharacterName.end;
        int randomIndex;
        for (int i = 0; i < a; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, totalCharacterAvailable);
            squad[i].characterName = (Define.CharacterName)randomIndex;
            SquadWindowParent.transform.GetChild(2 * i).GetChild(0).GetComponent<Image>().sprite = portSprites[randomIndex];
            GameObject newStat = StatWindow.transform.GetChild(i + 1).gameObject;
            for (int j = 0; j < 7; j++)
            {
                if (j == 0)
                {
                    newStat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{(totalInitialCharStat[randomIndex][Define.CharacterStat.이동속도] + squad[i].quest_stat[Define.CharacterStat.이동속도]):0.##}";
                }
                else
                {
                    newStat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{(totalInitialCharStat[randomIndex][(Define.CharacterStat)stats[j]] + squad[i].quest_stat[(Define.CharacterStat)stats[j]]):0.##}";
                }
            }
        }
    }

    void reroll_newone(int a)
    {
        for(int i =0;i<a;i++)
        {
            int totalCharacterAvailable = (int)Define.CharacterName.end;
            int randomIndex = UnityEngine.Random.Range(0, totalCharacterAvailable);
            GameObject newGO = Instantiate(buttonPrefab, SquadWindowParent.transform);
            CharacterData tmp = new CharacterData(randomIndex);
            for (int j = 0; j < (int)Define.CharacterStat.end; j++)
            {
                tmp.quest_stat.Add((Define.CharacterStat)j, 0);
            }
            newGO.GetComponent<Troop>().Init(tmp);
            newGO.GetComponent<Troop>().setIsSquad(true);
            squad.Add(tmp);
            Instantiate(emptyPrefab, SquadWindowParent.transform);

            GameObject newStat = Instantiate(statPrefab, StatWindow.transform);
            for (int j = 0; j < 7; j++)
            {
                if (j == 0)
                {
                    newStat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{totalInitialCharStat[randomIndex][Define.CharacterStat.이동속도]:0.##}";
                }
                else
                {
                    newStat.transform.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{totalInitialCharStat[randomIndex][(Define.CharacterStat)stats[j]]:0.##}";
                }
            }
        }
        
    }
}

[Serializable]
public class CharacterData
{
    public Define.CharacterName characterName = Define.CharacterName.swordman;
    public List<Define.QuestRewardStat> questRewardStats = new List<Define.QuestRewardStat>();
    public Dictionary<Define.CharacterStat, float> quest_stat = new Dictionary<Define.CharacterStat, float>();
    public int additional_gold;
    public int quest_pos = 0;
    public string name = "John Doe";

    public CharacterData(int iIndex)
    {
        characterName = (Define.CharacterName)iIndex;
    }
}

public class SaveSystem
{
    public static void Save<T>(T _data, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream;
        if (File.Exists(filePath))
            stream = new FileStream(filePath, FileMode.Open);
        else
            stream = new FileStream(filePath, FileMode.Create);

        formatter.Serialize(stream, _data);
        stream.Close();
    }

    public static T Load<T>(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            T data = (T)formatter.Deserialize(stream);

            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found in" + filePath);
            return default(T);
        }
    }
}