using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [Title("Output events")]
    [SerializeField] RSE_DeclareHole declareHole;
    [SerializeField] RSE_CompleteHole completeHole;

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