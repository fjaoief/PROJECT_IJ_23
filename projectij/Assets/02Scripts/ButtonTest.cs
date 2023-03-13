using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    bool clicked = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onButtonClicked()
    {
        if (clicked)
        {
            clicked = false;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            clicked = true;
            GetComponent<Image>().color = Color.gray;
        }
    }
}
