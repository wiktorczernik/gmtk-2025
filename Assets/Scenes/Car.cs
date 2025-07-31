using UnityEngine;

public class Car : MonoBehaviour
{
    private Rigidbody rb;
    private int speed = 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Finish.OnFinishReached += Finish_OnFinishReached;
    }

    private void Finish_OnFinishReached()
    {
        speed += 2;

    }



    private void Update()
    {
        float x;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            x = 1;
        }
        else
        {
            x = 0;
        }
        Debug.Log(x);

        transform.Rotate(new Vector3(0, x * 100 * Time.deltaTime, 0));
    }


    private void FixedUpdate()
    {
        rb.linearVelocity = -transform.right * speed;

    }
}
