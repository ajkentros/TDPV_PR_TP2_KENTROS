using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;                // Variable referencia al GameObject del tipo GameManager
    [SerializeField] private Slider waterIndicator;                  // Variable referencia al slider de agua refrigerante
    [SerializeField] private GameObject waterButton;                 // Variable referencia al panel de canvas de agua refrigerante
    [SerializeField] private TextMeshProUGUI waterTextLevel;         // Variable referencia al texto de nivel de enagua refrigeranteerg�a
    [SerializeField] private float waterLevel = 60f;                 // Variable nivel de agua refrigerante inicial

    private float decreaseInterval = 20f;                   // Variable intervalo de tiempo entre disminuciones de agua refrigerante
    private float lastDecreaseTime = 0f;                    // Variable tiempo del �ltimo decremento de agua refrigerante
    private int lastDecreaseMinute = 0;                     // Variable tiempo del primer minuto para modificar el decreaseInterval
    private readonly float minDecreaseAmount = 10f;                  // Variable cantidad m�nima a disminuir
    private readonly float maxDecreaseAmount = 20f;                  // Variable cantidad m�xima a disminuir
    private const float minWater = 0f;                      // Variable m�nimo de agua refrigerante
    private const float maxWater = 100f;                    // variable m�ximo de agua refrigerante
    private bool canChangeWater = false;                    // Variable cambia la agua refrigerante (false = bot�n sin accionar, true = bot�n accionado)
    private bool canChangeStationWater;                     // Variable referencia al estado del agua de la central


    // Start is called before the first frame update
    void Start()
    {
        // Busca el GameManager en la escena
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No se encontr� el GameManager en la escena.");
            return;
        }

        // Establece los valores minimos y m�ximos del indicador de agua refrigerante
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

        // Toma el estado de cambio del agua de la central
        canChangeStationWater = gameManager.GetChangeStationWater();

        // Si se puede cambiar el nivel del agua refrigerante (canChangeWater = true) y se clic en flecha arriba o abajo => actualiza el nivel de agua refrigerante
        if (canChangeWater && canChangeStationWater && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            ChangeWater();
        }

        // Actualiza el estado de la central nuclear
        UpdateStationLevel();
        
        // Actualiza el color del indicador de energ�a
        UpdateWaterIndicator();

        // Gestiona el decremento de la Energ�a en funci�n del tiempo
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
        // Actualiza el tama�o de la barra del slider seg�n el nivel de agua refrigerante
        waterIndicator.value = waterLevel;

        // Actualiza el texto de la barra del slider seg�n el nivel de agua refrigerante
        waterTextLevel.text = waterLevel.ToString("0") + "%";

        // Cambia el color del indicador de energ�a seg�n el nivel de agua refrigerante
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
        else if (waterLevel >= 40f)
        {
            waterColor = Color.green;
        }
        else if (waterLevel >= 20f)
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

    // Gestiona el bot�n EnergyButton
    private void ChangeWaterButton()
    {
        // Toma control de la imagen del button para luego cambiar
        Image waterButtonImage = waterButton.GetComponent<Image>();

        // Toma el estado de cambio del agua de la central
        bool canChangeStationEnergy = gameManager.GetChangeStationEnergy();

        // Cambia el estado del agua (F a T o T a F)
        canChangeWater = !canChangeWater;

        // Si clic en A la central admite cambiar el agua => cambia el color del indicador de agua y se deshabilita el cambio del energ�a
        if (canChangeWater && canChangeStationWater == true)
        {
            // Cambia a color azul el indicador de agua
            waterButtonImage.color = Color.blue;

            // Cambia el estado del agua de la central
            gameManager.SetChangeStationEnergy(false);
        }
        
        // Si clic en A = true y la central no admite cambiar el agua => se dehabilita el cambio de agua
        if (canChangeWater == true && canChangeStationWater == false && canChangeStationEnergy == true)
        {
            canChangeWater = false;
        }

        // Si clic en A = falso => cambia el color del indicador del agua, habilita el cambio de Energ�a y deshabilita el cambio de agua
        if (canChangeWater == false && canChangeStationWater == true && canChangeStationEnergy == false)
        {
            // Cambia a color blanco el indicador de agua
            waterButtonImage.color = Color.white;

            // Cambia el estado de la Energ�a de la central
            gameManager.SetChangeStationEnergy(true);

            // Cambia el estado del agua de la central
            //gameManager.SetChangeStationWater(false);
        }
        Debug.Log("clic A " + "w=" + canChangeWater + " " + "ws=" + canChangeStationWater);
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
            waterLevel += 0.1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            waterLevel -= 0.1f;
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

        // Obtiene un valor de decremento de agua refrigerante aleatorio entre el m�nimo y el m�ximo
        float decreaseWater = Random.Range(minDecreaseAmount, maxDecreaseAmount);

        // Calcula el tiempo transcurrido desde el �ltimo decremento
        float timeSinceLastDecrease = currentTime - lastDecreaseTime;

        /* Si el tiempo desde la �ltimo decrecimiento >= al intervalo de decrecimiento =>
         *      Actualiza lastDecreaseTime
         *      Si el minuto actual es diferente al �ltimo minuto de decremento =>
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
            // Setea la cantidad de fallas para que explote la central nuclear
            gameManager.SetFailures(5001);
        }else gameManager.SetStationWaterLevel(waterLevel);
    }

}
