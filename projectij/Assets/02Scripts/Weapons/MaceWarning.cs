using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaceWarning : Weapon
{
    public LayerMask EnemyLayer;
    [SerializeField]
    float radius = 5f; // 적 감지 거리

    protected bool attacked = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        attacked = false;
    }

    protected virtual void FixedUpdate() {
        if (attacked) return;

        Duration_Effect -= Time.fixedDeltaTime;
        if (Duration_Effect <= 0)
        {
            if (weaponManager.currentWeaponInfo[Define.Weapon.철퇴연계][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
            {
                attacked = true;
                Weapon newMace = weaponManager.SpawnWeapon(Define.Weapon.철퇴연계, weaponInfoCopy);
                newMace.SetWeapon(weaponInfoCopy, playerTransform, transform.position);
            }        
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }
    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);

        RaycastHit2D hit;
        List<Transform> targetTransforms = new List<Transform>();

        // 주변에 적이 있는지 확인
        for (int j = 0; j < 20; j++)
        {
            // Debug.DrawRay(pos, new Vector3(Mathf.Cos(Mathf.PI * j / 10), Mathf.Sin(Mathf.PI * j / 10)) * radius, Color.red, 2);
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
                                .ToArray()[num].position + new Vector3(Random.Range(0,0.5f), Random.Range(0,0.5f), 0);
        }
        else
        {
            transform.position = pos;
        }
    }

    public override Quaternion SetDirection()
    {
        return Quaternion.identity;
    }
}
