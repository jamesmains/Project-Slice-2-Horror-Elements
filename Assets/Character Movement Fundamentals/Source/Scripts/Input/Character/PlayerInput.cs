using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace CMF {
    //This character movement input class is an example of how to get input from a keyboard to control the character;
    public class PlayerInput : MonoBehaviour {
        public Player CurrentPlayer;
        [Range(0f, 200f)] public float GamePadLookSpeedModifier = 5f;
        public InputDevice CurrentDevice;

        private InputSystem_Actions playerInput;

        private void OnEnable() {
            playerInput ??= new InputSystem_Actions();
            playerInput.Enable();
            playerInput.Player.DropSwapLeftHand.performed += delegate { CurrentPlayer.TrySetHeld(WhichHand.Left); };
            playerInput.Player.DropSwapRightHand.performed += delegate { CurrentPlayer.TrySetHeld(WhichHand.Right); };
            playerInput.Player.UseLeftHand.performed += delegate { if(!playerInput.Player.DropSwapLeftHand.inProgress)
                CurrentPlayer.TryUse(WhichHand.Left); };
            playerInput.Player.UseRightHand.performed += delegate { CurrentPlayer.TryUse(WhichHand.Right); };
            playerInput.Player.Focus.performed += delegate { CurrentPlayer.TryFocus(); };
            InputSystem.onEvent += UpdateCurrentDevice;
        }

        private void UpdateCurrentDevice(InputEventPtr arg1, InputDevice device) {
            CurrentDevice = device;
        }

        private void OnDisable() {
            InputSystem.onEvent -= UpdateCurrentDevice;
            if (playerInput != null) {
                playerInput.Disable();
            }
        }

        public Vector2 GetLookInput() {
            if(CurrentPlayer.CurrentState == PlayerState.Focus) return Vector2.zero;
            var lookInput = playerInput.Player.Look.ReadValue<Vector2>();
            if (CurrentDevice?.device == Gamepad.current)
                lookInput *= GamePadLookSpeedModifier;
            return lookInput;
        }

        public float GetHorizontalMovementInput() {
            if(CurrentPlayer.CurrentState == PlayerState.Focus) return 0;
            return playerInput.Player.Move.ReadValue<Vector2>().x;
        }

        public float GetVerticalMovementInput() {
            if(CurrentPlayer.CurrentState == PlayerState.Focus) return 0;
            return playerInput.Player.Move.ReadValue<Vector2>().y;
        }

        public bool IsJumpKeyPressed() {
            if(CurrentPlayer.CurrentState == PlayerState.Focus) return false;
            return playerInput.Player.Jump.WasPressedThisFrame();
        }
    }
}