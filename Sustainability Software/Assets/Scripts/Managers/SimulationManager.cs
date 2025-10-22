using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public bool isSimulating { get; set; } = false;
    public RectTransform simulationContainer;
    public List<GameObject> simulationPrefabs;

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
        public void LoadSimulation(string simulationName)
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
        }
        else
        {
            Debug.LogWarning($"Simulation {simulationName} not found");
        }
    }

    public void StartSimulation() => currentSimulation?.StartSimulation();
    public void StopSimulation() => currentSimulation?.StopSimulation();
}
