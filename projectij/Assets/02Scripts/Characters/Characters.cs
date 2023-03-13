using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    public Define.Character character;
    [SerializeField]
    public float HP = 100;
    public float maxHP = 100;
    public float defense = 0;
    
    public SpriteRenderer spriteRenderer;
    protected Collider Charater_collider;

    protected virtual void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Charater_collider = GetComponent<Collider>();
        maxHP = HP;
    }
    public virtual void _Update()
    {

    }

    public virtual void Fixed_Update()
    {
        
    }

    public virtual void Collision_Function(Collision2D other)
    {
        // 같은 종류끼리 충돌 예외처리
        // if (other.transform.tag == transform.tag)
        //     return;
    }

    public virtual void Trigger_Function(Collider2D other)
    {
        // 같은 종류끼리 충돌 예외처리
        if (other.transform.tag == transform.tag)
            return;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Collision_Function(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Trigger_Function(other);
    }
}
