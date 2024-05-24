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
            other.transform.position = new Vector3(0, 0, 0);
        }
    }
}
