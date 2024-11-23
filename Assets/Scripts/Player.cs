using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variable of type Rigidbody2D, allows us to
    // manipulate the velocity of the player
    private Rigidbody2D body;
    private Animator anim;
    private Collider2D coll;
    private SpriteRenderer sprite;

    // variable to store the movement
    // direction of the player
    private float dirX = 0.0f;

    // variables controlling the velocity at which the
    // player moves or jumps. These can be edited in
    // the players inspector rather than this script
    [SerializeField] private float moveSpeed = 12.0f;
    [SerializeField] private float jumpForce = 12.0f;

    [SerializeField] private LayerMask ground;

    // Start is called before the first frame update
    private void Start()
    {
        // initalize the body variable
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // using Unity's built in Input Manager
        // (see Edit/Project Settings.../Input Manager)
        // detects A, D, left arrow, and right arrow
        // left = -1, right = +1
        dirX = Input.GetAxis("Horizontal");
        // sets the players velocity to +/- the movespeed
        body.velocity = new Vector2(dirX * moveSpeed, body.velocity.y);

        // see above, detects a press of the Space Bar
        if (Input.GetButtonDown("Jump") == true && IsGrounded())
        {
            // makes the players vertical velocity = jumpforce
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }

        UpdateAnim();
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, ground);
    }
    private void UpdateAnim()
    {
        if (dirX > 0)
        {
            anim.SetBool("isMoving", true);
            sprite.flipX = false;
        }
        else if (dirX < 0)
        {
            anim.SetBool("isMoving", true);
            sprite.flipX = true;
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

    }
}

