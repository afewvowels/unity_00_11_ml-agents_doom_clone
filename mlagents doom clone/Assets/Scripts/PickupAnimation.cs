using UnityEngine;
using System.Collections;

public class PickupAnimation: MonoBehaviour
{
    public bool isArmor;
    private void Start()
    {
        transform.localRotation = Quaternion.Euler(0.0f, 360.0f * Random.value, 0.0f);
        
        if (isArmor)
        {
            transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.y, 90.0f);
        }

        StartCoroutine(Float());
        StartCoroutine(Spin());
    }

    private IEnumerator Spin()
    {
        for (int i = 0; i < 360; i++)
        {
            if (!isArmor)
            {
                transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f));
            }
            else
            {
                transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f));
            }
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(Spin());
    }

    private IEnumerator Float()
    {
        Vector3 fromPosition = transform.position;
        Vector3 toPosition = new Vector3(fromPosition.x, fromPosition.y + 0.5f, fromPosition.z);

        for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 0.25f)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, t);
            yield return null;
        }

        for (float t = 1.0f; t > 0.0f; t -= Time.fixedDeltaTime * 0.25f)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, t);
            yield return null;
        }

        StartCoroutine(Float());
    }
}