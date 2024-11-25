using System;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("Input Events")]
    public RSE_CompleteHole completeHole;
    public RSE_DeclareHole declareHole;

    [Header("Output data")]
    public RSO_CompletedHoles completedHoles;
    public RSO_TotalHoles totalHoles;

    public void OnEnable()
    {
        declareHole.TriggerEvent += DeclareHole;
        completeHole.TriggerEvent += CompleteHole;

        completedHoles.Value = 0;
        totalHoles.Value = 0;
    }

    private void DeclareHole()
    {
        totalHoles.Value++;
    }

    private void CompleteHole()
    {
        completedHoles.Value++;
    }
}