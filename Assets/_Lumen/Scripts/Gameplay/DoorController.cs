using UnityEngine;
namespace Lumen.Gameplay
{
    public sealed class DoorController : MonoBehaviour
    {
        [SerializeField] private Collider2D barrier;
        [SerializeField] private SpriteRenderer panel;
        public bool IsOpen { get; private set; }
        public void SetOpen(bool open)
        {
            IsOpen = open;
            if (barrier != null) barrier.enabled = !open;
            if (panel != null) panel.color = open ? new Color(.7f,.75f,.7f,.16f) : new Color(.30f,.34f,.30f,1f);
        }
    }
}
