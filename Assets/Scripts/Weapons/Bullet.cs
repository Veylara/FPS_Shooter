using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 3;

    public int damage = 25;

    public GameObject blood;
    private bool hasHit = false;

    private void Awake()
    {
        Destroy(gameObject, bulletLife);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!hasHit && collision.gameObject.CompareTag("Enemy"))
        {
            hasHit = true; 
            Enemies enemy = collision.gameObject.GetComponent<Enemies>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
