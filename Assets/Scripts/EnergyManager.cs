using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;                 // Variable referencia al GameObject del tipo GameManager
    [SerializeField] private Slider energyIndicator;                  // Variable referencia al slider de energía
    [SerializeField] private GameObject energyButton;                 // Variable referencia al panel de canvas de energía
    [SerializeField] private TextMeshProUGUI energyTextLevel;         // Variable referencia al texto de nivel de energía
    [SerializeField] private float energyLevel = 50f;                 // Variable nivel de energía inicial

    private float decreaseInterval = 10f;                  // Variable intervalo de tiempo entre disminuciones de Energía
    private float lastDecreaseTime = 0f;                   // Variable tiempo del último decremento de Energía
    private int lastDecreaseMinute = 0;                    // Variable tiempo del primer minuto para modificar el decreaseInterval
    private readonly float minDecreaseAmount = 10f;                 // Variable cantidad mínima a disminuir
    private readonly float maxDecreaseAmount = 30f;                 // Variable cantidad máxima a disminuir
    private const float minEnergy = 0f;                    // Variable mínimo de energía
    private const float maxEnergy = 100f;                  // variable máximo de energía
    private bool canChangeEnergy = false;                  // Variable cambia la energía (false = botón sin accionar, true = botón accionado)
    private bool canChangeStationEnergy;                   // Variable referencia al estado de Energía de la central

    void Start()
    {
        // Busca el GameManager en la escena
        gameManager = FindObjectOfType<GameManager>(); 
        if (gameManager == null)
        {
            Debug.LogError("No se encontró el GameManager en la escena.");
            return;
        }

        // Establece los valores minimos y máximos del indicador de Energía
        // Configura el slider de energía con los valores man y max
        InitEnergyIndicator();
        UpdateEnergyIndicator();
    }

    void Update()
    {
        if(!gameManager.GetGamePaused())
        {
            // Si clic en la tecla "E" => cambia el estado de canChangeEnergy y el color del EnergyButton
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeEnergyButton();
            }

            // Toma el estado de cambio de la Energía de la central
            canChangeStationEnergy = gameManager.GetChangeStationEnergy();

            // Si se puede cambiar el nivel de la energía (canChangeEnergy = true) y se clic en flecha arriba o abajo => actualiza el nivel de energía
            if (canChangeEnergy && canChangeStationEnergy && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
            {
                ChangeEnergy();
            }

            // Actualiza el estado de la central nuclear
            UpdateStationLevel();

            // Actualiza el color del indicador de energía
            UpdateEnergyIndicator();

            // Gestiona el decremento de la Energía en función del tiempo
            DecreaseEnergy();
        }
        
    }

    // Inicia los valores del indicador de Energía
    void InitEnergyIndicator()
    {
        energyIndicator.minValue = minEnergy;
        energyIndicator.maxValue = maxEnergy;
        energyIndicator.value = energyLevel;
        
        // Actualiza el estado de la central nuclear
        gameManager.SetStationEnergyLevel(energyLevel);
    }

    // Gestiona el decremento de Energía
    void DecreaseEnergy()
    {
        // Obtiene el tiempo de la central nuclear sin fallas
        float currentTime = gameManager.GetStationLifeTime();
        //Debug.Log("currentTime" + currentTime);
       
        // Obtiene el minuto actual
        int currentMinute = (int)(currentTime / 60);
        //Debug.Log("currentMinute" + currentMinute);
        
        // Obtiene un valor de decremento de Energía aleatorio entre el mínimo y el máximo
        float decreaseEnergy = Random.Range(minDecreaseAmount, maxDecreaseAmount);

        // Calcula el tiempo transcurrido desde el último decremento
        float timeSinceLastDecrease = currentTime - lastDecreaseTime;

        /* Si el tiempo desde la último decrecimiento >= al intervalo de decrecimiento =>
         *      Actualiza lastDecreaseTime
         *      Si el minuto actual es diferente al último minuto de decremento =>
         *          Actualiza lastDecreaseMinute
         *          Reduce decreaseInterval 1 segundo en cada minuto
         *      Actualiza la el nivel de energía
         *      
         */


        if (timeSinceLastDecrease >= decreaseInterval)
        {
            lastDecreaseTime = currentTime;

            if (currentMinute != lastDecreaseMinute)
            { 
                lastDecreaseMinute = currentMinute;

                decreaseInterval -= 1;

                decreaseEnergy *= 1 + (currentMinute / 100);
                //Debug.Log("decremento por minuto");
            }

            energyLevel -= decreaseEnergy;
            energyLevel = Mathf.Clamp(energyLevel, minEnergy, maxEnergy);
            //Debug.Log("decreaseInterval " + decreaseInterval);
        }
    }

    // Gestiona el botón EnergyButton
    private void ChangeEnergyButton()
    {
        // Toma control de la imagen del button para luego cambiar
        Image energyButtonImage = energyButton.GetComponent<Image>();

        // Toma el estado de cambio del agua de la central
        bool canChangeStationWater = gameManager.GetChangeStationWater();


        // Cambia el estado de la energía (F a T o T a F)
        canChangeEnergy = !canChangeEnergy;

   
        //Si clic en E y la central admite cambiar la Energía => cambia el color del indicador de Energía y se deshabilita el cambio del agua
        if(canChangeEnergy && canChangeStationEnergy == true)
        {
            // Cambia a color verde el indicador de Energía
            energyButtonImage.color = Color.green;

            // Cambia el estado del agua de la central
            gameManager.SetChangeStationWater(false);
        }

        //Si clic en E = true y la central no admite cambiar la Energía => se dehabilita el cambio de Energía
        if (canChangeEnergy == true && canChangeStationEnergy == false && canChangeStationWater == true)
        {
            canChangeEnergy = false;
        }

        // Si clic en E = falso => cambia el color del indicador de Energía, habilita el cambio del agua y deshabilita el cambio de Energía
        if (canChangeEnergy == false && canChangeStationEnergy == true && canChangeStationWater == false)
        {
            // Cambia a color blanco el indicador de Energía
            energyButtonImage.color = Color.white;

            // Cambia el estado del agua de la central
            gameManager.SetChangeStationWater(true);

            // Cambia el estado de la Energía de la central
            //gameManager.SetChangeStationEnergy(false);
        }
        //Debug.Log("clic E "+"e=" + canChangeEnergy + " " + "es=" + canChangeStationEnergy);
        
    }

    // Gestiona los niveles de Energía
    void ChangeEnergy()
    {
        /*
         * Si clic en tecla arriba => incrementa el nivel de energía
         * Sino, Si clic en tecla abajo => decrementa el nivel de energía
         * Limita el nivel de energía entre 0% y 100%
         * Actualiza el valor del slider de energía
         * Actualiza el color del indicador de energía
         * Actualiza el texto del nivel de energía
         */
        if (Input.GetKey(KeyCode.UpArrow))
        {
            energyLevel += 0.05f; 
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            energyLevel -= 0.05f;
        }

        energyLevel = Mathf.Clamp(energyLevel, minEnergy, maxEnergy);        

    }

    // Actualiza el indicador de nivel de Energía
    void UpdateEnergyIndicator()
    {
        // Actualiza el tamaño de la barra del slider según el nivel de energía
        energyIndicator.value = energyLevel;

        // Actualiza el texto de la barra del slider según el nivel de energía
        energyTextLevel.text = energyLevel.ToString("0") + "%";

        // Cambia el color del indicador de energía según el nivel de energía
        // Cambia la cantidad de fallas de la central nuclear
        Color energyColor;

        if (energyLevel >= 95f)
        {
            energyColor = Color.red;
            gameManager.SetFailures(14);
        }
        else if (energyLevel >= 85f)
        {
            energyColor = Color.yellow;
            gameManager.SetFailures(10);

        }
        else if (energyLevel >= 15f)
        {
            energyColor = Color.green;
        }
        else if (energyLevel >= 10f)
        {
            energyColor = Color.yellow;
            gameManager.SetFailures(10);
        }
        else
        {
            energyColor = Color.red;
            gameManager.SetFailures(14);
        }
        // Cambia el color del slider
        energyIndicator.fillRect.GetComponent<Image>().color = energyColor; ;
    }

    // Actualiza el estado de la central nuclear
    private void UpdateStationLevel()
    {
        if(energyLevel >= 100f || energyLevel <= 0f)
        {
            Debug.Log("aplotó la energía");
            gameManager.SetExplotionstation(true);
        }
        gameManager.SetStationEnergyLevel(energyLevel);
    }
}
