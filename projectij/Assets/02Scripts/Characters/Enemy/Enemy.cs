using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Characters
{
    //protected float? timercheck = null;//float변수null만들기 위해서 ?표 붙임
    public float timercheck;
    protected Camera cam;
    public int player_clash_num = 5;
    public int flag_num;
    [SerializeField]
    private Animator animat;
    public Animation ac;

    public float Speed;
    public float maxSpeed;
    public float maxDefense;
    public float damage;
    public float[] population = new float[5] { 1, 2, 3, 4, 5 };
    public float[] time_upgrade = new float[6] { 0, 0, 0, 0, 0, 0};
    public int time_check;
    public bool first_disable;

    // 인스펙터창에서 Define에 정의된 적 이름 선택
    public Define.Enemy enemyName;
    protected Player player;
    
    protected Rigidbody2D rb;
    // 무기에 의해 트리거 되는 boolean
    public bool knockedback = false;
    public bool targeted = false;
    public bool stunned = false;
    public bool isSlow = false;
    public bool absorbed = false;
    public int poisonStack = 0;

    // 상태를 나타내는 boolean
    protected bool beingKnockedback = false;
    protected bool beingStunned = false;
    protected bool beingDied = false;

    //드롭상자
    public GameObject box;

    //public float stunTime = 1.5f;
    //public float knockbackTime = 0.07f;

    public IEnumerator heal(float time, int heal)
    {
        
        HP += heal;
        if (HP > maxHP)
            HP = maxHP;
        spriteRenderer.color = new Color(120 / 255f, 1, 150 / 255f, 1);
        animat.SetTrigger("healing");
        yield return new WaitForSeconds(time);
        animat.SetTrigger("heal_end");
        spriteRenderer.color = Color.white;
    }

    protected override void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        status();
        HP = maxHP;
        Speed = maxSpeed;
        defense = maxDefense;
        player = StageManager.Instance.player;
        cam = GameObject.Find("Main Camera").GetComponentInChildren<Camera>();
    }

    public void status()
    {
        population = new float[5]{ 1,2,3,4,5};
        Base_status();
        diff_status();
        army_status();
        time_status();
    }
    protected void Base_status()
    {
        switch (enemyName)
        {
            case Define.Enemy.Sword_infantry:
                base_status_sub(0, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.NormalEnemy:
                base_status_sub(1, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Healer:
                base_status_sub(2, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Ninja:
                base_status_sub(3, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Spear_Boss:
                base_status_sub(4, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Sword_Boss:
                base_status_sub(5, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Bow_Boss:
                base_status_sub(6, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Arrows:
                base_status_sub(7, ObjectPoolManager.Instance.base_CSV);
                break;
            case Define.Enemy.Chair_Boss:
                base_status_sub(8, ObjectPoolManager.Instance.base_CSV);
                break;
        }
    }

    protected void base_status_sub(int i, List<Dictionary<string, object>> base_CSV)
    {
        maxHP = float.Parse(base_CSV[i]["hp"].ToString());
        maxSpeed = float.Parse(base_CSV[i]["이동속도"].ToString());
        damage = float.Parse(base_CSV[i]["공격력"].ToString());
        maxDefense = float.Parse(base_CSV[i]["방어력"].ToString());
        rb.mass = float.Parse(base_CSV[i]["mass"].ToString());
        population[0] = float.Parse(base_CSV[i]["인구"].ToString());
    }

    protected void diff_status()
    {
        switch (StageManager.Instance.difficulty)
        {
            case 1:
                diff_status_sub(0, ObjectPoolManager.Instance.diff_CSV);
                break;
            case 2:
                diff_status_sub(1, ObjectPoolManager.Instance.diff_CSV);
                break;
            case 3:
                diff_status_sub(2, ObjectPoolManager.Instance.diff_CSV);
                break;
            case 4:
                diff_status_sub(3, ObjectPoolManager.Instance.diff_CSV);
                break;
            case 5:
                diff_status_sub(4, ObjectPoolManager.Instance.diff_CSV);
                break;
        }
    }

    protected void diff_status_sub(int i, List<Dictionary<string, object>> diff_CSV)
    {
        damage += float.Parse(diff_CSV[i]["공격력(더하기)"].ToString());
        maxDefense += float.Parse(diff_CSV[i]["방어력(더하기)"].ToString());
        maxHP *= float.Parse(diff_CSV[i]["hp(곱하기)"].ToString());
        maxSpeed *= float.Parse(diff_CSV[i]["이동속도(곱하기)"].ToString());
        rb.mass *= float.Parse(diff_CSV[i]["mass(곱하기)"].ToString());

    }

    protected void army_status()
    {
        switch (flag_num)
        {
            case 1:
                army_status_sub(0, ObjectPoolManager.Instance.army_CSV);
                break;
            case 2:
                army_status_sub(1, ObjectPoolManager.Instance.army_CSV);
                break;
            case 3:
                army_status_sub(2, ObjectPoolManager.Instance.army_CSV);
                break;
            case 4:
                army_status_sub(3, ObjectPoolManager.Instance.army_CSV);
                break;
            case 5:
                army_status_sub(4, ObjectPoolManager.Instance.army_CSV);
                break;
            case 6:
                army_status_sub(5, ObjectPoolManager.Instance.army_CSV);
                break;
            case 7:
                army_status_sub(6, ObjectPoolManager.Instance.army_CSV);
                break;
            case 8:
                army_status_sub(7, ObjectPoolManager.Instance.army_CSV);
                break;
            case 9:
                army_status_sub(8, ObjectPoolManager.Instance.army_CSV);
                break;
            default:
                break;
        }
    }

    protected void army_status_sub(int i, List<Dictionary<string, object>> army_CSV)
    {
        maxSpeed *= float.Parse(army_CSV[i]["이동속도"].ToString());
        maxDefense *= float.Parse(army_CSV[i]["방어력"].ToString());
        maxHP *= float.Parse(army_CSV[i]["hp"].ToString());
        damage *= float.Parse(army_CSV[i]["공격력"].ToString());
        rb.mass *= float.Parse(army_CSV[i]["mass"].ToString());
        for (int j = 0; j < population.Length; j++)
            population[j] *= float.Parse(army_CSV[i]["적 하나당 인구수"].ToString());
        float length = Mathf.Sqrt(float.Parse(army_CSV[i]["크기(x * y)"].ToString()));
        transform.localScale = new Vector3(length,length, 1);
        
    }


    protected virtual void time_status()
    {
        timercheck = StageManager.Instance.GetTimer().GetPlayTime() / 300;
        for(int i =0;i<(int)(timercheck+1)/2;i++)
        {
            time_upgrade[1] += float.Parse(ObjectPoolManager.Instance.time_CSV[0]["방어력"].ToString());//방어력
            time_upgrade[2] += float.Parse(ObjectPoolManager.Instance.time_CSV[0]["hp"].ToString());//hp
            time_upgrade[3] += float.Parse(ObjectPoolManager.Instance.time_CSV[0]["공격력"].ToString());//공격력
        }
        for(int i =0;i<(int)timercheck;i++)
        {
            time_upgrade[0] += float.Parse(ObjectPoolManager.Instance.time_CSV[1]["이동속도"].ToString());//이동속도
            time_upgrade[1] += float.Parse(ObjectPoolManager.Instance.time_CSV[1]["방어력"].ToString());//방어력
            time_upgrade[2] += float.Parse(ObjectPoolManager.Instance.time_CSV[1]["hp"].ToString());//hp
            time_upgrade[3] += float.Parse(ObjectPoolManager.Instance.time_CSV[1]["공격력"].ToString());//공격력
            time_upgrade[4] += float.Parse(ObjectPoolManager.Instance.time_CSV[1]["mass"].ToString());//mass
            time_upgrade[5] += float.Parse(ObjectPoolManager.Instance.time_CSV[1]["적 개당 인구수"].ToString());//인구수
        }
        /*if (StageManager.Instance.GetTimer().GetPlayTime() - timercheck*300 >300)
        {
            if(timercheck%2 == 0)//5분 타이머
            {
                time_upgrade[1] += (float)ObjectPoolManager.Instance.time_CSV[0]["방어력"];//방어력
                time_upgrade[2] += (float)ObjectPoolManager.Instance.time_CSV[0]["hp"];//hp
                time_upgrade[3] += (float)ObjectPoolManager.Instance.time_CSV[0]["공격력"];//공격력
                timercheck += 1;
            }
            else if(timercheck % 2 == 1)//10분 타이머
            {
                time_upgrade[0] += (float)ObjectPoolManager.Instance.time_CSV[1]["이동속도"];//이동속도
                time_upgrade[1] += (float)ObjectPoolManager.Instance.time_CSV[1]["방어력"];//방어력
                time_upgrade[2] += (float)ObjectPoolManager.Instance.time_CSV[1]["hp"];//hp
                time_upgrade[3] += (float)ObjectPoolManager.Instance.time_CSV[1]["공격력"];//공격력
                time_upgrade[4] += (float)ObjectPoolManager.Instance.time_CSV[1]["mass"];//mass
                time_upgrade[5] += (float)ObjectPoolManager.Instance.time_CSV[1]["인구수"];//인구수
                timercheck += 1;
            }
        }*/
        maxSpeed += time_upgrade[0];
        maxDefense += time_upgrade[1];
        maxHP += time_upgrade[2];
        damage += time_upgrade[3];
        rb.mass += time_upgrade[4];
        for (int i =0;i<population.Length;i++)
        {
            population[i] += time_upgrade[5];
        }
            
    }

    private void OnEnable()
    {
        knockedback = false;
        targeted = false;
        stunned = false;
        isSlow = false;
        absorbed = false;
        poisonStack = 0;
        timercheck = StageManager.Instance.GetComponent<Timer>().GetPlayTime();
        beingKnockedback = false;
        beingStunned = false;
        beingDied = false;
        

        status();
        player_clash_num = 5;
        HP = maxHP;
        Speed = maxSpeed;
        defense = maxDefense;
    }

    public override void Fixed_Update()
    {
        if(player!=null)
        {
            Vector3 playerPos = player.transform.position;
            //float step = Time.deltaTime * Speed;
            //transform.position = Vector2.MoveTowards(transform.position, playerPos, step);
            if(!beingDied)//죽는 애니메이션 실행중 아닐때
            {
                if (!beingKnockedback)
                {
                    if (!beingStunned) // 원래 이동 방식
                    {
                        rb.velocity = (playerPos - transform.position).normalized * Speed;
                    }
                    else // 스턴 걸림
                        rb.velocity = Vector2.zero;
                }
                if (playerPos.x > transform.position.x && spriteRenderer.flipX == false)
                {
                    spriteRenderer.flipX = true;
                    transform.GetChild(1).GetComponent<Collider2D>().offset = new Vector2(1.95f, 0.4850166f);
                }
                else if(playerPos.x < transform.position.x && spriteRenderer.flipX == true)
                {
                    spriteRenderer.flipX = false;
                    transform.GetChild(1).GetComponent<Collider2D>().offset = new Vector2(-1.556638f, 0.4850166f);
                }
                if (Vector3.Distance(transform.position, playerPos) > cam.orthographicSize / 5.4f * 11 + 0.5f)
                {
                    position_resetting(transform.position, playerPos);
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public override void Collision_Function(Collision2D other)
    {
        base.Collision_Function(other);
        //Debug.Log($"Enemy collision {other.transform.tag}");
        switch (other.transform.tag)
        {
            case "Player":
                {
                    //Debug.Log("플레이어 맞음");
                    // 적 사망
                    // 경험치 생성 x
                    // enemymanager에서 사망처리 -> object pooling 다시 pool안으로 넣기
                    break;
                }
            case "PlayerWeapon":
                {
                    // other에서 getcomponent<weapon> 해서 무기 정보 갖고와야함
                    // onHit(other.gameObject.GetComponent<Weapon>());
                    // 피격판정
                    break;
                }
            default:
                break;

        }
    }

    public override void Trigger_Function(Collider2D other)
    {
        base.Trigger_Function(other);
        //Debug.Log($"Enemy trigger {other.transform.tag}");
        switch (other.transform.tag)
        {
            case "Player":
                {
                    // 적 사망
                    // 경험치 생성 x
                    // enemymanager에서 사망처리 -> object pooling 다시 pool안으로 넣기
                    break;
                }
            case "PlayerWeapon":
                {
                    // other에서 getcomponent<weapon> 해서 무기 정보 갖고와야함
                    // onHit(other.gameObject.GetComponent<Weapon>());
                    // 피격판정
                    break;
                }
            default:
                break;
        }
    }

    Coroutine knockbackCoroutine;
    Coroutine stunCoroutine;

    protected void ShowDamage(float damage)
    {
        GameObject num = ObjectPoolManager.Instance.GetFloatingDmg(Define.FloatingDmg.FloatingDmg);
        num.transform.position = transform.position;
        num.transform.GetChild(0).GetComponent<TextMesh>().text = string.Format("{0}", (int)damage);
    }

    public virtual void GetHit(float damage, Vector2 force,bool ignore_defence ,float knockbackTime = 0.07f, float stunTime = 1.5f)
    {
        animat.SetTrigger("gethit");
        //Debug.Log($"{gameObject.name} hit , Damage = {damage}");
        if (ignore_defence)
        {
            HP -= damage;
        }
        else
        {
            if (damage - defense <= 1)
            {
                HP -= 1;
                ShowDamage(1);
            }
            else
            {
                HP -= ((int)damage - defense);
                ShowDamage((int)damage - defense);
            }
        }
        

        //GameObject num = Instantiate(floatingnum, transform.position, Quaternion.identity) as GameObject;


        if (HP <= 0)
        {
            if(!beingDied)
            {
                beingDied = true;
                //죽을때 콜라이더 비활성화
                this.GetComponent<Collider2D>().enabled = false;
                transform.GetChild(1).gameObject.SetActive(false);
                animat.SetTrigger("die"); //사망 애니메이션 트리거
                Invoke("die", 1f);
            }
        }
        else
        {
            if (knockedback)
            {
                knockedback = false;
                if (knockbackCoroutine != null)
                    StopCoroutine(knockbackCoroutine);
                knockbackCoroutine = StartCoroutine(KnockBack(force, knockbackTime));
            }
            if (stunned)
            {
                stunned = false;
                if (stunCoroutine != null)
                    StopCoroutine(stunCoroutine);
                stunCoroutine = StartCoroutine(Stun(stunTime));
            }
        }
    }
    
    IEnumerator Stun(float stunTime)
    {
        rb.velocity = Vector2.zero;
        beingStunned = true;
        yield return new WaitForSeconds(stunTime);
        beingStunned = false;
        yield return null;
    }

    IEnumerator KnockBack(Vector2 force, float knockbackTime)
    {
        rb.velocity = Vector2.zero;
        beingKnockedback = true;
        rb.AddForce(force, ForceMode2D.Impulse);
        rb.mass *= 1000;
        yield return new WaitForSeconds(knockbackTime);
        beingKnockedback = false;
        rb.mass /= 1000;
        yield return null;
    }

    public Coroutine PosionedCoroutine = null;
    public IEnumerator Poisoned(float damage)
    {
        poisonStack++;
        while (this.enabled)
        {
            HP -= damage + poisonStack;
            ShowDamage(damage + poisonStack);
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator Waitfordeath()
    {
        yield return new WaitForSeconds(5f);
    }

    [SerializeField]
    private GameObject expball;
    protected virtual void die()
    {
        //죽은 적의 종류에따라서 인구수 차감 다름
        if(gameObject.name == "sword_infantry")
            StageManager.Instance.GetEnemyManager().setenmeyNum(1);
        else if(gameObject.name == "Normal Enemy")
            StageManager.Instance.GetEnemyManager().setenmeyNum(2);
        else if(gameObject.name == "healer")
            StageManager.Instance.GetEnemyManager().setenmeyNum(3);
        else if (gameObject.name == "ninja")
            StageManager.Instance.GetEnemyManager().setenmeyNum(4);
        else if (gameObject.name == "Bow_Boss" || gameObject.name == "spear_Boss" || gameObject.name == "Chair_Boss" || gameObject.name == "Sword_Boss")
        {
            Instantiate(box);
            StageManager.Instance.GetEnemyManager().setenmeyNum(5);
        }
            
        // virtual로 한 이유는 boss랑 일반몹이랑 처리가 다르게 해야해서
        this.GetComponent<Collider2D>().enabled = true;
        transform.GetChild(1).gameObject.SetActive(true);
        // 경험치 구슬 생성 or 아이템/스킬레벨업 생성
        //Instantiate(expball, transform.position, Quaternion.identity);
        if (enemyName != Define.Enemy.Arrows)//화살은 경험치 만들지 않음
        {
            GameObject obj = ObjectPoolManager.Instance.GetExpball(Define.Exp.SmallExp);
            obj.transform.position = transform.position;
            StageManager.Instance.kill += 1;
        }
        


        // enemymanager에서 사망처리 -> object pooling 다시 pool안으로 넣기
        StageManager.Instance.GetEnemyManager().getEnemies().Remove(this);
        HP = maxHP;
        ObjectPoolManager.Instance.ReturnEnemy(this.gameObject);
    }
    //일정범위 벗어난적 위치 재조정
    public void position_resetting(Vector3 myPos, Vector3 playerPos)
    {
        float range = cam.orthographicSize/5.4f*11;
        Vector3 vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(range, range+0.5f));
        transform.position = playerPos + vec;
    }
}
