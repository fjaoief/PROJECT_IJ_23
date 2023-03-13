using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearUpgraded : Spear
{
    bool attacked = false;
    [SerializeField]
    float attackInterval = 1f;
    bool canAttack = false;
    float time = 0; 

    protected override void OnEnable()
    {
        base.OnEnable();
        attacked = false;
        canAttack = false;
    }
    
    protected override void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > attackInterval)
        {
            time = 0;
            StartCoroutine(Attack());
        }
        Duration -= Time.deltaTime;
        if (Duration < -12f)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        if (attacked) return;
       
        
        camHeight = _camera.orthographicSize;
        camWidth = camHeight * _camera.aspect;

        if (Duration < 0)
        {
            attacked = true;
           
        }

        transform.position += direction.normalized * Time.deltaTime * Speed;
    }

    IEnumerator Attack()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canAttack == false) return;

        base.OnTriggerEnter2D(other);
    }

}
