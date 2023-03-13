using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hyeonja : Weapon
{
    float nextWeaponAngle = 15;
    float nextWeaponTime = 0.1f;

    public LayerMask EnemyLayer;
    [SerializeField]
    float radius = 5f; // 적 감지 거리

    protected override void OnEnable()
    {
        base.OnEnable();
        soundplayer.PlayOneShot(startsound);
    }
    protected virtual void FixedUpdate() {
        Duration -= Time.fixedDeltaTime;
        if (Duration <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            transform.position += Time.fixedDeltaTime * Speed * transform.up;
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

            Transform targetTrans = targetTransforms.OrderBy(t => (t.position - transform.position).sqrMagnitude)
                                .Take(maxNum)
                                .ToArray()[num];

            _direction = targetTrans.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, Direction);
        }


        for (int i = 0; i < _projectilePercentage - 1; i++)
        {
            CreateExtraWeapon(pos, nextWeaponAngle * (i + 1));
        }
        
        // StartCoroutine(HyeonjaAttack(pos));
    }

    IEnumerator HyeonjaAttack(Vector3 pos)
    {
        
        CreateExtraWeapon(pos, nextWeaponAngle);
        CreateExtraWeapon(pos, -nextWeaponAngle);
        yield return new WaitForSeconds(nextWeaponTime);

        // for (int j = 0; j < 2; j++)
        // {
        //     float tempAngle = nextWeaponAngle;
        //     for (int i = 0; i < 3; i++)
        //     {
        //         CreateExtraWeapon(pos, tempAngle);
        //         tempAngle -= nextWeaponAngle;
        //     }
        //     yield return new WaitForSeconds(nextWeaponTime);
        // }
        yield return null;
    }

    protected virtual void CreateExtraWeapon(Vector3 pos, float angle)
    {
        Hyeonja newHyeonja = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.현자총통).GetComponent<Hyeonja>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newHyeonja);
    
        newHyeonja.SetExtraWeapon(pos, angle + transform.rotation.eulerAngles.z);
        newHyeonja.playerTransform = playerTransform;
        
    }

    public void SetExtraWeapon(Vector3 pos, float angle)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = Vector3.one * Size;
    }
}
