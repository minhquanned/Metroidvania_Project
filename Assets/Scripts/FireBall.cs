using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] int speed;
    [SerializeField] float lifeTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        transform.position += speed * transform.right;
    }

    // Detect hit
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.tag == "Enemy")
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(damage, (_other.transform.position - transform.position).normalized, -hitForce);
        }
    }
}
