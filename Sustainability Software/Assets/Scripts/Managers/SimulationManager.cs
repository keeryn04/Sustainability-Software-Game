using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    [Header("Simulation Setup")]
    public RectTransform simulationContainer;
    public List<GameObject> simulationPrefabs;
    [SerializeField] private GameObject simButtonClearPrefab;
    [SerializeField] private RectTransform clearContainer;

    public bool isSimulating { get; private set; } = false;

    public void SetSimulating(bool value)
    {
        isSimulating = value;
    }

    private SimulationBase currentSimulation;
    private Button simClearButtonInstance;
    private GameObject slideSimButton; //original trigger button

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LoadSimulation(string simulationName, GameObject triggerButton = null)
    {
        StopSimulation(); //ensure no simulation is running

        GameObject prefab = simulationPrefabs.Find(p => p.GetComponent<SimulationBase>().SimulationName == simulationName);
        if (prefab == null)
        {
            Debug.LogWarning($"Simulation {simulationName} not found");
            return;
        }

        GameObject instance = Instantiate(prefab, simulationContainer);
        instance.GetComponent<RectTransform>().SetParent(simulationContainer, false);

        currentSimulation = instance.GetComponent<SimulationBase>();
        currentSimulation.StartSimulation();
        isSimulating = true;

        slideSimButton = triggerButton;
        if (slideSimButton != null)
            slideSimButton.SetActive(false);

        CreateClearButton();
    }

    private void CreateClearButton()
    {
        GameObject btnObj = Instantiate(simButtonClearPrefab, clearContainer);
        simClearButtonInstance = btnObj.GetComponent<Button>();
        simClearButtonInstance.onClick.AddListener(StopSimulation);
    }

    public void StartSimulation()
    {
        currentSimulation?.StartSimulation();
        isSimulating = true;
    }

    public void StopSimulation()
    {
        if (currentSimulation != null)
        {
            currentSimulation.StopSimulation();
            Destroy(currentSimulation.gameObject);
            currentSimulation = null;
        }

        isSimulating = false;

        if (simClearButtonInstance != null)
        {
            Destroy(simClearButtonInstance.gameObject);
            simClearButtonInstance = null;
        }

        if (slideSimButton != null)
            slideSimButton.SetActive(true);
    }
}

