using UnityEngine;
using Lumen.Core;
namespace Lumen.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        private RectTransform root, menu, settings, about;
        private UnityEngine.UI.Button start;
        private void Start()
        {
            AppRoot.Instance.Session.EnterMenu(); root = UiFactory.Root("MenuCanvas");
            menu = UiFactory.Panel(root,"MainMenu",new Vector2(1000,620));
            UiFactory.Label(menu,"L U M E N",80,new Vector2(0,225),new Vector2(850,110),TextAnchor.MiddleCenter);
            UiFactory.Label(menu,"Poprowadź światło. Otwórz sobie drogę.",23,new Vector2(0,140),new Vector2(850,45),TextAnchor.MiddleCenter);
            start = UiFactory.Button(menu,AppRoot.Instance.Progress.TutorialCompleted ? "Powtórz wprowadzenie" : "Rozpocznij",new Vector2(0,40),StartGame);
            UiFactory.Button(menu,"Ustawienia",new Vector2(0,-30),()=>Show(settings));
            UiFactory.Button(menu,"O projekcie",new Vector2(0,-100),()=>Show(about));
            if (!Application.isMobilePlatform) UiFactory.Button(menu,"Wyjdź",new Vector2(0,-170),Quit);
            UiFactory.Label(menu,"MARTYNA MAZUR  /  PROTOTYP 2D",14,new Vector2(0,-270),new Vector2(900,35),TextAnchor.MiddleCenter).color=UiFactory.Muted;
            settings = UiFactory.Panel(root,"Settings",new Vector2(850,560)); settings.gameObject.AddComponent<SettingsController>().Build(()=>Show(menu));
            about = UiFactory.Panel(root,"About",new Vector2(850,560));
            UiFactory.Label(about,"O projekcie",42,new Vector2(0,160),new Vector2(750,70),TextAnchor.MiddleCenter);
            UiFactory.Label(about,"LUMEN to spokojna gra logiczna 2D o prowadzeniu światła przez otoczenie.\n\nAutorka koncepcji i obrazów: Martyna Mazur.\n\nTa wersja zawiera robocze wprowadzenie.\nZagadki oraz oprawa zostaną dodane w kolejnych etapach.",22,Vector2.zero,new Vector2(700,220),TextAnchor.MiddleCenter);
            UiFactory.Button(about,"Powrót",new Vector2(0,-200),()=>Show(menu)); Show(menu);
        }
        private void Show(RectTransform panel) { menu.gameObject.SetActive(panel==menu); settings.gameObject.SetActive(panel==settings); about.gameObject.SetActive(panel==about); UiFactory.Select(panel.GetComponentInChildren<UnityEngine.UI.Button>()); }
        private async void StartGame() { if (AppRoot.Instance.Scenes.IsLoading) return; start.interactable=false; try { await AppRoot.Instance.Scenes.LoadAsync("Prototype"); } catch(System.Exception e) { Debug.LogException(e); if(start!=null)start.interactable=true; } }
        private void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying=false;
#else
            Application.Quit();
#endif
        }
    }
}
