using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // 지금은 일단 weaponPrefabs의 모든 오브젝트를 생성하는데 
    // 상황에 따라 필요한 무기나 적만 생성하도록 수정해야 할 것 같아요. 
    // 적 : 시간이 지남에 따라 나오지 않는 적들 풀에서 삭제 + 새로운 적 풀에 추가
    // 무기 : 얻을 때마다 하나씩 추가

    // 완전중요!!! 다시 사용하는 오브젝트는 초기화 필요 (예를들어 Bullet.spawntime = 0)

    // 잘 구현하셨는데 pooling된 오브젝트에 부모를 한단계 더 내리면 좋을 것 같아요
    // inspector에서 / 적 object 따로 / 무기 따로 / 등등 이렇게 보일 수 있도록?

    // child 0: 무기 / 1: 적 / 2: 경험치 구슬 / 3: 데미지
    public static ObjectPoolManager Instance;
    // 처음 생성하는 최대 개수
    [SerializeField]
    int poolCount_weapon = 10;
    [SerializeField]
    int poolCount_enemy = 50;
    [SerializeField]
    int poolCount_special = 20;
    [SerializeField]
    int poolCount_boss = 3;
    [SerializeField]
    int poolCount_expball = 10;
    
    [SerializeField]
    int poolCount_floatingDmg = 30;
    
    // 인스펙터창에서 관리
    public GameObject[] weaponPrefabs; 
    public GameObject[] enemyPrefabs;
    public GameObject[] expballPrefabs;
    public GameObject[] floatingDmgPrefabs;
    
    // hierarchy창 정리를 위한 빈 오브젝트
    GameObject objects;

    Dictionary<Define.Weapon, Queue<GameObject>> pooledWeapons = new Dictionary<Define.Weapon, Queue<GameObject>>();
    public Dictionary<Define.Enemy, Queue<GameObject>> pooledEnemies = new Dictionary<Define.Enemy, Queue<GameObject>>();
    Dictionary<Define.Exp, Queue<GameObject>> pooledExpballs = new Dictionary<Define.Exp, Queue<GameObject>>();
    Dictionary<Define.FloatingDmg, Queue<GameObject>> pooledFloatingDmg = new Dictionary<Define.FloatingDmg, Queue<GameObject>>();
    public int[] a = new int [6];

    public Sprite[] ExpSprite = new Sprite[4];
    public Sprite[] FlagSprite = new Sprite[11];

    //csv
    public List<Dictionary<string, object>> base_CSV;
    public List<Dictionary<string, object>> army_CSV;
    public List<Dictionary<string, object>> diff_CSV;
    public List<Dictionary<string, object>> time_CSV;

    private void Awake()
    {
        Instance = this;
        base_CSV = CSVReader.Read("CSV/enemy_unit_stats");
        army_CSV = CSVReader.Read("CSV/enemy_army_bonus");
        diff_CSV = CSVReader.Read("CSV/enemy_difficulty_buff");
        time_CSV = CSVReader.Read("CSV/enemy_time_fortified");

        CreateNewEnemies();
        CreateNewExpballs();
        CreateNewFloatingDmgs();

        objects = GameObject.Find("Objects").gameObject;
        ExpSprite = Resources.LoadAll<Sprite>("exp_ball");
        FlagSprite = Resources.LoadAll<Sprite>("flag");
        
    }

    // 무기 풀링
    public void CreateNewWeapons(Define.Weapon weaponName)
    {
        int tempIndex = -1;

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            if (weaponPrefabs[i].GetComponent<Weapon>().weaponName == weaponName)
            {
                tempIndex = i;
                break;
            }
        }

        Queue<GameObject> newQueue = new Queue<GameObject>();
        pooledWeapons.Add(weaponName, newQueue);
        
        for (int j = 0; j < poolCount_weapon; j++)
        {
            GameObject newObject = Instantiate(weaponPrefabs[tempIndex], transform.GetChild(0));
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
            newObject.SetActive(false);
            pooledWeapons[weaponName].Enqueue(newObject);
        }
        
    }
    
    public Weapon GetWeaponClass(Define.Weapon name)
    {
        return pooledWeapons[name].Peek().GetComponent<Weapon>();
    }

    public GameObject GetWeapon(Define.Weapon name)
    {
        // 딕셔너리에 키가 없을 때
        if (!pooledWeapons.ContainsKey(name))
        {
            CreateNewWeapons(name);
        }
        
        //Debug.Log($"{name} : {pooledWeapons[name].Count}개");
        if (pooledWeapons[name].Count > 1)
        { // 오브젝트 개수가 넉넉할 때
            GameObject obj = pooledWeapons[name].Dequeue();
            obj.transform.SetParent(objects.transform);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        { // 개수가 부족하면 오브젝트 새로 생성
          //Debug.Log("부족해!!!!!!!!!!!!!!!!");
            GameObject newObj = Instantiate(pooledWeapons[name].Peek().gameObject, null);
            newObj.name = newObj.name.Split('(')[0];
            newObj.transform.SetParent(objects.transform);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnWeapon(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform.GetChild(0));
        pooledWeapons[obj.GetComponent<Weapon>().weaponName].Enqueue(obj);
    }

    // 적 풀링
    private void CreateNewEnemies()
    {
        Debug.Log("~~~"+enemyPrefabs.Length);
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {

            Queue<GameObject> newQueue = new Queue<GameObject>();
            pooledEnemies.Add(enemyPrefabs[i].GetComponent<Enemy>().enemyName, newQueue);
            if (i == 0)
                CNE(poolCount_enemy, i);
            else if (i == 1 || i == 2 || i == 3)
                CNE(poolCount_special, i);
            else if (i == 4 || i == 5 || i == 6 || i == 7)
                CNE(poolCount_boss, i);
            else
                CNE(10, i);
        }
    }
    private void CNE(int a, int i)//(create new enemy)일반적, 특별적, 보스 구별해서 초기 생성값조정
    {
        for (int j = 0; j < a; j++)
        {
            GameObject newObject = Instantiate(enemyPrefabs[i], transform.GetChild(1));
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
            newObject.SetActive(false);
            newObject.GetComponent<Enemy>().first_disable = true;
            pooledEnemies[enemyPrefabs[i].GetComponent<Enemy>().enemyName].Enqueue(newObject);
        }
    }

    public GameObject GetEnemy(Define.Enemy name)
    {
        if (pooledEnemies.ContainsKey(name))
        {
            if (pooledEnemies[name].Count >= 1)
            { // 오브젝트 개수가 넉넉할 
                GameObject obj = pooledEnemies[name].Dequeue();
                obj.transform.SetParent(objects.transform);
                if(name == Define.Enemy.Sword_infantry || name == Define.Enemy.NormalEnemy)
                    flag(obj);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            { // 개수가 부족하면 오브젝트 새로 생성
                GameObject newObj = this.gameObject;
                if (name == Define.Enemy.Sword_infantry)
                {
                    newObj = Instantiate(enemyPrefabs[0], null);
                    flag(newObj);
                }
                else if(name == Define.Enemy.NormalEnemy)
                {
                    newObj = Instantiate(enemyPrefabs[1], null);
                    flag(newObj);
                }  
                else if (name == Define.Enemy.Healer)
                    newObj = Instantiate(enemyPrefabs[2], null);
                else if (name == Define.Enemy.Ninja)
                    newObj = Instantiate(enemyPrefabs[3], null);
                else if (name == Define.Enemy.Bow_Boss)
                    newObj = Instantiate(enemyPrefabs[4], null);
                else if (name == Define.Enemy.Spear_Boss)
                    newObj = Instantiate(enemyPrefabs[5], null);
                else if (name == Define.Enemy.Chair_Boss)
                    newObj = Instantiate(enemyPrefabs[6], null);
                else if (name == Define.Enemy.Sword_Boss)
                    newObj = Instantiate(enemyPrefabs[7], null);
                else
                    newObj = Instantiate(enemyPrefabs[8], null);

                newObj.name = newObj.name.Split('(')[0];
                newObj.transform.SetParent(objects.transform);
                newObj.SetActive(true);
                return newObj;
            }
        }
        else
        { // 딕셔너리에 키가 없을 때
            return null;
        }
    }
    void flag(GameObject obj)
    {
        GameObject flag = obj.transform.GetChild(0).GetChild(0).gameObject;
        switch(StageManager.Instance.cur_flag)
        {
            case 0:
                flag.GetComponent<SpriteRenderer>().sprite = FlagSprite[StageManager.Instance.flag_num[0]-1];
                obj.GetComponent<Enemy>().flag_num = StageManager.Instance.flag_num[0];
                break;
            case 1:
                flag.GetComponent<SpriteRenderer>().sprite = FlagSprite[StageManager.Instance.flag_num[1]-1];
                obj.GetComponent<Enemy>().flag_num = StageManager.Instance.flag_num[1];
                break;
            case 2:
                flag.GetComponent<SpriteRenderer>().sprite = FlagSprite[StageManager.Instance.flag_num[2]-1];
                obj.GetComponent<Enemy>().flag_num = StageManager.Instance.flag_num[2];
                break;
        }
    }
    public void ReturnEnemy(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform.GetChild(1));
        pooledEnemies[obj.GetComponent<Enemy>().enemyName].Enqueue(obj);
    }



    // 경험치 구슬 풀링
    private void CreateNewExpballs()
    {

        for (int i = 0; i < expballPrefabs.Length; i++)
        {

            Queue<GameObject> newQueue = new Queue<GameObject>();
            pooledExpballs.Add(expballPrefabs[i].GetComponent<ExpBall>().ExpName, newQueue);

            for (int j = 0; j < poolCount_expball; j++)
            {
                GameObject newObject = Instantiate(expballPrefabs[i], transform.GetChild(2));
                newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
                newObject.SetActive(false);
                pooledExpballs[expballPrefabs[i].GetComponent<ExpBall>().ExpName].Enqueue(newObject);
            }
        }
    }

    public GameObject GetExpball(Define.Exp name)
    {
        if (pooledExpballs.ContainsKey(name))
        {
            if (pooledExpballs[name].Count > 1)
            { // 오브젝트 개수가 넉넉할 때
                GameObject obj = pooledExpballs[name].Dequeue();
                obj.transform.SetParent(objects.transform);
                obj.gameObject.SetActive(true);
                obj.GetComponent<SpriteRenderer>().sprite = ExpSprite[Random.Range(0, 4)];
                StageManager.Instance.allExpBalls.Add(obj.GetComponent<ExpBall>());
                return obj;
            }
            else
            { // 개수가 부족하면 오브젝트 새로 생성
                GameObject newObj = Instantiate(pooledExpballs[name].Peek().gameObject, null);
                newObj.name = newObj.name.Split('(')[0];
                newObj.transform.SetParent(objects.transform);
                newObj.SetActive(true);
                newObj.GetComponent<SpriteRenderer>().sprite = ExpSprite[Random.Range(0, 4)];
                StageManager.Instance.allExpBalls.Add(newObj.GetComponent<ExpBall>());
                return newObj;
            }
        }
        else
        { // 딕셔너리에 키가 없을 때
            return null;
        }
    }

    public void ReturnExpball(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform.GetChild(2));
        StageManager.Instance.allExpBalls.Remove(obj.GetComponent<ExpBall>());
        pooledExpballs[obj.GetComponent<ExpBall>().ExpName].Enqueue(obj);
    }


    // 데미지 풀링
    private void CreateNewFloatingDmgs()
    {

        for (int i = 0; i < floatingDmgPrefabs.Length; i++)
        {

            Queue<GameObject> newQueue = new Queue<GameObject>();
            pooledFloatingDmg.Add(floatingDmgPrefabs[i].GetComponent<floatingnumber>().FloatingDmgName, newQueue);

            for (int j = 0; j < poolCount_floatingDmg; j++)
            {
                GameObject newObject = Instantiate(floatingDmgPrefabs[i], transform.GetChild(3));
                newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
                newObject.SetActive(false);
                pooledFloatingDmg[floatingDmgPrefabs[i].GetComponent<floatingnumber>().FloatingDmgName].Enqueue(newObject);
            }
        }
    }

    public GameObject GetFloatingDmg(Define.FloatingDmg name)
    {
        if (pooledFloatingDmg.ContainsKey(name))
        {
            if (pooledFloatingDmg[name].Count > 1)
            { // 오브젝트 개수가 넉넉할 때
                GameObject obj = pooledFloatingDmg[name].Dequeue();
                obj.transform.SetParent(objects.transform);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            { // 개수가 부족하면 오브젝트 새로 생성
                GameObject newObj = Instantiate(pooledFloatingDmg[name].Peek().gameObject, null);
                newObj.name = newObj.name.Split('(')[0];
                newObj.gameObject.SetActive(true);
                newObj.SetActive(true);
                return newObj;
            }
        }
        else
        { // 딕셔너리에 키가 없을 때
            return null;
        }
    }

    public void ReturnFloatingDmg(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform.GetChild(3));
        pooledFloatingDmg[obj.GetComponent<floatingnumber>().FloatingDmgName].Enqueue(obj);
    }

}

