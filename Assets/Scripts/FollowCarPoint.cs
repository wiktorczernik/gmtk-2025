using UnityEngine;

public class FollowCarPoint : MonoBehaviour
{
    public Transform car;

    public void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, car.position, 0.01f);
        Vector3 angles = car.eulerAngles;
        angles.x = 0;
        angles.z = 0;
        transform.eulerAngles = angles;
    }
}
