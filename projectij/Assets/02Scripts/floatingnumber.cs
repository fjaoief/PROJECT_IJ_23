using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingnumber : MonoBehaviour
{
    public Define.FloatingDmg FloatingDmgName;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, 1);
    }
    private void OnEnable() {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.3f)
        {
            ObjectPoolManager.Instance.ReturnFloatingDmg(this.gameObject);
        }
    }
}
