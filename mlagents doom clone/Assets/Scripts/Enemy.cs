using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int health;
    public bool isAttacking;
    public GameObject enemyProjectile;
    public GameObject instantiationPoint;

    private void Start()
    {
    }

    public virtual void DoStartStuff()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("enemyinstantiationpoint"))
            {
                instantiationPoint = transform.GetChild(i).gameObject;
            }
        }
        isAttacking = false;
        StartCoroutine(DoWander());
    }

    public void GotHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    public void Wander()
    {
        StopAllCoroutines();
        StartCoroutine(DoWander());
    }

    public IEnumerator DoWander()
    {
        if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
        {
            Quaternion fromRotation = transform.rotation;
            Quaternion toRotation = Quaternion.Euler(0.0f, transform.rotation.y, 0.0f);

            for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 2.0f)
            {
                transform.rotation = Quaternion.Lerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        float newRotation = 360.0f * Random.value;

        while (Mathf.Abs(transform.eulerAngles.y - newRotation) > 5.0f)
        {
            if (transform.eulerAngles.y - newRotation > 0.0f)
            {
                transform.Rotate(new Vector3(0.0f, -3.0f, 0.0f));
                yield return null;
            }
            else
            {
                transform.Rotate(new Vector3(0.0f, 3.0f, 0.0f));
                yield return null;
            }

            if (transform.eulerAngles.y >= 360.0f || transform.eulerAngles.y <= -360.0f)
            {
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                yield return null;
            }
        }

        // int layerMask = 1 << 8;
        // layerMask = ~layerMask;
        Debug.DrawRay((transform.position + transform.up), transform.forward * 5.0f, Color.red, 0.1f);
        if (!Physics.Raycast((transform.position + transform.up), transform.forward, 3.0f))
        {
            Vector3 fromPosition = transform.position;
            Vector3 toPosition = transform.position + (transform.forward * 2.0f);

            for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime)
            {
                transform.position = Vector3.Lerp(fromPosition, toPosition, t);
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.5f);
        Wander();
    }

    public bool IsAgentVisible(Transform agentPosition)
    {
        Debug.DrawRay((transform.position + transform.up), (agentPosition.position + agentPosition.up), Color.red, 0.1f);
        float distanceMag = (transform.position - agentPosition.position).magnitude;
        return Physics.Raycast((transform.position + transform.up), (agentPosition.position + agentPosition.up), distanceMag, 8);
    }

    public void Attack(DoomAgent agent)
    {
        StopAllCoroutines();
        StartCoroutine(LookAtAgent(agent));
        StartCoroutine(AttackAgent(agent));
    }

    public IEnumerator AttackAgent(DoomAgent agent)
    {
        isAttacking = true;
        GameObject bullet = (GameObject)Instantiate(enemyProjectile);
        bullet.transform.position = instantiationPoint.transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 10.0f, ForceMode.Impulse);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(AttackAgent(agent));
    }

    public IEnumerator LookAtAgent(DoomAgent agent)
    {
        transform.LookAt(agent.transform);
        yield return null;
        StartCoroutine(LookAtAgent(agent));
    }

    public IEnumerator Die()
    {
        Destroy(GetComponent<SphereCollider>());

        Vector3 startScale = gameObject.transform.localScale;

        for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime)
        {
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            transform.Rotate(new Vector3(0.0f, 5.0f, 0.0f));
            yield return null;
        }

        Destroy(gameObject);
    }
}