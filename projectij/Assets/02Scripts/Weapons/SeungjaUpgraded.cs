using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeungjaUpgraded : Seungja
{
    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        spriteRenderer.enabled = false;
        col.enabled = false;
        count = 12;

        rb.velocity = Vector2.zero;
        rb.AddForce(transform.right * Speed, ForceMode2D.Impulse);
        StartCoroutine(SeungjaAttack(playerTransform));
    }

    protected override void CreateExtraWeapon(Transform playerTransform, float angle)
    {
        SeungjaUpgraded newSeungja = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.승자총통_삼연자포).GetComponent<SeungjaUpgraded>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newSeungja);

        newSeungja.SetExtraWeapon(playerTransform, angle + transform.rotation.eulerAngles.z);
        newSeungja.playerTransform = playerTransform;
        newSeungja.rb.velocity = Vector2.zero;
        newSeungja.rb.AddForce(newSeungja.transform.right * Speed, ForceMode2D.Impulse);
    }
}
