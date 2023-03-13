using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Weapon
{
    Rigidbody2D rb;
    float velY;
    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        rb.velocity = Vector2.zero;
        float tempX = Random.Range(1.5f, 3f) * (Random.Range(0, 2) * 2 - 1);
        rb.AddForce(new Vector2(tempX, Random.Range(7f, 10f)), ForceMode2D.Impulse);
        velY = rb.velocity.y;
    }

    private void FixedUpdate() {
        velY -= Time.fixedDeltaTime * Speed;
        rb.velocity = new Vector2(rb.velocity.x, velY);

        camHeight = _camera.orthographicSize;
        camWidth = camHeight * _camera.aspect;

        if (transform.position.y < _camera.transform.position.y - camHeight || PenetrationCount <= 0)
        {
            ObjectPoolManager.Instance.ReturnWeapon(this.gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.tag == "Enemy")
            PenetrationCount--;
    }
}
