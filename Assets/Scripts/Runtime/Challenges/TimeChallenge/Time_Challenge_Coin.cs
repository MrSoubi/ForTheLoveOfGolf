using UnityEngine;

public class TimeChallengeCoin : MonoBehaviour
{
    [Header("References")]
    public GameObject mesh;
    public TimeChallenge challenge;

    [Header("Settings")]
    public int value;

    [Header("__DEBUG__")]
    public bool isPickingUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickingUp)
        {
            challenge.CollectCoin(this);
            isPickingUp = true;
        }
    }
}