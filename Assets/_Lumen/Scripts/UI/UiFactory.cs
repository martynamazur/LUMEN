using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
namespace Lumen.UI
{
    public static class UiFactory
    {
        public static readonly Color Paper = new Color(.93f, .93f, .90f);
        public static readonly Color Muted = new Color(.64f, .67f, .64f);
        public static RectTransform Root(string name)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;
            var safe = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaPanel)); safe.transform.SetParent(root.transform, false);
            if (Object.FindAnyObjectByType<EventSystem>() == null) new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            return safe.GetComponent<RectTransform>();
        }
        public static RectTransform Panel(Transform parent, string name, Vector2 size)
        {
            var obj = new GameObject(name, typeof(RectTransform)); obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.5f,.5f); rect.sizeDelta = size; return rect;
        }
        public static Text Label(Transform parent, string message, int fontSize, Vector2 position, Vector2 size, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var rect = Panel(parent, message.Length > 20 ? "Label" : message, size); rect.anchoredPosition = position;
            var text = rect.gameObject.AddComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.text = message; text.fontSize = fontSize; text.color = Paper; text.alignment = alignment; text.raycastTarget = false; return text;
        }
        public static Button Button(Transform parent, string title, Vector2 position, UnityAction click, Vector2? size = null)
        {
            var rect = Panel(parent, title, size ?? new Vector2(330,54)); rect.anchoredPosition = position;
            var image = rect.gameObject.AddComponent<Image>(); image.color = new Color(.16f,.18f,.17f);
            var button = rect.gameObject.AddComponent<Button>(); var colors = button.colors; colors.highlightedColor = new Color(.72f,.75f,.72f); colors.pressedColor = new Color(.50f,.53f,.50f); button.colors = colors; button.targetGraphic = image; button.onClick.AddListener(click);
            Label(rect, title, 20, Vector2.zero, rect.sizeDelta, TextAnchor.MiddleCenter); return button;
        }
        public static void Select(Button button) => EventSystem.current?.SetSelectedGameObject(button.gameObject);
    }
}
