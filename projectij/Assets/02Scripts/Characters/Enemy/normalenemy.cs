using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalenemy : Enemy
{
    public override void Fixed_Update()
    {
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            //float step = Time.deltaTime * Speed;
            //transform.position = Vector2.MoveTowards(transform.position, playerPos, step);
            if (!beingDied)//죽는 애니메이션 실행중 아닐때
            {
                if (!beingKnockedback)
                {
                    if (!beingStunned) // 원래 이동 방식
                    {
                        rb.velocity = (playerPos - transform.position).normalized * Speed;
                    }
                    else // 스턴 걸림
                        rb.velocity = Vector2.zero;
                }
                if (playerPos.x > transform.position.x && spriteRenderer.flipX == false)
                {
                    spriteRenderer.flipX = true;
                    transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
                    transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(-0.132f, transform.GetChild(0).GetChild(0).transform.localPosition.y, 0);
                    transform.GetChild(1).GetComponent<BoxCollider2D>().offset = new Vector2(1.95f, 0.4850166f);
                }
                else if (playerPos.x < transform.position.x && spriteRenderer.flipX == true)
                {
                    spriteRenderer.flipX = false;
                    transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                    transform.GetChild(0).GetChild(0).transform.localPosition = new Vector3(0.132f, transform.GetChild(0).GetChild(0).transform.localPosition.y, 0);
                    transform.GetChild(1).GetComponent<BoxCollider2D>().offset = new Vector2(-1.556638f, 0.4850166f);
                }
                if (Vector3.Distance(transform.position, playerPos) > cam.orthographicSize / 5.4f * 11 + 4)
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
