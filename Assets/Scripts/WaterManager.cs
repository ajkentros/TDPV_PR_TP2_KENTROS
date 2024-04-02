using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterManager : MonoBehaviour
{
    [SerializeField] public GameManager gameManager;                // Variable referencia al GameObject del tipo GameManager
    [SerializeField] public Slider waterIndicator;                  // Variable referencia al slider de agua refrigerante
    [SerializeField] public GameObject waterButton;                 // Variable referencia al panel de canvas de agua refrigerante
    [SerializeField] public TextMeshProUGUI waterTextLevel;         // Variable referencia al texto de nivel de enagua refrigeranteergía
    [SerializeField] public float waterLevel = 60f;                 // Variable nivel de agua refrigerante inicial

    private float decreaseInterval = 20f;                   // Variable intervalo de tiempo entre disminuciones de agua refrigerante
    private float lastDecreaseTime = 0f;                    // Variable tiempo del último decremento de agua refrigerante
    private int lastDecreaseMinute = 0;                     // Variable tiempo del primer minuto para modificar el decreaseInterval
    private readonly float minDecreaseAmount = 10f;                  // Variable cantidad mínima a disminuir
    private readonly float maxDecreaseAmount = 20f;                  // Variable cantidad máxima a disminuir
    private const float minWater = 0f;                      // Variable mínimo de agua refrigerante
    private const float maxWater = 100f;                    // variable máximo de agua refrigerante
    private bool canChangeWater = false;                    // Variable cambia la agua refrigerante (false = botón sin accionar, true = botón accionado)


    // Start is called before the first frame update
    void Start()
    {
        // Busca el GameManager en la escena
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No se encontró el GameManager en la escena.");
            return;
        }

        // Establece los valores minimos y máximos del indicador de agua refrigerante
        // Configura el slider de agua refrigerante con los valores man y max
        InitWaterIndicator();
        UpdateWaterIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        // Si clic en la tecla "A" => cambia el estado de canChangeWater y el color del WaterButton
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeWaterButton();
        }

        // Si se puede cambiar el nivel del agua refrigerante (canChangeWater = true) y se clic en flecha arriba o abajo => actualiza el nivel de agua refrigerante
        if (canChangeWater && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            ChangeWater();
        }

        // Actualiza el estado de la central nuclear
        UpdateStationLevel();
        
        // Actualiza el color del indicador de energía
        UpdateWaterIndicator();

        // Gestiona el decremento de la Energía en función del tiempo
        DecreaseWater();
    }

    // Inicia los valores del indicador de agua refrigerante
    void InitWaterIndicator()
    {
        waterIndicator.minValue = minWater;
        waterIndicator.maxValue = maxWater;
        waterIndicator.value = waterLevel;

        // Actualiza el estado de la central nuclear
        gameManager.SetStationWaterLevel(waterLevel);
    }

    // Actualiza el indicador de nivel de agua refrigerante
    void UpdateWaterIndicator()
    {
        // Actualiza el tamaño de la barra del slider según el nivel de agua refrigerante
        waterIndicator.value = waterLevel;

        // Actualiza el texto de la barra del slider según el nivel de agua refrigerante
        waterTextLevel.text = waterLevel.ToString("0") + "%";

        // Cambia el color del indicador de energía según el nivel de agua refrigerante
        // Cambia la cantidad de fallas de la central nuclear
        Color waterColor;

        if (waterLevel >= 90f)
        {
            waterColor = Color.red;
            gameManager.SetFailures(10);
        }
        else if (waterLevel >= 80f)
        {
            waterColor = Color.yellow;
            gameManager.SetFailures(1);

        }
        else if (waterLevel >= 25f)
        {
            waterColor = Color.green;
        }
        else if (waterLevel >= 15f)
        {
            waterColor = Color.yellow;
            gameManager.SetFailures(1);
        }
        else
        {
            waterColor = Color.red;
            gameManager.SetFailures(10);
        }
        // Cambia el color del slider
        waterIndicator.fillRect.GetComponent<Image>().color = waterColor;
    }

    // Gestiona el botón EnergyButton
    private void ChangeWaterButton()
    {
        canChangeWater = !canChangeWater;

        // Cambia el color del WaterButton según si se puede cambiar el nivel de agua refrigerante
        Image waterButtonImage = waterButton.GetComponent<Image>();
        waterButtonImage.color = canChangeWater ? Color.blue : Color.white;
    }

    // Gestiona los niveles de agua refrigerante
    void ChangeWater()
    {
        /*
         * Si clic en tecla arriba => incrementa el nivel de agua refrigerante
         * Sino, Si clic en tecla abajo => decrementa el nivel de agua refrigerante
         * Limita el nivel de agua refrigerante entre 0% y 100%
         * Actualiza el valor del slider de agua refrigerante
         * Actualiza el color del indicador de agua refrigerante
         * Actualiza el texto del nivel de agua refrigerante
         */
        if (Input.GetKey(KeyCode.UpArrow))
        {
            waterLevel += 0.8f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            waterLevel -= 0.8f;
        }

        waterLevel = Mathf.Clamp(waterLevel, minWater, maxWater);
    }

    // Gestiona el decremento de agua refrigerante
    void DecreaseWater()
    {
        // Obtiene el tiempo de la central nuclear sin fallas
        float currentTime = gameManager.GetStationLifeTime();
        //Debug.Log("currentTime" + currentTime);

        // Obtiene el minuto actual
        int currentMinute = (int)(currentTime / 60);
        //Debug.Log("currentMinute" + currentMinute);

        // Obtiene un valor de decremento de agua refrigerante aleatorio entre el mínimo y el máximo
        float decreaseWater = Random.Range(minDecreaseAmount, maxDecreaseAmount);

        // Calcula el tiempo transcurrido desde el último decremento
        float timeSinceLastDecrease = currentTime - lastDecreaseTime;

        /* Si el tiempo desde la último decrecimiento >= al intervalo de decrecimiento =>
         *      Actualiza lastDecreaseTime
         *      Si el minuto actual es diferente al último minuto de decremento =>
         *          Actualiza lastDecreaseMinute
         *          Reduce decreaseInterval 1 segundo en cada minuto
         *      Actualiza la el nivel de agua refrigerante
         *      
         */

        if (timeSinceLastDecrease >= decreaseInterval)
        {
            lastDecreaseTime = currentTime;

            if (currentMinute != lastDecreaseMinute)
            {
                lastDecreaseMinute = currentMinute;

                decreaseInterval -= 1;

                decreaseWater *= 1 + (currentMinute / 100);
                //Debug.Log("decremento por minuto");
            }

            waterLevel -= decreaseWater;
            waterLevel = Mathf.Clamp(waterLevel, minWater, maxWater);
            //Debug.Log("decreaseInterval " + decreaseInterval);
        }
    }

    // Actualiza el estado de la central nuclear
    private void UpdateStationLevel()
    {
        if(waterLevel > 99f || waterLevel < 1)
        {
            gameManager.SetFailures(5001);
        }else gameManager.SetStationWaterLevel(waterLevel);
    }

}
