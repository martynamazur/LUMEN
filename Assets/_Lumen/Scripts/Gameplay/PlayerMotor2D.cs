using Lumen.Core;
using Lumen.Input;
using UnityEngine;
namespace Lumen.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class PlayerMotor2D : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpSpeed = 8f;
        private Rigidbody2D body;
        private GameInputReader input;
        private bool requestedJump;
        public bool MovementEnabled { get; set; } = true;
        private void Awake() { body = GetComponent<Rigidbody2D>(); input = GetComponent<GameInputReader>(); }
        private void OnEnable() { if (input != null) input.Jump += QueueJump; }
        private void OnDisable() { if (input != null) input.Jump -= QueueJump; }
        private void QueueJump() => requestedJump = true;
        private void FixedUpdate()
        {
            if (AppRoot.Instance.Session.State != SessionState.Playing) { requestedJump = false; return; }
            var hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down * .56f, Vector2.down, .12f, LayerMask.GetMask("Ground"));
            var velocity = body.linearVelocity; velocity.x = MovementEnabled ? input.Movement * speed : 0f;
            if (MovementEnabled && requestedJump && hit.collider != null) velocity.y = jumpSpeed;
            requestedJump = false; body.linearVelocity = velocity;
            if (transform.position.y < -8f) { transform.position = new Vector3(-6f, 0f, 0f); body.linearVelocity = Vector2.zero; }
        }
    }
}
