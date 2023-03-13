using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class follower : PlayerCharacters
{
    public float CriticalPercentage;
    public float CriticalDamage;

    float f1_sin, f1_cos, f2_sin, f2_cos;
    [SerializeField]
    Vector3 pos; // 플레이어를 기준으로 하는 상대적 위치
    [SerializeField]
    Vector3 realPos; // 목적지
    [SerializeField]
    float distance; // 이동속도 보정을 위한 player와의 거리

    // 이동 방식
    // 플레이어를 기준으로 120도(follower1) / -120도(follower2) 회전한 위치좌표를 구하고 그곳으로 이동

    private new void Awake()
    {
        base.Awake();
        f1_sin = (float)Math.Sin(Math.PI * 2 / 3);
        f1_cos = (float)Math.Cos(Math.PI * 2 / 3);
        f2_sin = (float)Math.Sin(-Math.PI * 2 / 3);
        f2_cos = (float)Math.Cos(-Math.PI * 2 / 3);
    }

    public void Set_Follower1_Pos((float, float) MoveDirection, Vector3 playerPos)
    {
        Vector3 moveVec = new Vector3(MoveDirection.Item1, MoveDirection.Item2, 0).normalized * characterStat[Define.CharacterStat.size];
        h = moveVec.x;
        v = moveVec.y;
        if (h == 0 && v == 0) return;
        pos = new Vector3(h * f1_cos - v * f1_sin, h * f1_sin + v * f1_cos, 0);
        realPos = playerPos - moveVec + pos;
        distance = Vector3.Distance(transform.position, realPos);
    }

    public void Set_Follwer2_Pos((float, float) MoveDirection, Vector3 playerPos)
    {
        Vector3 moveVec = new Vector3(MoveDirection.Item1, MoveDirection.Item2, 0).normalized * characterStat[Define.CharacterStat.size];
        h = moveVec.x;
        v = moveVec.y;
        if (h == 0 && v == 0) return;
        pos = new Vector3(h * f2_cos - v * f2_sin, h * f2_sin + v * f2_cos, 0);
        realPos = playerPos - moveVec + pos;
        distance = Vector3.Distance(transform.position, realPos);
    }

    public override void Fixed_Update()
    {
        base.Fixed_Update();
        transform.position = Vector3.MoveTowards(transform.position, realPos, Time.deltaTime * characterStat[Define.CharacterStat.이동속도] * distance);
        /*
        if (FollowType)
        {
            transform.position = follow_target.position + follow_position;
        }
        else
        {
            if (Vector3.Distance(follow_target.position, transform.position) >= follow_distance)
            {
                transform.position = Vector2.MoveTowards(transform.position, follow_target.position, Time.deltaTime * follow_speed);
            }
        }*/
    }


}
