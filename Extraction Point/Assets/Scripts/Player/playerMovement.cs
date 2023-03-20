using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 0.0f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool canMove = true;
    private float collisionOffset = 0.02f;
    private Vector2 movementInput;
    private ContactFilter2D movementFilter;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    
    void FixedUpdate(){
    if(canMove && !InventoryController.isInvOpen){
            if(movementInput != Vector2.zero){
                bool success = TryMove(movementInput);
                if(!Input.GetKey(KeyCode.LeftShift)){
                    if(!success && movementInput.x > 0){
                    success = TryMove(new Vector2(movementInput.x, 0));
                    }
                    if(!success && movementInput.y > 0){
                    success = TryMove(new Vector2(0, movementInput.y));
                    }
                    animator.SetBool("isSprinting", false);
                    animator.SetBool("isMoving", success);
                    animator.SetFloat("inputX", movementInput.x);
                    animator.SetFloat("inputY", movementInput.y);
                }else{
                    if(!success && movementInput.x > 0){
                    success = TryMove(new Vector2(movementInput.x, 0));
                    }
                    if(!success && movementInput.y > 0){
                    success = TryMove(new Vector2(0, movementInput.y));
                    }

                    animator.SetBool("isMoving", success);
                    animator.SetBool("isSprinting", success);
                    animator.SetFloat("inputX", movementInput.x);
                    animator.SetFloat("inputY", movementInput.y);
                }                 
            }else{
                animator.SetBool("isMoving", false);
                animator.SetBool("isSprinting", false);
            }
        }
    }

    private bool TryMove(Vector2 direction){
        if(direction != Vector2.zero){
        int count = rb.Cast(
                direction, // X Y values between -1 and 1 that rep direction
                movementFilter, //determine where collision occurs
                castCollisions, //list of collisions to store found in collisions into after cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset); //ammount to cast equal to movement plus offset

            if(count == 0){
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }else{
                return false;
            }
        }else{
            //cant move with no direction to move in
            return false;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }
    public void lockMovement(){
        canMove = false;
    }
    public void unlockMovement(){
        canMove = true;
    }
}
