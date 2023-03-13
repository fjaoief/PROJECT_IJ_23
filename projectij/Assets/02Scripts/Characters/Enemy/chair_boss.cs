using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chair_boss : Enemy
{
    public override void Fixed_Update()
    {
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            if (!beingDied)//죽는 애니메이션 실행중 아닐때
            {
                if (StageManager.Instance.GetComponent<Timer>().GetPlayTime() - timercheck >= 1)//보스 생존시 1초지날때마다 최대인구증가
                {
                    StageManager.Instance.MAXenemy += 1;
                    timercheck += 1;
                }
                if (!beingKnockedback)
                {
                    if (!beingStunned) // 원래 이동 방식
                    {
                        rb.velocity = (playerPos - transform.position).normalized * Speed;
                    }
                    else // 스턴 걸림
                        rb.velocity = Vector2.zero;
                }
                if (playerPos.x > transform.position.x)
                {
                    spriteRenderer.flipX = true;
                    transform.GetChild(1).GetComponent<BoxCollider2D>().offset = new Vector2(1.95f, 0.4850166f);
                }
                else
                {
                    spriteRenderer.flipX = false;
                    transform.GetChild(1).GetComponent<BoxCollider2D>().offset = new Vector2(-1.556638f, 0.4850166f);
                }
                if (Vector3.Distance(transform.position, playerPos) > 15f)
                {
                    position_resetting(transform.position, playerPos);
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
}
