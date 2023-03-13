using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeokgoongField : Weapon
{
    [SerializeField]
    float attackInterval = 0.5f;
    bool canAttack = false;
    float time = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(OnEnableAttack());
        canAttack = false;
    }

    private void FixedUpdate()
    {
        Duration -= Time.fixedDeltaTime;
        if (Duration <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }

        else
        {
            time += Time.fixedDeltaTime;
            if (time >= attackInterval)
            {
                time = 0;
                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator OnEnableAttack()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }
    IEnumerator Attack()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canAttack == false) return;

        base.OnTriggerEnter2D(other);
    }
}
