using Lumen.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Lumen.UI
{
    public sealed class SettingsController : MonoBehaviour
    {
        private AudioClip tone; private AudioSource source;
        public void Build(UnityAction back)
        {
            UiFactory.Label(transform,"Ustawienia",42,new Vector2(0,180),new Vector2(750,70),TextAnchor.MiddleCenter);
            UiFactory.Label(transform,"Głośność",22,new Vector2(0,90),new Vector2(360,40));
            var rect=UiFactory.Panel(transform,"Volume",new Vector2(360,25)); rect.anchoredPosition=new Vector2(0,45);
            var background=rect.gameObject.AddComponent<Image>(); background.color=new Color(.23f,.25f,.23f);
            var handle=UiFactory.Panel(rect,"Handle",new Vector2(20,38)); var handleImage=handle.gameObject.AddComponent<Image>(); handleImage.color=UiFactory.Paper;
            var slider=rect.gameObject.AddComponent<Slider>(); slider.handleRect=handle; slider.targetGraphic=handleImage; slider.minValue=0; slider.maxValue=1; slider.value=AppRoot.Instance.Settings.Volume; slider.onValueChanged.AddListener(AppRoot.Instance.Settings.SetVolume);
            var mute=UiFactory.Button(transform,AppRoot.Instance.Settings.Muted?"Włącz dźwięk":"Wycisz",new Vector2(0,-15),()=>{});
            mute.onClick.AddListener(()=>{var store=AppRoot.Instance.Settings;store.SetMuted(!store.Muted);mute.GetComponentInChildren<Text>().text=store.Muted?"Włącz dźwięk":"Wycisz";});
            source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;
            tone=AudioClip.Create("TestTone",11025,1,44100,false);var samples=new float[11025];for(int i=0;i<samples.Length;i++)samples[i]=Mathf.Sin(2*Mathf.PI*440*i/44100f)*.12f*Mathf.Sin(Mathf.PI*i/samples.Length);tone.SetData(samples,0);source.clip=tone;
            UiFactory.Button(transform,"Sprawdź dźwięk",new Vector2(0,-85),()=>source.Play());
            UiFactory.Label(transform,"Strzałki: ruch  •  Spacja: skok  •  Esc: pauza",16,new Vector2(0,-160),new Vector2(720,40),TextAnchor.MiddleCenter).color=UiFactory.Muted;
            UiFactory.Button(transform,"Powrót",new Vector2(0,-225),back);
        }
        private void OnDestroy() { if(tone!=null)Destroy(tone); }
    }
}
