using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Thank you to plai on youtube for the series that halped me develop this!! - Aneesh
public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;
    float sprint = 1.0f;
    bool coolDown = false;
    public float dodgeTime;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = .4f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.4f;
    

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + .5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);
        ControlDrag();
        MyInput();
        

        if(Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown("left shift") && !(Input.GetKey("w")) && !coolDown)
            Dodge();

        //print(this.gameObject.transform.rotation);

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void Dodge()
    {
        if ((Input.GetKey("a")))
        {
            print("Dodging Left");
            rb.AddForce(-transform.right * 15.0f, ForceMode.Impulse);
        }
        else if ((Input.GetKey("d")))
        {
            print("Dodging Right");
            rb.AddForce(transform.right * 15.0f, ForceMode.Impulse);
        }
        else if ((Input.GetKey("s")))
        {
            print("Dodging Back");
            rb.AddForce(-transform.forward * 15.0f, ForceMode.Impulse);
        }

        coolDown = true;
        Invoke("ResetCoolDown", dodgeTime);
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void ResetCoolDown()
    {
        coolDown = false;
    }

    void MovePlayer()
    {
        if (Input.GetKey("left shift") && !(Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")))
            sprint = 1.5f;
        else
            sprint = 1.0f;

        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * (sprint * moveSpeed) * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

    }
}
