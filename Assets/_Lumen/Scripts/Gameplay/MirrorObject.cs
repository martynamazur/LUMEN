using UnityEngine;
namespace Lumen.Gameplay
{
    public interface IInteractable { string InteractionLabel { get; } bool CanInteract(Vector2 playerPosition); void Interact(); }
    public sealed class MirrorObject : MonoBehaviour, IInteractable
    {
        public bool IsHeld { get; private set; }
        public string InteractionLabel => IsHeld ? "Puść lustro" : "Chwyć lustro";
        public Vector2 SurfaceNormal => transform.right;
        public bool CanInteract(Vector2 playerPosition) => Vector2.Distance(playerPosition, transform.position) < 1.8f;
        public void Interact() { IsHeld = !IsHeld; if(!IsHeld && Mathf.Abs(transform.position.x-3f)<.15f) { var p=transform.position; p.x=3f;transform.position=p; } }
        public void Release() => IsHeld = false;
        public void Slide(float delta)
        {
            var position = transform.position;
            position.x = Mathf.Clamp(position.x + delta, .5f, 4.3f);
            transform.position = position;
        }
        public void Rotate(float degrees) { transform.Rotate(0, 0, degrees); }
    }
}
