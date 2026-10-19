using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Lumen.Core;
using Lumen.Gameplay;
using Lumen.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
public class FoundationTests
{
    private string savePath;
    private float originalVolume;
    private bool originalMuted;
    private Keyboard keyboard;
    private InputSettings.BackgroundBehavior originalBackground;
    private InputSettings.EditorInputBehaviorInPlayMode originalEditorInput;
    [UnitySetUp] public IEnumerator SetUp()
    {
        yield return SceneManager.LoadSceneAsync("MainMenu"); yield return null;
        var app=AppRoot.Instance;Assert.NotNull(app);
        originalVolume=app.Settings.Volume;originalMuted=app.Settings.Muted;
        savePath=Path.Combine(Path.GetTempPath(),"lumen-playtest-"+Guid.NewGuid()+".json");
        typeof(AppRoot).GetField("<Progress>k__BackingField",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(app,new ProgressStore(savePath));
        originalBackground=InputSystem.settings.backgroundBehavior;originalEditorInput=InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        keyboard=InputSystem.AddDevice<Keyboard>();
    }
    [UnityTearDown] public IEnumerator TearDown()
    {
        Time.timeScale=1;if(keyboard!=null)InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=originalBackground;InputSystem.settings.editorInputBehaviorInPlayMode=originalEditorInput;
        if(AppRoot.Instance!=null){AppRoot.Instance.Settings.SetVolume(originalVolume);AppRoot.Instance.Settings.SetMuted(originalMuted);}
        if(File.Exists(savePath))File.Delete(savePath);
        yield return SceneManager.LoadSceneAsync("MainMenu");yield return null;
    }
    private static Button Button(string name) { foreach(var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))if(button.gameObject.name==name)return button;Assert.Fail("Missing button "+name);return null; }
    private static IEnumerator WaitForScene(string name)
    {
        for(int i=0;i<600&&SceneManager.GetActiveScene().name!=name;i++)yield return null;
        Assert.AreEqual(name,SceneManager.GetActiveScene().name);yield return null;
    }
    [UnityTest] public IEnumerator MenuSettingsPauseAndReturn()
    {
        Button("Ustawienia").onClick.Invoke();yield return null;
        var slider=UnityEngine.Object.FindAnyObjectByType<Slider>();Assert.NotNull(slider);slider.value=.27f;
        Assert.AreEqual(.27f,AppRoot.Instance.Settings.Volume,.001f);
        Assert.AreEqual(.27f,new SettingsStore().Volume,.001f);
        Button("Powrót").onClick.Invoke();Button("Rozpocznij").onClick.Invoke();yield return WaitForScene("Prototype");
        var input=UnityEngine.Object.FindAnyObjectByType<GameInputReader>();input.RequestPause();yield return null;
        Assert.AreEqual(SessionState.Paused,AppRoot.Instance.Session.State);Assert.AreEqual(0f,Time.timeScale);
        Button("Wznów").onClick.Invoke();Assert.AreEqual(1f,Time.timeScale);
        input.RequestPause();Button("Wróć do menu").onClick.Invoke();yield return WaitForScene("MainMenu");
        Assert.AreEqual(SessionState.Menu,AppRoot.Instance.Session.State);
        Assert.AreEqual(1,UnityEngine.Object.FindObjectsByType<AppRoot>(FindObjectsSortMode.None).Length);
    }
    [UnityTest] public IEnumerator TutorialDoorCompletionAndSave()
    {
        Button("Rozpocznij").onClick.Invoke();yield return WaitForScene("Prototype");yield return null;
        var mirror=UnityEngine.Object.FindAnyObjectByType<MirrorObject>();var receiver=UnityEngine.Object.FindAnyObjectByType<LightReceiver>();var door=UnityEngine.Object.FindAnyObjectByType<DoorController>();
        Assert.IsFalse(receiver.IsLit);Assert.IsFalse(door.IsOpen);
        mirror.transform.position=new Vector3(3,-.95f,0);mirror.transform.rotation=Quaternion.Euler(0,0,45);Physics2D.SyncTransforms();yield return null;yield return null;
        Assert.IsTrue(receiver.IsLit);Assert.IsTrue(door.IsOpen);Assert.IsFalse(door.GetComponent<Collider2D>().enabled);
        mirror.transform.position=new Vector3(1,-.95f,0);Physics2D.SyncTransforms();yield return null;yield return null;Assert.IsFalse(door.IsOpen);
        mirror.transform.position=new Vector3(3,-.95f,0);Physics2D.SyncTransforms();yield return null;yield return null;
        GameObject.Find("Player").transform.position=new Vector3(8.3f,-.95f,0);yield return null;yield return null;
        Assert.AreEqual(SessionState.Completed,AppRoot.Instance.Session.State);Assert.IsTrue(new ProgressStore(savePath).TutorialCompleted);
        Button("Wróć do menu").onClick.Invoke();yield return WaitForScene("MainMenu");Assert.NotNull(Button("Powtórz wprowadzenie"));
        Button("Powtórz wprowadzenie").onClick.Invoke();yield return WaitForScene("Prototype");Assert.AreEqual(1f,UnityEngine.Object.FindAnyObjectByType<MirrorObject>().transform.position.x,.01f);
    }
    [UnityTest] public IEnumerator KeyboardMovementAndJump()
    {
        Button("Rozpocznij").onClick.Invoke();yield return WaitForScene("Prototype");
        var player=GameObject.Find("Player");yield return new WaitForSeconds(.7f);Assert.AreEqual(SessionState.Playing,AppRoot.Instance.Session.State);float x=player.transform.position.x;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.RightArrow));yield return new WaitForSeconds(.3f);
        Assert.Greater(player.transform.position.x,x+.4f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
        Assert.Greater(player.GetComponent<Rigidbody2D>().linearVelocity.y,1f);
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());
    }
}
