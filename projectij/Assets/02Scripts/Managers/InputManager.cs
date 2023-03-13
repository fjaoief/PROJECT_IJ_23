using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float h = 0f, v = 0f;

    public (float, float) Get_Input_Direction()
    {
        return (h, v);
    }
    public void _Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }
}
