using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float movespeed;

    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private bool canPenetrate;

    private float spawnTime = 0f;

    public void FixedUpdate()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime > 5f){
            spawnTime = 0f;
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            // 방향 초기화 시 Y축 방향이 목표물 방향임
            transform.position += Time.deltaTime * movespeed * transform.up;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            //other.gameObject.GetComponent<Enemy>().GetHit(weapon.Damage);
            if (!canPenetrate)
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }
}
