using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Variable of type Rigidbody2D, allows us to
    // manipulate the velocity of the player
    private Rigidbody2D body;
    private Animator anim;
    private Collider2D coll;
    private SpriteRenderer sprite;
    private float lastTime;
    // variable to store the movement
    // direction of the player
    private float dirX = 0.0f;

    // variables controlling the velocity at which the
    // player moves or jumps. These can be edited in
    // the players inspector rather than this script
    [SerializeField] private float moveSpeed = 12.0f;
    [SerializeField] private float jumpForce = 12.0f;

    [SerializeField] private LayerMask ground;


    [SerializeField] private AudioClip callBlobClip1;
    [SerializeField] private AudioClip callBlobClip2;
    [SerializeField] private AudioClip callBlobClip3;
    [SerializeField] private AudioClip callBlobClip4;
    [SerializeField] private AudioClip callBlobClip5;
    [SerializeField] private AudioClip callBlobClip6;
    [SerializeField] private AudioClip callBlobClip7;
    [SerializeField] private AudioClip callBlobClip8;
    [SerializeField] private AudioClip callBlobClip9;
    [SerializeField] private AudioClip callBlobClip10;
    [SerializeField] private AudioClip callBlobClip11;
    [SerializeField] private AudioClip callBlobClip12;

    [SerializeField] private AudioClip overHere1;
    [SerializeField] private AudioClip overHere2;

    [SerializeField] private AudioClip thisWay1;
    [SerializeField] private AudioClip thisWay2;

    [SerializeField] private AudioClip comeHere1;
    [SerializeField] private AudioClip comeHere2;




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

            anim.SetTrigger("Jumping");
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            SoundManager.Instance.RandomSoundEffect(new AudioClip[] { overHere1, overHere2, comeHere1, comeHere2, thisWay1, thisWay2});

            anim.SetTrigger("CallBlob");

        }

        if (!Input.GetKey(KeyCode.E)) lastTime = Time.time;
        else if (Time.time - lastTime >= 1)
        {
            lastTime = 10000000;
            SoundManager.Instance.RandomSoundEffect(new AudioClip[] { callBlobClip1, callBlobClip2, callBlobClip3, callBlobClip4, callBlobClip5, callBlobClip6, callBlobClip7, callBlobClip8, callBlobClip9, callBlobClip10, callBlobClip11, callBlobClip12 });
            anim.SetTrigger("CallBlob");

        }

        UpdateAnim();
    }

    private bool IsGrounded()
    {
        if (Mathf.Abs(body.velocity.y) > 0.2) return false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("PlayerDies");

        //body.bodyType = RigidbodyType2D.Static;
        anim.applyRootMotion = true;
        Invoke("RestartLevel", 2.0f);
    }
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void FixedUpdate()
    {
        if (anim)
        {
            anim.SetFloat("Speed", Mathf.Abs(body.velocity.x));
            
            anim.SetBool("isGrounded", IsGrounded());
        }
    }
}

