using System;
using UnityEngine;

public class Flashlight : MonoBehaviour {
    [SerializeField] private Light Light;
    [SerializeField] private bool FlashlightOn;

    private void Awake() {
        ToggleLight();
    }

    public void ToggleLight() {
        FlashlightOn = !FlashlightOn;
        Light.enabled = FlashlightOn;
    }
}
