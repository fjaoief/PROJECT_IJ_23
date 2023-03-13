using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected float _size;
    [SerializeField]
    protected float _speed;
    [SerializeField]
    protected float _force; // 넉백 정도
    [SerializeField]
    protected float _knockbackTime; // 넉백 시간
    [SerializeField]
    protected float _stunTime; // 스턴 시간
    [SerializeField]
    protected float _damage;
    [SerializeField]
    protected float _coolTime;
    [SerializeField]
    protected float _projectilePercentage = 1; // 퍼센트로 관리
    [SerializeField]
    protected int _penetrationCount = 0;
    [SerializeField]
    protected float _duration;
    [SerializeField]
    protected float _duration_effect; // 성능과는 상관 없는 이펙트를 위한 지속시간
    [SerializeField]
    protected float _duration_noAttack = 0.3f; // 이펙트가 사라지는 동안 적을 공격하지 않게 하기 위해
    [SerializeField]
    protected bool _isChained; // 연계된 무기 : true
    protected bool _reducesCoolTime; // 정지 시 쿨타임 감소

    protected Transform _targetEnemy; // 나중에 환도같은 무기에서 쓰지 않을까

    protected AudioSource soundplayer;
    [SerializeField]
    protected AudioClip startsound;

    public Define.Weapon weaponName;
    public Define.Mastery masteryName;
    public Define.Weapon upgradedWeaponName;
    public Define.Weapon chainedWeaponName;

    // get, set
    public float Size { get { return _size; } set { _size = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float Force { get { return _force; } set { _force = value; } }
    public float KnockbackTime { get { return _knockbackTime; } set { _knockbackTime = value; } }
    public float StunTime { get { return _stunTime; } set { _stunTime = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public float CoolTime { get { return _coolTime; } set { _coolTime = value; } }
    public float ProjectilePercentage { get { return _projectilePercentage; } set { _projectilePercentage = value; } }
    public int PenetrationCount { get { return _penetrationCount; } set { _penetrationCount = value; } }
    public float Duration { get { return _duration; } set { _duration = value; } }
    public float Duration_Effect { get { return _duration_effect; } set { _duration_effect = value; } }
    public float Duration_noAttack { get { return _duration_noAttack; } set { _duration_noAttack = value; } }
    public bool IsChained { get { return _isChained; } set { _isChained = value; } }
    public bool ReducesCoolTime { get { return _reducesCoolTime; } set { _reducesCoolTime = value; } }
    public Transform TargetEnemy {get { return _targetEnemy; } set { _targetEnemy = value; } }

    protected Vector2 _direction;
    public Vector2 Direction {get { return _direction; } set { _direction = value; } }

    protected int Index { get; set; }

    protected Transform playerTransform;

    public WeaponInfo weaponInfoCopy;

    public bool isFirstWeapon;
    protected Collider2D col;
    protected SpriteRenderer spriteRenderer;

    protected Camera _camera;
    protected float camHeight;
    protected float camWidth;

    protected WeaponManager weaponManager;

    public bool enemyDeath = false;

    protected virtual void Awake() {
        weaponInfoCopy = new WeaponInfo();
        soundplayer = GetComponent<AudioSource>();
        _camera = Camera.main;
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Start() 
    {
        // weaponManager = StageManager.Instance.GetWeaponManager();
    }

    protected virtual void OnEnable() 
    {
        if (weaponManager == null)
        {
            weaponManager = StageManager.Instance.GetWeaponManager();
        }
    }
    public virtual void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        Index = weaponInfo.charIndex;
        playerTransform = playerTrans;
        transform.position = pos;
        transform.rotation = SetDirection();
        //Size
        transform.localScale = Vector3.one * Size;

        int count = (int)_projectilePercentage;
        float percentage = _projectilePercentage - count;
        if (Random.Range(0f, 1f) < percentage)
            count++;
        _projectilePercentage = (float) count;
    }

    public virtual Quaternion SetDirection()
    {
        float angle = Random.Range(0, 360);
        //Debug.Log($"angle : {angle}");
        return Quaternion.Euler(0, 0, angle);
    }

    // 방어구 계열만 사용
    public virtual void PassiveOn()
    {
        
    }

    public virtual void PassiveOff()
    {

    }

    // 나중에 함수 몇 가지로 정리해서 사용하기. (넉백, 지속시간, 관통, 스턴)
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
        {
            EnemyTrigger(other);
            
            // 도검 시너지
            // 석회가루 때문에 따로 빼줌
            BladeSynergy(other.gameObject.GetComponent<Enemy>());  
        }
        else if(other.gameObject.tag == "RandObject")
        {
            other.GetComponent<RandObject>().GetDistroy();
        }
        else if (other.gameObject.tag == "BossObject")
        {
            other.GetComponent<boss_box>().GetDistroy();
        }
    }

    protected void Trigger_stun(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
        {
            other.gameObject.GetComponent<Enemy>().stunned = true;
            EnemyTrigger(other);
            BladeSynergy(other.gameObject.GetComponent<Enemy>());
        }
        else if(other.gameObject.tag == "RandObject" || other.gameObject.tag == "BossObject")
        {
            other.GetComponent<RandObject>().GetDistroy();
        }
        else if (other.gameObject.tag == "BossObject")
        {
            other.GetComponent<boss_box>().GetDistroy();
        }
    }

    protected void EnemyTrigger(Collider2D other)
    {
        float damage = Damage;
        float criticalPercentage = Index == 0 ? StageManager.Instance.player.CriticalPercentage : StageManager.Instance.followers[Index - 1].CriticalPercentage;
        float criticalDamage = Index == 0 ? StageManager.Instance.player.CriticalDamage : StageManager.Instance.followers[Index - 1].CriticalDamage;

        // 치명타 계산
        if (other.gameObject.activeSelf)
        {
            if (Random.Range(0f, 1f) < criticalPercentage)
            {
                damage *= criticalDamage;
            }
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.knockedback = true;
            Vector3 temp = other.gameObject.transform.position - playerTransform.position;
            Direction = new Vector2(temp.x, temp.y);    

            // 둔기 시너지
            if (other.gameObject.tag == "Enemy" && enemyDeath)
            {
                damage = 99999;
                Debug.Log("즉사!");
            }

            enemy.GetHit(damage, Direction.normalized * Force, false ,KnockbackTime, StunTime);

            // 검정무기
            if (weaponInfoCopy.rarity == 4)
            {
                // 타격 시 hp 회복
                StageManager.Instance.playerCharacters[Index].GainHP(10);

                // 검정무기로 적 처치 => 최대 hp 증가
                if (enemy.HP <= 0 && !enemy.absorbed)
                {
                    enemy.absorbed = true;
                    PlayerCharacters character = StageManager.Instance.playerCharacters[weaponInfoCopy.charIndex];
                    character.PureMaxHP += 1;
                }

            }
        }
    }
    
    void BladeSynergy(Enemy enemy)
    {
        if (weaponManager.synergyOn[0])
        {
            if (enemy.PosionedCoroutine != null)
                enemy.StopCoroutine(enemy.PosionedCoroutine);
            enemy.PosionedCoroutine = enemy.StartCoroutine(enemy.Poisoned(weaponManager.masteryWeaponCount[(int)Define.Mastery.도검] - 3));
        }    
    }

    int padding = 1;
    protected bool CheckIfOutofScreen()
    {
        if (transform.position.x > _camera.transform.position.x + camWidth + padding || transform.position.x < _camera.transform.position.x - camWidth - padding
            || transform.position.y > _camera.transform.position.y + camHeight + padding || transform.position.y < _camera.transform.position.y - camHeight - padding)
            return true;
        else
            return false;
    }

}
