using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadaWeapon : MonoBehaviour
{
    public GameObject grenada;
    public Camera mainCamera;
    public Transform spawnGrenada;

    public float force;
    public float spread;

    public Animator animator;

    private float timeBetweenShots = 0.1f; 
    private float timeSinceLastShot = 0f;

    void FixedUpdate()
    {
        timeSinceLastShot += Time.fixedDeltaTime;

        if (Input.GetMouseButtonDown(0)  && timeSinceLastShot >= timeBetweenShots)
        {
            Toos();
            animator.SetTrigger("Grenada");
            timeSinceLastShot = 0f;
        }
    }

    private void Toos()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }
        
        Vector3 dirWithoutSpread = targetPoint - spawnGrenada.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 dirWithSpread = dirWithoutSpread + new Vector3(x, y, 0);

        GameObject currentGrenada = Instantiate(grenada, spawnGrenada.position, Quaternion.identity);

        currentGrenada.transform.forward = dirWithSpread.normalized;

        currentGrenada.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * force, ForceMode.Impulse);
    }
}
