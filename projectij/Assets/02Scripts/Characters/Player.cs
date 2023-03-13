using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayerCharacters
{
    private Vector3 map_min, map_max;
    public float CriticalPercentage;
    public float CriticalDamage;

    public GameObject boundingbox; // player y축 한계 지정할 때 필요한 경계

    protected new void Awake()
    {
        base.Awake();
    }

    public override void Fixed_Update()
    {
        base.Fixed_Update();
        Vector3 moveVec = new Vector3(h, v, 0).normalized;
        transform.position += moveVec * characterStat[Define.CharacterStat.이동속도] * Time.deltaTime;

        float yClamp = Mathf.Clamp(transform.position.y, map_min.y, map_max.y - 1); // y축 한계 지정
        transform.position = new Vector3(transform.position.x, yClamp, 0);
        boundingbox.transform.position = new Vector3(transform.position.x, boundingbox.transform.position.y, 0); // player와 함께 x축 이동

    }

    public override void Collision_Function(Collision2D other)
    {
        switch (other.transform.tag)
        {
            // 기획 보고 더 추가해야할수도
            case "Enemy":
            case "EnemyWeapon":
            case "Boss":
                break;
            case "ExpBall":
            // 팀 경험치 추가 함수 호출
            case "Item":
            case "Portal":
                break;
            default:
                break;
        }
    }

    public override void Trigger_Function(Collider2D other)
    {
        switch (other.transform.tag)
        {
            case "Enemy":
            case "EnemyWeapon":
            case "Boss":
                break;
            case "ExpBall":
            // 팀 경험치 추가 함수 호출
            case "Item":
            case "Portal":
                break;
            // 맵 오브젝트 투명화
            case "Object":
                break;
            default:
                break;
        }
    }

    private void GetExp()
    {
    }

    // 플레이어 y축 이동 범위 지정 (맵 밖으로 못 나가게)
    public void SetLimits(Vector3 min, Vector3 max)
    {
        this.map_min = min;
        this.map_max = max;
    }


}
