using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ninja : Enemy
{
    Vector3 playerPos, startPos;
    protected override void Start()
    {
        base.Start();
        playerPos = player.transform.position;
        startPos = transform.position;
        if (playerPos.x > transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }
    public override void Fixed_Update()
    {
        if (player != null)
        {
            if (!beingDied)//죽는 애니메이션 실행중 아닐때
            {
                if (!beingKnockedback)
                {
                    if (!beingStunned) // 원래 이동 방식
                        rb.velocity = (playerPos - startPos).normalized * Speed;
                    else // 스턴 걸림
                        rb.velocity = Vector2.zero;
                }
                if (Vector3.Distance(transform.position, playerPos) > cam.orthographicSize / 5.4f * 11 + 4)
                {
                    playerPos = player.transform.position;
                    position_resetting(transform.position, playerPos);
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
    public new void position_resetting(Vector3 myPos, Vector3 pPos)
    {
        float range = cam.orthographicSize / 5.4f * 11;
        Vector3 vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(range, range + 4));
        transform.position = pPos + vec;
        playerPos = player.transform.position;
        startPos = transform.position;
        if (playerPos.x > transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }
}
