using System;
using System.IO;
using SessionState = Lumen.Core.SessionState;
using Lumen.Core;
using Lumen.Gameplay;
using Lumen.Input;
using Lumen.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Lumen.Editor
{
    public static class LumenProjectSetup
    {
        private const string Scenes="Assets/_Lumen/Scenes/";
        [MenuItem("LUMEN/Utwórz sceny podstawowe (nadpisuje sceny)")]
        public static void CreateScenes()
        {
            EditorSettings.defaultBehaviorMode=EditorBehaviorMode.Mode2D;
            PlayerSettings.companyName="Martyna Mazur";PlayerSettings.productName="LUMEN";
            PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=720;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);var input=settings.FindProperty("activeInputHandler");if(input!=null){input.intValue=1;settings.ApplyModifiedPropertiesWithoutUndo();}
            var tags=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);tags.FindProperty("layers").GetArrayElementAtIndex(6).stringValue="Ground";tags.FindProperty("layers").GetArrayElementAtIndex(7).stringValue="Optics";tags.ApplyModifiedPropertiesWithoutUndo();
            var menu=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);CameraObject();new GameObject("Menu").AddComponent<MainMenuController>();EditorSceneManager.SaveScene(menu,Scenes+"MainMenu.unity");
            var game=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);CameraObject();
            var ground=Block("GroundLeft",new Vector3(-5.8f,-2,0),new Vector3(6.4f,1,1),new Color(.27f,.30f,.28f));ground.layer=6;ground.AddComponent<BoxCollider2D>();
            var rightGround=Block("GroundRight",new Vector3(4.25f,-2,0),new Vector3(11.5f,1,1),new Color(.27f,.30f,.28f));rightGround.layer=6;rightGround.AddComponent<BoxCollider2D>();
            var player=Block("Player",new Vector3(-6,0,0),new Vector3(.65f,1.1f,1),new Color(.94f,.94f,.90f));
            player.AddComponent<BoxCollider2D>();var body=player.AddComponent<Rigidbody2D>();body.freezeRotation=true;body.gravityScale=2;
            player.AddComponent<GameInputReader>();player.AddComponent<PlayerMotor2D>();
            var mirror=Block("Mirror",new Vector3(1,-.95f,0),new Vector3(.1f,1,1),new Color(.8f,.85f,.8f));mirror.layer=7;mirror.AddComponent<BoxCollider2D>();var mirrorObject=mirror.AddComponent<MirrorObject>();
            var interaction=player.AddComponent<InteractionController>();Assign(interaction,"mirror",mirrorObject);
            var door=Block("Door",new Vector3(7.5f,-.2f,0),new Vector3(.3f,2.6f,1),new Color(.3f,.34f,.3f));var barrier=door.AddComponent<BoxCollider2D>();var doorController=door.AddComponent<DoorController>();Assign(doorController,"barrier",barrier);Assign(doorController,"panel",door.GetComponent<SpriteRenderer>());
            var receiver=Block("Receiver",new Vector3(7.25f,-.95f,0),new Vector3(.38f,.38f,1),new Color(.45f,.48f,.45f));receiver.layer=7;receiver.AddComponent<BoxCollider2D>();var lightReceiver=receiver.AddComponent<LightReceiver>();Assign(lightReceiver,"symbol",receiver.GetComponent<SpriteRenderer>());Assign(lightReceiver,"door",doorController);
            var source=Block("Moon",new Vector3(3,3,0),new Vector3(.65f,.65f,1),Color.white);
            var circle=new Texture2D(64,64);for(int x=0;x<64;x++)for(int y=0;y<64;y++)circle.SetPixel(x,y,Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,31.5f))<30?Color.white:Color.clear);circle.Apply();File.WriteAllBytes("Assets/_Lumen/Art/Game/Moon.png",circle.EncodeToPNG());UnityEngine.Object.DestroyImmediate(circle);AssetDatabase.ImportAsset("Assets/_Lumen/Art/Game/Moon.png");var moonImporter=(TextureImporter)AssetImporter.GetAtPath("Assets/_Lumen/Art/Game/Moon.png");moonImporter.textureType=TextureImporterType.Sprite;moonImporter.spritePixelsPerUnit=64;moonImporter.SaveAndReimport();source.GetComponent<SpriteRenderer>().sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Lumen/Art/Game/Moon.png");
            var line=source.AddComponent<LineRenderer>();line.useWorldSpace=true;line.startWidth=line.endWidth=.035f;line.startColor=line.endColor=new Color(1,.8f,.3f);line.sortingOrder=3;
            var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Lumen/Data/Beam.mat");if(material==null){material=new Material(Shader.Find("Sprites/Default"));AssetDatabase.CreateAsset(material,"Assets/_Lumen/Data/Beam.mat");}line.sharedMaterial=material;
            var tracer=source.AddComponent<LightTracer2D>();Assign(tracer,"receiver",lightReceiver);
            new GameObject("PauseUI").AddComponent<PauseMenuController>();var guide=new GameObject("TutorialGuide").AddComponent<TutorialController>();Assign(guide,"player",player.transform);Assign(guide,"interaction",interaction);Assign(guide,"receiver",lightReceiver);
            EditorSceneManager.SaveScene(game,Scenes+"Prototype.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scenes+"MainMenu.unity",true),new EditorBuildSettingsScene(Scenes+"Prototype.unity",true)};
            AssetDatabase.SaveAssets();EditorSceneManager.OpenScene(Scenes+"MainMenu.unity");Debug.Log("LUMEN_SETUP_OK");
        }
        private static void Assign(UnityEngine.Object target,string field,UnityEngine.Object value){var serialized=new SerializedObject(target);serialized.FindProperty(field).objectReferenceValue=value;serialized.ApplyModifiedPropertiesWithoutUndo();}
        private static void CameraObject(){var obj=new GameObject("Main Camera");obj.tag="MainCamera";var camera=obj.AddComponent<Camera>();camera.orthographic=true;camera.orthographicSize=5;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.06f,.08f,.07f);obj.transform.position=new Vector3(0,0,-10);obj.AddComponent<AudioListener>();}
        private static GameObject Block(string name,Vector3 position,Vector3 size,Color color){
            const string path="Assets/_Lumen/Art/Game/White.png";
            if(!File.Exists(path)){var texture=new Texture2D(4,4);var pixels=new Color[16];for(int i=0;i<16;i++)pixels[i]=Color.white;texture.SetPixels(pixels);texture.Apply();File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spritePixelsPerUnit=4;importer.filterMode=FilterMode.Point;importer.SaveAndReimport();}
            var obj=new GameObject(name);obj.transform.position=position;obj.transform.localScale=size;var renderer=obj.AddComponent<SpriteRenderer>();renderer.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);renderer.color=color;return obj;
        }
        public static void ValidateAndBuild()
        {
            var session=new GameSession();session.Pause();if(session.State!=SessionState.Menu)throw new Exception("Menu cannot pause");session.BeginPlay();session.Pause();if(session.State!=SessionState.Paused)throw new Exception("Pause failed");session.Resume();if(session.State!=SessionState.Playing)throw new Exception("Resume failed");session.EnterMenu();if(session.State!=SessionState.Menu)throw new Exception("Menu failed");
            var testPath=Path.Combine(Path.GetTempPath(),"lumen-progress-"+Guid.NewGuid()+".json");try{var progress=new ProgressStore(testPath);if(progress.TutorialCompleted)throw new Exception("New progress must be empty");progress.CompleteTutorial();if(!new ProgressStore(testPath).TutorialCompleted)throw new Exception("Progress failed to persist");}finally{if(File.Exists(testPath))File.Delete(testPath);}
            foreach(var scene in EditorBuildSettings.scenes)if(!File.Exists(scene.path))throw new Exception("Missing scene: "+scene.path);
            EditorSceneManager.OpenScene(Scenes+"Prototype.unity");
            var testMirror=UnityEngine.Object.FindAnyObjectByType<MirrorObject>();var testReceiver=UnityEngine.Object.FindAnyObjectByType<LightReceiver>();
            testMirror.transform.position=new Vector3(3,-.95f,0);testMirror.transform.rotation=Quaternion.Euler(0,0,45);Physics2D.SyncTransforms();
            var opticalMask=LayerMask.GetMask("Ground","Optics");var firstHit=Physics2D.Raycast(new Vector2(3,3),Vector2.down,30,opticalMask);
            if(firstHit.collider==null||firstHit.collider.GetComponent<MirrorObject>()!=testMirror)throw new Exception("Moon beam does not reach mirror");
            var reflected=Vector2.Reflect(Vector2.down,testMirror.SurfaceNormal).normalized;var secondHit=Physics2D.Raycast(firstHit.point+reflected*.18f,reflected,30,opticalMask);
            if(secondHit.collider==null||secondHit.collider.GetComponent<LightReceiver>()!=testReceiver)throw new Exception("Reflected beam does not reach receiver");
            var testDoor=UnityEngine.Object.FindAnyObjectByType<DoorController>();testReceiver.SetLit(true);if(!testDoor.IsOpen||testDoor.GetComponent<Collider2D>().enabled)throw new Exception("Door failed to open");testReceiver.SetLit(false);if(testDoor.IsOpen||!testDoor.GetComponent<Collider2D>().enabled)throw new Exception("Door failed to close");
            EditorSceneManager.OpenScene(Scenes+"MainMenu.unity");Debug.Log("LUMEN_TUTORIAL_GEOMETRY_AND_PROGRESS_OK");
            Directory.CreateDirectory("Builds/macOS");var report=BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,"Builds/macOS/LUMEN.app",BuildTarget.StandaloneOSX,BuildOptions.Development);
            if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Build failed: "+report.summary.result);
            Debug.Log("LUMEN_VALIDATE_AND_BUILD_OK");
        }
    }
}
