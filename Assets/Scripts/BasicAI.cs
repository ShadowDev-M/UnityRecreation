using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolBase : MonoBehaviour
{

    [SerializeField] private int moveSpeed = 3;
    [SerializeField] private Transform[] patrolPoint;
    private int targetPoint = 0;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPoint == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolPoint[0].position.x, transform.position.y), moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, new Vector2(patrolPoint[0].position.x, transform.position.y)) < 0.2f)
            {
                sprite.flipX = true;
                targetPoint = 1;
            }
        }

        if (targetPoint == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(patrolPoint[1].position.x, transform.position.y), moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, new Vector2(patrolPoint[1].position.x, transform.position.y)) < 0.2f)
            {
                sprite.flipX = false;
                targetPoint = 0;
            }
        }
    }
}