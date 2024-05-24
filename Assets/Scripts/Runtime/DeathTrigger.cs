using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //GameManager.instance.Respawn(other.gameObject);
            other.transform.position = new Vector3(2.5f, 0.5f, 2f);
        }
    }
}
