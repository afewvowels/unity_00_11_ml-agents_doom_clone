using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRocketLauncher : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Rocket Launcher";
        weaponClass = Weapon.WeaponClass.RocketLauncher; 
        coolDown = 0.65f;  
    }

    public override IEnumerator DoShoot(DoomAgent agent)
    {
        isShooting = true;
        GameObject bullet = (GameObject)Instantiate(projectilePrefab);
        bullet.transform.SetParent(instantiationPoint.transform, false);
        bullet.transform.position = instantiationPoint.transform.position - (transform.right * 0.25f);
        bullet.transform.localRotation = agent.transform.localRotation;
        bullet.GetComponent<Rigidbody>().AddRelativeForce(agent.transform.forward * 10.0f, ForceMode.Impulse);

        bullet = (GameObject)Instantiate(projectilePrefab);
        bullet.transform.SetParent(instantiationPoint.transform, false);
        bullet.transform.position = instantiationPoint.transform.position + (transform.right * 0.25f);
        bullet.transform.localRotation = agent.transform.localRotation;
        bullet.GetComponent<Rigidbody>().AddRelativeForce(agent.transform.forward * 10.0f, ForceMode.Impulse);
        agent.ammoArray[(int)weaponClass]--;
        agent.UpdateDisplayedAmmo();
        yield return new WaitForSeconds(coolDown);
        isShooting = false;
    }
}
