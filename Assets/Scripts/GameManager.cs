using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Slider stationIndicator;               // Variable referencia al slider de la central nuclear
    [SerializeField] public TextMeshProUGUI stationTextLevel;      // Variable referencia al texto del nivel de la central nuclear
    [SerializeField] public TextMeshProUGUI useTimeText;           // Variable referencia al texto del tiempo transcurrido sin fallas
    [SerializeField] public TextMeshProUGUI failuresText;          // Variable referencia al texto cantidad de fallas

    [SerializeField] public GameObject stationPanel;                // Variable referencia al StationPanel
    [SerializeField] public GameObject energyPanel;                 // Variable referencia al EnergyPanel
    [SerializeField] public GameObject stationExplosionPanel;        // Variable referencia al StationExplosionPanel

    private float stationLifeTime = 0f;                             // Variable tiempo de vida de la central nuclear sin fallas
    private float stationLevel = 0f;                                // Variable nivel de la central nuclear (valor de la energía + agua + mantenimiento)
    private int failures = 0;                                       // Variable cantidad de fallas en la central nuclear
    private const float minEnergy = 0f;                             // Variable mínimo de la central nuclear
    private const float maxEnergy = 100f;                           // variable máximo de la central nuclear
    private float stationEnergyLevel = 0f;                          // Variable energía inicial
    private float stationWaterLevel = 0f;                           // Variable agua inicial
    private int hours = 0;                                          // Variable horas                              
    private int minutes = 0;                                        // Variable minutos    
    private int seconds = 0;                                        // Variable segundos    

    // Start is called before the first frame update
    void Start()
    {
        // Reanuda el tiempo del juego
        Time.timeScale = 1f;

        // Establece los valores minimos y máximos del indicador de Energía
        // Configura el slider de energía con los valores man y max
        InitStationIndicator();
        UpdateStationIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        // Incrementa el tiempo de vida de la central nuclear sin fallas
        stationLifeTime += Time.deltaTime;

        // Actualiza indicadores
        UpdateStationIndicator();

        // Comprueba si hay alguna acción adicional que deba realizar el GameManager en función del estado del juego
        CheckGameState();
    }

    // Establece los valores minimos y máximos del indicador de Energía
    void InitStationIndicator()
    {
        // Activa los paneles
        stationPanel.SetActive(true);
        energyPanel.SetActive(true);
        stationExplosionPanel.SetActive(false);

        // Establece valores mínimo y máximo del indicador de nivel de la central nuclear en el slider
        stationIndicator.minValue = minEnergy;
        stationIndicator.maxValue = maxEnergy;
        stationIndicator.value = stationLevel;
    }

    // Configura el slider de energía con los valores man y max
    void UpdateStationIndicator()
    {
        // Actualiza el tamaño de la barra del slider según el nivel de energía
        stationIndicator.value = stationLevel;

        // Actualiza el texto de la barra del slider según el nivel de energía
        stationTextLevel.text = stationLevel.ToString("0") + "%";

        // Convierte el tiempo transcurrido en formato de horas:minutos:segundos
        hours = (int)(stationLifeTime / 3600);
        minutes = (int)((stationLifeTime % 3600) / 60);
        seconds = (int)(stationLifeTime % 60);

        // Actualiza el texto del TextMeshProUGUI
        useTimeText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");

        // Actualiza el texto de cantidad de fallas
        failuresText.text = failures.ToString();

        // Cambia el color del indicador de energía según el nivel de energía
        Color energyColor;

        if (stationLevel >= 95f)
        {
            energyColor = Color.red;
        }
        else if (stationLevel >= 85f)
        {
            energyColor = Color.yellow;
        }
        else if (stationLevel >= 15f)
        {
            energyColor = Color.green;
        }
        else if (stationLevel >= 10f)
        {
            energyColor = Color.yellow;
        }
        else
        {
            energyColor = Color.red;
        }
        // Cambia el color del slider
        stationIndicator.fillRect.GetComponent<Image>().color = energyColor; ;
    }

    // Chequea el estado de la central nuclear
    private void CheckGameState()
    {
        /*
         * Si stationExplosion = true o cantidad de fallas >= 50
         *      Avisar que la central nuclear explotó
         *      Reiniciar el juego
         *   
         */
        
        if (stationLevel >= 95f || stationLevel < 2f || failures >= 5000)
        {
            stationPanel.SetActive(false);
            energyPanel.SetActive(false);
            stationExplosionPanel.SetActive(true);
            Debug.Log("explotó");
            Time.timeScale = 0f;    // Pausa el tiempo del juego
            ResetStation();
        }

    }
   
    // Actualiza el estado de la central nuclear
    public void SetStationEnergyLevel (float energyLevels)
    {
        stationEnergyLevel = energyLevels / 2;
        SetStationLevel();
    }
    
    // Actualiza el estado de la central nuclear
    public void SetStationWaterLevel(float waterLevels)
    {
        stationWaterLevel = waterLevels / 2;
        SetStationLevel();
    }

    // Actualiza el estado de la central nuclear
    public void SetStationLevel()
    {
        stationLevel = stationEnergyLevel + stationWaterLevel;
        //Debug.Log("stationLevel " + stationLevel);
    }

    // Get tiempo transcurrido en minutos
    public float GetStationLifeTime()
    {
        return stationLifeTime;
    }

    // Set fallas en la central nuclear
    public void SetFailures(int _failures)
    {
        //failures += (int)(_failures / 2);
        failures += _failures;
    }

    // Get fallas en la central nuclear
    public int GetFailures()
    {
        return failures;
    }

    // Reestablece valores iniciales del juego
    private void ResetStation()
    {
        stationLevel = 0;
        stationLifeTime = 0;
        failures = 0;
        Debug.Log("restauración");
    }

    public void ReturnMenu()
    {
        
        /*
        * Obtiene el índice de la escena actual
        * Calcula el índice de la siguiente escena en orden
        * Carga la siguiente escena en orden
             */

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = (currentSceneIndex - 1) % SceneManager.sceneCountInBuildSettings;

        SceneManager.LoadScene(nextSceneIndex);
        

    }
}

