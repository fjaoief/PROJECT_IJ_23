using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_fix : MonoBehaviour
{
    int deviceWidth, deviceHeight, time;
    bool setting;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }
    private void Start()
    {
        SetResolution(); // �ʱ⿡ ���� �ػ� ����
    }

    void Update()
    {
        //time = (int)(StageManager.Instance.GetComponent<Timer>().GetPlayTime());
        //text.text = time / 60 + ":" + time % 60;
        if (deviceWidth != Screen.width || deviceHeight != Screen.height && setting == false)
            SetResolution();
    }

    /* �ػ� �����ϴ� �Լ� */
    public void SetResolution()
    {
        setting = true;
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        deviceWidth = Screen.width; // ��� �ʺ� ����
        deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(deviceWidth, deviceHeight, false); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
        setting = false;
    }
}
