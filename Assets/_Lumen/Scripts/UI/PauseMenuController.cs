using UnityEngine;
using Lumen.Core;
using Lumen.Input;
namespace Lumen.UI
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        private RectTransform overlay; private GameInputReader input;
        private GameSession session;
        private UnityEngine.UI.Button resume;
        private void Start()
        {
            session=AppRoot.Instance.Session;input=FindAnyObjectByType<GameInputReader>();input.Pause+=Toggle;session.Changed+=Changed;
            var root=UiFactory.Root("PrototypeCanvas");
            UiFactory.Label(root,"LUMEN / WPROWADZENIE",17,new Vector2(0,285),new Vector2(900,40),TextAnchor.MiddleCenter).color=UiFactory.Muted;
            UiFactory.Button(root,"Pauza",new Vector2(480,270),Toggle,new Vector2(140,50));
            UiFactory.Label(root,"Ruch: strzałki  •  Skok: Spacja  •  Chwyć: E  •  Obrót: Q / R  •  Pauza: Esc",17,new Vector2(0,-280),new Vector2(900,40),TextAnchor.MiddleCenter);
            UiFactory.Label(root,"Ustaw lustro. Skieruj światło na drzwi.",20,new Vector2(0,220),new Vector2(900,40),TextAnchor.MiddleCenter);
            if(Application.isMobilePlatform){
                var left=UiFactory.Button(root,"←",new Vector2(-460,-230),()=>{},new Vector2(95,70));var right=UiFactory.Button(root,"→",new Vector2(-340,-230),()=>{},new Vector2(95,70));
                var l=left.gameObject.AddComponent<HoldButton>();l.Input=input;l.Direction=-1;var r=right.gameObject.AddComponent<HoldButton>();r.Input=input;r.Direction=1;
                UiFactory.Button(root,"Skok",new Vector2(440,-230),input.RequestJump,new Vector2(130,70));
                UiFactory.Button(root,"Chwyć",new Vector2(250,-230),input.RequestInteract,new Vector2(130,70));
                UiFactory.Button(root,"Obrót ←",new Vector2(250,-150),()=>input.RequestRotate(-15),new Vector2(140,65));
                UiFactory.Button(root,"Obrót →",new Vector2(440,-150),()=>input.RequestRotate(15),new Vector2(140,65));
            }
            overlay=UiFactory.Panel(root,"PauseOverlay",new Vector2(1280,720));var image=overlay.gameObject.AddComponent<UnityEngine.UI.Image>();image.color=new Color(.05f,.07f,.06f,.97f);
            UiFactory.Label(overlay,"Pauza",48,new Vector2(0,150),new Vector2(700,80),TextAnchor.MiddleCenter);
            resume=UiFactory.Button(overlay,"Wznów",new Vector2(0,40),()=>session.Resume());
            UiFactory.Button(overlay,"Zacznij od nowa",new Vector2(0,-35),()=>Load("Prototype"));
            UiFactory.Button(overlay,"Wróć do menu",new Vector2(0,-110),()=>Load("MainMenu"));
            session.BeginPlay();Changed(session.State);
        }
        private void Toggle(){if(session.State==SessionState.Playing)session.Pause();else if(session.State==SessionState.Paused)session.Resume();}
        private void Changed(SessionState state){input.SetTouchDirection(0);overlay.gameObject.SetActive(state==SessionState.Paused);if(state==SessionState.Paused)UiFactory.Select(resume);}
        private async void Load(string name){try{await AppRoot.Instance.Scenes.LoadAsync(name);}catch(System.Exception e){Debug.LogException(e);}}
        private void OnDestroy(){if(input!=null)input.Pause-=Toggle;if(session!=null)session.Changed-=Changed;}
    }
}
