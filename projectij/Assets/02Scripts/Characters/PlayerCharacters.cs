using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacters : Characters
{
    // 캐릭터 초기 스탯 저장
    protected Dictionary<Define.CharacterStat, float> initialCharStat = new Dictionary<Define.CharacterStat, float>();

    // 무기 추가 효과 적용된 스탯
    [SerializeField]
    public Dictionary<Define.CharacterStat, float> characterStat = new Dictionary<Define.CharacterStat, float>();
    // 퀘스트 스탯
    public Dictionary<Define.CharacterStat, float> questStat = new Dictionary<Define.CharacterStat, float>();

    // 숙련도 (레벨, 경험치)
    public Dictionary<Define.Mastery, int> masteryLevel = new Dictionary<Define.Mastery, int>();
    public Dictionary<Define.Mastery, float> masteryExp = new Dictionary<Define.Mastery, float>();

    public bool isInvincible = false;
    float invincibleTime = 0.5f;
    public Coroutine InvincibleCoroutine;
    Define.Mastery highestMastery;
    [SerializeField]
    protected bool isWounded = false;

    public bool HasBlackWeapon = false;
    public bool IsWounded { get { return isWounded; } set { isWounded = value; } }
    public Dictionary<Define.CharacterStat, float> InitialCharStat { get { return initialCharStat; } }

    // 무기의 extraStat 적용을 받지 않는 스탯
    protected float pureMaxHP;
    protected float pureDefense;
    protected float pureMoveSpeed;

    public float PureMaxHP { get { return pureMaxHP; } set { pureMaxHP = value; characterStat[Define.CharacterStat.최대체력] = pureMaxHP + extraMaxHP; } }
    public float PureDefense { get { return pureDefense; } set { pureDefense = value; characterStat[Define.CharacterStat.방어도] = pureDefense + extraDefense; } }
    public float PureMoveSpeed { get { return pureMoveSpeed; } set { pureMoveSpeed = value; characterStat[Define.CharacterStat.이동속도] = pureMoveSpeed + extraMoveSpeed; } }
    // 무기의 extraStat 저장용
    protected float extraMaxHP = 0;
    protected float extraDefense = 0;
    protected float extraMoveSpeed = 0;

    public void SetClashDmg(float a)
    {
        characterStat[Define.CharacterStat.clashDamage] = a;
    }

    protected float h, v;

    float blackWeaponTimer = 0;

    WeaponManager weaponManager;

    // 방패
    public int shieldCount = 0;

    // 신장대
    public float ExpBuff = 0;

    // 포탈
    public bool end = false;
    public int total_portal = 0;
    public int got_portal = 0;

    public void GainHP(float amount)
    {
        HP += amount;
        if (HP > characterStat[Define.CharacterStat.최대체력])
            HP = characterStat[Define.CharacterStat.최대체력];
    }
    
    public void ReduceHP(float amount)
    {
        HP -= amount;
        StageManager.Instance.StartCoroutine(StageManager.Instance.RedScreenOn(0.7f));
    }

    public void SetDirection((float, float) MoveDirection)
    {
        if (MoveDirection.Item1 > 0)
            spriteRenderer.flipX = true;
        else if (MoveDirection.Item1 < 0)
            spriteRenderer.flipX = false;

        h = MoveDirection.Item1;
        v = MoveDirection.Item2;
    }

    protected void Collision_Function(GameObject enemyObj)
    {
        if (shieldCount <= 0 && HP > 0)
        {
            ReduceHP(enemyObj.GetComponent<Enemy>().damage * 100/(100+characterStat[Define.CharacterStat.방어도]));
            /*if (enemyObj.GetComponent<Enemy>().damage - characterStat[Define.CharacterStat.방어도] > 0)
                ReduceHP(enemyObj.GetComponent<Enemy>().damage - characterStat[Define.CharacterStat.방어도]);
            else
                ReduceHP(0);*/
        }
        else
        {
            shieldCount--;
        }

        // 대포 시너지
        if (weaponManager.synergyOn[3])
        {
            weaponManager.CannonSynergyAttack();
        }

        if (enemyObj.GetComponent<Enemy>().player_clash_num > 0)
        {
            //Debug.Log("몸통박치기");
            float tempClash = characterStat[Define.CharacterStat.clashDamage];
            if (HasBlackWeapon)
                tempClash += characterStat[Define.CharacterStat.최대체력] * 0.1f;

            enemyObj.GetComponent<Enemy>().player_clash_num -= 1;
            enemyObj.GetComponent<Enemy>().GetHit(tempClash, Vector2.zero,true);
            GameObject num = ObjectPoolManager.Instance.GetFloatingDmg(Define.FloatingDmg.FloatingDmg);
            num.transform.position = enemyObj.transform.position;
            if (tempClash == (int)tempClash)
                num.transform.GetChild(0).GetComponent<TextMesh>().text = string.Format("{0}", tempClash);
            else
                num.transform.GetChild(0).GetComponent<TextMesh>().text = string.Format("{0:N1}", tempClash);
        }
        InvincibleCoroutine = StartCoroutine(BeingInvincible(invincibleTime, true));
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        if ((other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss") && isInvincible == false)
        {
            Collision_Function(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {

        if (other.gameObject.tag == "EnemyWeapon" && isInvincible == false)
        {
            if (shieldCount <= 0 && HP > 0)
                ReduceHP(other.transform.parent.GetComponent<Enemy>().damage * 100 / (100 + characterStat[Define.CharacterStat.방어도]));
            else
            {
                shieldCount--;
            }
            if (weaponManager.masteryWeaponCount[(int)Define.Mastery.대포] >= 3)
            {
                weaponManager.CannonSynergyAttack();
            }
            InvincibleCoroutine = StartCoroutine(BeingInvincible(invincibleTime, true));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss") && isInvincible == false)
        {
            Collision_Function(other.gameObject);
        }
        else if (other.gameObject.tag == "ExpBall")
        {
            ExpBall expBall = other.gameObject.GetComponent<ExpBall>();
            StageManager.Instance.GainTeamEXP(expBall.ExpAmount * (1 + ExpBuff));
            //Debug.Log($"경험치: {expBall.ExpAmount * (1 + ExpBuff)}");
            ObjectPoolManager.Instance.ReturnExpball(other.gameObject);
        }
        //랜덤 아이템
        else if(other.gameObject.tag == "RandItem")
        {
            switch(other.gameObject.name)
            {
                case "rand_item1"://자석
                    /*GameObject[] exp_ball = GameObject.FindGameObjectsWithTag("ExpBall");
                    Debug.Log(exp_ball.Length);
                    foreach(GameObject expball in exp_ball)
                    {
                        expball.transform.position = Vector3.MoveTowards(expball.transform.position, gameObject.transform.position, 1f);
                    }*/
                    foreach (ExpBall exp in StageManager.Instance.allExpBalls)
                    {
                        exp.StartCoroutine(exp.MagnetOn(transform));
                    }
                    Destroy(other.gameObject);
                    break;
                case "rand_item2"://골드
                    break;
                case "rand_item3"://많은 골드
                    break;
                case "rand_item4"://공격 아이템
                    break;
                case "rand_item5"://뭐지????
                    break;
            }
        }
        // 종료 조건
        else if (other.gameObject.tag == "Portal" && got_portal == total_portal)
        {
            end = true;
        }
        else if (other.gameObject.tag == "Portal")
        {
            Debug.Log("충돌!");
            got_portal++;
            other.gameObject.SetActive(false);
            float[] enemey_arr = new float[4] { 20, 2, 0.5f, 0 };
            StartCoroutine(StageManager.Instance.GetEnemyManager().SpawnEnemy(Define.Enemy.Bow_Boss, enemey_arr, false));
        }
    }

    public IEnumerator BeingInvincible(float time, bool transparent)
    {
        isInvincible = true;
        if (transparent && !isWounded)
        {
            for(int i =0;i<2;i++)
            {
                Color color = spriteRenderer.color;
                color.a = 0.5f;
                spriteRenderer.color = color;
                yield return new WaitForSeconds(time/4);
                color.a = 1f;
                spriteRenderer.color = color;
                yield return new WaitForSeconds(time /4);
            }
            //Debug.Log($"시작! {time}");
        }
        isInvincible = false;
        /*if (transparent)
        {
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
            //Debug.Log("끝!");
        }*/
    }

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        // 나중에 캐릭터 별 스탯 CSV로 받아서 적용
        for (int i = 0; i < (int)Define.CharacterStat.end; i++)
        {
            initialCharStat.Add((Define.CharacterStat)i, 1);
            characterStat.Add((Define.CharacterStat)i, 1);
        }
        Define.CharacterName characterName = GameManager.gameManager_Instance.squad[(int)character].characterName;
        foreach (KeyValuePair<Define.CharacterStat, float> stat in GameManager.gameManager_Instance.totalInitialCharStat[(int)characterName])
        {
            initialCharStat[stat.Key] = stat.Value;
            characterStat[stat.Key] = stat.Value;
        }
        // initialCharStat[Define.CharacterStat.데미지] += float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["Damage"].ToString());
        // initialCharStat[Define.CharacterStat.공격크기] += float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["Size"].ToString());
        // initialCharStat[Define.CharacterStat.쿨타임감소] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["CoolTime"].ToString());
        // initialCharStat[Define.CharacterStat.추가투사체확률] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["ProjectilePercentage"].ToString());
        // initialCharStat[Define.CharacterStat.투사체속도] += float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["Speed"].ToString());
        // initialCharStat[Define.CharacterStat.관통횟수] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["PenetrationCount"].ToString());
        // initialCharStat[Define.CharacterStat.넉백] += float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["Force"].ToString());
        // initialCharStat[Define.CharacterStat.방어도] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["Defense"].ToString());
        // initialCharStat[Define.CharacterStat.최대체력] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["HP"].ToString());
        // initialCharStat[Define.CharacterStat.이동속도] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["MoveSpeed"].ToString());
        // initialCharStat[Define.CharacterStat.size] = float.Parse(GameManager.gameManager_Instance.characterInitialStatCSV[(int)characterName]["CharacterDistance"].ToString());
        // initialCharStat[Define.CharacterStat.clashDamage] = 5;

        // foreach(KeyValuePair<Define.CharacterStat, float> stat in InitialCharStat)
        // {
        //     characterStat[stat.Key] = stat.Value; 
        // }

        // 모든 무기 숙련도 1로 세팅
        for (int i = 0; i < (int)Define.Mastery.end; i++)
        {
            masteryLevel[(Define.Mastery)i] = 1;
            masteryExp[(Define.Mastery)i] = 0;
        }
        HP = characterStat[Define.CharacterStat.최대체력];
        pureMaxHP = characterStat[Define.CharacterStat.최대체력];
        pureDefense = characterStat[Define.CharacterStat.방어도];
        pureMoveSpeed = characterStat[Define.CharacterStat.이동속도];
    }

    private void Start() {
        StageManager.Instance.highestMasteries[(int)character] = (highestMastery, 0);
        BuffOn(highestMastery, 1);
        StageManager.Instance.ManageMasteryBuff((int)character, Define.Mastery.도검, 1);
        weaponManager = StageManager.Instance.GetWeaponManager();
    }

    public override void Fixed_Update()
    {
        if (HP <= 0 && !isWounded)
        {
            //생존 미션 실패(일정시간 부상안당하기 실패)
            if ((StageManager.Instance.all_quest & 2) == 2 && StageManager.Instance.GetTimer().GetPlayTime()<StageManager.Instance.time_for_succes)
            {
                StageManager.Instance.anyone_wounded = true;
            }
            isWounded = true;

            //isInvincible = false;
            StopCoroutine(InvincibleCoroutine);
            Debug.Break();
            Debug.Log($"{character} 부상!");
            StageManager.Instance.IconOff((int)character);
            StageManager.Instance.ManageMasteryBuff((int)character, Define.Mastery.도검, 1);
            weaponManager.DowngradeOption(character);
            StageManager.Instance.isWounded[(int)character] = true;
            spriteRenderer.color = new Color(1, 100/255f, 100/255f, 1);

            for (int i = 0; i < (int)Define.Mastery.end; i++)
            {
                masteryLevel[(Define.Mastery)i] = 1;
                masteryExp[(Define.Mastery)i] = 0;
            }

            StageManager.Instance.highestMasteries[(int)character] = (Define.Mastery.도검, 0);
            highestMastery = Define.Mastery.도검;
            StageManager.Instance.ManageMasteryBuff((int)character, Define.Mastery.도검, 1);
            // StageManager.Instance.SetMasteryText(character, Define.Mastery.도검);

            // 검정무기 있으면 치료
            if (HasBlackWeapon)
            {
                Invoke("GetCured", 2);
            }
            else
            {
                bool checkGameOver = true;
                for (int i = 0; i < 3; i++)
                {
                    if (StageManager.Instance.isWounded[i] == false)
                        checkGameOver = false;
                }
                if (checkGameOver)
                    StageManager.Instance.GameOver();
            }
        }

        if (HasBlackWeapon)
        {
            blackWeaponTimer += Time.fixedDeltaTime;
            if (blackWeaponTimer >= 60)
            {
                blackWeaponTimer = 0;
                foreach (ExpBall exp in StageManager.Instance.allExpBalls)
                {
                    exp.StartCoroutine(exp.MagnetOn(transform));
                }
            }
        }
    }


    // 버프: 숙련도 레벨에 따라서 +1% (최대 30%까지)
    public void BuffOn(Define.Mastery masteryName, int level)
    {
        float increaseAmount = 0.01f * level;
        switch (masteryName)
        {
            // 맨몸 충돌 데미지 증가
            case Define.Mastery.도검:
                characterStat[Define.CharacterStat.clashDamage] += initialCharStat[Define.CharacterStat.clashDamage] * increaseAmount;
                break;
            // 방어력 증가, 최대체력 증가
            case Define.Mastery.둔기:
                pureDefense += initialCharStat[Define.CharacterStat.방어도] * increaseAmount;
                pureMaxHP += initialCharStat[Define.CharacterStat.최대체력] * increaseAmount;

                characterStat[Define.CharacterStat.최대체력] = pureMaxHP + extraMaxHP;
                characterStat[Define.CharacterStat.방어도] = pureDefense + extraDefense;
                break;
            
            case Define.Mastery.개인화기:
                break;

            case Define.Mastery.대포:
                break;
        }

        StageManager.Instance.UpdateStatText();
    }
    
    public void BuffOff(Define.Mastery masteryName, int level)
    {
        float decreaseAmount = 0.01f * level;
        switch (masteryName)
        {
            // 맨몸 충돌 데미지 증가
            case Define.Mastery.도검:
                characterStat[Define.CharacterStat.clashDamage] -= initialCharStat[Define.CharacterStat.clashDamage] * decreaseAmount;
                break;
            // 방어력 증가, 최대체력 증가
            case Define.Mastery.둔기:
                pureDefense -= initialCharStat[Define.CharacterStat.방어도] * decreaseAmount;
                pureMaxHP -= initialCharStat[Define.CharacterStat.최대체력] * decreaseAmount;
                
                characterStat[Define.CharacterStat.최대체력] = pureMaxHP + extraMaxHP;
                characterStat[Define.CharacterStat.방어도] = pureDefense + extraDefense;
                break;
                
            case Define.Mastery.개인화기:
                break;
                
            case Define.Mastery.대포:
                break;
        }
        StageManager.Instance.UpdateStatText();
    }

    public void GainMasteryExp(Define.Mastery masteryName, float amount)
    {
        if (isWounded) return;
        if (masteryLevel[masteryName] >= 30) return;

        masteryExp[masteryName] += amount;

        bool levelUp = false;

        // 일단 레벨업에 필요한 경험치를 200으로 고정
        if (masteryExp[masteryName] >= 200)
        {
            levelUp = true;
            masteryExp[masteryName] = 0;
            masteryLevel[masteryName]++;
        }

        if (highestMastery != masteryName)
        {
            // 이 mastery가 highestMastery를 추월한 경우
            if (masteryLevel[masteryName] > masteryLevel[highestMastery] || (masteryLevel[masteryName] == masteryLevel[highestMastery] && masteryExp[masteryName] > masteryExp[highestMastery]))
            {
                int oldLevel = masteryLevel[highestMastery];
                int newLevel = masteryLevel[masteryName];
                // 이전 버프 제거
                BuffOff(highestMastery, oldLevel);

                highestMastery = masteryName;
                StageManager.Instance.ColorIcon((int)character);

                // 새로운 버프 적용
                BuffOn(masteryName, newLevel);

                // 다른 캐릭터의 마스터리 레벨과 비교하여 버프 관리
                StageManager.Instance.ManageMasteryBuff((int)character, masteryName, newLevel);

                // UI 텍스트 변경
                // StageManager.Instance.SetMasteryText(character, masteryName);
            }
        }
        // 텍스트, 슬라이더 변경
        else
        {
            // UI 텍스트 변경
            // StageManager.Instance.SetMasteryText(character, masteryName);
            if (levelUp)
            {
                // 버프 수치 추가
                BuffOn(masteryName, 1);

                // 다른 캐릭터의 마스터리 레벨과 비교하여 버프 관리
                StageManager.Instance.ManageMasteryBuff((int)character, masteryName, masteryLevel[masteryName]);
            }
        }
    }

    public Define.Mastery GetHighestMastery()
    {
        return highestMastery;
    }

    public Dictionary<Define.CharacterStat, float> GetCharacterStat()
    {
        return characterStat;
    }

    public void SetCharacterStat(Dictionary<Define.CharacterStat, float> extraCharStat)
    {
        foreach (KeyValuePair<Define.CharacterStat, float> kvp in extraCharStat)
        {
            characterStat[kvp.Key] = (initialCharStat[kvp.Key] + extraCharStat[kvp.Key]);
        }
        extraMaxHP = extraCharStat[Define.CharacterStat.최대체력];
        extraDefense = extraCharStat[Define.CharacterStat.방어도];
        extraMoveSpeed = extraCharStat[Define.CharacterStat.이동속도];

        characterStat[Define.CharacterStat.최대체력] = pureMaxHP + extraMaxHP;
        characterStat[Define.CharacterStat.방어도] = pureDefense + extraDefense;
        characterStat[Define.CharacterStat.이동속도] = pureMoveSpeed + extraMoveSpeed;
    }

    public int GetMasteryLevel(Define.Mastery masteryName)
    {
        return masteryLevel[masteryName];
    }

    public bool GetIsWounded()
    {
        return isWounded;
    }

    private void GetCured()
    {
        StageManager.Instance.isWounded[(int)character] = false;
        isWounded = false;
        HP = characterStat[Define.CharacterStat.최대체력];
        StageManager.Instance.IconOn((int)character);
        StageManager.Instance.ColorIcon((int)character);
        StageManager.Instance.GetWeaponManager().RestoreOption(character);
    }
}
