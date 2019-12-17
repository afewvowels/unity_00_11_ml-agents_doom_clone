using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShotgun : Weapon
{
    public override void Start()
    {   
        base.Start();
        weaponName = "Shotgun";
        weaponClass = Weapon.WeaponClass.Shotgun;
        coolDown = 0.5f;
    }

    public override void Shoot(DoomAgent agent)
    {
        base.Shoot(agent);
    }

    public override IEnumerator DoShoot(DoomAgent agent)
    {
        isShooting = true;

        for (int i = 0; i < 25; i++)
        {
            GameObject bullet = (GameObject)Instantiate(projectilePrefab);
            Vector3 randomize = Random.insideUnitSphere * 0.1f;
            bullet.transform.position = instantiationPoint.transform.position + randomize;
            bullet.transform.localRotation = agent.transform.localRotation;
            randomize = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(-3.0f, 3.0f), 0.0f);
            bullet.GetComponent<Rigidbody>().AddRelativeForce(agent.transform.forward * 10.0f + randomize, ForceMode.Impulse);
            yield return null;
        }
        agent.ammoArray[(int)weaponClass]--;
        agent.UpdateDisplayedAmmo();
        yield return new WaitForSeconds(coolDown);
        isShooting = false;
    }
}
