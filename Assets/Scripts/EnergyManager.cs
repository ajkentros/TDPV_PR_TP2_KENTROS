using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] public Slider energyIndicator;                  // Variable referencia al slider de energía
    [SerializeField] public GameObject energyButton;                 // Variable referencia al panel de canvas de energía
    [SerializeField] public TextMeshProUGUI energyTextLevel;         // Variable referencia al texto de nivel de energía
    [SerializeField] public float energyLevel = 50f;                 // Variable nivel de energía inicial

    private readonly float decreaseInterval = 10f;                  // Variable intervalo de tiempo entre disminuciones de Energía
    private readonly float minDecreaseAmount = 10f;                 // Variable cantidad mínima a disminuir
    private readonly float maxDecreaseAmount = 30f;                 // Variable cantidad máxima a disminuir
    private const float minEnergy = 0f;                             // Variable mínimo de energía
    private const float maxEnergy = 100f;                           // variable máximo de energía
    private bool canChangeEnergy = false;                           // Variable cambia la energía (false = botón sin accionar, true = botón accionado)
   

    void Start()
    {
        // Establece los valores minimos y máximos del indicador de Energía
        // Configura el slider de energía con los valores man y max
        // Gestiona el decremento de la Energía en función del tiempo
        InitEnergyIndicator();
        UpdateEnergyIndicator();
        InvokeRepeating(nameof(DecreaseEnergy), 0f, decreaseInterval); 

    }

    void Update()
    {
        // Si clic en la tecla "E" => cambia el estado de canChangeEnergy y el color del EnergyButton
        if (Input.GetKeyDown(KeyCode.E))
        {
            canChangeEnergy = !canChangeEnergy;

            // Cambia el color del EnergyButton según si se puede cambiar la energía o no
            Image energyButtonImage = energyButton.GetComponent<Image>();
            energyButtonImage.color = canChangeEnergy ? Color.green : Color.white;
        }

        // Si se puede cambiar la energía (canChangeEnergy = true) y se clic en flecha arriba o abajo => actualiza el nivel de energía
        if (canChangeEnergy && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            ChangeEnergy();
        }
       
        // Actualiza el color del indicador de energía
        UpdateEnergyIndicator();


    }
    // Inicia los valores del indicador de Energía
    void InitEnergyIndicator()
    {
        energyIndicator.minValue = minEnergy;
        energyIndicator.maxValue = maxEnergy;
        energyIndicator.value = energyLevel;
    }

    // Gestiona el decremento de Energía
    void DecreaseEnergy()
    {
        // Disminuye la energía

        float decreaseEnergy = Random.Range(minDecreaseAmount, maxDecreaseAmount);
        energyLevel -= decreaseEnergy;
        energyLevel = Mathf.Clamp(energyLevel, minEnergy, maxEnergy);
        
    }
    
    // Gestiona los niveles de Energía
    void ChangeEnergy()
    {
        /*
         * Si clic en tecla arriba => incrementa el nivel de energía en 1%
         * Sino, Si clic en tecla abajo => decrementa el nivel de energía en 1%
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

    void UpdateEnergyIndicator()
    {
        // Actualiza el tamaño de la barra del slider según el nivel de energía
        energyIndicator.value = energyLevel;

        // Actualiza el texto de la barra del slider según el nivel de energía
        energyTextLevel.text = energyLevel.ToString("0") + "%";

        // Cambia el color del indicador de energía según el nivel de energía
        Color energyColor;

        if (energyLevel >= 95f)
        {
            energyColor = Color.red;
        }
        else if (energyLevel >= 85f)
        {
            energyColor = Color.yellow;
        }
        else if (energyLevel >= 15f)
        {
            energyColor = Color.green;
        }
        else if (energyLevel >= 10f)
        {
            energyColor = Color.yellow;
        }
        else
        {
            energyColor = Color.red;
        }
        // Cambia el color del slider
        energyIndicator.fillRect.GetComponent<Image>().color = energyColor; ;
    }
}
