using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [Header("Output")]
    public RSE_DeclareHole declareHole;
    public RSE_CompleteHole completeHole;

    private bool isCompleted;

    private void Start()
    {
        declareHole.TriggerEvent?.Invoke();
        isCompleted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCompleted && other.CompareTag("Player"))
        {
            Complete();
        }
    }

    [Button]
    private void Complete()
    {
        isCompleted = true;
        completeHole.TriggerEvent?.Invoke();
    }
}