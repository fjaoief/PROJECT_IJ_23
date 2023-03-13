using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandObject : MonoBehaviour
{
    [SerializeField]
    private GameObject[] randitem = new GameObject[5];
    public void GetDistroy()
    {
        int i = Random.Range(0, 100);
        //랜덤하 확률로 아이템 생성 
        if (i < 80)
        {
            GameObject newObject =  Instantiate(randitem[0], this.transform.position, Quaternion.identity);
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
        }
        else if (i < 10)
        {
            GameObject newObject = Instantiate(randitem[1], this.transform.position, Quaternion.identity);
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
        }
        else if (i < 20)
        {
            GameObject newObject = Instantiate(randitem[2], this.transform.position, Quaternion.identity);
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
        }
        else if(i<40)
        {
            GameObject newObject = Instantiate(randitem[3], this.transform.position, Quaternion.identity);
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
        }
        else
        {
            GameObject newObject = Instantiate(randitem[4], this.transform.position, Quaternion.identity);
            newObject.name = newObject.name.Split('(')[0]; // 뒤에 붙는 (clone) 제거
        }
        Destroy(this.gameObject);
    }
}
