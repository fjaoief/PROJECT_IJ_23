using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    protected Vector3 direction;

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    protected virtual void FixedUpdate() {
        Duration -= Time.fixedDeltaTime;

        camHeight = _camera.orthographicSize;
        camWidth = camHeight * _camera.aspect;
        
        if (Duration <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }

        transform.position += direction.normalized * Time.fixedDeltaTime * Speed;
    }

    private void LateUpdate() 
    {
        if (transform.position.x > _camera.transform.position.x + camWidth) // 우
        {
            direction.x = -Random.Range(1f, camWidth);
            direction.y = (direction.y >= 0 ? 1 : -1) * Random.Range(1f, camHeight);
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }

        else if (transform.position.x < _camera.transform.position.x - camWidth) // 좌
        {
            direction.x = Random.Range(1f, camWidth);
            direction.y = (direction.y >= 0 ? 1 : -1) * Random.Range(1f, camHeight);
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }

        else if (transform.position.y > _camera.transform.position.y + camHeight) // 상
        {
            direction.y = -Random.Range(1f, camWidth);
            direction.x = (direction.x >= 0 ? 1 : -1) * Random.Range(1f, camHeight);
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }

        else if (transform.position.y < _camera.transform.position.y - camHeight) // 하
        {
            direction.y = Random.Range(1f, camWidth);
            direction.x = (direction.x >= 0 ? 1 : -1) * Random.Range(1f, camHeight);
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
    }   
}
