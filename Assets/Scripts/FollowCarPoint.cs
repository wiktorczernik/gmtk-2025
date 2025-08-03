using UnityEngine;

public class FollowCarPoint : MonoBehaviour
{
    public Transform car;

    public void FixedUpdate()
    {
        transform.position = car.position; // if someone can get this working, I would be very happy. Vector3.Lerp(transform.position, car.position, 0.5f);
        Vector3 angles = car.eulerAngles;
        angles.x = 0;
        angles.z = 0;
        transform.eulerAngles = angles;
    }
}
