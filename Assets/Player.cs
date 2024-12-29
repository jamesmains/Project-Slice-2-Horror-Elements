using System;
using TMPro;
using UnityEngine;

public enum WhichHand {
    Left,
    Right,
}

public enum PlayerState {
    Active,
    Focus,
    Inactive
}

public class Player : MonoBehaviour {
    [SerializeField] public PlayerState CurrentState = PlayerState.Active;
    [SerializeField] private TextMeshProUGUI InteractText;
    [SerializeField] private LayerMask PickupLayer;
    [SerializeField] private Pickup CurrentPickupTarget;
    [SerializeField] private float PickupRange = 3f;
    [SerializeField] private Hand LeftHand;
    [SerializeField] private Hand RightHand;

    [SerializeField] private GameObject FocusMenu;
    [SerializeField] private TextMeshProUGUI FocusObjectTitleText;
    [SerializeField] private TextMeshProUGUI FocusObjectDescriptionText;
    [SerializeField] private MeshFilter FocusObjectFilter;
    [SerializeField] private MeshRenderer FocusObjectRenderer;

    private InputSystem_Actions playerInput;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable() {
        playerInput ??= new InputSystem_Actions();
        playerInput.Enable();
    }

    private void OnDisable() {
        if (playerInput != null) {
            playerInput.Disable();
        }
    }

    private void FixedUpdate() {
        LookForPickup();
    }

    private void LookForPickup() {
        var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        var origin = Camera.main.ScreenToWorldPoint(screenCenter);
        if (Physics.Raycast(origin, Camera.main.gameObject.transform.forward, out var hit, PickupRange,
                PickupLayer)) {
            CurrentPickupTarget = hit.collider.GetComponent<Pickup>();
        }
        else CurrentPickupTarget = null;
    }

    public void TryUse(WhichHand whichHand) {
        if (whichHand == WhichHand.Left) {
            LeftHand.TryUse();
        }
        else if (whichHand == WhichHand.Right) {
            RightHand.TryUse();
        }
    }

    public void TrySetHeld(WhichHand whichHand) {
        if (whichHand == WhichHand.Left) {
            LeftHand.SetHeld(CurrentPickupTarget);
        }
        else if (whichHand == WhichHand.Right) {
            RightHand.SetHeld(CurrentPickupTarget);
        }
    }

    /// <summary>
    /// Many issues with this.
    /// 1.) It currently REQUIRES target to be a pickup
    /// 2.) Many things going on in one function
    /// 3.) Should be in its own class.
    /// </summary>
    public void TryFocus() {
        if (CurrentPickupTarget == null) {
            CurrentState = PlayerState.Active;
            FocusMenu.SetActive(false);
            return;
        }

        if (CurrentState != PlayerState.Focus) {
            FocusMenu.SetActive(true);
            CurrentState = PlayerState.Focus;
            FocusObjectTitleText.text = CurrentPickupTarget.PickupName;
            FocusObjectDescriptionText.text = CurrentPickupTarget.PickupDescription;
        }
        else {
            CurrentState = PlayerState.Active;
            FocusMenu.SetActive(false);
            return;
        }
        var targetFilter = CurrentPickupTarget.gameObject.GetComponent<MeshFilter>();
        var targetMeshRenderer = CurrentPickupTarget.gameObject.GetComponent<MeshRenderer>();
        FocusObjectFilter.mesh = targetFilter.mesh;
        FocusObjectRenderer.material = targetMeshRenderer.material;
    }
}

[Serializable]
public class Hand {
    public Transform HandTransform;
    private Pickup HeldItem;
    public bool IsFree() => HeldItem == null;

    public void SetHeld(Pickup pickup) {
        if (!IsFree())
            HeldItem.Drop();
        HeldItem = pickup;
        HeldItem?.PickUp(HandTransform);
    }

    public void Drop() {
        if (IsFree()) return;
        HeldItem.Drop();
        SetHeld(null);
    }

    public void TryUse() {
        if (HeldItem != null) {
            HeldItem.Use();
        }
    }
}