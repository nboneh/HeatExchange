using UnityEngine;
using System.Collections;

public class Float : MonoBehaviour
{

    float floatingCycleAngle = 0;
    float y = 0;
    float angz = 0;
    float angx = 0;

    // Use this for initialization
    void Start()
    {
        y = transform.position.y;
        angz = transform.rotation.eulerAngles.z;
        angx = transform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.unscaledDeltaTime;
        FloatAnim(t);
    }

    void FloatAnim(float t)
    {
        floatingCycleAngle += t * 90;
        float floatingY = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f) * 2) * .07f;
        float floatingRoll = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f)) * 5;
        float floatingYaw = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f) * 3) * 2;
        if (floatingCycleAngle > 360)
        {
            floatingCycleAngle = floatingCycleAngle % 360;
        }
        transform.position = new Vector3(transform.position.x, y + floatingY, transform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(angx +floatingRoll, transform.rotation.eulerAngles.y, angz +floatingYaw));
    }
}
