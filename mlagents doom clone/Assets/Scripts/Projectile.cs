using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public int damage;
    public GameObject trailPrefab;

    public void Start()
    {
        StartCoroutine(CreateTrail());
    }

    public virtual void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("doomenemy"))
        {
            c.gameObject.GetComponent<Enemy>().GotHit(damage);
        }

        StopAllCoroutines();
        Destroy(gameObject);
    }

    public virtual IEnumerator CreateTrail()
    {
        yield return new WaitForSeconds(0.001f);
        GameObject trailItem = (GameObject)Instantiate(trailPrefab);
        trailItem.transform.position = transform.position;
        trailItem.GetComponent<ProjectileTrail>().StartShrink();
        StartCoroutine(CreateTrail());
    }
}