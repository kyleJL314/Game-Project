using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed=100;
    public float jumpForce = 10;
    public float stepOver = .01f;
    public float stepheight = 1.1f;


    private List<ContactPoint> allCPs = new List<ContactPoint>();

    float groundPos;
    bool isGounded = false;
    Rigidbody rb;


    float x;
    float z;
    Vector3 preVelocity = new Vector3(0, 0, 0);
    bool jump = false;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        if(!jump) jump = Input.GetButtonDown("Jump");
     
        CheckifWalfUpBlock();

    }
    private void FixedUpdate()
    {
        upDateGound();
        float verticalAxis=0;
       
     
        if (jump && isGounded)
        {
            jump = false;
            if(!GetComponent<PlayerInventory>().isOpen) verticalAxis = jumpForce;


        }
        else
        {
            verticalAxis = transform.InverseTransformVector(rb.velocity).y;
        }
        Vector3 move = Vector3.zero;
        if (!GetComponent<PlayerInventory>().isOpen) move = (Vector3.right * x + Vector3.forward * z) * speed;
        move.y = verticalAxis;


        CheckifWalfUpBlock();
        rb.velocity = transform.TransformVector(move);
        preVelocity = rb.velocity;
        allCPs.Clear();
    }
    void OnCollisionEnter(Collision col)
    {

        allCPs.AddRange(col.contacts);
        
    }

    void OnCollisionStay(Collision col)
    {

        allCPs.AddRange(col.contacts);
   
    }
    void upDateGound()
    {
        foreach (ContactPoint c in allCPs)
        {
           

            if (transform.InverseTransformVector(c.normal).y*transform.localScale.y > .8)
            {
            ;
                isGounded = true;
                groundPos = transform.InverseTransformPoint(c.point).y;
                
                return;
            }
        }
        isGounded = false;
    }

    void CheckifWalfUpBlock()
    {
        
        if (isGounded)
        {
            
            float largestPoint = groundPos;
            ContactPoint largestContactPoint = new ContactPoint();
            foreach (ContactPoint c in allCPs)
            {
                if (largestPoint < transform.InverseTransformPoint(c.point).y)
                {
                    largestPoint = transform.InverseTransformPoint(c.point).y;
                    largestContactPoint = c;
                }
            }
            if (largestPoint > groundPos + .1 && (largestPoint - groundPos)<stepheight)
            {
                allCPs.Clear();
                float moveup = (largestPoint - groundPos) + .01f;
                transform.position += transform.up * (moveup);
                transform.position += largestContactPoint.normal *-1* stepOver;
                rb.velocity = preVelocity;
               
            }
        }
    }
}
