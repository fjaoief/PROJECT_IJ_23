using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bow_boss : Enemy
{
    int arrow_create;

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
            if (!beingDied)//죽는 애니메이션 실행중 아닐때
            {
                if (StageManager.Instance.GetComponent<Timer>().GetPlayTime() - timercheck >= 1)//보스 생존시 1초지날때마다 최대인구증가
                {
                    StageManager.Instance.MAXenemy += 1;
                    timercheck = StageManager.Instance.GetComponent<Timer>().GetPlayTime();
                    arrow_create += 1;
                }
                if(arrow_create == 5)
                {
                    float range = cam.orthographicSize / 5.4f * 11;
                    Vector3 vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(range, range + 4));

                    GameObject obj = ObjectPoolManager.Instance.GetEnemy(Define.Enemy.Arrows);
                    obj.transform.position = playerPos + vec;
                    StageManager.Instance.GetEnemyManager().getEnemies().Add(obj.GetComponent<Enemy>());
                    Vector2 velocity = (playerPos - obj.transform.position).normalized * obj.GetComponent<Enemy>().Speed;
                    obj.GetComponent<Rigidbody2D>().velocity = velocity;

                    for (int i =0;i<10;i++)
                    {
                        Vector3 rand_vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * 1f);
                        obj = ObjectPoolManager.Instance.GetEnemy(Define.Enemy.Arrows);
                        obj.transform.position = playerPos + vec + rand_vec;
                        obj.GetComponent<Rigidbody2D>().velocity = velocity;
                        StageManager.Instance.GetEnemyManager().getEnemies().Add(obj.GetComponent<Enemy>());
                    }
                    arrow_create = 0;
                }
                if (!beingKnockedback)
                {
                    if (!beingStunned) // 원래 이동 방식
                    {
                        if (Vector3.Distance(transform.position, playerPos) > 5f)
                            rb.velocity = (playerPos - transform.position).normalized * Speed;
                        else
                            rb.velocity = Vector2.zero;
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
