using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    private float horizontalInput;
    private float verticalInput;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = transform.Find("Sprite").GetComponent<Animator>();
        sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get player input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized * speed;

        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("X", movement.x);

            animator.SetBool("IsMoving", true);
            
            if (movement.x < 0)
            {
                sprite.flipX = true;
            }
            else if (movement.x > 0)
            {
                sprite.flipX = false;
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement;
    }
}
