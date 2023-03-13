using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JapanSword : Weapon
{
    [SerializeField]
    float attackDelay = 1f; // 시전 시간
    protected bool attacked = false;
    
    protected (float, float) lastDir = (2, 2);
    protected int count;

    protected override void OnEnable() {
        base.OnEnable();
        attacked = false;
        isFirstWeapon = false;
        lastDir = (2, 2);
        //soundplayer.PlayOneShot(startsound);
    }
    
    protected virtual void FixedUpdate()
    {
        if (isFirstWeapon)
        {
            if (StageManager.Instance.inputDir != (0, 0))
            {
                lastDir = StageManager.Instance.inputDir;

                if (weaponManager.currentWeaponInfo[Define.Weapon.왜검][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
                    weaponManager.currentWeaponInfo[Define.Weapon.왜검][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex].coolTime += Time.fixedDeltaTime;
            }

            if (attacked)
            {
                if (count <= 0)
                {
                    ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
                }
            }
        }
        else
        {
            Duration_Effect -= Time.fixedDeltaTime;
            transform.position += Time.fixedDeltaTime * Speed * transform.right;
            if (Duration_Effect <= 0)
            {
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
            }
        }
        
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        transform.localScale = Vector3.zero;
        count = (int)_projectilePercentage;
        StartCoroutine(Attack(playerTrans, pos));
    }

    IEnumerator Attack(Transform playerTrans, Vector3 pos)
    {
        while (StageManager.Instance.inputDir != (0, 0)) yield return null; // 정지할 때까지 기다림

        // 공격 시 숙련도 경험치 추가
        weaponManager.playerCharacters[weaponInfoCopy.charIndex].GainMasteryExp(masteryName, 10);

        attacked = true;
        
        // 추가투사체개수만큼 무기 생성
        for (int i = 0; i < (int)_projectilePercentage; i++)
        {
            CreateExtraWeapon(playerTransform, lastDir);
            yield return new WaitForSeconds(0.11f);
        }
    }

    protected virtual void CreateExtraWeapon(Transform playerTransform, (float, float) dir)
    {
        JapanSword newSword = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.왜검).GetComponent<JapanSword>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newSword);
        newSword.SetExtraWeapon(playerTransform, dir);
        newSword.playerTransform = playerTransform;
        count--;
    }

    public void SetExtraWeapon(Transform playerTransform, (float, float) dir)
    {
        transform.position = playerTransform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(dir.Item1, dir.Item2, 0));
        transform.localScale = Vector3.one * Size;
    }
}
