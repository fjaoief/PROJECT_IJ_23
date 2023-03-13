using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hwando : Weapon
{
    // 사거리 내에서 적 위치에 십자가 모양의 참격 이펙트

    protected float nextWeaponDistance = 0.5f; // 연속하여 나오는 무기 거리
    public LayerMask EnemyLayer;
    [SerializeField]
    float radius = 5f; // 적 감지 거리

    protected int count = 3;

    protected override void Awake() 
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isFirstWeapon = false;
        nextWeaponDistance = 0.5f;
        soundplayer.PlayOneShot(startsound);
    }

    private void FixedUpdate()
    {
        if (isFirstWeapon)
        {
            if (count == 0)
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            Duration_Effect -= Time.fixedDeltaTime;
            if (Duration_Effect <= 0)
            {
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
            }
        }
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);

        spriteRenderer.enabled = false;
        col.enabled = false;
        count = 3;

        RaycastHit2D hit;
        List<Transform> targetTransforms = new List<Transform>();

        // 주변에 적이 있는지 확인
        for (int j = 0; j < 20; j++)
        {
            //Debug.DrawRay(pos, new Vector3(Mathf.Cos(Mathf.PI * j / 10), Mathf.Sin(Mathf.PI * j / 10)) * distance, Color.red);
            hit = Physics2D.Raycast(pos, new Vector3(Mathf.Cos(Mathf.PI * j / 10), Mathf.Sin(Mathf.PI * j / 10)), radius, EnemyLayer);
            if (hit)
            {
                targetTransforms.Add(hit.transform);
            }
        }

        // 적 발견 성공
        if (targetTransforms.Count > 0)
        {
            int length = targetTransforms.Count;
            // 가까운 적 최대 3개 선택
            int maxNum = length < 3 ? length : 3;
            // 랜덤한 적 transform 선택
            Random.InitState(System.DateTime.Now.Millisecond);
            int num = Random.Range(0, maxNum);

            transform.position = targetTransforms.OrderBy(t => (t.position - transform.position).sqrMagnitude)
                                .Take(maxNum)
                                .ToArray()[num].position;

            // 공격 시 숙련도 경험치 추가
            weaponManager.playerCharacters[weaponInfoCopy.charIndex].GainMasteryExp(masteryName, 10);
            for (int i = 0; i < 3; i++)
            {
                Invoke("CreateExtraWeapon", 0.15f);
            }
        }
    }

    protected virtual void CreateExtraWeapon()
    {
        //if (weaponManager.currentWeaponInfo[Define.Weapon.환도][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] == null) return;
        GameObject newHwando = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.환도);
        weaponManager.UpdateWeapon(weaponInfoCopy, newHwando.GetComponent<Hwando>());
        newHwando.GetComponent<Hwando>().SetExtraWeapon(transform.position + new Vector3(nextWeaponDistance, 0, 0));
        newHwando.GetComponent<Hwando>().playerTransform = playerTransform;
        nextWeaponDistance += nextWeaponDistance;
    }

    public void SetExtraWeapon(Vector3 pos)
    {
        spriteRenderer.enabled = true;
        col.enabled = true;

        transform.position = pos;
        transform.localScale = Vector3.one * Size;
        transform.rotation = SetDirection();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (Duration_Effect < Duration_noAttack) return; // 공격 x, 이펙트 용

        base.OnTriggerEnter2D(other);
    }
}
