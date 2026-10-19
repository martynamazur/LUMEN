using Lumen.Core;
using Lumen.UI;
using UnityEngine;
using UnityEngine.UI;
namespace Lumen.Gameplay
{
    public sealed class TutorialController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private InteractionController interaction;
        [SerializeField] private LightReceiver receiver;
        private Text instruction;
        private RectTransform complete;
        private bool finished;
        
        private void Start()
        {
            var root = UiFactory.Root("TutorialGuide");
            instruction = UiFactory.Label(root,"",20,new Vector2(0,150),new Vector2(1030,65),TextAnchor.MiddleCenter);
            complete=UiFactory.Panel(root,"Completed",new Vector2(1280,720));complete.gameObject.AddComponent<Image>().color=new Color(.05f,.07f,.06f,.97f);
            UiFactory.Label(complete,"Wprowadzenie ukończone",38,new Vector2(0,120),new Vector2(950,80),TextAnchor.MiddleCenter);
            UiFactory.Label(complete,"Ustawiłaś lustro i otworzyłaś przejście światłem.\nPierwszy i drugi poziom powstaną w kolejnym etapie.",22,new Vector2(0,30),new Vector2(900,100),TextAnchor.MiddleCenter);
            UiFactory.Button(complete,"Wróć do menu",new Vector2(0,-100),Back);complete.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (finished || AppRoot.Instance.Session.State != SessionState.Playing) return;

            if (player.position.x > 8f && receiver.IsLit)
            {
                finished=true;
                try { AppRoot.Instance.Progress.CompleteTutorial(); } catch(System.Exception e) { Debug.LogWarning("Nie udało się zapisać postępu: "+e.Message); }
                AppRoot.Instance.Session.Complete();complete.gameObject.SetActive(true);
                UiFactory.Select(complete.GetComponentInChildren<Button>());return;
            }
            if(receiver.IsLit)instruction.text="Drzwi otwarte! Puść lustro (E), a potem idź do wyjścia po prawej.";
            else if(interaction.Holding)instruction.text="Ustaw lustro pod księżycem (strzałki). Obracaj je klawiszami Q / R, aż światło trafi w symbol.";
            else if(interaction.HasInteracted)instruction.text="Możesz ponownie chwycić lustro (E), aby poprawić jego ustawienie.";
            else if(player.position.x > -.5f)instruction.text="Podejdź do lustra i naciśnij E, aby je chwycić.";
            else instruction.text="Idź w prawo (strzałki / A, D). Przeskocz niewielką przerwę (Spacja).";
        }
        private async void Back() { try { await AppRoot.Instance.Scenes.LoadAsync("MainMenu"); } catch(System.Exception e){Debug.LogException(e);} }
    }
}
