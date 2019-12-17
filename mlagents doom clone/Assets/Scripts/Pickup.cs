using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public abstract void Triggered(DoomAgent agent);
}