using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healer : Enemy
{
    protected override void Start()
    {
        timercheck = StageManager.Instance.GetComponent<Timer>().GetPlayTime();
        base.Start();
    }
    public override void Fixed_Update()
    {
        if (player != null)
        {
            Vector3 playerPos = player.transform.position;
            if (!beingDied)
            {
                if (StageManager.Instance.GetComponent<Timer>().GetPlayTime() - timercheck >= 10)
                {
                    timercheck += 10;//회복 시작 시점부터 시간 카운트
                    Collider2D[] HealTarget = Physics2D.OverlapCircleAll(transform.position, 2f);
                    for(int i=0;i<HealTarget.Length;i++)
                    {
                        if (HealTarget[i].gameObject.tag == "Enemy" && HealTarget[i].gameObject.GetComponent<Enemy>().enemyName != Define.Enemy.Arrows)
                        {
                            Enemy enemy = HealTarget[i].gameObject.GetComponent<Enemy>();
                            enemy.StartCoroutine(enemy.heal(0.3f, 5));
                        }
                            
                            //HealTarget[i].gameObject.GetComponent<Enemy>().setHP(1);
                    }
                }
                if (!beingKnockedback)
                {
                    if (!beingStunned)
                    {
                        if (Vector3.Distance(transform.position, playerPos) > 5f)
                            rb.velocity = (playerPos - transform.position).normalized * Speed;
                        else
                            rb.velocity = Vector2.zero;
                    }
                    else
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
