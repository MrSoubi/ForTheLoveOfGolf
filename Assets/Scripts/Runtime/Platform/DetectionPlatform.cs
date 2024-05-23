using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionPlatform : MonoBehaviour
{
    [SerializeField] SurfacePlatformData data;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            print("SET PLAYER ¨STATISTICS");

            switch (data.type)
            {
                case PlatformType.Slow:
                    break;
                case PlatformType.Ice:
                    break;
            }
        }
    }
}