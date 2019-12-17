using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float coolDown;
    public string weaponName;
    public enum WeaponClass { Pistol, Shotgun, RocketLauncher, Railgun };
    public WeaponClass weaponClass;
    public GameObject projectilePrefab;
    public GameObject instantiationPoint;
    public DoomAgent agent;
    public bool isShooting;

    public virtual void Start()
    {
        isShooting = false;
        agent = transform.parent.GetComponentInParent<DoomAgent>();
        instantiationPoint = transform.GetChild(0).gameObject;
    }

    public virtual void Shoot(DoomAgent agent)
    {
        if (!isShooting && agent.ammoArray[(int)weaponClass] > 0)
        {
            StartCoroutine(DoShoot(agent));
        }
    }

    public virtual IEnumerator DoShoot(DoomAgent agent)
    {
        isShooting = true;
        GameObject bullet = (GameObject)Instantiate(projectilePrefab);
        bullet.transform.SetParent(instantiationPoint.transform, false);
        bullet.transform.position = instantiationPoint.transform.position;
        bullet.transform.localRotation = agent.transform.localRotation;
        bullet.GetComponent<Rigidbody>().AddRelativeForce(agent.transform.forward * 10.0f, ForceMode.Impulse);
        agent.ammoArray[(int)weaponClass]--;
        agent.UpdateDisplayedAmmo();
        yield return new WaitForSeconds(coolDown);
        isShooting = false;
    }
}
