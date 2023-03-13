using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBall : MonoBehaviour
{
    public Define.Exp ExpName;
    public int ExpAmount = 1;

    public IEnumerator MagnetOn(Transform characterTrans)
    {
        while (this.enabled)
        {
            transform.Translate((characterTrans.position - transform.position) * Time.fixedDeltaTime * 1.5f);
            yield return null;
        }
    }
    
    // 경험치 획득: 신장대 효과를 위해 플레이어캐릭터 스크립트로 이사갔어요
}
