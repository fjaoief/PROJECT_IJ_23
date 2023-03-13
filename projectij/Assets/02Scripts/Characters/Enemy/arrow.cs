using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : Enemy
{
    Vector3 playerPos, startPos;
    Vector2 velocity;

    protected override void Start()
    {
        base.Start();
        velocity = rb.velocity;
        playerPos = player.transform.position;
        startPos = transform.position;
        if (playerPos.x > transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        Vector3 v3 = playerPos - startPos;
        float angle = Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public override void Fixed_Update()
    {
        if (player != null)
        {
            if (!beingDied)//죽는 애니메이션 실행중 아닐때
            {
                rb.velocity = velocity;
                if (Vector3.Distance(transform.position, playerPos) > cam.orthographicSize / 5.4f * 11 + 4)
                {
                    Invoke("die", 0f);
                }
            }
            else
            {
                //rb.velocity = Vector2.zero;
            }
        }
    }

    public override void GetHit(float damage, Vector2 force, bool ignore_defence ,float knockbackTime = 0.07f, float stunTime = 1.5f)
    {
        //Debug.Log($"{gameObject.name} hit , Damage = {damage}");
        if(ignore_defence)
        {
            HP -= damage;
        }
        else
        {
            if (damage - defense <= 1)
            {
                HP -= 1;
                ShowDamage(1);
            }
            else
            {
                HP -= ((int)damage - defense);
                ShowDamage((int)damage - defense);
            }
        }
        
        //GameObject num = Instantiate(floatingnum, transform.position, Quaternion.identity) as GameObject;
        GameObject num = ObjectPoolManager.Instance.GetFloatingDmg(Define.FloatingDmg.FloatingDmg);
        num.transform.position = transform.position;
        if (damage == (int)damage)
            num.transform.GetChild(0).GetComponent<TextMesh>().text = string.Format("{0}", damage);
        else
            num.transform.GetChild(0).GetComponent<TextMesh>().text = string.Format("{0:N1}", damage);
        if (HP <= 0)
        {
            if (!beingDied)
            {
                Invoke("die", 0f);
            }
        }
    }

    public override void Trigger_Function(Collider2D other)
    {
        base.Trigger_Function(other);
        //Debug.Log($"Enemy trigger {other.transform.tag}");
        switch (other.transform.tag)
        {
            case "Player":
                {
                    //Debug.Log("플레이어관통");
                    // 적 사망
                    // 경험치 생성 x
                    // enemymanager에서 사망처리 -> object pooling 다시 pool안으로 넣기
                    break;
                }
            case "PlayerWeapon":
                {
                    // other에서 getcomponent<weapon> 해서 무기 정보 갖고와야함
                    // onHit(other.gameObject.GetComponent<Weapon>());
                    // 피격판정
                    break;
                }
            default:
                break;
        }
    }
}
