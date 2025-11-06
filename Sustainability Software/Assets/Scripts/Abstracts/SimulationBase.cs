using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationBase : MonoBehaviour
{
    [SerializeField] private string simulationName;
    public virtual string SimulationName { get => simulationName; protected set => simulationName = value; }

    public virtual void StartSimulation()
    {
        SimulationManager.Instance.SetSimulating(true);
        gameObject.SetActive(true);
    }

    public virtual void StopSimulation()
    {
        gameObject.SetActive(false);
        SimulationManager.Instance.SetSimulating(false);
    }

    public virtual void Cleanup()
    {
        Destroy(gameObject);
    }
}
