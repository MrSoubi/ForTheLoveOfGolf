using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCamParent : MonoBehaviour
{
    [SerializeField] private Camera cameraTarget;
    [SerializeField] private Camera cameraObject;

    // Update is called once per frame
    void Update()
    {
        cameraObject.fieldOfView = cameraTarget.fieldOfView;
    }
}
