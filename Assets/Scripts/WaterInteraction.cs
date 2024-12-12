using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterInteraction : MonoBehaviour
{

    [SerializeField]
    private PlayerMovement player;

    [Header("----------------- Water Interactions -----------------")]
    [SerializeField] private AudioClip enemyFalls;
    [SerializeField] private AudioClip playerFalls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

            SoundManager.Instance.Play(enemyFalls);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.Play(playerFalls);
            player.Die();
        }
    }
}
