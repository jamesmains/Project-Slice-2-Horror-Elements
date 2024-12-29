using System;
using UnityEngine;
using UnityEngine.Events;

public enum PickupSize {
    OneHand,
    TwoHand
}
public class Pickup : MonoBehaviour {
    [SerializeField] public string PickupName;
    [SerializeField, TextArea] public string PickupDescription; // Todo: this and former should be an SO
    [SerializeField] private PickupSize Size = PickupSize.OneHand;
    [SerializeField] private Vector3 PickupRotation;
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private Collider Col;
    [SerializeField] private Transform CachedParent;
    public UnityEvent OnUse = new();

    private void Awake() {
        CachedParent = transform.parent;
    }

    public void PickUp(Transform handParent) {
        Rb.isKinematic = true;
        Col.enabled = false;
        transform.SetParent(handParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(PickupRotation);
    }

    public void Drop() {
        Rb.isKinematic = false;
        Col.enabled = true;
        transform.parent = CachedParent;
    }
    public void Use() {
        OnUse.Invoke();
    }
}
