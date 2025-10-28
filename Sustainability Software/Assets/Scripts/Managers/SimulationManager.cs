using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    public bool isSimulating { get; set; } = false;
    public RectTransform simulationContainer;
    public List<GameObject> simulationPrefabs;
    [SerializeField] private GameObject simButtonClearPrefab;
    [SerializeField] private RectTransform clearContainer;

    private Button simClearButtonInstance;
    private GameObject slideSimButton;
    private SimulationBase currentSimulation;
    public static SimulationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //Destroy duplicate instances
        }
        else
        {
            Instance = this;
        }
    }
    public void LoadSimulation(string simulationName, GameObject triggerButton = null)
    {
        if (currentSimulation != null)
        {
            currentSimulation.Cleanup();
        }

        GameObject simPrefab = simulationPrefabs
            .Find(p => p.GetComponent<SimulationBase>().SimulationName == simulationName);

        if (simPrefab != null)
        {
            GameObject instance = Instantiate(simPrefab, simulationContainer);
            Debug.Log(instance.name);

            //Reset positioning
            RectTransform rt = instance.GetComponent<RectTransform>();
            rt.SetParent(simulationContainer, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;
            rt.localPosition = Vector3.zero;

            currentSimulation = instance.GetComponent<SimulationBase>();
            currentSimulation.StartSimulation();

            //Assign Sim button for stop simulation
            slideSimButton = triggerButton;

            //Disable and hide button
            if (slideSimButton != null)
                slideSimButton.SetActive(false);

            GameObject btnSimClear = Instantiate(simButtonClearPrefab, clearContainer);
            simClearButtonInstance = btnSimClear.GetComponent<Button>();

            simClearButtonInstance.onClick.AddListener(() => this.StopSimulation());
        }
        else
        {
            Debug.LogWarning($"Simulation {simulationName} not found");
        }
    }

    public void StartSimulation() => currentSimulation?.StartSimulation();
    public void StopSimulation()
    {
        if (currentSimulation != null)
        {
            currentSimulation.StopSimulation();
            Destroy(currentSimulation.gameObject);
            currentSimulation = null;
            isSimulating = false;
        }

        //Destroy clear button
        if (simClearButtonInstance != null)
        {
            Destroy(simClearButtonInstance.gameObject);
            simClearButtonInstance = null;
        }

        //Reactivate sim button
        if (slideSimButton != null)
            slideSimButton.SetActive(true);
    }

}
