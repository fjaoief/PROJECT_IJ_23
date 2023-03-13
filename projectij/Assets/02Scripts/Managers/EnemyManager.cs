using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // 게임 내에 존재하는 적의 최대 숫자가 정해져 있어서
    // 적들 추적하기위해 만든 manager에요
    // 자세하게 설계하진 않았는데 아직
    // 아마 Dictionary로 적들 고유번호 등의 정보를 저장할까 해요

    List<Enemy> enemies = new List<Enemy>();
    bool [] Bosstimer = new bool[5];
    bool spawn_working = true;
    int enemyNum = 0;
    float special_check = 0;
    public int[] wait_for_spawn = new int[3] { 0, 0, 10 };

    public List<Enemy> getEnemies()
    {
        return enemies;
    }

    public bool getBosstimer(int i)
    {
        return Bosstimer[i];
    }
    public void setBosstimer(int i)
    {
        Bosstimer[i] = true;
    }
    public bool getspawn_working()
    {
        return spawn_working;
    }
    public void setspawn_working(bool a)
    {
        spawn_working = a;
    }
    public int getenemyNum()
    {
        return enemyNum;
    }
    public void setenmeyNum(int a)
    {
        enemyNum -= a;
    }
    public List<Transform> enemiesPosition()
    {
        List<Transform> result = new List<Transform>();
        foreach (Enemy enemy in enemies)
        {
            result.Add(enemy.transform);
        }
        return result;
    }

    public IEnumerator SpawnEnemy(Define.Enemy name, float[] array, bool population_check, float spwan_delay = 0.3f)//array[0] : 소환할 무리수, array[1] : 무리의 일반적수, array[2] : 무리의 특별적수, array[3] : 무리의 보스의종류
    {
        spawn_working = true;
        Vector3 vec = new Vector3(0,0,0);
        wait_for_spawn[1] = (int)((StageManager.Instance.MAXenemy - enemyNum) * 0.2f);
        wait_for_spawn[0] = StageManager.Instance.MAXenemy - enemyNum - wait_for_spawn[1];
        int r = Random.Range(0, 3);
        if(array[3] == 0)
        {
            if(array[2] == 0)//일반적만
            {
                for(int i =0;i<array[0];i++)
                {
                    vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(11f, 15f));
                    normal(vec, array[1], population_check);
                    yield return new WaitForSeconds(spwan_delay);
                } 
            }
            else//일반적 + 특수적
            {
                for (int i = 0; i < array[0]; i++)
                {
                    vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(11f, 15f));
                    special(vec, r, array[2], population_check);
                    normal(vec, array[1], population_check);
                    yield return new WaitForSeconds(spwan_delay);
                }   
            }
        }
        else
        {
            if (array[2] == 0)//일반적+보스
            {
                for (int i = 0; i < array[0]; i++)
                {
                    vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(11f, 15f));
                    normal(vec, array[1], population_check);
                    boss(vec, array[3], population_check);
                    yield return new WaitForSeconds(spwan_delay);
                }   
            }
            else//일반적 + 특수적 +보스
            {
                for (int i = 0; i < array[0]; i++)
                {
                    vec = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * (Vector2.right * Random.Range(11f, 15f));
                    boss(vec, array[3], population_check);
                    special(vec, r, array[2], population_check);
                    normal(vec, array[1], population_check);
                    yield return new WaitForSeconds(spwan_delay);
                }
            }
        }
        spawn_working = false;
    }
    private void normal(Vector3 vec, float num, bool population_check)
    {
        if (wait_for_spawn[0] > 0)
        {
            for (int j = 0; j < num; j++)
            {
                Vector3 vec2 = Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward) * Vector2.right;//적 흩뿌리기
                spawn(Define.Enemy.Sword_infantry, vec + vec2,1, population_check);
                wait_for_spawn[0] -= 1;
            }
        }
    }

    private void special(Vector3 vec, int type, float num, bool population_check)
    {
        if(wait_for_spawn[1] > 0)
        {
            special_check += num;
            if (special_check >=1)
            {
                for(int i =0;i<(int)special_check;i++)
                {
                    if (type == 0)//창지기 - 인구수2
                    {
                        spawn(Define.Enemy.NormalEnemy, vec, 2, population_check);
                        wait_for_spawn[1] -= 2;
                    }
                    else if (type == 1)//힐러 - 인구수3
                    {
                        spawn(Define.Enemy.Healer, vec, 3, population_check);
                        wait_for_spawn[1] -= 3;
                    }
                    else if (type == 2)//닌자 - 인구수4
                    {
                        spawn(Define.Enemy.Ninja, vec, 4, population_check);
                        wait_for_spawn[1] -= 4;
                    }
                    special_check -= 1;
                }
            }
            
        }
    }

    private void boss(Vector3 vec, float type, bool population_check)
    {
        if (type == 1)
            spawn(Define.Enemy.Bow_Boss, vec, 5,population_check);
        else if (type == 2)
            spawn(Define.Enemy.Spear_Boss, vec, 5, population_check);
        else if (type == 3)
            spawn(Define.Enemy.Chair_Boss, vec, 5, population_check);
        else if (type == 4)
            spawn(Define.Enemy.Sword_Boss, vec, 5, population_check);
        //Debug.Log("보스 두둥등장");
    }

    private void spawn(Define.Enemy name, Vector3 vec, int population, bool population_check)
    {
        GameObject obj = ObjectPoolManager.Instance.GetEnemy(name);
        obj.transform.position = StageManager.Instance.player.transform.position + vec;
        enemies.Add(obj.GetComponent<Enemy>());
        if(population_check)
            enemyNum += population;
    }

    public void Fixed_Update()
    {
        // TODO : 스폰은 여기서 추가해야할듯
        //list관련 foreach에서 remove사용시 오류 발생해서 for문으로 변경 
        for(int i =0;i<enemies.Count;i++)
        {
            enemies[i].Fixed_Update();
        }

        /*foreach (Enemy enemy in enemies)
        {
            enemy.Fixed_Update();
        }*/
    }

    public void _Update()
    {
        //적 풀숫자 확인용
        ObjectPoolManager.Instance.a[0] = ObjectPoolManager.Instance.pooledEnemies[Define.Enemy.Sword_infantry].Count;
        ObjectPoolManager.Instance.a[1] = ObjectPoolManager.Instance.pooledEnemies[Define.Enemy.NormalEnemy].Count;
        ObjectPoolManager.Instance.a[2] = ObjectPoolManager.Instance.pooledEnemies[Define.Enemy.Healer].Count;
        ObjectPoolManager.Instance.a[3] = ObjectPoolManager.Instance.pooledEnemies[Define.Enemy.Ninja].Count;
        ObjectPoolManager.Instance.a[4] = ObjectPoolManager.Instance.pooledEnemies[Define.Enemy.Bow_Boss].Count;
        ObjectPoolManager.Instance.a[5] = ObjectPoolManager.Instance.pooledEnemies[Define.Enemy.Sword_Boss].Count;
        foreach (Enemy enemy in enemies)
        {
            enemy._Update();
        }
    }

}
