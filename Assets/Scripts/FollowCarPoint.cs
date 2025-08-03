using UnityEngine;

public class FollowCarPoint : MonoBehaviour
{
    public Transform car;
    public void FixedUpdate()
    {
        // camera fly towards player like drone
        transform.position = Vector3.Lerp(transform.position, car.position, 0.5f);
        Vector3 angles = car.eulerAngles;
        angles.x = 0;
        angles.z = 0;
        transform.eulerAngles = angles;
    }
}
