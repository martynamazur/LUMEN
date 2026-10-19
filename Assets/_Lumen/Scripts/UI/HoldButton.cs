using UnityEngine;
using UnityEngine.EventSystems;
using Lumen.Input;
namespace Lumen.UI
{
    public sealed class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GameInputReader Input { get; set; }
        public float Direction { get; set; }
        private bool held;
        public void OnPointerDown(PointerEventData eventData) { held = true; Input.SetTouchDirection(Direction); }
        public void OnPointerUp(PointerEventData eventData) { if (!held) return; held = false; Input.SetTouchDirection(0); }
        private void OnDisable() { if (held && Input != null) Input.SetTouchDirection(0); held = false; }
    }
}
