using Lumen.Core;
using Lumen.Input;
using UnityEngine;
namespace Lumen.Gameplay
{
    public sealed class InteractionController : MonoBehaviour
    {
        [SerializeField] private MirrorObject mirror;
        private GameInputReader input;
        private PlayerMotor2D motor;
        private float side = -1f;
        public bool HasInteracted { get; private set; }
        public bool Holding => mirror != null && mirror.IsHeld;
        private void Awake() { input = GetComponent<GameInputReader>(); motor = GetComponent<PlayerMotor2D>(); }
        private void OnEnable() { input.Interact += Toggle; input.Rotate += Rotate; }
        private void OnDisable() { input.Interact -= Toggle; input.Rotate -= Rotate; if(mirror!=null)mirror.Release(); if(motor!=null)motor.MovementEnabled=true; }
        private void Toggle()
        {
            if (mirror == null || (!mirror.IsHeld && !mirror.CanInteract(transform.position))) return;
            if (!mirror.IsHeld) side = transform.position.x <= mirror.transform.position.x ? -1f : 1f;
            mirror.Interact(); HasInteracted = true; motor.MovementEnabled = !mirror.IsHeld;
        }
        private void Rotate(float delta) { if (mirror != null && mirror.IsHeld) mirror.Rotate(delta); }
        private void Update()
        {
            if (!Holding || AppRoot.Instance.Session.State != SessionState.Playing) return;
            mirror.Slide(input.Movement * 2f * Time.deltaTime);
            transform.position = new Vector3(mirror.transform.position.x + side * .9f, transform.position.y, 0f);
        }
    }
}
