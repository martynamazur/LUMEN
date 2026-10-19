using System;
using Lumen.Core;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Lumen.Input
{
    public interface IGameInput { float Movement { get; } event Action Jump; event Action Pause; event Action Interact; event Action<float> Rotate; }
    public sealed class GameInputReader : MonoBehaviour, IGameInput
    {
        public float Movement => AppRoot.Instance.Session.State != SessionState.Playing ? 0f : Mathf.Clamp(move.ReadValue<float>() + touchDirection, -1f, 1f);
        public event Action Jump;
        public event Action Pause; public event Action Interact; public event Action<float> Rotate;
        private InputAction move, jump, pause, interact, rotateLeft, rotateRight;
        private float touchDirection;
        private void Awake()
        {
            move = new InputAction("Move", InputActionType.Value);
            move.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");
            move.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
            move.AddBinding("<Gamepad>/leftStick/x");
            jump = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space"); jump.AddBinding("<Keyboard>/upArrow"); jump.AddBinding("<Gamepad>/buttonSouth");
            pause = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape"); pause.AddBinding("<Gamepad>/start");
            interact = new InputAction("Interact",InputActionType.Button,"<Keyboard>/e"); rotateLeft = new InputAction("RotateLeft",InputActionType.Button,"<Keyboard>/q"); rotateRight = new InputAction("RotateRight",InputActionType.Button,"<Keyboard>/r");
            interact.performed += OnInteract; rotateLeft.performed += OnRotateLeft; rotateRight.performed += OnRotateRight;
            jump.performed += OnJump; pause.performed += OnPause;
        }
        private void OnEnable() { move.Enable(); jump.Enable(); pause.Enable(); interact.Enable(); rotateLeft.Enable(); rotateRight.Enable(); }
        private void OnDisable() { move.Disable(); jump.Disable(); pause.Disable(); interact.Disable(); rotateLeft.Disable(); rotateRight.Disable(); touchDirection = 0f; }
        private void OnDestroy() { jump.performed -= OnJump; pause.performed -= OnPause; interact.performed -= OnInteract; rotateLeft.performed -= OnRotateLeft; rotateRight.performed -= OnRotateRight; move.Dispose(); jump.Dispose(); pause.Dispose(); interact.Dispose(); rotateLeft.Dispose(); rotateRight.Dispose(); }
        private void OnJump(InputAction.CallbackContext context) => RequestJump();
        private void OnPause(InputAction.CallbackContext context) => RequestPause();
        public void SetTouchDirection(float value) => touchDirection = value;
        private void OnInteract(InputAction.CallbackContext c) => RequestInteract();
        private void OnRotateLeft(InputAction.CallbackContext c) => RequestRotate(-15f);
        private void OnRotateRight(InputAction.CallbackContext c) => RequestRotate(15f);
        public void RequestInteract() { if(AppRoot.Instance.Session.State == SessionState.Playing)Interact?.Invoke(); }
        public void RequestRotate(float delta) { if(AppRoot.Instance.Session.State == SessionState.Playing)Rotate?.Invoke(delta); }
        public void RequestJump() { if (AppRoot.Instance.Session.State == SessionState.Playing) Jump?.Invoke(); }
        public void RequestPause() => Pause?.Invoke();
        private void OnApplicationFocus(bool focused) { if (!focused) touchDirection = 0f; }
        private void OnApplicationPause(bool paused) { if (paused) touchDirection = 0f; }
    }
}
