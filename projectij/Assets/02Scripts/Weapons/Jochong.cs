using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jochong : Weapon
{
    [SerializeField]
    float attackDelay = 0f; // 시전 시간
    bool attacked = false;

    protected override void OnEnable() {
        base.OnEnable();
        attacked = false;
        soundplayer.PlayOneShot(startsound);
    }
    
    private void FixedUpdate() {
        if (attacked == false)
        {
            if (weaponManager.currentWeaponInfo[Define.Weapon.조총][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
                weaponManager.currentWeaponInfo[Define.Weapon.조총][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex].coolTime += Time.fixedDeltaTime;
            else
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            Duration_Effect -= Time.fixedDeltaTime;
            if (Duration_Effect <= 0)
            {
                weaponManager.currentWeaponInfo[Define.Weapon.조총][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex].coolTime += attackDelay;
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
            }
        }
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        transform.localScale = Vector3.zero;
        StartCoroutine(JochongAttack(playerTrans, pos));
    }

    IEnumerator JochongAttack(Transform playerTrans, Vector3 pos)
    {
        while (StageManager.Instance.inputDir != (0, 0)) // 정지할 때까지 기다림
            yield return null;
        yield return new WaitForSeconds(attackDelay);
        base.SetWeapon(weaponInfoCopy, playerTrans, playerTrans.transform.position); // 공격
        // 공격 시 숙련도 경험치 추가
        //if (isFirstWeapon)
        weaponManager.playerCharacters[weaponInfoCopy.charIndex].GainMasteryExp(masteryName, 10);

        attacked = true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (PenetrationCount <= 0) return;
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "Enemy")
        {
            PenetrationCount--;
        }
    }
}
