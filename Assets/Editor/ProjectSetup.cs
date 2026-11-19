using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using CannonTargets;

[InitializeOnLoad]
public static class ProjectSetup
{
    static ProjectSetup()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().isDirty) return;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Main.unity")) return;
            CreateProjectScene();
        };
    }

    [MenuItem("Cannon Targets/Rebuild demo scene")]
    public static void CreateProjectScene()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground"; ground.transform.position = new Vector3(0, -0.5f, 25); ground.transform.localScale = new Vector3(120, 1, 100);
        ground.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Ground Material", new Color(0.24f, 0.32f, 0.28f));

        var cannon = new GameObject("Cannon"); cannon.transform.position = new Vector3(0, 1.1f, 0);
        var baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder); baseObj.name = "Base"; baseObj.transform.SetParent(cannon.transform, false); baseObj.transform.localPosition = new Vector3(0, -0.45f, 0); baseObj.transform.localScale = new Vector3(1.3f, 0.45f, 1.3f); Object.DestroyImmediate(baseObj.GetComponent<Collider>()); baseObj.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Cannon Material", new Color(0.12f, 0.16f, 0.2f));
        var aimPivot = new GameObject("Aim Pivot").transform; aimPivot.SetParent(cannon.transform, false); aimPivot.localRotation = Quaternion.Euler(-12f, 0f, 0f);
        var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder); barrel.name = "Barrel"; barrel.transform.SetParent(aimPivot, false); barrel.transform.localPosition = new Vector3(0, 0, 1.15f); barrel.transform.localRotation = Quaternion.Euler(90, 0, 0); barrel.transform.localScale = new Vector3(0.38f, 1.2f, 0.38f); Object.DestroyImmediate(barrel.GetComponent<Collider>()); barrel.GetComponent<Renderer>().sharedMaterial = baseObj.GetComponent<Renderer>().sharedMaterial;
        var muzzle = new GameObject("Muzzle").transform; muzzle.SetParent(aimPivot, false); muzzle.localPosition = new Vector3(0, 0, 2.35f);
        var lineObj = new GameObject("Trajectory"); lineObj.transform.SetParent(cannon.transform, false); var line = lineObj.AddComponent<LineRenderer>(); line.useWorldSpace = true; line.startWidth = 0.08f; line.endWidth = 0.025f; line.material = MakeMaterial("Trajectory Material", new Color(1f, 0.8f, 0.15f)); line.numCapVertices = 4;
        var trajectory = cannon.AddComponent<TrajectoryRenderer>(); trajectory.Muzzle = muzzle; trajectory.Line = line;
        var controller = cannon.AddComponent<CannonController>(); controller.Muzzle = muzzle; controller.AimPivot = aimPivot; controller.Trajectory = trajectory;

        var spawner = new GameObject("Target Spawner"); spawner.AddComponent<TargetSpawner>().MaxTargets = 5;
        var manager = new GameObject("HUD"); manager.AddComponent<Hud>();
        var cameraObj = new GameObject("Main Camera"); cameraObj.tag = "MainCamera"; cameraObj.transform.SetParent(cannon.transform, false); cameraObj.transform.localPosition = new Vector3(0, 8, -18); cameraObj.transform.localRotation = Quaternion.Euler(8, 0, 0); var cam = cameraObj.AddComponent<Camera>(); cam.fieldOfView = 65; cameraObj.AddComponent<AudioListener>();
        var lightObj = new GameObject("Directional Light"); lightObj.transform.rotation = Quaternion.Euler(50, -30, 0); lightObj.AddComponent<Light>().type = LightType.Directional;
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        Debug.Log("Cannon Targets scene created at Assets/Scenes/Main.unity");
    }

    private static Material MakeMaterial(string name, Color color)
    {
        string path = "Assets/" + name.Replace(" ", "") + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path); if (existing) { existing.color = color; return existing; }
        var shader = Shader.Find("Universal Render Pipeline/Lit"); if (!shader) shader = Shader.Find("Standard");
        var material = new Material(shader) { name = name, color = color }; AssetDatabase.CreateAsset(material, path); return material;
    }
}
