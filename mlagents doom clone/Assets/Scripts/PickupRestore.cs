using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRestore : Pickup
{
    public enum PickupType {
        Health,
        Armor
    };

    public PickupType pickupType;
    public int amount;

    public override void Triggered(DoomAgent agent)
    {
        if (pickupType == PickupRestore.PickupType.Health)
        {
            if (agent.health < 100)
            {
                agent.DoRestorePickup(pickupType, amount);
                Destroy(gameObject);
            }
        }
        else
        {
            if (agent.armor < 150)
            {
                agent.DoRestorePickup(pickupType, amount);
                Destroy(gameObject);
            }
        }
    }

    public void SetToHealth()
    {
        pickupType = PickupRestore.PickupType.Health;
        amount = 50;
    }

    public void SetToArmor()
    {
        pickupType = PickupRestore.PickupType.Armor;
        amount = 25;
    }
}
