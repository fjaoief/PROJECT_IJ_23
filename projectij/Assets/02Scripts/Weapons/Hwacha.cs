using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hwacha : Weapon
{
    protected Rigidbody2D rb;
    float nextWeaponAngle = 10;
    float nextWeaponTime = 0.1f;

    [SerializeField]
    float maxAngle = 120; // 화차가 이루는 최대 각도
    int numbering = 0;

    protected int count = 50;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        isFirstWeapon = false;
    }

    private void FixedUpdate()
    {
        if (isFirstWeapon)
        {
            if (count == 0)
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
        else
        {
            camHeight = _camera.orthographicSize;
            camWidth = camHeight * _camera.aspect;
            
            if (CheckIfOutofScreen() || PenetrationCount <= 0)
            {
                ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
            }
        }
    }

    public override void SetWeapon(WeaponInfo weaponInfo, Transform playerTrans, Vector3 pos)
    {
        base.SetWeapon(weaponInfo, playerTrans, pos);
        spriteRenderer.enabled = false;
        col.enabled = false;
        count = 12;

        rb.velocity = Vector2.zero;
        rb.AddForce(this.transform.right * Speed, ForceMode2D.Impulse);
        StartCoroutine(HwachaAttack(playerTrans.position));
    }

    protected IEnumerator HwachaAttack(Vector3 pos)
    {
        float baseAngle = transform.rotation.eulerAngles.z;
        for (int i = 0; i < 1; i++)
        {
            float tempAngle = baseAngle;
            int num = (int) (maxAngle / nextWeaponAngle);
            for (int j = 0; j < num; j++)
            {
                CreateExtraWeapon(pos, tempAngle);
                tempAngle += nextWeaponAngle;
            }
            yield return new WaitForSeconds(nextWeaponTime);
        }
        yield return null;
    }

    protected virtual void CreateExtraWeapon(Vector3 pos, float angle)
    {
        Hwacha newHwacha = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.화차).GetComponent<Hwacha>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newHwacha);
        newHwacha.SetExtraWeapon(pos, angle);
        newHwacha.playerTransform = playerTransform;
        newHwacha.rb.velocity = Vector2.zero;
        newHwacha.rb.AddForce(newHwacha.transform.right * Speed, ForceMode2D.Impulse);
        count--;
    }

    public void SetExtraWeapon(Vector3 pos, float angle)
    {
        spriteRenderer.enabled = true;
        col.enabled = true;

        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = Vector3.one * Size;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "Enemy")
            PenetrationCount--;
    }
}
