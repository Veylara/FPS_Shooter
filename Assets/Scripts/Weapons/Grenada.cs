using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenada : MonoBehaviour
{
    public float grenadaLife = 5f;
    public int damage = 10;

    public GameObject smoke;
    
    private void Awake() {
        Destroy(gameObject, grenadaLife);
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Terrain"))
        {
            Instantiate(smoke, transform.position, Quaternion.identity);
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            Enemies enemy = collision.gameObject.GetComponent<Enemies>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
