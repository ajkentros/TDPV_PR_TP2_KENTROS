using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Gestiona la selección y carga de la escena
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

    // Gestiona el cierre de la aplicación
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
