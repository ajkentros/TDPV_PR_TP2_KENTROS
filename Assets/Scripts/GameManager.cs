using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] public EnergyManager energyManager;           // Variable referencia al script EnergyManager
    [SerializeField] public Slider stationIndicator;               // Variable referencia al slider de la central nuclear
    [SerializeField] public TextMeshProUGUI stationTextLevel;      // Variable referencia al texto del nivel de la central nuclear
    [SerializeField] public TextMeshProUGUI useTimeText;           // Variable referencia al texto del tiempo transcurrido sin fallas
    [SerializeField] public TextMeshProUGUI failuresText;          // Variable referencia al texto cantidad de fallas

    private float stationLifeTime = 0f;                             // Variable tiempo de vida de la central nuclear sin fallas
    private float stationLevel = 0f;                                // Variable nivel de la central nuclear (valor de la energía + agua + mantenimiento)
    private int failures = 0;                                       // Variable cantidad de fallas en la central nuclear
    private const float minEnergy = 0f;                             // Variable mínimo de la central nuclear
    private const float maxEnergy = 100f;                           // variable máximo de la central nuclear



    // Start is called before the first frame update
    void Start()
    {
        // Instancia el componente EnergyManager en el objeto GameManager
        energyManager = GetComponent<EnergyManager>();

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

        // Obtiene el nivel de energía del EnergyManager y lo asigna a la variable stationLevel
        stationLevel = energyManager.energyLevel;

        // Actualiza indicadores
        UpdateStationIndicator();


        // Comprueba si hay alguna acción adicional que deba realizar el GameManager en función del estado del juego
        //CheckGameState();
    }

    // Establece los valores minimos y máximos del indicador de Energía
    void InitStationIndicator()
    {
        stationLevel = energyManager.energyLevel; 
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
        int hours = (int)(stationLifeTime / 3600);
        int minutes = (int)((stationLifeTime % 3600) / 60);
        int seconds = (int)(stationLifeTime % 60);

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

}

