using System.Collections;
using UnityEngine;

public class ProjectileTrail : MonoBehaviour
{
    private Material material;
    private Color fromColor;
    public Color toColor;

    public void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        fromColor = material.color;
    }

    public void StartShrink()
    {
        DoShrink();
    }

    public void DoShrink()
    {
        StartCoroutine(Shrink());
    }

    public IEnumerator Shrink()
    {
        Vector3 fromScale = transform.localScale;

        for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 1.5f)
        {
            transform.localScale = Vector3.Lerp(fromScale, Vector3.zero, t);
            material.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}