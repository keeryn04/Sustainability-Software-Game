using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationBase : MonoBehaviour
{
    [SerializeField] private string simulationName;
    public virtual string SimulationName { get => simulationName; protected set => simulationName = value; }

    public virtual void StartSimulation()
    {
        SimulationManager.Instance.isSimulating = true;
        gameObject.SetActive(true);
        Debug.Log($"{SimulationName} started.");
    }

    public virtual void StopSimulation()
    {
        gameObject.SetActive(false);
        Debug.Log($"{SimulationName} stopped.");
    }

    public virtual void Cleanup()
    {
        Destroy(gameObject);
        Debug.Log($"{SimulationName} destroyed.");
    }
}
