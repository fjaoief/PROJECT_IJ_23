using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HwachaUpgraded : Hwacha
{
    float invincibleTime = 3f;

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        // 플레이어 무적
        PlayerCharacters playerCharacter = StageManager.Instance.playerCharacters[weaponInfoCopy.charIndex];
        if (playerCharacter.InvincibleCoroutine != null)
            playerCharacter.StopCoroutine(playerCharacter.InvincibleCoroutine);
        playerCharacter.InvincibleCoroutine = playerCharacter.StartCoroutine(playerCharacter.BeingInvincible(invincibleTime, true));

        base.SetWeapon(weaponInfo, playerTrans, pos);
        spriteRenderer.enabled = false;
        col.enabled = false;
        count = 12;

        rb.velocity = Vector2.zero;
        StartCoroutine(HwachaAttack(pos));
    }

    protected override void CreateExtraWeapon(Vector3 pos, float angle)
    {
        HwachaUpgraded newHwacha = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.변이중화차).GetComponent<HwachaUpgraded>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newHwacha);
        newHwacha.SetExtraWeapon(pos, angle);
        newHwacha.playerTransform = playerTransform;
        newHwacha.rb.velocity = Vector2.zero;
        newHwacha.rb.AddForce(newHwacha.transform.right * Speed, ForceMode2D.Impulse);
        count--;
    }
}
