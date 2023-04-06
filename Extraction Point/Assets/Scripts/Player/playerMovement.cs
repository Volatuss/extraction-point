using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    public bool isAiming = false, isMoving = false, isSprinting = false, canMove = true;
    private float sprintSpeed, aimSpeed;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        sprintSpeed = moveSpeed*2f;
        aimSpeed = moveSpeed/2f;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(PlayerHealth.isPlayerDead){return; }
        CheckInputs();
    }

    private void FixedUpdate()
    {
        

        if (canMove && !GameHandler.isUIOpen && !PlayerHealth.isPlayerDead)
        {
            if (movementInput == Vector2.zero) { return; }
            if (isSprinting)
            { //sprinting move speed is double
                rb.MovePosition(rb.position + movementInput * sprintSpeed * Time.fixedDeltaTime);
            }
            else if (isAiming)
            { //aiming, move speed is halved
                rb.MovePosition(rb.position + movementInput * aimSpeed * Time.fixedDeltaTime);
            }
            else
            { //not sprinting, not aiming
                rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }
    }

    private void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<Vector2>();
    }


}
