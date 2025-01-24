using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour {
    [SerializeField] public Transform mainCamera;
    [SerializeField] Canvas healthBarCanvas;

    void Start () {
        mainCamera = Camera.main.transform;
    }

    void Update () {
        //Constantly turn the health bars to face the Player
        healthBarCanvas.transform.LookAt (mainCamera.position, Vector3.up);
    }

    public void UpdateHealthValue (float _incomingValue) {
        healthBarCanvas.GetComponentInChildren<Image> ().fillAmount = _incomingValue;
    }
}