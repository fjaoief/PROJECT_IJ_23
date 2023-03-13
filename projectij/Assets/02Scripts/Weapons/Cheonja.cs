using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheonja : Weapon
{
    protected override void OnEnable()
    {
        base.OnEnable();
        camHeight = _camera.orthographicSize;
        camWidth = camHeight * _camera.aspect;
        soundplayer.PlayOneShot(startsound);
    }
    
    private void FixedUpdate()
    {
        Duration_Effect -= Time.fixedDeltaTime;
        if (Duration_Effect <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        float tempX = Random.Range(-camWidth, camWidth);
        float tempY = Random.Range(-camHeight, camHeight);
        transform.position = pos + new Vector3(tempX, tempY, 0);
    }

    public void SetSynergyWeapon(Vector3 pos)
    {
        float tempX = Random.Range(-camWidth, camWidth);
        float tempY = Random.Range(-camHeight, camHeight);
        transform.position = pos + new Vector3(tempX, tempY, 0);
        transform.localScale = Vector3.one * Size;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (Duration_Effect < Duration_noAttack) return; // 공격 x, 이펙트 용
        base.OnTriggerEnter2D(other);
    }
}
