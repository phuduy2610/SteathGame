using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8;
    float angle;
    float smoothInputMatgnitude;
    float smoothMoveVelocity;
    bool disabled;
    Rigidbody myRigidBody;
    // Start is called before the first frame update
    float xDir, zDir;
    Vector3 velocity;
    Vector3 direction;
    void Start()
    {
        disabled = false;
        myRigidBody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    // Update is called once per frame
    void Update()
    {
        direction = Vector3.zero;
        if (!disabled)
        {

            xDir = Input.GetAxisRaw("Horizontal");
            zDir = Input.GetAxisRaw("Vertical");
            direction = (new Vector3(xDir, 0, zDir)).normalized;

        }



    }

    private void FixedUpdate()
    {
        smoothInputMatgnitude = Mathf.SmoothDamp(smoothInputMatgnitude, direction.magnitude, ref smoothMoveVelocity, smoothMoveTime);
        //Moving
        Vector3 moveAmount = direction * speed * Time.deltaTime;

        //Rotate
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * direction.magnitude);

        velocity = transform.forward * speed * smoothInputMatgnitude;
        myRigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.deltaTime);
    }

    void Disable()
    {
        disabled = true;
    }

    // IEnumerator Move()
    // {
    //     while (true)
    //     {
    //         if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
    //         {
    //             xDir = Input.GetAxisRaw("Horizontal");
    //             zDir = Input.GetAxisRaw("Vertical");
    //             Vector3 direction = (new Vector3(xDir, 0, zDir)).normalized;
    //             transform.position = Vector3.MoveTowards(transform.position, direction, speed * Time.deltaTime);
    //         }
    //         yield return null;
    //     }
    // }
    // IEnumerator Rotate()
    // {
    //     yield return null;
    // }


}
