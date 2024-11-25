using TMPro;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class CoinPanelBehaviour : MonoBehaviour
{
    [Header("Input")]
    public RSO_CollectedCoins collectedCoins;

    [Header("Settings")]
    [SerializeField] private int coinAmountNeeded;

    [Header("References")]
    [SerializeField] private TMP_Text textCoins;
     
    [Header("Ouput")]
    public RSE_StandardGameEvent panelThresholdReached;

    private void OnEnable()
    {
        collectedCoins.onValueChanged += UpdatePanel;
    }

    private void OnDisable()
    {
        collectedCoins.onValueChanged -= UpdatePanel;
    }

    private void Start()
    {
        UpdatePanel(collectedCoins.Value);
    }

    private void UpdatePanel(int collectedCoinAmount)
    {
        textCoins.text = collectedCoinAmount.ToString() + "/" + coinAmountNeeded.ToString();

        if (collectedCoins.Value >= coinAmountNeeded)
        {
            panelThresholdReached.TriggerEvent?.Invoke();
        }
    }
}
