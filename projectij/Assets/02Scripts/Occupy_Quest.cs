using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occupy_Quest : MonoBehaviour
{
    bool LastFrame, CurFrame;
    float lasttime, curtime;
    Collider2D enemey;
    public Sprite[] area_img = new Sprite[2];

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StageManager.Instance.player_num++;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StageManager.Instance.player_num--;
    }
    private void FixedUpdate()
    {
        if(StageManager.Instance.player_num>0)
        {
            curtime = StageManager.Instance.GetTimer().GetPlayTime();
            CurFrame = true;
            enemey = Physics2D.OverlapBox(transform.position + new Vector3(0, 0.4f, 0), new Vector2(4 * 0.9f, 2 * 0.7f), 0, 1 << 6);
            if (enemey == null)
            {
                this.GetComponent<SpriteRenderer>().sprite = area_img[1];
                if (LastFrame && CurFrame)//지난 프레임 O, 이번프레임 O
                {
                    //curtime = StageManager.Instance.GetTimer().GetPlayTime();
                    StageManager.Instance.occupied_percentage += curtime - lasttime;
                    LastFrame = true;
                    lasttime = curtime;
                    if (StageManager.Instance.occupied_percentage == StageManager.Instance.percentage_needed)// 점령 퀘스트 조건 달성
                    {
                        StageManager.Instance.quest_clear += 1;

                    }
                }
                else if (!LastFrame && CurFrame)//지난 프레임 X, 이번프레임 O
                {
                    lasttime = curtime;
                    LastFrame = true;
                }
                CurFrame = false;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().sprite = area_img[0];
                Debug.Log("적침입중");//Break();
            }
                
        }
        else//지난 프레임 O, 이번프레임 X
        {
            this.GetComponent<SpriteRenderer>().sprite = area_img[0];
            LastFrame = false;
        }
            
    }
    /*private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("호출");
        if (collision.tag == "Player")
        {
            curtime = StageManager.Instance.GetTimer().GetPlayTime();
            enemey = Physics2D.OverlapBox(transform.position + new Vector3(0, 0.4f, 0), new Vector2(4*0.9f, 2*0.7f), 0, 1<<6);
            if(enemey == null)
            {
                CurFrame = true;
                if (LastFrame && CurFrame)//지난 프레임 O, 이번프레임 O
                {
                    //curtime = StageManager.Instance.GetTimer().GetPlayTime();
                    StageManager.Instance.occupied_percentage += curtime - lasttime;
                    LastFrame = true;
                    lasttime = curtime;
                }
                else if (LastFrame && !CurFrame)//지난 프레임 O, 이번프레임 X
                {
                    LastFrame = false;
                }
                else if (!LastFrame && CurFrame)//지난 프레임 X, 이번프레임 O
                {
                    lasttime = StageManager.Instance.GetTimer().GetPlayTime();
                    LastFrame = true;
                }
                
                CurFrame = false;
            }
            else
                Debug.Break();
            enemey = null;
        }
    }*/
    /*private void OnDrawGizmos()
    {
        RaycastHit hit;
        // Physics.BoxCast (레이저를 발사할 위치, 사각형의 각 좌표의 절판 크기, 발사 방향, 충돌 결과, 회전 각도, 최대 거리)
        //bool isHit = Physics.BoxCast(transform.position, transform.lossyScale / 2, transform.forward, out hit, transform.rotation, maxDistance);

        Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
        Gizmos.DrawWireCube(transform.position+new Vector3(0,0.4f,0), new Vector3(4*0.9f,2*0.7f,1));
    }*/
}
