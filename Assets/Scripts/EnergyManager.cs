using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] public Slider energyIndicator;                  // Variable referencia al slider de energ�a
    [SerializeField] public GameObject energyButton;                 // Variable referencia al panel de canvas de energ�a
    [SerializeField] public TextMeshProUGUI energyTextLevel;         // Variable referencia al texto de nivel de energ�a
    [SerializeField] public float energyLevel = 50f;                 // Variable nivel de energ�a inicial

    private readonly float decreaseInterval = 10f;                  // Variable intervalo de tiempo entre disminuciones de Energ�a
    private readonly float minDecreaseAmount = 10f;                 // Variable cantidad m�nima a disminuir
    private readonly float maxDecreaseAmount = 30f;                 // Variable cantidad m�xima a disminuir
    private const float minEnergy = 0f;                             // Variable m�nimo de energ�a
    private const float maxEnergy = 100f;                           // variable m�ximo de energ�a
    private bool canChangeEnergy = false;                           // Variable cambia la energ�a (false = bot�n sin accionar, true = bot�n accionado)
   

    void Start()
    {
        // Establece los valores minimos y m�ximos del indicador de Energ�a
        // Configura el slider de energ�a con los valores man y max
        // Gestiona el decremento de la Energ�a en funci�n del tiempo
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

            // Cambia el color del EnergyButton seg�n si se puede cambiar la energ�a o no
            Image energyButtonImage = energyButton.GetComponent<Image>();
            energyButtonImage.color = canChangeEnergy ? Color.green : Color.white;
        }

        // Si se puede cambiar la energ�a (canChangeEnergy = true) y se clic en flecha arriba o abajo => actualiza el nivel de energ�a
        if (canChangeEnergy && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)))
        {
            ChangeEnergy();
        }
       
        // Actualiza el color del indicador de energ�a
        UpdateEnergyIndicator();


    }
    // Inicia los valores del indicador de Energ�a
    void InitEnergyIndicator()
    {
        energyIndicator.minValue = minEnergy;
        energyIndicator.maxValue = maxEnergy;
        energyIndicator.value = energyLevel;
    }

    // Gestiona el decremento de Energ�a
    void DecreaseEnergy()
    {
        // Disminuye la energ�a

        float decreaseEnergy = Random.Range(minDecreaseAmount, maxDecreaseAmount);
        energyLevel -= decreaseEnergy;
        energyLevel = Mathf.Clamp(energyLevel, minEnergy, maxEnergy);
        
    }
    
    // Gestiona los niveles de Energ�a
    void ChangeEnergy()
    {
        /*
         * Si clic en tecla arriba => incrementa el nivel de energ�a en 1%
         * Sino, Si clic en tecla abajo => decrementa el nivel de energ�a en 1%
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

    void UpdateEnergyIndicator()
    {
        // Actualiza el tama�o de la barra del slider seg�n el nivel de energ�a
        energyIndicator.value = energyLevel;

        // Actualiza el texto de la barra del slider seg�n el nivel de energ�a
        energyTextLevel.text = energyLevel.ToString("0") + "%";

        // Cambia el color del indicador de energ�a seg�n el nivel de energ�a
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
