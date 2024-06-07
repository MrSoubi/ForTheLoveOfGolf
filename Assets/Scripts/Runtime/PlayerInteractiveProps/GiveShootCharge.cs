using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveShootCharge : MonoBehaviour
{
    [SerializeField] private int quantityToGive = 1;
    [SerializeField] private int timeReset = 3;

    private bool take;

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(timeReset);

        take = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !take)
        {
            take = true;
            StartCoroutine(Reset());
            PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
            PC.AddShootCharges(quantityToGive);
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
