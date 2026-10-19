using UnityEngine;
namespace Lumen.Gameplay
{
    public sealed class LightReceiver : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer symbol;
        [SerializeField] private DoorController door;
        public bool IsLit { get; private set; }
        public void SetLit(bool lit)
        {
            IsLit = lit;
            if (symbol != null) symbol.color = lit ? new Color(1f, .8f, .3f) : new Color(.45f, .48f, .45f);
            if (door != null) door.SetOpen(lit);
        }
    }
}
