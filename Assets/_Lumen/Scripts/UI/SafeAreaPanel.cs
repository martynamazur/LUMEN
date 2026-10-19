using UnityEngine;
namespace Lumen.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaPanel : MonoBehaviour
    {
        private Rect last; private Vector2Int size;
        private void OnEnable() => Apply();
        private void Update() { if (last != Screen.safeArea || size.x != Screen.width || size.y != Screen.height) Apply(); }
        private void Apply()
        {
            if (Screen.width < 1 || Screen.height < 1) return;
            last = Screen.safeArea; size = new Vector2Int(Screen.width, Screen.height);
            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(last.xMin / Screen.width, last.yMin / Screen.height);
            rect.anchorMax = new Vector2(last.xMax / Screen.width, last.yMax / Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
