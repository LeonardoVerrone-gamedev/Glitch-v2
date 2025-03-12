using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManagerScript : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    //0 regular
    //1 planando
    private int currentCameraIndex = 0;

    void Start()
    {
        SwitchCamera(currentCameraIndex);
    }

    public void SwitchCamera(int newCameraIndex){
        cameras[currentCameraIndex].gameObject.SetActive(false);

        currentCameraIndex = newCameraIndex;
        cameras[currentCameraIndex].gameObject.SetActive(true);

    }
}
