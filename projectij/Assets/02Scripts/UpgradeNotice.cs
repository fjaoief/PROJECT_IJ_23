using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeNotice : MonoBehaviour
{
    CanvasGroup canvasGroup;
    float time = 0f;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        time = 0;
        canvasGroup.alpha = 1;
    }
    private void Update() 
    {
        if (time < 3f)
        {
            time += Time.deltaTime;
        }
        else
            canvasGroup.alpha = Mathf.MoveTowards( canvasGroup.alpha, 0f, Time.deltaTime);
    }
}
