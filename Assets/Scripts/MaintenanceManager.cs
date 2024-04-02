using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceManager : MonoBehaviour
{
    [SerializeField] public GameManager gameManager;                // Variable referencia al GameObject del tipo GameManager
    [SerializeField] public GameObject maintenanceButton;           // Variable referencia al panel de canvas del mantenimiento
    [SerializeField] public TextMeshProUGUI maintenanceInfo;        // Variable referencia al texto del mantenimiento

    private Color normalColor;                                      // Variable referencia al color normal del botón de mantenimiento
    private string originalButtonText;                              // Variable referencia al texto original del botón de mantenimiento
    private bool maintenanceExecute = true;                                // Variable para activar el mantenimiento

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

        InitMaintenance();
    }

    // Update is called once per frame
    void Update()
    {
        // Si clic en la tecla "M" => cambia el estado de la central nuclear y el color del MaintenanceButton
        if (Input.GetKeyDown(KeyCode.M) && maintenanceExecute == true)
        {
            ActivateMaintenance();
        }

        // Chequea si se necesita mantenimiento
        CheckMaintenance();
    }

    // Inicia el mantenimineto
    private void InitMaintenance()
    {
        // Guarda el color normal del botón de mantenimiento 
        normalColor = maintenanceButton.GetComponent<Image>().color;

        // Guardar el texto original del botón de mantenimiento 
        originalButtonText = maintenanceButton.GetComponentInChildren<TextMeshProUGUI>().text;

        // Oculta el texto de información
        maintenanceInfo.enabled = false;
    }

    // Gestiona las tareas de mantenimiento de la central nuclear
    private void ActivateMaintenance()
    {
        // Botón de mantenimiento inhabilitado
        maintenanceExecute = false;

        // Cambia el color del botón de mantenimiento a verde
        maintenanceButton.GetComponent<Image>().color = Color.green;

        // Cambia el texto del botón de mantenimiento
        maintenanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";

        // Muestra el texto del mantenimiento
        maintenanceInfo.text = "In maintenance"; 
        maintenanceInfo.enabled = true;
        
        // Después de 10 segundos, vuelve al color normal y ocultar el texto
        StartCoroutine(ResetMaintenanceIndicator(5f));

    }

    private void CheckMaintenance()
    {
        // Obtiene las fallas acumuladas en la central nuclear
        int failures = gameManager.GetFailures();

        // Si la cantidad de fallas > 2500 =>
        //      se reduce la cantidad de fallas un 10%
        //      pasa al GameManager
        //      habilita el botón de mantenimiento
        
        if (failures > 2500) 
        {
            failures = (int)(failures * 0.9f);
            
            gameManager.SetFailures(failures);
            
            maintenanceExecute = true;
        }
    }

    // Restablecer el indicador de mantenimiento después de cierto tiempo
    private IEnumerator ResetMaintenanceIndicator(float delay)
    {
        // Retasa la ejecución un tiempo = delay
        yield return new WaitForSeconds(delay);

        // Vuelve al color normal del botón de mantenimiento
        maintenanceButton.GetComponent<Image>().color = normalColor;

        // Restaurar el texto original del botón de mantenimiento
        maintenanceButton.GetComponentInChildren<TextMeshProUGUI>().text = originalButtonText;

        // Oculta el texto del mantenimiento de mantenimiento
        maintenanceInfo.enabled = false;

        // Habilita el botón de mantenimiento
        maintenanceExecute = true;
    }
}
