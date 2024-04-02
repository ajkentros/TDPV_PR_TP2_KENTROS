using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayNextScene()
    {
        /*
         * Obtiene el índice de la escena actual
         * Calcula el índice de la siguiente escena en orden
         * Carga la siguiente escena en orden
         */

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        SceneManager.LoadScene(nextSceneIndex);
    }

}
