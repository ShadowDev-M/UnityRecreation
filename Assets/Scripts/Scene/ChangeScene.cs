using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(int sceneID){
        // Menu BGM Code
        if (sceneID > 0) //1
            UIBGM.instance.GetComponent<AudioSource>().Stop();

        // End of Menu BGM Code

        SceneManager.LoadScene(sceneID);
    }

    public void QuitGame(){
        Application.Quit();
    }

    
}