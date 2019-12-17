using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRailgun : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Railgun";
        weaponClass = Weapon.WeaponClass.Railgun;
        coolDown = 0.75f;
    }
}
