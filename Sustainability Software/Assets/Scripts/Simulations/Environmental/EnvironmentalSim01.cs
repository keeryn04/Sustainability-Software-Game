using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalSim01 : SimulationBase
{
    public override void StartSimulation()
    {
        Debug.Log("Starting current Environmental Simulation");
        base.StartSimulation();
    }

    public override void StopSimulation()
    {
        Debug.Log("Saving current state before stop");
        base.StopSimulation();
    }
}
