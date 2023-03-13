using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seokgoong : Weapon
{
    public LayerMask EnemyLayer;
    [SerializeField]
    float radius = 7f; // 적 감지 거리

    protected override void OnEnable()
    {
        base.OnEnable();
        soundplayer.PlayOneShot(startsound);
    }

    private void FixedUpdate()
    {
        camHeight = _camera.orthographicSize;
        camWidth = camHeight * _camera.aspect;

        if (CheckIfOutofScreen() || PenetrationCount <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            transform.position += Time.fixedDeltaTime * Speed * transform.right;
        }
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);

        RaycastHit2D hit;
        Transform target_Pos = null;

        // 주변에 적이 있는지 확인
        for (int j = 0; j < 20; j++)
        {
            hit = Physics2D.Raycast(pos, new Vector3(Mathf.Cos(Mathf.PI * j / 10), Mathf.Sin(Mathf.PI * j / 10)), radius, EnemyLayer);
            if (hit)
            {
                target_Pos = hit.transform;
                break;
            }
        }

        // 적 발견 성공
        if (target_Pos != null)
        {
            Vector3 rand = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            _direction = target_Pos.position - transform.position + rand;
            transform.rotation = Quaternion.FromToRotation(Vector3.right, Direction);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "Enemy")
            PenetrationCount--;
    }
}
