using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;                 // Variable referencia al GameObject del tipo GameManager
    [SerializeField] private Slider energyIndicator;                  // Variable referencia al slider de energ�a
    [SerializeField] private GameObject energyButton;                 // Variable referencia al panel de canvas de energ�a
    [SerializeField] private TextMeshProUGUI energyTextLevel;         // Variable referencia al texto de nivel de energ�a
    [SerializeField] private float energyLevel = 50f;                 // Variable nivel de energ�a inicial

    private float decreaseInterval = 10f;                  // Variable intervalo de tiempo entre disminuciones de Energ�a
    private float lastDecreaseTime = 0f;                   // Variable tiempo del �ltimo decremento de Energ�a
    private int lastDecreaseMinute = 0;                    // Variable tiempo del primer minuto para modificar el decreaseInterval
    private readonly float minDecreaseAmount = 10f;                 // Variable cantidad m�nima a disminuir
    private readonly float maxDecreaseAmount = 30f;                 // Variable cantidad m�xima a disminuir
    private const float minEnergy = 0f;                    // Variable m�nimo de energ�a
    private const float maxEnergy = 100f;                  // variable m�ximo de energ�a
    private bool canChangeEnergy = false;                  // Variable cambia la energ�a (false = bot�n sin accionar, true = bot�n accionado)
    private bool canChangeStationEnergy;                   // Variable referencia al estado de Energ�a de la central

    void Start()
    {
        // Busca el GameManager en la escena
        gameManager = FindObjectOfType<GameManager>(); 
        if (gameManager == null)
        {
            Debug.LogError("No se encontr� el GameManager en la escena.");
            return;
        }

        // Establece los valores minimos y m�ximos del indicador de Energ�a
        // Configura el slider de energ�a con los valores man y max
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

            // Toma el estado de cambio de la Energ�a de la central
            canChangeStationEnergy = gameManager.GetChangeStationEnergy();

            // Si se puede cambiar el nivel de la energ�a (canChangeEnergy = true) y se clic en flecha arriba o abajo => actualiza el nivel de energ�a
            if (canChangeEnergy && canChangeStationEnergy && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
            {
                ChangeEnergy();
            }

            // Actualiza el estado de la central nuclear
            UpdateStationLevel();

            // Actualiza el color del indicador de energ�a
            UpdateEnergyIndicator();

            // Gestiona el decremento de la Energ�a en funci�n del tiempo
            DecreaseEnergy();
        }
        
    }

    // Inicia los valores del indicador de Energ�a
    void InitEnergyIndicator()
    {
        energyIndicator.minValue = minEnergy;
        energyIndicator.maxValue = maxEnergy;
        energyIndicator.value = energyLevel;
        
        // Actualiza el estado de la central nuclear
        gameManager.SetStationEnergyLevel(energyLevel);
    }

    // Gestiona el decremento de Energ�a
    void DecreaseEnergy()
    {
        // Obtiene el tiempo de la central nuclear sin fallas
        float currentTime = gameManager.GetStationLifeTime();
        //Debug.Log("currentTime" + currentTime);
       
        // Obtiene el minuto actual
        int currentMinute = (int)(currentTime / 60);
        //Debug.Log("currentMinute" + currentMinute);
        
        // Obtiene un valor de decremento de Energ�a aleatorio entre el m�nimo y el m�ximo
        float decreaseEnergy = Random.Range(minDecreaseAmount, maxDecreaseAmount);

        // Calcula el tiempo transcurrido desde el �ltimo decremento
        float timeSinceLastDecrease = currentTime - lastDecreaseTime;

        /* Si el tiempo desde la �ltimo decrecimiento >= al intervalo de decrecimiento =>
         *      Actualiza lastDecreaseTime
         *      Si el minuto actual es diferente al �ltimo minuto de decremento =>
         *          Actualiza lastDecreaseMinute
         *          Reduce decreaseInterval 1 segundo en cada minuto
         *      Actualiza la el nivel de energ�a
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

    // Gestiona el bot�n EnergyButton
    private void ChangeEnergyButton()
    {
        // Toma control de la imagen del button para luego cambiar
        Image energyButtonImage = energyButton.GetComponent<Image>();

        // Toma el estado de cambio del agua de la central
        bool canChangeStationWater = gameManager.GetChangeStationWater();


        // Cambia el estado de la energ�a (F a T o T a F)
        canChangeEnergy = !canChangeEnergy;

   
        //Si clic en E y la central admite cambiar la Energ�a => cambia el color del indicador de Energ�a y se deshabilita el cambio del agua
        if(canChangeEnergy && canChangeStationEnergy == true)
        {
            // Cambia a color verde el indicador de Energ�a
            energyButtonImage.color = Color.green;

            // Cambia el estado del agua de la central
            gameManager.SetChangeStationWater(false);
        }

        //Si clic en E = true y la central no admite cambiar la Energ�a => se dehabilita el cambio de Energ�a
        if (canChangeEnergy == true && canChangeStationEnergy == false && canChangeStationWater == true)
        {
            canChangeEnergy = false;
        }

        // Si clic en E = falso => cambia el color del indicador de Energ�a, habilita el cambio del agua y deshabilita el cambio de Energ�a
        if (canChangeEnergy == false && canChangeStationEnergy == true && canChangeStationWater == false)
        {
            // Cambia a color blanco el indicador de Energ�a
            energyButtonImage.color = Color.white;

            // Cambia el estado del agua de la central
            gameManager.SetChangeStationWater(true);

            // Cambia el estado de la Energ�a de la central
            //gameManager.SetChangeStationEnergy(false);
        }
        //Debug.Log("clic E "+"e=" + canChangeEnergy + " " + "es=" + canChangeStationEnergy);
        
    }

    // Gestiona los niveles de Energ�a
    void ChangeEnergy()
    {
        /*
         * Si clic en tecla arriba => incrementa el nivel de energ�a
         * Sino, Si clic en tecla abajo => decrementa el nivel de energ�a
         * Limita el nivel de energ�a entre 0% y 100%
         * Actualiza el valor del slider de energ�a
         * Actualiza el color del indicador de energ�a
         * Actualiza el texto del nivel de energ�a
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

    // Actualiza el indicador de nivel de Energ�a
    void UpdateEnergyIndicator()
    {
        // Actualiza el tama�o de la barra del slider seg�n el nivel de energ�a
        energyIndicator.value = energyLevel;

        // Actualiza el texto de la barra del slider seg�n el nivel de energ�a
        energyTextLevel.text = energyLevel.ToString("0") + "%";

        // Cambia el color del indicador de energ�a seg�n el nivel de energ�a
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
            Debug.Log("aplot� la energ�a");
            gameManager.SetExplotionstation(true);
        }
        gameManager.SetStationEnergyLevel(energyLevel);
    }
}
