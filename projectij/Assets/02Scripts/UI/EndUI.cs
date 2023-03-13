using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
    // 캐릭터
    private Player player;
    private List<follower> followers = new List<follower>();
    private follower follower;
    public GameObject UI_characterObj;
    private List<Image> UI_characters;
    private Image character;
    private bool isWounded;

    // 무기
    public GameObject UI_finalweaponObj;
    public GameObject currentweaponInfoObj;
    private Weapon[] finalweaponInfo;
    private List<Image> UI_final_weapons = new List<Image>();
    //private List<Sprite[]> weaponIcons = new List<Sprite[]>();
    private List<Sprite> weaponIcons = new List<Sprite>();

    //돈
    public GameObject UI_Gold;
    List<Dictionary<string, object>> gold_CSV;

    void OnEnable()
    {
        //난이도에 따른 보상골드 지급용
        gold_CSV = CSVReader.Read("CSV/quest_reward_gold");
        // character 정보 받아서, 살아있는 캐릭터는 경험치와 함께 sprie 띄우고, 죽은 캐릭터는 sprite에 어두운 색 섞어서 죽었다는 걸 표시
        player = StageManager.Instance.player;
        followers = StageManager.Instance.followers;
        for(int i=0;i<3;i++)
        {
            //UI_characters.Add(UI_characterObj.transform.GetChild(i).gameObject.GetComponent<Image>());
            character = UI_characterObj.transform.GetChild(i).gameObject.GetComponent<Image>();
            if (i==0)
            {
                character.sprite = player.spriteRenderer.sprite;
                isWounded = player.GetIsWounded();
                if (isWounded) 
                    character.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            } 
            else
            {
                follower = followers[i-1];
                character.sprite = follower.spriteRenderer.sprite;
                isWounded = follower.GetIsWounded();
                if (isWounded) 
                    character.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }        

        // 얻은 돈은 나중에처리,,
        // 무기 정보 받아서 보유하고 있는 무기 나열        
        for(int i=0;i<3;i++)
        {
            for(int j=0;j<2;j++)
            {
                Sprite cur_sprite = currentweaponInfoObj.transform.GetChild(i).GetChild(j).GetChild(1).GetComponent<Image>().sprite;
                if (cur_sprite.name != "blank")
                    weaponIcons.Add(cur_sprite);
            }
        }
        for(int i=0;i<weaponIcons.Count;i++)
        {
            Image cur_img = UI_finalweaponObj.transform.GetChild(i).gameObject.GetComponent<Image>();
            cur_img.sprite = weaponIcons[i];
            cur_img.color = new Color(1f, 1f, 1f, 1f);
        }

        //생존 퀘스트 성공여부
        if((StageManager.Instance.all_quest & 2) == 2)
        {
            if (StageManager.Instance.GetTimer().GetPlayTime() >= StageManager.Instance.time_for_succes && !StageManager.Instance.anyone_wounded)
                StageManager.Instance.quest_clear += 2;
        }

        //임시 돈처리(난이도에 따라 지급)
        int difficulty_gold = int.Parse(gold_CSV[StageManager.Instance.difficulty - 1]["보상 금액"].ToString());
        UI_Gold.GetComponent<TextMeshProUGUI>().text = "획득한 돈 :" + difficulty_gold;

        //퀘스트 성공여부(받은 퀘스트와 클리어한 퀘스트가 같은지)
        if (StageManager.Instance.all_quest == StageManager.Instance.quest_clear)
        {
            Debug.Log(StageManager.Instance.all_quest + "     " + StageManager.Instance.quest_clear);
            Debug.Log("퀘스트 성공");
            //퀘스트 성공
            if (GameManager.gameManager_Instance != null)//퀘스트 보상 지급
            {
                //퀘스트 보상및 골드 획득
                for (int i = 0; i < 3; i++)
                    GameManager.gameManager_Instance.squad[i].questRewardStats.Add(StageManager.Instance.quest_reward);//퀘스트 보상 스탯 지급
                GameManager.gameManager_Instance.gold += StageManager.Instance.difficulty * 100;//퀘스트 보상 골드 지급
            }

        }
        else
        {
            //퀘스트 실패
            Debug.Log(StageManager.Instance.all_quest + "     " + StageManager.Instance.quest_clear);
            Debug.Log("퀘스트 실패");
        }
    }


    public void ToStartScene()
    {
        StageManager.Instance.setPause(1);

        // 사망처리
        for(int i = 2; i >= 0; i--)
        {
            if (GameManager.gameManager_Instance.squad[i].characterName == Define.CharacterName.stonesling)
            {
                GameManager.gameManager_Instance.squad.RemoveAt(i);
                continue;
            }

            if (i == 0)
            {
                if (player.GetIsWounded())
                {
                    GameManager.gameManager_Instance.SendToTomb(GameManager.gameManager_Instance.squad[0]);
                }
            }
            else
            {
                if (followers[i - 1].GetIsWounded())
                {
                    GameManager.gameManager_Instance.SendToTomb(GameManager.gameManager_Instance.squad[i]);
                }
            }
        }

        GameManager.gameManager_Instance.SaveStatus();

        // TODO : 돈처리

        SceneManager.LoadScene("StartScene");
    }
}
