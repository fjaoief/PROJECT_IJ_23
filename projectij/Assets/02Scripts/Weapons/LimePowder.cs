using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimePowder : Weapon
{
    [SerializeField]
    float speedDown = 0.5f;
    [SerializeField]
    protected float attackInterval = 0.5f;
    bool canAttack = false;
    protected float time = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(OnEnableAttack());
    }

    protected virtual void FixedUpdate()
    {
        Duration -= Time.fixedDeltaTime;
        if (Duration <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }

        time += Time.fixedDeltaTime;
        if (time >= attackInterval)
        {
            time = 0;
            StartCoroutine(Attack());
        }
    }
    IEnumerator OnEnableAttack()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }
    protected IEnumerator Attack()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (canAttack == false) return;

        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
        {
            EnemyTrigger(other);
            Enemy enemy = other.GetComponent<Enemy>();
            // 첫 타격
            if (enemy.isSlow == false)
            {
                enemy.isSlow = true;
                enemy.Speed *= speedDown;
                float damageOption = 0;
                if (weaponManager.extraCharStat.ContainsKey(Define.CharacterStat.데미지))
                    damageOption = weaponManager.extraCharStat[Define.CharacterStat.데미지];
                enemy.defense -= Mathf.Pow(2, weaponInfoCopy.rarity - 1) + damageOption;
            }
        }
        else if(other.gameObject.tag == "RandObject")
        {
            other.GetComponent<RandObject>().GetDistroy();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.isSlow == true)
            {
                enemy.isSlow = false;
                enemy.Speed = enemy.maxSpeed;
            }
        }
    }
}
