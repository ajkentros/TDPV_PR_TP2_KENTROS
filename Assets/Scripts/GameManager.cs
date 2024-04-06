using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Slider stationIndicator;               // Variable referencia al slider de la central nuclear
    [SerializeField] private Slider criticalIndicator;              // Variable referencia al slider del estado crítico de la central
    [SerializeField] private TextMeshProUGUI stationTextLevel;      // Variable referencia al texto del nivel de la central nuclear
    [SerializeField] private TextMeshProUGUI useTimeText;           // Variable referencia al texto del tiempo transcurrido sin fallas
    [SerializeField] private TextMeshProUGUI failuresText;          // Variable referencia al texto cantidad de fallas

    [SerializeField] private GameObject stationPanel;                // Variable referencia al StationPanel
    [SerializeField] private GameObject energyPanel;                 // Variable referencia al EnergyPanel
    [SerializeField] private GameObject waterPanel;                 // Variable referencia al EnergyPanel
    [SerializeField] private GameObject maintenancePanel;                 // Variable referencia al EnergyPanel
    [SerializeField] private GameObject stationExplosionPanel;       // Variable referencia al StationExplosionPanel

    private float stationLifeTime = 0f;                             // Variable tiempo de vida de la central nuclear sin fallas
    private float stationLevel = 0f;                                // Variable nivel de la central nuclear (valor de la energía + agua + mantenimiento)
    private float criticalLevel = 0;                                // Variable nivel de la criticidad de la central nuclear (valor de la energía + agua + mantenimiento)
    private int failures = 0;                                       // Variable cantidad de fallas en la central nuclear
    private const int maxFailures = 5000;                           // variable máximo de fallas
    private const float minEnergy = 0f;                             // Variable mínimo de la central nuclear
    private const float maxEnergy = 100f;                           // variable máximo de la central nuclear
    private float stationEnergyLevel = 0f;                          // Variable energía inicial
    private float stationWaterLevel = 0f;                           // Variable agua inicial
    private int hours = 0;                                          // Variable horas                              
    private int minutes = 0;                                        // Variable minutos    
    private int seconds = 0;                                        // Variable segundos    
    private bool canChangeStationEnergy = true;                     // Variable para cambiar la Energía de la central nuclear
    private bool canChangeStationWater = true;                      // Variable para cambiar el agua de la central nuclear
    private bool isGamePaused = false;                              // Variable para controlar la pausa del juego
    private bool isExplotionStation = false;                        // Variable para saber si la cenral nuclear ha explotado

    // Start is called before the first frame update
    void Start()
    {
        // Reanuda el tiempo del juego
        Time.timeScale = 1f;

        // Establece los valores minimos y máximos del indicador de Energía
        // Configura el slider de energía con los valores man y max
        InitStationIndicator();
        UpdateStationIndicators();
    }

    // Update is called once per frame
    void Update()
    {
        // Incrementa el tiempo de vida de la central nuclear sin fallas
        stationLifeTime += Time.deltaTime;

        // Gestiona la pausa del juego
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Actualiza indicadores
        UpdateStationIndicators();

        // Comprueba si hay alguna acción adicional que deba realizar el GameManager en función del estado del juego
        CheckGameState();
    }

    // Establece los valores minimos y máximos del indicador de Energía
    void InitStationIndicator()
    {
        // Activa los paneles
        stationPanel.SetActive(true);
        energyPanel.SetActive(true);
        waterPanel.SetActive(true);
        maintenancePanel.SetActive(true);
        stationExplosionPanel.SetActive(false);

        // Establece valores mínimo y máximo del indicador de nivel de la central nuclear en el slider
        stationIndicator.minValue = minEnergy;
        stationIndicator.maxValue = maxEnergy;
        stationIndicator.value = stationLevel;
        criticalIndicator.value = criticalLevel;
    }

    // Configura el slider de energía con los valores man y max
    void UpdateStationIndicators()
    {
        // Actualiza el tamaño de la barra del slider según el nivel de energía
        stationIndicator.value = stationLevel;

        // Actualiza el tamaño de la barra del slider según la criticidad de la central
        criticalLevel = (float)failures / maxFailures * 360;
        Debug.Log(" failures " +  failures + " criticalLevel " + criticalLevel);
        criticalIndicator.value = criticalLevel;

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
        Color criticalColor = Color.red;

        if (stationLevel >= 95f)
        {
            energyColor = Color.red;
        }
        else if (stationLevel >= 85f)
        {
            energyColor = Color.yellow;
        }
        else if (stationLevel >= 30f)
        {
            energyColor = Color.green;
        }
        else if (stationLevel >= 15f)
        {
            energyColor = Color.yellow;
        }
        else
        {
            energyColor = Color.red;
        }
        // Cambia el color del slider
        stationIndicator.fillRect.GetComponent<Image>().color = energyColor;

        // Colorea el slider dcriticidad de la central
        criticalIndicator.fillRect.GetComponent<Image>().color = criticalColor;
    }

    // Chequea el estado de la central nuclear
    private void CheckGameState()
    {
        /*
         * 
         * Si stationExplosion = true o cantidad de fallas >= 5000
         *      Avisar que la central nuclear explotó
         *      Reiniciar el juego
         */
        
        if (stationLevel >= 95f || stationLevel < 10f || failures >= maxFailures || isExplotionStation)
        {
            stationPanel.SetActive(false);
            energyPanel.SetActive(false);
            waterPanel.SetActive(false);
            maintenancePanel.SetActive(false);
            stationExplosionPanel.SetActive(true);
            //Debug.Log("explotó");
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
        // Si no se pausa el juego => se calcula las fallas y la criticidad de la central nuclear
        if(!isGamePaused)
        {
            failures += (int)(_failures * 0.1);
        }
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
        //Debug.Log("restauración");
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

    // Get ver cambiar la Energía de la central
    public bool GetChangeStationEnergy()
    {
        return canChangeStationEnergy;
    }

    // Set para cambiar la Energía de la central
    public void SetChangeStationEnergy(bool _canChangeStationEnergy)
    {
        canChangeStationEnergy = _canChangeStationEnergy;
    }

    // Get ver cambiar el agua de la central
    public bool GetChangeStationWater()
    {
        return canChangeStationWater;
    }

    // Set para cambiar el agua de la central
    public void SetChangeStationWater(bool _canChangeStationWater)
    {
        canChangeStationWater = _canChangeStationWater;
    }

    // Gestiona la pausa del juego
    public void TogglePause()
    {
        // Si clic en escape =>, establece el timescale a 0
        // Sino => estabablece el timescale a 1
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f; 
    }

    // Get Game Pausado
    public bool GetGamePaused()
    {
        return isGamePaused;
    }

    // Get ExplotionStation
    public void SetExplotionstation(bool _explotionStation)
    {
        isExplotionStation = _explotionStation;
    }
}

