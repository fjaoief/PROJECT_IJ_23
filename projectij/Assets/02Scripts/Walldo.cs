using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walldo : Weapon
{
    // protected new IEnumerator WeaponAttack()
    // {
    //     // 월도
    //     spriteRenderer.flipX = player.toLeft;
    //     float zRotation = 0;
    //     if (player.toLeft == false){
    //         while (zRotation < 30)
    //         {
    //             zRotation += 1f;
    //             transform.rotation = Quaternion.Euler(0, 0, zRotation);
    //             yield return new WaitForSeconds(0.005f);
    //         }
    //     }
    //     else{
    //         while (zRotation > -30)
    //         {
    //             zRotation -= 1f;
    //             transform.rotation = Quaternion.Euler(0, 0, zRotation);
    //             yield return new WaitForSeconds(0.005f);
    //         }
    //     }
    //     yield return new WaitForSeconds(0.1f);
    //     ObjectPoolManager.Instance.ReturnObject(this.gameObject);
    // }
}
