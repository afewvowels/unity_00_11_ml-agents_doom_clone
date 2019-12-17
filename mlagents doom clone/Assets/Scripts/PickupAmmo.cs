using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : Pickup
{
    public enum PickupType {
        ShotgunAmmo,
        RocketAmmo,
        RailgunAmmo
    };

    public Color shotgunLight;
    public Color shotgunDark;
    public Color rocketLight;
    public Color rocketDark;
    public Color railgunLight;
    public Color railgunDark;
    public MeshRenderer boxRenderer;

    public PickupType pickupType;
    public int amount;

    public override void Triggered(DoomAgent agent)
    {
        agent.DoAmmoPickup(pickupType, amount);
        Destroy(gameObject);
    }

    public void SetToShotgun()
    {
        SetMaterialsColors(shotgunLight, shotgunDark);

        pickupType = PickupAmmo.PickupType.ShotgunAmmo;
        amount = 20;
    }

    public void SetToRocket()
    {
        SetMaterialsColors(rocketLight, rocketDark);

        pickupType = PickupAmmo.PickupType.RocketAmmo;
        amount = 10;
    }

    public void SetToRailgun()
    {
        SetMaterialsColors(railgunLight, railgunDark);

        pickupType = PickupAmmo.PickupType.RailgunAmmo;
        amount = 6;
    }

    private void SetMaterialsColors(Color a, Color b)
    {
        boxRenderer.materials[0].color = a;
        boxRenderer.materials[1].color = b;
    }
}
