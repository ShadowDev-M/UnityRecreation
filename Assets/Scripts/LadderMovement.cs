using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LadderMovement : MonoBehaviour
{
    private float vertical;
    private float speed = 2f;
    private bool isLadder;
    public bool isClimbing;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D collider2D;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        if (isLadder)
        {
            isClimbing = true;
            collider2D.isTrigger = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isClimbing)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.position = new Vector3(transform.position.x - 1, transform.position.y - 0.5f, 0.0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position = new Vector3(transform.position.x + 1, transform.position.y + 0.5f, 0.0f);
            }
        }


    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(0, vertical * speed);
        }
        else
        {
            rb.gravityScale = 1f;
        }

        animator.SetBool("isClimbing", isClimbing);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
            transform.position = new Vector3(collision.transform.position.x, transform.position.y, 0.0f);

            print("test");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            collider2D.isTrigger = false;

            isLadder = false;
            isClimbing = false;
        }
    }
    
    //public bool ReturnIsClimbing() { return isClimbing; }
}