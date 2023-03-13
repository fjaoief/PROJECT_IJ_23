using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    
    private Vector3 player_position;
    [SerializeField]
    private float Play_Time = 0f;
    public float GetPlayTime()
    {
        return Play_Time;
    }
    public void SetPlayTime(float cur_Time)
    {
        Play_Time = cur_Time;
        return;
    }

    public void _Update()
    {
        SetPlayTime(Time.timeSinceLevelLoad); // scene이 load된 후를 기준으로

    }

    // TODO : Timer의 Play_Time으로 예정되어 있던 게임 내 이벤트들 ( 적 소환, 탈출구 생성 등 ) 처리
    // 이벤트 내용은 csv나 xml등에서 받아오는 게 좋을 것 같아요 만들때나 testing 할때나
    // 어떤 파일에서 언제 받아와서 어떻게 저장해놓을 지는 아직 미정이에요
}
