using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seungja : Weapon
{
    protected Rigidbody2D rb;

    protected int count = 4; // 추가무기 개수

    protected override void OnEnable()
    {
        base.OnEnable();
        isFirstWeapon = false;
        soundplayer.PlayOneShot(startsound);
    }

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
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
        count = 4 + ((int)_projectilePercentage - 1);   

        rb.velocity = Vector2.zero;
        StartCoroutine(SeungjaAttack(playerTrans));
    }

    protected virtual void CreateExtraWeapon(Transform playerTransform, float angle)
    {
        Seungja newSeungja = ObjectPoolManager.Instance.GetWeapon(Define.Weapon.승자총통).GetComponent<Seungja>();
        weaponManager.UpdateWeapon(weaponInfoCopy, newSeungja);

        newSeungja.SetExtraWeapon(playerTransform, angle + transform.rotation.eulerAngles.z);
        newSeungja.playerTransform = playerTransform;
        newSeungja.rb.velocity = Vector2.zero;
        newSeungja.rb.AddForce(newSeungja.transform.right * Speed, ForceMode2D.Impulse);
        count--;
    }

    public void SetExtraWeapon(Transform playerTransform, float angle)
    {
        spriteRenderer.enabled = true;
        col.enabled = true;

        transform.position = playerTransform.position;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = Vector3.one * Size;
    }

    protected virtual IEnumerator SeungjaAttack(Transform playerTransform)
    {
        yield return new WaitForSeconds(0.05f);
        float tempAngle = 0f;
        float fixedAngle = 360f / (float)count;
        int fixedCount = count;
        for (int i = 0; i < fixedCount; i++)
        {
            tempAngle -= fixedAngle;
            CreateExtraWeapon(playerTransform, tempAngle);
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "Enemy")
            PenetrationCount--;
    }
}
