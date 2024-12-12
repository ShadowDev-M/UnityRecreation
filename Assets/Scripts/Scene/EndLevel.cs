using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private int loadNextScene;
    // Start is called before the first frame update
    void Start()
    {
        loadNextScene = SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(loadNextScene);

            print("nextlevel :)");

            if (loadNextScene == PlayerPrefs.GetInt("lvlAt"))
            {
                PlayerPrefs.SetInt("lvlAt", loadNextScene);
            }
        }
    }
}
