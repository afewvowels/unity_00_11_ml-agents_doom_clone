using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPistol : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Pistol";
        weaponClass = Weapon.WeaponClass.Pistol;
        coolDown = 0.25f;
    }

    public override void Shoot(DoomAgent agent)
    {
        base.Shoot(agent);
    }
}
