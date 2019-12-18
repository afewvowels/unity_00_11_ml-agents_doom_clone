using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public int damage;
    public GameObject trailPrefab;
    public enum Owner { Enemy, Player };
    public Owner owner;

    public void Start()
    {
        StartCoroutine(CreateTrail());
    }

    public virtual void OnCollisionEnter(Collision c)
    {
        if (owner == Projectile.Owner.Player)
        {
            if (c.gameObject.CompareTag("doomenemy"))
            {
                c.gameObject.GetComponent<Enemy>().GotHit(damage);
            }
        }
        else
        {
            if (c.gameObject.CompareTag("doomguy"))
            {
                c.gameObject.GetComponent<DoomAgent>().GotHit(damage);
            }
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