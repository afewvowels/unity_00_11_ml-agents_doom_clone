using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Pickup
{
    public enum PickupType {
        Pistol,
        Shotgun,
        RocketLauncher,
        Railgun
    };

    public PickupType pickupType;
    public int amount;

    public override void Triggered(DoomAgent agent)
    {
        agent.DoWeaponPickup(pickupType);
        Destroy(gameObject);
    }
}
