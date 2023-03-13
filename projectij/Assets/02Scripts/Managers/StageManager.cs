using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEditor;
using System.Text;

public class StageManager : MonoBehaviour
{
    private Timer timer = null;
    public Timer GetTimer()
    {
        return timer;
    }
    private InputManager inputManager = null;
    public InputManager GetInputManager()
    {
        return inputManager;
    }
    private EnemyManager enemyManager = null;
    public EnemyManager GetEnemyManager()
    {
        return enemyManager;
    }

    private WeaponManager weaponManager = null;
    public WeaponManager GetWeaponManager()
    {
        return weaponManager;
    }
    public AudioSource lvlsounds;

    public Player player = null;
    public List<follower> followers = new List<follower>();
    public Transform _Camera;
    public static StageManager Instance;
    public Text playtimeBoard;

    public GameObject Canvas;

    [SerializeField]
    Tilemap tilemap; // 차후에는 퀘스트에 따라 tilemap을 뽑아 넣는 과정 필요.
    float yMin, yMax; // camera y축 한계들
    public GameObject boundingbox; // player y축 한계 지정할 때 필요한 경계

    [SerializeField] CinemachineVirtualCamera vcam;
    public float min = 5.4f;
    public float max = 10.8f;
    public float initialMin;
    public float initialMax;

    public Slider[] HPSlider = new Slider[3];
    private float Team_EXP = 0;
    private int Team_Level = 1;
    private float EXP_require = 2;
    [SerializeField]
    private Slider EXP_Slider;
    [SerializeField]
    public GameObject LevelUpWindow;
    [SerializeField]
    public RectTransform weaponIconsTrans;
    [SerializeField]
    public Sprite[] blank;
    [SerializeField]
    public Sprite[] weaponIcons_1;
    [SerializeField]
    public Sprite[] weaponIcons_2;
    [SerializeField]
    public Sprite[] weaponIcons_3;
    [SerializeField]
    public Sprite[] weaponIcons_4;

    public Transform synergySlotsTrans;
    [SerializeField]
    public RectTransform masteryUITrans;
    [SerializeField]
    public AudioClip lvlup;
    [SerializeField]
    public AudioClip enemyspawn;
    public List<Sprite[]> weaponIcons = new List<Sprite[]>();
    public List<Animator> CharacterAnimators = new List<Animator>();

    public Transform weaponNoticeUITrans;

    //적관련
    public IEnumerator coroutine;//적생성 코루틴
    public int MAXenemy = 50;//적 최대인구수
    public float population_add_check;
    float[] AllenmeyNum = new float[4];//종류별 적  인구수
    public int[] flag_num = new int[3];//생성하는 적 부대마크 식별번호(한 게임당 세종류)
    public int cur_flag; //0~2범위로 부대 3개 순서표현
    public int flag_change_timer = 350;//몇초마다 부대 바뀔지
    public float flag_change_checker = 0;//부대 바뀔시간 체크용
    public bool flag_timer_half;
    public int kill = 0;//킬수
    public Text killboard;//킬수 표시될 UI
    public GameObject RandObject;//일정 킬당 생성되는 랜덤 오브젝트
    public int RO_check;//랜덤오브젝트 생성 체크용(킬수기반)


    private bool Loaded = false;

    // 게임 종료용 포탈
    public GameObject portal;
    public GameObject portal_cam;
    private Vector3 portal_pos;
    public GameObject end_ui;
    public GameObject indicatorObj;
    public Indicator indicator;

    public List<PlayerCharacters> playerCharacters = new List<PlayerCharacters>();
    public bool[] isWounded = new bool[3];
    // 캐릭터별 가장 높은 마스터리와 그 레벨
    public (Define.Mastery, int) [] highestMasteries = new (Define.Mastery, int)[3];
    // 마스터리별 가장 높은 레벨 (캐릭터 전체)
    public float[] masteryLevel = { 0, 0, 0, 0 };

    // charIndex, iconIndex, weaponInfo
    public (int, int, WeaponInfo) firstClickedInfo = (-2, -2, null);
    public (int, int, WeaponInfo) secondClickedInfo = (-2, -2, null);

    // 무기 합성용
    public GameObject recombinationIconObj;
    private int SquadCount = 0;
    public float endgametime = 180;

    // 스탯 UI
    public RectTransform totalStatPanel;
    private bool popUp;
    public GameObject optionPanel;
    [SerializeField]
    public Sprite[] statIcons;
    private Transform targetStat;
    private int[] stats = new int[7] {-1, 9, 2, 3, 4, 7, 8};
    public GameObject statInfoWindow;
    public GameObject SynergyInfoWindow;

    // 세팅창 UI
    public GameObject settingWindow;

    // 무기 설명창
    public GameObject weaponInfoWindow;
    List<Dictionary<string, object>> explanationCSV;
    public Dictionary<Define.Weapon, string> explanation = new Dictionary<Define.Weapon, string>();

    // 연출
    public GameObject redScreen;

    // 검정 무기 능력을 위한 경험치 관리
    public List<ExpBall> allExpBalls = new List<ExpBall>();

    // 무기 인벤토리 UI
    private bool inventoryPopUp;
    public Transform cursorTrans;
    InventoryCursor cursor;

    private bool swapModeOn = false;
    private bool recombinationModeOn = false;

    // addWeaponIcon의 정보
    public (int, int) addedIconIndex;

    //GameManager로부터 전달받는/하는 변수
    public int difficulty;
    public Define.QuestRewardStat quest_reward;
    public string quest_type;
    public int gold;

    //퀘스트 관련
    public int all_quest;

    public GameObject occupied_territory;
    public GameObject territory_indicatorObj;
    public Indicator terrotory_indicator;
    public float occupied_percentage;
    public float percentage_needed = 100;
    public int player_num;

    public float time_for_succes = 100;
    public bool anyone_wounded;

    public int supply_needed = 20;
    public int supply_get;

    public int quest_clear;//퀘스트 성공여부

    
    public void SetWeaponNotice(Define.Weapon weaponName, int rarity, Dictionary<Define.CharacterStat, float> option)
    {
        
        StringBuilder sb = new StringBuilder();
        sb.Append($"{weaponName}\n");
        if (option != null)
        {
            sb.Append("옵션: ");
            foreach (KeyValuePair<Define.CharacterStat, float> opt in option)
            {
                sb.Append($"{opt.Key} +{opt.Value}\n");
            }
        }
        sb.Length--;

        if (weaponNoticeCoroutine != null)
        {
            weaponNoticeUITrans.gameObject.SetActive(false);
            StopCoroutine(weaponNoticeCoroutine);
        }
        StartCoroutine(WeaponNoticeOn(weaponName, rarity, sb));
    }

    Coroutine weaponNoticeCoroutine = null;
    public IEnumerator WeaponNoticeOn(Define.Weapon weaponName, int rarity, StringBuilder sb)
    {
        weaponNoticeUITrans.gameObject.SetActive(true);
        weaponNoticeUITrans.GetChild(0).GetChild(0).GetComponent<Image>().sprite = weaponIcons[rarity][(int)weaponName];
        weaponNoticeUITrans.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = sb.ToString();
        yield return new WaitForSeconds(5f);
        weaponNoticeUITrans.gameObject.SetActive(false);
    }

    public void UpdateStatText()
    {


        for (int i = 0; i < 3; i++)
        {
            targetStat = optionPanel.transform.GetChild(i+1);
            for (int j = 0; j < 7; j++)
            {
                //Debug.Log((Define.CharacterStat)stats[j]);
                if (j==0){
                    targetStat.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{playerCharacters[i].characterStat[Define.CharacterStat.이동속도]:0.##}";
                }
                else{
                    targetStat.GetChild(j).GetComponent<TextMeshProUGUI>().text = $"{playerCharacters[i].characterStat[(Define.CharacterStat)stats[j]]:0.##}";
                }
            }

            // StringBuilder sb = new StringBuilder();
            // for (int j = 0; j < (int)Define.CharacterStat.end; j++)
            // {

            //     sb.Append($"{(Define.CharacterStat)j} : {playerCharacters[i].characterStat[(Define.CharacterStat)j]}\n");
            // }
            // optionPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = sb.ToString();


        }
    }

    private float GetTeamEXP()
    {
        return Team_EXP;
    }

    Coroutine expCoroutine = null;
    public void GainTeamEXP(float exp)
    {
        if (expCoroutine != null)
            StopCoroutine(expCoroutine);
        expCoroutine = StartCoroutine(GainTeamEXPCoroutine(exp));
    }

    IEnumerator GainTeamEXPCoroutine(float exp)
    {
        Team_EXP += exp;
        //Debug.Log($"Team_Exp: {Team_EXP}");

        while (Team_EXP >= EXP_require)
        {
            //lvlsounds.PlayOneShot(lvlup);
            SoundManager.Instance.Play("lvlup");
            EXP_Slider.value = (float)Team_EXP / EXP_require;

            Team_EXP -= EXP_require;
            Team_Level++;
            player.SetClashDmg(Team_Level+5);
            
            EXP_require = Team_Level * 3 + 15;
            // Level Up Window 켜기
            LevelUpWindow.SetActive(true);
            EventSystem.current.SetSelectedGameObject(LevelUpWindow.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
            setPause(0);

            weaponManager.CheckPersonalWeaponSynergy();
            yield return new WaitForSeconds(0.001f);
        }
        // Slider value 변경
        EXP_Slider.value = (float)Team_EXP / EXP_require;
    }

    private void Awake()
    {
        // 외부 시작Scene 에서 Squad 정보 받아옴
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            if (GameManager.gameManager_Instance == null)
            {
                SquadCount = 3;
                followers.Add(GameObject.Find("Follower1").GetComponent<follower>());
                followers.Add(GameObject.Find("Follower2").GetComponent<follower>());
                followers[0].gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"RuntimeSprites/bow");
                followers[1].gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"RuntimeSprites/mace");
            }
            else//스쿼드 정보에 따라서 적 이미지 변경
            {
                SquadCount = GameManager.gameManager_Instance.squad.Count;
                Define.CharacterName PlayerTypeName = GameManager.gameManager_Instance.squad[0].characterName;
                // Sprite playerSprite = Resources.Load<Sprite>($"RuntimeSprites/{PlayerTypeName.ToString()}");
                Sprite playerSprite = GameManager.gameManager_Instance.buttonSprites[(int)PlayerTypeName];
                GameObject.Find("Player").transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = playerSprite;
                for (int iIndex = 1; iIndex < SquadCount; iIndex++)
                {
                    Define.CharacterName FollowerType = GameManager.gameManager_Instance.squad[iIndex].characterName;
                    GameObject FollowerGO = GameObject.Find($"Follower{iIndex}");
                    // 굳이 sprite까지 바꿔야하나?
                    // 아직 애니메이션 완성안된애가 있네
                    Sprite targetSprite = GameManager.gameManager_Instance.buttonSprites[(int)FollowerType];
                    FollowerGO.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = targetSprite;
                    //EditorApplication.isPaused = true; //빌드 시 이 부분 주석 처리 필요합니다
                    if (FollowerGO.transform.GetChild(1).GetComponent<Animator>() != null)
                        FollowerGO.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController = null;
                    followers.Add(FollowerGO.GetComponent<follower>());
                }
                //GameManager로부터 정보전달받음(난이도, 보상)
                difficulty = GameManager.gameManager_Instance.difficulty;//난이도
                quest_reward = GameManager.gameManager_Instance.quest_reward;//보상
                quest_type = GameManager.gameManager_Instance.quest_type;
            }
        }

        // Manager 등 초기화
        {
            Instance = this;
            gameObject.AddComponent<Timer>();
            timer = gameObject.GetComponent<Timer>();

            gameObject.AddComponent<InputManager>();
            inputManager = gameObject.GetComponent<InputManager>();

            gameObject.AddComponent<EnemyManager>();
            enemyManager = gameObject.GetComponent<EnemyManager>();

            _Camera = Camera.main.transform;


            gameObject.AddComponent<WeaponManager>();
            weaponManager = gameObject.GetComponent<WeaponManager>();
        }

        gameObject.AddComponent<AudioSource>();
        lvlsounds = gameObject.GetComponent<AudioSource>();

        // TileMap 및 BoundingBox 초기화
        {
            //tilemap
            // 타일 좌표가 가장 낮은것과 가장 높은것의 Vector3 값을 찾는다.
            tilemap = GameObject.FindGameObjectWithTag("Background").transform.Find("Tilemap").gameObject.GetComponent<Tilemap>();
            Vector3 minTile = tilemap.CellToWorld(tilemap.cellBounds.min);
            Vector3 maxTile = tilemap.CellToWorld(tilemap.cellBounds.max);

            yMin = minTile.y;
            yMax = maxTile.y;
            boundingbox.GetComponent<BoxCollider2D>().size = new Vector3(50, yMax - yMin, 0);
            player.GetComponent<Player>().SetLimits(minTile, maxTile);
        }

        weaponIcons.Add(blank);
        weaponIcons.Add(weaponIcons_1);
        weaponIcons.Add(weaponIcons_2);
        weaponIcons.Add(weaponIcons_3);
        weaponIcons.Add(weaponIcons_4);

        playerCharacters[0] = player.GetComponent<Player>();
        playerCharacters[1] = followers[0];
        playerCharacters[2] = followers[1];

        //이번판에등장할 부대 세개 고르기
        List<int> list = new List<int>(){ 1,2,3,4,5,6,7,8,9};
        for (int i = 0; i < 3;i++)
        {
            flag_num[i] = GameManager.gameManager_Instance.flag_num[i];
            /*int rand = Random.Range(0, list.Count);
            flag_num[i] = list[rand];
            list.RemoveAt(rand);*/
        }

        // 무기 설명 정보 받아오기 (InventorySlot에서 사용)
        explanationCSV = CSVReader.Read("CSV/AllWeaponExplanation");
        for (int i = 0; i < (int)Define.Weapon.upgradedWeaponEnd; i++)
        {
            explanation.Add((Define.Weapon)i, explanationCSV[i]["무기 설명"].ToString());
        }

        initialMin = min;
        initialMax = max;

        cursor = cursorTrans.GetComponent<InventoryCursor>();
    }

    private void Start()
    {
        Loaded = true;
        vcam.m_Lens.OrthographicSize = min;
        Spawn_setting(60, 1, 0.1f, 0);
        coroutine = enemyManager.SpawnEnemy(Define.Enemy.Sword_infantry,AllenmeyNum, true);
        StartCoroutine(coroutine);

        // 나중에 다른 조건으로 무기 추가
        if (GameManager.gameManager_Instance == null)
        {
            weaponManager.AddWeapon(Define.Weapon.환도, 0, 1, null, true, false);
            weaponManager.AddWeapon(Define.Weapon.환도, 1, 1, null, true, false);
        }
        else
        {
            for(int i =0;i< GameManager.gameManager_Instance.squad.Count;i++)
            {
                //직업에따른 시작무기
                switch (GameManager.gameManager_Instance.squad[i].characterName)
                {
                    case Define.CharacterName.swordman:
                        weaponManager.AddWeapon(Define.Weapon.환도, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.seung:
                        weaponManager.AddWeapon(Define.Weapon.승자총통, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.flag:
                        weaponManager.AddWeapon(Define.Weapon.깃발, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.hwacha:
                        weaponManager.AddWeapon(Define.Weapon.화차, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.spearman:
                        weaponManager.AddWeapon(Define.Weapon.창, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.shaman:
                        weaponManager.AddWeapon(Define.Weapon.신장대, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.stonesling:
                        weaponManager.AddWeapon(Define.Weapon.돌팔매, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.monk:
                        weaponManager.AddWeapon(Define.Weapon.월도, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.bow:
                        weaponManager.AddWeapon(Define.Weapon.각궁, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.shieldman:
                        weaponManager.AddWeapon(Define.Weapon.방패, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.mace:
                        weaponManager.AddWeapon(Define.Weapon.철퇴, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.cheonja:
                        weaponManager.AddWeapon(Define.Weapon.천자총통, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.flail:
                        weaponManager.AddWeapon(Define.Weapon.편곤, i, 1, null, true, true);
                        break;
                    case Define.CharacterName.samurai:
                        weaponManager.AddWeapon(Define.Weapon.왜검, i, 1, null, true, true);
                        break;

                }
                //퀘스트 보상에 따른 스텟 추가
                for(int j =0;j< GameManager.gameManager_Instance.squad[j].questRewardStats.Count;j++)
                {

                }
            }
            //퀘스트 타입에 따른 세팅
            if(quest_type.Contains("점령"))
            {
                all_quest += 1;
                Vector3 vec = player.transform.position +Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(11f, 15f));
                occupied_territory.SetActive(true);
                occupied_territory.transform.position = vec;
            }
            if (quest_type.Contains("생존"))
            {
                all_quest += 2;
            }

            if (quest_type.Contains("보급"))
            {
                all_quest += 4;
            }

        }
    }

    public void survive()
    {

    }

    public (float, float) inputDir { get; set; }
    public void setPause(int timeScale)
    {
        Time.timeScale = timeScale;
    }
    private void Update()
    {
        // 게임에서 Update를 사용한 루프는 최대한 줄이는 방향으로 할게요
        // Unity 내에서 Update를 여러 곳에서 사용하면 Update간의 순서 등의 문제가 발생할 수 있어서 이렇게 할게요

        // 사망, 일시 정지등 루프가 정지해야하는 조건 추가 예정
        // 종료 포탈에 entered 했을 때
        if (player.end == true)
        {
            player.end = false;
            indicatorObj.SetActive(false);
            setPause(0);
            end_ui.SetActive(true);

        }

        if (Loaded)
        {
            timer._Update();
            inputManager._Update();
            inputDir = inputManager.Get_Input_Direction();
            player.SetDirection(inputDir);

            if (SquadCount >= 2)
            {
                followers[0].SetDirection(inputDir);
                followers[0].Set_Follower1_Pos(inputDir, player.transform.position);
            }
            if (SquadCount == 3)
            {
                followers[1].SetDirection(inputDir);
                followers[1].Set_Follwer2_Pos(inputDir, player.transform.position);
            }

            // 카메라 줌인줌아웃
            if (Time.timeScale != 0)
            {
                if ((inputDir.Item1 != 0) || (inputDir.Item2 != 0))
                {
                    vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, min, 0.25f * Time.deltaTime);
                }
                else if ((inputDir.Item1 == 0) && (inputDir.Item2 == 0))
                {

                    vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, max, 0.35f * Time.deltaTime);
                }
            }

            // HP 표시
            for(int i=0; i<3; i++)
            {
                HPSlider[i].value = playerCharacters[i].HP / playerCharacters[i].characterStat[Define.CharacterStat.최대체력];
            }

            enemyManager._Update();
            TimeChecking();
            killboard.GetComponent<Text>().text = kill.ToString();
            playtimeBoard.GetComponent<Text>().text = ((int)(timer.GetPlayTime()/60)).ToString() +" : " + (int)(timer.GetPlayTime()%60) + "//" + occupied_percentage;
            ObjectSpawn();

        }
        //최대인구수 초과시 생성시 스폰 정지
        if (enemyManager.getenemyNum() > MAXenemy)
        {
            StopCoroutine(coroutine);
            enemyManager.setspawn_working(false);
        }
        //현재인구가 최대인구수 20%이하면 스폰 재시작
        if (enemyManager.getenemyNum() <= (int)(MAXenemy * 0.33f) && !enemyManager.getspawn_working())
        {
            coroutine = enemyManager.SpawnEnemy(Define.Enemy.Sword_infantry,AllenmeyNum, true);
            //lvlsounds.PlayOneShot(enemyspawn);
            SoundManager.Instance.Play("enemyspawn");
            StartCoroutine(coroutine);
        }

        // 포탈이 생겼을 때 인디케이터 움직임
        if (portal.activeInHierarchy)
        {
            if (indicatorObj.activeInHierarchy && IsPortalOffScreen(portal))
            {
                indicator.DrawIndicator(portal, indicatorObj);
            }
            else if (IsPortalOffScreen(portal) == false)
            {
                indicatorObj.SetActive(false);
            }
            else if (IsPortalOffScreen(portal))
            {
                indicatorObj.SetActive(true);
            }
        } else{
            indicatorObj.SetActive(false);
        }
        // 점령지 생겼을 때 인디케이터 움직임
        if ((all_quest & 1 )== 1 )
        {
            if (territory_indicatorObj.activeInHierarchy && IsPortalOffScreen(occupied_territory))
                terrotory_indicator.DrawIndicator(occupied_territory, territory_indicatorObj);
            else if (IsPortalOffScreen(occupied_territory) == false)
                territory_indicatorObj.SetActive(false);
            else if (IsPortalOffScreen(occupied_territory))
                territory_indicatorObj.SetActive(true);
        }
        else
            territory_indicatorObj.SetActive(false);

        // 키보드 숫자 1 입력 => 무기 교환 모드
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 2 상태로 1 => 교환모드로 변환
            if (recombinationModeOn)
            {
                recombinationModeOn = false;
                swapModeOn = true;
            }
            else
            {
                if (!swapModeOn)
                {
                    swapModeOn = true;
                    EnterMode();
                }
                else
                {
                    swapModeOn = false;
                    ExitMode();
                }
            }
        }
        // 키보드 숫자 2 입력 => 무기 합성 모드
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 1 상태로 2 => 합성모드로 변환
            if (swapModeOn)
            {
                swapModeOn = false;
                recombinationModeOn = true;
                UndoClick();
            }
            else
            {
                if (!recombinationModeOn)
                {
                    recombinationModeOn = true;
                    EnterMode();
                }
                else
                {
                    recombinationModeOn = false;
                    ExitMode();
                }
            }
        }

        if (swapModeOn)
        {
            SwapMode();
        }

        else if (recombinationModeOn)
        {
            RecombinationMode();
        }

        // 스탯창 팝업
        if (Input.GetKeyDown(KeyCode.R) || Input.GetAxis("LT")==1)
        {
            popUp = true;
        }
        else if (Input.GetKeyUp(KeyCode.R) || Input.GetAxis("LT") == 1)
        {
            popUp = false;
        }
        if (popUp){
            totalStatPanel.pivot = Vector2.Lerp(totalStatPanel.pivot, new Vector2(0, 0.0f), 5 * Time.unscaledDeltaTime);
        }
        else{
            totalStatPanel.pivot = Vector2.Lerp(totalStatPanel.pivot, new Vector2(0, 0.6f), 10 * Time.unscaledDeltaTime);
        }

        // 무기고 팝업
        if (!swapModeOn && !recombinationModeOn)
        {
            if (Input.mousePosition.y < player.transform.position.y - Camera.main.orthographicSize + 250 || Input.GetAxis("RT") == 1)
            {
                inventoryPopUp = true;
            }
            else
            {
                inventoryPopUp = false;
            }
            if (inventoryPopUp)
            {
                weaponIconsTrans.pivot = Vector2.Lerp(weaponIconsTrans.pivot, new Vector2(0.5f, 0.0f), 5 * Time.unscaledDeltaTime);
                // masteryUITrans.pivot = Vector2.Lerp(masteryUITrans.pivot, new Vector2(0.5f, 0.0f), 5 * Time.unscaledDeltaTime);
            }
            else
            {
                weaponIconsTrans.pivot = Vector2.Lerp(weaponIconsTrans.pivot, new Vector2(0.5f, 0.5f), 10 * Time.unscaledDeltaTime);
                // masteryUITrans.pivot = Vector2.Lerp(masteryUITrans.pivot, new Vector2(0.5f, 0.8f), 10 * Time.unscaledDeltaTime);
            }
        }

        // setting창 켜기
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (settingWindow.activeInHierarchy)
                settingWindow.SetActive(false);
            else
                settingWindow.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            player.Fixed_Update();
            enemyManager.Fixed_Update();
            weaponManager.Fixed_Update();
            foreach (follower _follower in followers)
            {
                _follower.Fixed_Update();
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        setPause(0);
        end_ui.SetActive(true);
    }

    void EnterMode()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        weaponIconsTrans.pivot = new Vector2(0.5f, 0.0f);
        // masteryUITrans.pivot = new Vector2(0.5f, 0.0f);
        cursorTrans.gameObject.SetActive(true);
    }
    void ExitMode()
    {
        UndoClick();
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        weaponIconsTrans.pivot = new Vector2(0.5f, 0.5f);
        // masteryUITrans.pivot = new Vector2(0.5f, 0.8f);
        cursorTrans.gameObject.SetActive(false);
    }

    void UndoClick()
    {
        weaponInfoWindow.SetActive(false);
        if (firstClickedInfo.Item1 == -2) return;
        if (firstClickedInfo.Item1 == -1)
            weaponIconsTrans.GetChild(3).GetChild(firstClickedInfo.Item2).GetChild(0).GetComponent<Image>().color = Color.white;
        else
        {
            if (!isWounded[firstClickedInfo.Item1])
                weaponIconsTrans.GetChild(firstClickedInfo.Item1).GetChild(firstClickedInfo.Item2).GetChild(0).GetComponent<Image>().color = Color.white;
            else
                weaponIconsTrans.GetChild(firstClickedInfo.Item1).GetChild(firstClickedInfo.Item2).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
    }

    void RecombinationMode()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cursor.ColumnIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            cursor.ColumnIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursor.RowIndex = 1 - cursor.RowIndex;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            RecombinationSlot recombiSlot1 = recombinationIconObj.transform.GetChild(0).GetComponent<RecombinationSlot>();
            RecombinationSlot recombiSlot2 = recombinationIconObj.transform.GetChild(1).GetComponent<RecombinationSlot>();

            InventorySlot slot = cursor.currentTransform.GetComponent<InventorySlot>();
            if (slot.weaponInfo == null) return;
            if (slot.weaponInfo.isStartWeapon) return;

            // 같은 슬롯 선택 => 취소
            if (recombiSlot1.weaponInfo != null && recombiSlot1.weaponInfo == slot.weaponInfo)
            {
                recombiSlot1.EmptySlot();
            }
            else if (recombiSlot2.weaponInfo != null && recombiSlot2.weaponInfo == slot.weaponInfo)
            {
                recombiSlot2.EmptySlot();
            }

            // 합성무기1 선택
            else if (recombiSlot1.weaponInfo == null)
            {
                recombiSlot1.weaponInfo = slot.weaponInfo;
                recombiSlot1.transform.GetChild(1).GetComponent<Image>().sprite = slot.transform.GetChild(1).GetComponent<Image>().sprite;
            }

            // 합성무기2 선택
            else if (recombiSlot2.weaponInfo == null)
            {
                recombiSlot2.weaponInfo = slot.weaponInfo;
                recombiSlot2.transform.GetChild(1).GetComponent<Image>().sprite = slot.transform.GetChild(1).GetComponent<Image>().sprite;
            }

            if (recombiSlot1.weaponInfo != null && recombiSlot2.weaponInfo != null)
            {
                OnButtonClickRandomWeapon();
            }

        }
    }

    void SwapMode()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cursor.ColumnIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            cursor.ColumnIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursor.RowIndex = 1 - cursor.RowIndex;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            InventorySlot slot = cursor.currentTransform.GetComponent<InventorySlot>();
            {
                // 같은 슬롯 선택 => 취소
                if (firstClickedInfo.Item3 != null && firstClickedInfo.Item3 == slot.weaponInfo)
                {
                    UndoClick();
                    firstClickedInfo = (-2, -2, null);
                }

                // 교환무기1 선택
                else if (firstClickedInfo.Item3 == null)
                {
                    slot.SetClickedInfo(0);
                    Debug.Log("slot1");
                }

                // 교환무기2 선택
                else if (secondClickedInfo.Item3 == null)
                {
                    slot.SetClickedInfo(1);
                    Debug.Log("slot2");
                }

                if (firstClickedInfo.Item1 != -2 && secondClickedInfo.Item1 != -2)
                {
                    SwapSlot();
                }
            }
        }
    }

    bool buttonClicked = false;
    bool isWarning = false;
    public void OnButtonClickRandomWeapon()
    {
        if (recombinationIconObj.transform.GetChild(0).GetComponent<RecombinationSlot>().weaponInfo == null) return;
        if (recombinationIconObj.transform.GetChild(1).GetComponent<RecombinationSlot>().weaponInfo == null) return;

        // currentWeaponInfo에 있는 무기 받아오기 (어떤 이유로 사라졌다면 null)
        WeaponInfo firstRecombinationInfo = weaponManager.CheckIfWeaponExists(recombinationIconObj.transform.GetChild(0).GetComponent<RecombinationSlot>().weaponInfo);
        WeaponInfo secondRecombinationInfo = weaponManager.CheckIfWeaponExists(recombinationIconObj.transform.GetChild(1).GetComponent<RecombinationSlot>().weaponInfo);
        if (firstRecombinationInfo == null)
        {
            Debug.Log("첫 번째 무기 존재하지 않음");
            return;
        }
        if (secondRecombinationInfo == null)
        {
            Debug.Log("두 번째 무기 존재하지 않음");
            return;
        }

        if (buttonClicked == false && firstRecombinationInfo.rarity == secondRecombinationInfo.rarity)
            StartCoroutine(Recombinator(firstRecombinationInfo, secondRecombinationInfo));

        // 희귀도 안 맞을 경우 경고창 띄우기
        else if (firstRecombinationInfo.rarity != secondRecombinationInfo.rarity)
        {
            if (isWarning == false)
                StartCoroutine(WarningMessage());
            return;
        }

        // 버튼 클릭 음
        SoundManager.Instance.Play("mergeWeapon");
    }

    IEnumerator Recombinator(WeaponInfo firstRecombinationInfo, WeaponInfo secondRecombinationInfo)
    {
        buttonClicked = true;
        WeaponInfo newWeaponInfo = weaponManager.GetNewRandomWeapon(firstRecombinationInfo, secondRecombinationInfo);

        // 이펙트
        recombinationIconObj.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        recombinationIconObj.transform.GetChild(3).gameObject.SetActive(false);

        // 재조합
        weaponManager.DoRecombination(newWeaponInfo, firstRecombinationInfo, secondRecombinationInfo);

        // 슬롯 비우기
        for (int i = 0; i < 2; i++)
        {
            recombinationIconObj.transform.GetChild(i).GetComponent<RecombinationSlot>().weaponInfo = null;
            recombinationIconObj.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = weaponIcons[0][0];
        }
        buttonClicked = false;

        SetWeaponNotice(newWeaponInfo.weaponName, newWeaponInfo.rarity, newWeaponInfo.option);
    }

    IEnumerator WarningMessage()
    {
        isWarning = true;
        recombinationIconObj.transform.GetChild(4).gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        recombinationIconObj.transform.GetChild(4).gameObject.SetActive(false);
        isWarning = false;
    }

    public void EmptyRecombinationSlot()
    {
        for (int i = 0; i < 2; i++)
        {
            //if (recombinationIconObj.transform.GetChild(i).GetComponent<RecombinationSlot>().weaponInfo == null) continue;
            //WeaponInfo weaponInfo = weaponManager.CheckIfWeaponExists(recombinationIconObj.transform.GetChild(i).GetComponent<RecombinationSlot>().weaponInfo);
            //if (weaponInfo == null)
            {
                //Debug.Log("비운다~");
                recombinationIconObj.transform.GetChild(i).GetComponent<RecombinationSlot>().weaponInfo = null;
                recombinationIconObj.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = weaponIcons[0][0];
            }
        }
    }

    // 캐릭터 공통으로 적용되는 버프만.
    // 캐릭터의 최고숙련도에 변화가 생길 때 호출
    // 매개변수: 최고숙련도 이름, 최고숙련도 레벨
    // 호출 시의 highestMasteries는 레벨업 전의 정보
    public void ManageMasteryBuff(int charIndex, Define.Mastery masteryName, int level)
    {
        // 숙련도별 버프 관리
        // float[] oldLevel = { 0, 0, 0, 0 };
        // for (int i = 0; i < highestMasteries.Length; i++)
        // {
        //     if (oldLevel[(int)highestMasteries[i].Item1] < highestMasteries[i].Item2)
        //         oldLevel[(int)highestMasteries[i].Item1] = highestMasteries[i].Item2;
        // }

        // 최고숙련도 업데이트
        highestMasteries[charIndex] = (masteryName, level);

        float[] newLevel = { 0, 0, 0, 0 };
        for (int i = 0; i < highestMasteries.Length; i++)
        {
            if (newLevel[(int)highestMasteries[i].Item1] < highestMasteries[i].Item2)
                newLevel[(int)highestMasteries[i].Item1] = highestMasteries[i].Item2;
        }

        // 도검, (둔기), 개인화기, 대포 순
        float[] increaseAmount = new float[4]; // 이전 숙련도 레벨과의 차이 고려
        for (int i = 0; i < 4; i++)
        {
            if (i != 1)  // 둔기 제외
                increaseAmount[i] = (newLevel[i] - masteryLevel[i]) * 0.01f;
            // masteryLevel 갱신
            // Debug.Log($"{(Define.Mastery)i} : {newLevel[i] - masteryLevel[i]}");
            masteryLevel[i] = newLevel[i];

        }

        // 도검
        if (increaseAmount[(int)Define.Mastery.도검] != 0)
        {
            foreach (PlayerCharacters playerCharacters in playerCharacters)
            {
                playerCharacters.PureMoveSpeed += playerCharacters.InitialCharStat[Define.CharacterStat.이동속도] * increaseAmount[(int)Define.Mastery.도검];
            }
        }

        // 개인화기
        if (increaseAmount[(int)Define.Mastery.개인화기] != 0)
        {
            min += initialMin * increaseAmount[(int)Define.Mastery.개인화기];
        }

        // 대포
        if (increaseAmount[(int)Define.Mastery.대포] != 0)
        {
            foreach (PlayerCharacters characters in playerCharacters)
            {
                characters.characterStat[Define.CharacterStat.size] -= characters.InitialCharStat[Define.CharacterStat.size] * increaseAmount[(int)Define.Mastery.대포];
            }
            max += initialMax * increaseAmount[(int)Define.Mastery.대포];
        }
    }

    public void SetMasteryText(Define.Character character, Define.Mastery masteryName)
    {
        Transform masteryTransform = masteryUITrans.GetChild((int)character);
        Slider masteryExpSlider = masteryTransform.GetChild(0).GetComponent<Slider>();
        TextMeshProUGUI masteryText = masteryTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
        masteryExpSlider.value = playerCharacters[(int)character].masteryExp[masteryName] / 200f;
        masteryText.text = $"{masteryName} Lv.{playerCharacters[(int)character].masteryLevel[masteryName]}";
    }

    private void TimeChecking()
    {
        //3초마다 인구수 최대치 증가
        if(timer.GetPlayTime() - population_add_check >= 3)
        {
            population_add_check = (int)timer.GetPlayTime();
            MAXenemy += 1;
        }
        // 게임 종료 포탈 생성(일정시간 도달 또는 퀘스트클리어 또는 생존퀘스트 실패)
        if ((timer.GetPlayTime() > endgametime || anyone_wounded) && portal.activeInHierarchy == false)
        {
            // 포탈 생성 및 위치 랜덤 지정
            portal.SetActive(true);
            Vector3 player_position = player.transform.position;
            int random_percent = Random.Range(0, 2);
            if (random_percent == 0){
                portal_pos = new Vector3(player_position.x + Random.Range(-30f, 0)-20f, Random.Range(-35f, 35f), player_position.z);
            } else{
                portal_pos = new Vector3(player_position.x + Random.Range(0, 30f)+20f, Random.Range(-35f, 35f), player_position.z);
            }
            portal.transform.position = portal_pos;

            // 포탈 생성 위치 보여주기
            // StartCoroutine(showPortal());

            // 인디케이터
            indicatorObj.SetActive(true);
        }
        if(timer.GetPlayTime() - flag_change_checker >flag_change_timer)//일정시간 경과시 부대마크 변경 && 보스 생성
        {
            cur_flag = (cur_flag + 1) % 3;
            flag_change_checker = timer.GetPlayTime();
            flag_timer_half = false;
            //4가지 보스중 랜덤한 한종류 생성
            float[] a = new float[4] { 1, 0, 0, Random.Range(1, 5) };
            StartCoroutine(enemyManager.SpawnEnemy(Define.Enemy.Bow_Boss, a, true));
        }
        else if(timer.GetPlayTime() - flag_change_checker > flag_change_timer/2 && flag_timer_half == false)//부대 변경시간 절반 경과시 보스 생성
        {
            flag_timer_half = true;
            //4가지 보스중 랜덤한 한종류 생성
            float[] a = new float[4] { 1, 0, 0, Random.Range(1, 5) };
            StartCoroutine(enemyManager.SpawnEnemy(Define.Enemy.Bow_Boss, a, true));
        }

    }

    IEnumerator showPortal()
    {
        portal_cam.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        portal_cam.SetActive(false);
    }

    private bool IsPortalOffScreen(GameObject target)
    {
        Vector2 vec = Camera.main.WorldToViewportPoint(target.transform.position);
        if (vec.x <= 1 && vec.y <= 1 && vec.x >= 0 && vec.y >= 0) // 화면 안쪽 범위
            return false;
        else
            return true;
    }


    private void Spawn_setting(float a, float b, float c, float d)
    {
        AllenmeyNum[0] = a;
        AllenmeyNum[1] = b;
        AllenmeyNum[2] = c;
        AllenmeyNum[3] = d;
    }

    #region weaponIcon
    public void SwapSlot()
    {
        UndoClick();

        if (secondClickedInfo.Item1 != -2)
        {
            if (firstClickedInfo == secondClickedInfo) return;

            weaponManager.SwapWeapon(firstClickedInfo.Item1, secondClickedInfo.Item1, firstClickedInfo.Item3, secondClickedInfo.Item3);
        }

        firstClickedInfo = (-2, -2, null);
        secondClickedInfo = (-2, -2, null);
        EmptyRecombinationSlot();
    }

    // 선택된 둘 다 무기 정보가 있을 때
    public void SwapIcon()
    {
        // Debug.Log($"{firstClickedInfo.Item1} / {secondClickedInfo.Item1}");
        if (firstClickedInfo.Item1 == -2 || secondClickedInfo.Item1 == -2) return;

        int firstIndex = firstClickedInfo.Item1;
        int secondIndex = secondClickedInfo.Item1;
        if (firstClickedInfo.Item1 == -1)
            firstIndex = 3;
        if (secondClickedInfo.Item1 == -1)
            secondIndex = 3;

        Transform firstIcon = weaponIconsTrans.GetChild(firstIndex).GetChild(firstClickedInfo.Item2);
        Transform secondIcon = weaponIconsTrans.GetChild(secondIndex).GetChild(secondClickedInfo.Item2);
        //Debug.Log($"{firstClickedInfo.Item1}의 {firstClickedInfo.Item2}슬롯 / {secondClickedInfo.Item1}의 {secondClickedInfo.Item2}슬롯");
        Image firstImage = firstIcon.GetChild(1).GetComponent<Image>();
        Image secondImage = secondIcon.GetChild(1).GetComponent<Image>();

        //Debug.Log($"교환 전: {firstImage.sprite.name} {firstImage.GetComponent<WeaponIcon>().weaponInfo.weapon.weaponName}/ {secondImage.sprite.name}");

        Sprite tempSprite = firstImage.sprite;

        // 2 -> 1
        firstImage.sprite = secondImage.sprite;
        firstIcon.GetComponent<InventorySlot>().weaponInfo = secondClickedInfo.Item3;
        // CopyWeaponInfo_Icon(secondClickedInfo.Item3, firstIcon.GetComponent<InventorySlot>().weaponInfo);

        // 1 -> 2
        secondImage.sprite = tempSprite;
        secondIcon.GetComponent<InventorySlot>().weaponInfo = firstClickedInfo.Item3;
        // CopyWeaponInfo_Icon(tempInfo, secondIcon.GetComponent<InventorySlot>().weaponInfo);
        secondIcon.GetComponent<InventorySlot>().SetText(firstClickedInfo.Item3);
        //Debug.Log($"교환 후: {firstImage.sprite.name} {firstImage.GetComponent<WeaponIcon>().weaponInfo.weapon.weaponName}/ {secondImage.sprite.name}");
    }

    public void IconOn(int charIndex)
    {
        for (int i = 0; i < 2; i++)
        {
            weaponIconsTrans.GetChild(charIndex).GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
            weaponIconsTrans.GetChild(charIndex).GetChild(i).GetChild(1).GetComponent<Image>().color = Color.white;
        }
    }

    public void IconOff(int charIndex)
    {
        for (int i = 0; i < 2; i++)
        {
            weaponIconsTrans.GetChild(charIndex).GetChild(i).GetChild(0).GetComponent<Image>().color = Color.gray;
            weaponIconsTrans.GetChild(charIndex).GetChild(i).GetChild(1).GetComponent<Image>().color = Color.gray;
            weaponIconsTrans.GetChild(charIndex).GetChild(i).GetChild(1).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 0f;
        }
    }

    public void ColorIcon(int charIndex)
    {
        //if (playerCharacters[charIndex].masteryLevel[mastery] == 1) return;
        if (isWounded[charIndex]) return;
        if (charIndex == -1) return;
        //Debug.Log($"color icon : {charIndex} {playerCharacters[charIndex].GetHighestMastery()}");
        // ResetIconColor(charIndex);
        for (int i = 0; i < 2; i++)
        {
            Transform icon = weaponIconsTrans.GetChild(charIndex).GetChild(i);
            if (icon.GetComponent<InventorySlot>().weaponInfo == null)
            {
                icon.GetChild(1).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 0;
            }
            else
            {
                if (icon.GetComponent<InventorySlot>().weaponInfo.weapon.masteryName == playerCharacters[charIndex].GetHighestMastery())
                {
                    icon.GetChild(1).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 1f;
                }
                else
                {
                    icon.GetChild(1).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 0;
                }
            }
            //Debug.Log($"{charIndex} {i} {icon.GetComponent<Image>().GetComponent<WeaponIcon>().weaponInfo.weapon.masteryName}");
        }
    }

    public IEnumerator HighlightIcon(int charIndex, int iconIndex)
    {
        Transform targetIcon;

        if (charIndex == -1)
        {
            targetIcon = weaponIconsTrans.GetChild(3).GetChild(iconIndex).GetChild(1);
        }
        else
        {
            targetIcon = weaponIconsTrans.GetChild(charIndex).GetChild(iconIndex).GetChild(1);
        }

        float intensity = targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity;
        float radius = targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius;
        Color color = targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color;

        if (charIndex == -1)
        {
            targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 0.8f;
        }
        else
        {
            targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 1.2f;
        }

        targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 6;
        targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = Color.white;
        yield return new WaitForSecondsRealtime(1.5f);
        targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = intensity;
        targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = radius;
        targetIcon.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = color;
    }

    public void ResetIconColor(int charIndex)
    {
        for (int j = 0; j < 2; j++)
        {
            if (weaponIconsTrans.GetChild(charIndex).GetChild(j).GetChild(0).GetComponent<Image>().color != Color.gray)
            {
                weaponIconsTrans.GetChild(charIndex).GetChild(j).GetChild(1).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().pointLightOuterRadius = 0;
            }
        }
    }


    public void AddWeaponIcon(int charIndex, WeaponInfo weaponInfo)
    {
        if (charIndex > -1 && weaponManager.charWeaponCount[charIndex] > 1) return; // 자리 없음 => 아이콘 추가 무시

        Image iconImage;
        // 저장소에 아이콘 추가
        if (charIndex == -1)
        {
            iconImage = weaponIconsTrans.GetChild(3).GetChild(weaponManager.storageCount).GetChild(1).GetComponent<Image>();
        }
        // 사용중 무기에 아이콘 추가
        else
        {
            // 빈 곳 찾기
            if (weaponIconsTrans.GetChild(charIndex).GetChild(0).GetComponent<InventorySlot>().weaponInfo == null)
                iconImage = weaponIconsTrans.GetChild(charIndex).GetChild(0).GetChild(1).GetComponent<Image>();
            else
                iconImage = weaponIconsTrans.GetChild(charIndex).GetChild(1).GetChild(1).GetComponent<Image>();
        }

        // 스프라이트 변경
        if (weaponInfo.rarity >= 3)
        {
            iconImage.sprite = weaponIcons[weaponInfo.rarity][(int)weaponManager.GetOriginalWeaponName(weaponInfo.weapon.weaponName)];
        }
        else
        {
            iconImage.sprite = weaponIcons[weaponInfo.rarity][(int)weaponInfo.weapon.weaponName];
        }
        iconImage.transform.parent.GetComponent<InventorySlot>().weaponInfo = weaponInfo;
        // CopyWeaponInfo_Icon(weaponInfo, iconImage.transform.parent.GetComponent<InventorySlot>().weaponInfo);
        //Debug.Log("AddWeaponIcon");
        addedIconIndex = (charIndex, iconImage.transform.parent.GetSiblingIndex());
        return;
    }

    public void DeleteWeaponIcon(WeaponInfo weaponInfo)
    {
        //Debug.Log("DeleteIcon!!!!");

        int iconIndex = -1;

        // 저장소에서 무기 찾기
        if (weaponInfo.charIndex == -1)
        {
            for (int i = 0; i < 12; i++)
            {
                WeaponInfo targetWeaponInfo = weaponIconsTrans.GetChild(3).GetChild(i).GetComponent<InventorySlot>().weaponInfo;
                if (targetWeaponInfo == null) continue;
                if (CompareWeaponInfo(targetWeaponInfo, weaponInfo))
                {
                    iconIndex = i;
                    break;
                }
            }
        }
        // 사용중 무기에서 무기 찾기
        else
        {
            for (int i = 0; i < 2; i++)
            {
                WeaponInfo targetWeaponInfo = weaponIconsTrans.GetChild(weaponInfo.charIndex).GetChild(i).GetComponent<InventorySlot>().weaponInfo;
                if (targetWeaponInfo == null) continue;
                if (CompareWeaponInfo(targetWeaponInfo, weaponInfo))
                {
                    iconIndex = i;
                    break;
                }
            }
        }

        //Debug.Log($"아이콘 삭제!! {weaponInfo.charIndex}의 {iconIndex}번째 아이콘");
        if (iconIndex == -1) return;
        Image iconImage;
        if (weaponInfo.charIndex == -1)
            iconImage = weaponIconsTrans.GetChild(3).GetChild(iconIndex).GetChild(1).GetComponent<Image>();
        else
            iconImage = weaponIconsTrans.GetChild(weaponInfo.charIndex).GetChild(iconIndex).GetChild(1).GetComponent<Image>();
        iconImage.sprite = weaponIcons[0][0]; // 빈 칸으로 변경
        iconImage.transform.parent.GetComponent<InventorySlot>().weaponInfo = null;

        SortWeaponIcon(weaponInfo.charIndex);
    }

    // 왼쪽 정렬 (빈 칸이 없도록)
    public void SortWeaponIcon(int charIndex)
    {
        // storage만 정렬

        if (charIndex > -1) return;
        // {
        //     for (int i = 0; i < 1; i++)
        //     {
        //         Transform leftIcon = weaponIconsTrans.GetChild(charIndex).GetChild(i);
        //         Transform rightIcon = weaponIconsTrans.GetChild(charIndex).GetChild(i + 1);
        //         Image leftImage = leftIcon.GetChild(1).GetComponent<Image>();
        //         Image rightImage = rightIcon.GetChild(1).GetComponent<Image>();
        //         WeaponInfo leftInfo = leftIcon.GetComponent<InventorySlot>().weaponInfo;
        //         WeaponInfo rightInfo = rightIcon.GetComponent<InventorySlot>().weaponInfo;

        //         if (leftInfo == null && rightInfo != null)
        //         {
        //             leftIcon.GetComponent<InventorySlot>().weaponInfo = new WeaponInfo();
        //             CopyWeaponInfo_Icon(rightIcon.GetComponent<InventorySlot>().weaponInfo, leftIcon.GetComponent<InventorySlot>().weaponInfo);
        //             rightIcon.GetComponent<InventorySlot>().weaponInfo = null;
        //             leftImage.sprite = rightImage.sprite;
        //             rightImage.sprite = weaponIcons[0][0];
        //         }
        //     }
        // }

        {
            for (int i = 0; i < 11; i++)
            {
                Transform leftIcon = weaponIconsTrans.GetChild(3).GetChild(i);
                Transform rightIcon = weaponIconsTrans.GetChild(3).GetChild(i + 1);
                Image leftImage = weaponIconsTrans.GetChild(3).GetChild(i).GetChild(1).GetComponent<Image>();
                Image rightImage = weaponIconsTrans.GetChild(3).GetChild(i + 1).GetChild(1).GetComponent<Image>();
                WeaponInfo leftInfo = leftIcon.GetComponent<InventorySlot>().weaponInfo;
                WeaponInfo rightInfo = rightIcon.GetComponent<InventorySlot>().weaponInfo;

                if (leftInfo == null && rightInfo != null)
                {
                    leftIcon.GetComponent<InventorySlot>().weaponInfo = rightIcon.GetComponent<InventorySlot>().weaponInfo;
                    // CopyWeaponInfo_Icon(rightIcon.GetComponent<InventorySlot>().weaponInfo, leftIcon.GetComponent<InventorySlot>().weaponInfo);
                    rightIcon.GetComponent<InventorySlot>().weaponInfo = null;
                    leftImage.sprite = rightImage.sprite;
                    rightImage.sprite = weaponIcons[0][0];
                }
            }

        }

    }

    public void CopyWeaponInfo_Icon(WeaponInfo srcInfo, WeaponInfo dstInfo)
    {
        dstInfo.weaponName = srcInfo.weaponName;
        dstInfo.charIndex = srcInfo.charIndex;
        dstInfo.rarity = srcInfo.rarity;
        dstInfo.weapon = srcInfo.weapon;
        dstInfo.weaponIndex = srcInfo.weaponIndex;
        dstInfo.option = srcInfo.option;
        dstInfo.coolTime = srcInfo.coolTime;
        dstInfo.isStartWeapon = srcInfo.isStartWeapon;
    }
    #endregion

    public bool CompareWeaponInfo(WeaponInfo info1, WeaponInfo info2)
    {
        if (info1.weapon.weaponName == info2.weapon.weaponName && info1.rarity == info2.rarity && info1.charIndex == info2.charIndex && info1.weaponIndex == info2.weaponIndex)
            return true;
        return false;
    }

    public void SetStartingWeaponUI()
    {
        for (int i = 0; i < 3; i++)
        {
            weaponIconsTrans.GetChild(i).GetChild(0).GetComponent<InventorySlot>().SetStartingWeaponUI();
        }
    }

    bool isShaking = false;
    bool isDamaged = false;

    public void SetSynergyIcon(bool[] synergy)
    {
        for (int i = 0; i < 4; i++)
        {
            if (synergy[i])
            {
                synergySlotsTrans.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
                synergySlotsTrans.GetChild(i).GetChild(1).GetComponent<Image>().color = Color.white;
            }
            else
            {
                synergySlotsTrans.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.gray;
                synergySlotsTrans.GetChild(i).GetChild(1).GetComponent<Image>().color = Color.gray;
            }
        }
    }

    // 대포 연출
    public IEnumerator ShakeCamera(float intensity, float time)
    {
        if (isShaking) yield break;
        isShaking = true;
        CinemachineBasicMultiChannelPerlin noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        yield return new WaitForSecondsRealtime(time);
        noise.m_AmplitudeGain = 0;
        isShaking = false;
    }
    // 플레이어 피격 연출
    public IEnumerator RedScreenOn(float time)
    {
        if (isDamaged) yield break;
        isDamaged = true;
        redScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(time);
        redScreen.SetActive(false);
        isDamaged = false;
    }

    public void ObjectSpawn()
    {
        if (kill - RO_check > 10)//100킬마다 오브젝트 생성
        {
            Vector3 pos = player.transform.position + Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(11f, 15f));
            Instantiate(RandObject, pos, Quaternion.identity);
            RO_check += 10;
        }
    }
}
