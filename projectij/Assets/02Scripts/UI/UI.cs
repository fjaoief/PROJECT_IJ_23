using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;


public class UI : MonoBehaviour
{
    public GameObject detail;
    public GameObject Quest_UI;
    public GameObject Setting_UI;
    public GameObject Troop_UI;
    public GameObject Tomb_UI;
    public Animator scene_transition;

    [SerializeField]
    public List<GameObject> main_btns = new List<GameObject>();
    public GameObject first_btn;

    private void Start() {
        EventSystem.current.SetSelectedGameObject(main_btns[0]);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Quest_UI.activeInHierarchy)
            {
                Quest_UI.SetActive(false);
            }
            if (Setting_UI.activeInHierarchy)
            {
                Setting_UI.SetActive(false);
            }
            if (Troop_UI.activeInHierarchy)
            {
                clickExitTroop();
                EventSystem.current.SetSelectedGameObject(main_btns[1]);
            }
            if (Tomb_UI.activeInHierarchy)
            {
                clickExitTomb();
                EventSystem.current.SetSelectedGameObject(main_btns[2]);
                
            }
        }
    }

    // 메인 메뉴에 존재하는 버튼
    public void clickStartBtn(){
        Quest_UI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(Quest_UI.transform.GetChild(2).gameObject);
    }

    public void clickSettingBtn(){
        Setting_UI.SetActive(true);
    }

    public void clickTombBtn(){
        Tomb_UI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        foreach (GameObject btn in main_btns)
        {
            btn.GetComponent<Button>().interactable = false;
        }
        GameManager.gameManager_Instance.InitTombWindow();
    }

    public void clickQuestBtn()
    {
        GameManager.gameManager_Instance.quest_selected = false;
        GameObject QM = Quest_UI.transform.gameObject;
        if (QM.GetComponent<QuestManager>().selected_num !=0)
        {
            // 빈 스쿼드자리 민병대로 채우기
            {
                for (int iIndex = GameManager.gameManager_Instance.squad.Count; iIndex < 3; iIndex++)
                {
                    GameManager.gameManager_Instance.squad.Add(new CharacterData(6));
                }
            }

            GameObject click = EventSystem.current.currentSelectedGameObject;
            //유지비 차감및 적용시각화
            GameManager.gameManager_Instance.gold -= GameManager.gameManager_Instance.squad.Count * 100;//스쿼드 인원 *100만큼 돈 차감
            GameManager.gameManager_Instance.gold_text.text = "G : " + GameManager.gameManager_Instance.gold;
            //StageManager로 정보 넘김
            int i = click.transform.parent.GetSiblingIndex();
            GameManager.gameManager_Instance.difficulty = GameManager.gameManager_Instance.quest_list[i - 1].a;
            GameManager.gameManager_Instance.quest_reward = GameManager.gameManager_Instance.quest_list[i - 1].b;
            GameManager.gameManager_Instance.quest_type = GameManager.gameManager_Instance.quest_list[i - 1].c;
            for (int j = 0; j < 3; j++)
            {
                GameManager.gameManager_Instance.flag_num[j] = GameManager.gameManager_Instance.quest_list[i - 1].d[j];
            }
            StartCoroutine("LoadAsyncOperation", 1);
        }
    }

    public void clickTroopBtn(){
        Troop_UI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        GameManager.gameManager_Instance.InitAllUIsinScene();
    }

    public void clickTutorialBtn(){
        SceneManager.LoadScene("TutorialScene");
    }

    public void clickExitBtn(){
        GameManager.gameManager_Instance.SaveStatus();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // 어플리케이션 종료
        #endif
    }
    
    public void clickExitTroop(){
        Transform t = Troop_UI.transform.GetChild(1).GetChild(1);
        for(int i = 0; i < t.childCount; i++)
        {
            Destroy(t.GetChild(i).gameObject);
        }
        t = Troop_UI.transform.GetChild(1).GetChild(2).GetChild(0);
        for (int i = 1; i < t.childCount; i++)
        {
            Destroy(t.GetChild(i).gameObject);
        }
        t = Troop_UI.transform.GetChild(4).GetChild(0).GetChild(0);
        for (int i = 0; i < t.childCount; i++)
        {
            Destroy(t.GetChild(i).gameObject);
        }
        Troop_UI.SetActive(false);
    }

    public void clickExitTomb(){
        Transform t = Tomb_UI.transform.GetChild(2).GetChild(0).GetChild(0);
        for(int i = 0; i < t.childCount; i++)
        {
            Destroy(t.GetChild(i).gameObject);
        }
        foreach (GameObject btn in main_btns)
        {
            btn.GetComponent<Button>().interactable = true;
        }
        Tomb_UI.SetActive(false);
    }

    IEnumerator LoadAsyncOperation(int SceneNumber)
    {
        scene_transition.SetTrigger("load_scene");
        yield return new WaitForSeconds(1f);
        //AsyncOperation gameLevel = SceneManager.LoadSceneAsync(SceneNumber);
        SceneManager.LoadScene(SceneNumber);
        /*gameLevel.allowSceneActivation = false; // stop the level from activating

        while (gameLevel.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        gameLevel.allowSceneActivation = true; // this will enter the level now
        gameLevel.allowSceneActivation = true; // this will enter the level now*/
    }


}
