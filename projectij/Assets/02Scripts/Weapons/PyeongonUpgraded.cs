using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyeongonUpgraded : Pyeongon
{
    bool attacked = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        attacked = false;
    }
    protected override void FixedUpdate()
    {
        if (attacked) return;

        Duration -= Time.fixedDeltaTime;
        transform.position = playerTransform.position;
        if (Duration >= 0) // 빙글빙글
        {
            rotation -= Time.fixedDeltaTime * Speed;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
        else // 지속시간 끝
        {
            attacked = true;
            StartCoroutine(PyeongonAttack());
        }
    }
    IEnumerator PyeongonAttack()
    {
        float time = 0;
        // 무기 투척
        rotation -= 0.01f * Speed;
        Vector3 dir = new Vector3(Mathf.Cos(rotation / 180 * Mathf.PI), Mathf.Sin(rotation / 180 * Mathf.PI), 0) * Speed / 1300;
        while (time < 0.2f)
        {
            time += Time.deltaTime;
            transform.Translate(dir);
            yield return null;
        }

        if (weaponInfoCopy.isUpgradedByFlag || weaponManager.currentWeaponInfo[Define.Weapon.마상편곤연계][weaponInfoCopy.rarity][weaponInfoCopy.weaponIndex] != null)
        {
            Weapon newPyeongon = weaponManager.SpawnWeapon(Define.Weapon.마상편곤연계, weaponInfoCopy);
            newPyeongon.SetWeapon(weaponInfoCopy, playerTransform, transform.GetChild(0).transform.position);
        }
        ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
    }
}