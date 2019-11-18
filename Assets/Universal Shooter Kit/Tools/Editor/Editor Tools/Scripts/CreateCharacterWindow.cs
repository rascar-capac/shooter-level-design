// GercStudio
// © 2018-2019

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;   
using System.Reflection;
using System;
using System.IO;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
    public class CreateCharacterWindow : EditorWindow
    {

        public GameObject CharacterModel;
        public GameObject Ragdoll;

//        public CharacterParameters parameters;
//        public CameraParameters CameraParameters;

//        private CharacterParameters tempParameters;
//        private CameraParameters tempCameraParameters;

        private bool characterError;
//        private bool parametersError;
        private bool animControllerError;
        private bool CharacterAdded;
//        private bool animationsError;
//        private bool animationsErrorGUI;
        private bool saveRagdoll = true;
        private bool ragdollError;
        private bool CameraParametersError;
        private bool hasCreated;
        private bool startCreation;
//        private bool createAnyway;
//        private bool fontError;
//        private bool fontErrorGUI;
        private bool animControllerErrorGUI;

        private float startVal;
        private float progress;

        private AnimatorController AnimatorController;

//        private AnimationClip[] MovementAnimations = new AnimationClip[20];

        private Vector2 scrollPos;

        private GUIStyle LabelStyle;

//        private Font font;

        [MenuItem("Window/USK/Create/Character")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CreateCharacterWindow), true, "", true).ShowUtility();
        }

        private void Awake()
        {
//            GetDefaultParameters();
//            GetDefaultCameraParameters();

            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle();
                LabelStyle.normal.textColor = Color.black;
                LabelStyle.fontStyle = FontStyle.Bold;
                LabelStyle.fontSize = 12;
                LabelStyle.alignment = TextAnchor.MiddleCenter;
            }
            
//            font = AssetDatabase.LoadAssetAtPath(
//                "Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
        }

        void OnEnable()
        {
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (CharacterModel)
            {
                if (Ragdoll & !saveRagdoll)
                {
                    if (Ragdoll.GetComponent<Animator>())
                        if (Ragdoll.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>())
                        {
                            SaveRagdollToPrefab();
                            saveRagdoll = true;
                        }
                }

                if (!Ragdoll)
                {
                    ragdollError = true;
                }
                else
                {
                    if (AssetDatabase.LoadAssetAtPath(
                        "Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" + CharacterModel.name + "Ragdoll.prefab",
                        typeof(GameObject)) as GameObject)
                        ragdollError = false;
                }

                if (!CharacterAdded)
                {
                    ResetErrors();
//                    if (!CharacterModel.activeInHierarchy)
//                    {
//                        CharacterModel = Instantiate(CharacterModel, Vector3.zero, Quaternion.Euler(Vector3.zero));
//                        CharacterModel.SetActive(true);
//                    }

                    CharacterAdded = true;
                }
                else
                {
                    if (CharacterModel.GetComponent<Animator>())
                    {
                        if (CharacterModel.GetComponent<Animator>().avatar)
                        {
                            if (!CharacterModel.GetComponent<Animator>().avatar.isHuman)
                            {
                                CharacterModel = null;
                                characterError = true;
                            }
                            else
                            {
                                characterError = false;
                            }
                        }
                        else
                        {
                            DestroyImmediate(CharacterModel.GetComponent<Animator>());
                            CharacterModel.AddComponent<Animator>();

                            if (!CharacterModel.GetComponent<Animator>().avatar)
                            {
                                DestroyImmediate(CharacterModel.GetComponent<Animator>());
                                CharacterModel = null;
                                characterError = true;
                            }
                        }
                    }
                    else
                    {
                        CharacterModel.AddComponent<Animator>();
                    }
                }

                if (startCreation & progress > 1.16f)
                {
                    ResetErrors();
                    GetAnimatorController();
//                    GetAnimations();
                   // CheckFont();

                   if (!animControllerError)
                       /*if (!animationsError || createAnyway)*/
                   {
                       AddScripts();
                       SetAnimatorController();
                       SetVariables();
                       CreateUI();
                       CreateCamera();
//                            SetAnimations();
                       SaveCharacterToPrefab();
                       hasCreated = true;
//                       createAnyway = false;
                       startVal = (float) EditorApplication.timeSinceStartup;
                   }

                   startCreation = false;
                }
            }
            else
            {
                Ragdoll = null;
                CharacterAdded = false;
                ResetErrors();
//                ResetVariables();
            }

//            if (!parameters) // & !CameraParameters)
//            {
////                ResetVariables();
//                ResetErrors();
//            }

            if (hasCreated)
            {
                ResetErrors();
                if (progress > 10)
                {
                    hasCreated = false;

                    CharacterModel = null;
                    Ragdoll = null;
//                    ResetVariables();
                }
            }
        }

        private void OnGUI()
        {
            scrollPos =
                EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width),
                    GUILayout.Height(position.height));

            EditorGUILayout.Space();
            GUILayout.Label("Create Character", LabelStyle);
            EditorGUILayout.Space();
            if (hasCreated)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.green;
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 10;
                style.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField("Character has been created", style);
                EditorGUILayout.HelpBox("Open the Adjustment scene to regulate your character" + "\n" +
                                        "[Window -> USK -> Adjust]", MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginVertical("box");
            if (characterError)
            {
                EditorGUILayout.HelpBox(
                    "Character model must be a Humanoid type.",
                    MessageType.Warning);
            }

            CharacterModel = (GameObject) EditorGUILayout.ObjectField("Character Model", CharacterModel, typeof(GameObject), false);
            EditorGUILayout.EndVertical();
//            EditorGUILayout.Space();
//            EditorGUILayout.BeginVertical("box");
//            if (!parameters)// || !CameraParameters)
//            {
//                EditorGUILayout.HelpBox(
//                    "Please set Character and Camera presets from [Universal Shooter Kit/Presets/Characters/] or create your own.",
//                    MessageType.Info);
//                EditorGUILayout.Space();
//            }
//
//            parameters = (CharacterParameters) EditorGUILayout.ObjectField("Character Preset", parameters, typeof(CharacterParameters), false);
////            CameraParameters = (CameraParameters) EditorGUILayout.ObjectField("Camera Preset", CameraParameters, typeof(CameraParameters), false);
//            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (CharacterModel)// & parameters)// & CameraParameters)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.BeginDisabledGroup(true);
                Ragdoll = (GameObject) EditorGUILayout.ObjectField("Ragdoll", Ragdoll, typeof(GameObject), true);
                EditorGUI.EndDisabledGroup();
                if (Ragdoll == null)
                {
                    EditorGUILayout.HelpBox("Create Ragdoll before create Character", MessageType.Info);

                    if (!(Ragdoll = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" + CharacterModel.name + "Ragdoll.prefab",
                        typeof(GameObject)) as GameObject))
                    {
                        if (GUILayout.Button("Create Ragdoll"))
                        {
                            Ragdoll = Instantiate(CharacterModel);
                            Ragdoll.name = CharacterModel.name + " Ragdoll";
                            CreateRagdoll();
                        }
                    }
                }
                else
                {
                    if (AssetDatabase.GetAssetPath(Ragdoll) == "Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" +
                        CharacterModel.name + "Ragdoll.prefab")
                    {
                        EditorGUILayout.HelpBox("The ragdoll for this character is in " + "\n" + "[Assets/Universal Shooter Kit/Prefabs/_Ragdolls/]. " + "\n" +
                                                "But you can create a new one.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("The ragdoll for this character isn't in " + "\n" +
                                                "[Assets/Universal Shooter Kit/Prefabs/_Ragdolls/]. " + "\n" +
                                                "Please try creating it again.", MessageType.Warning);
                    }

                    if (GUILayout.Button("Create New Ragdoll"))
                    {
                        Ragdoll = Instantiate(CharacterModel);
                        Ragdoll.name = CharacterModel.name + " Ragdoll";
                        CreateRagdoll();
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

//                if (CameraParameters == null)
//                {
//                    EditorGUILayout.HelpBox("Camera Parameters not set.", MessageType.Warning);
//                    EditorGUILayout.Space();
//                }

                if (animControllerErrorGUI)
                {
                    EditorGUILayout.BeginVertical("box");

                    if (animControllerError)
                        EditorGUILayout.HelpBox("Standard Animator Controller was not found. Set it yourself from: " + "\n" +
                            "[Universal Shooter Kit/Animations/Character/Character.controller]", MessageType.Warning);

                    AnimatorController = (AnimatorController) EditorGUILayout.ObjectField("Animator Controller", AnimatorController, typeof(AnimatorController), false);

                    animControllerError = !AnimatorController;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

//                if (animationsErrorGUI)
//                {
//                    if (animationsError)
//                        EditorGUILayout.HelpBox(
//                            "All animations not found. Set them yourself from" + "\n" +
//                            " [Universal Shooter Kit/Animations/]",
//                            MessageType.Warning);
//                    EditorGUILayout.Space();
//                    EditorGUILayout.BeginVertical("box");
//                    MovementAnimations[8] = (AnimationClip) EditorGUILayout.ObjectField("Idle",
//                        MovementAnimations[8], typeof(AnimationClip), false);
//                    
//                    MovementAnimations[17] = (AnimationClip) EditorGUILayout.ObjectField("Turn right",
//                        MovementAnimations[17], typeof(AnimationClip), false);
//                    
//                    MovementAnimations[18] = (AnimationClip) EditorGUILayout.ObjectField("Turn left",
//                        MovementAnimations[18], typeof(AnimationClip), false);
//                    
//                    MovementAnimations[19] = (AnimationClip) EditorGUILayout.ObjectField("Flying in air",
//                        MovementAnimations[19], typeof(AnimationClip), false);
//                    EditorGUILayout.EndVertical();
//                    EditorGUILayout.Space();
//                    EditorGUILayout.LabelField("Walk Animations", EditorStyles.boldLabel);
//                    EditorGUILayout.BeginVertical("box");
//                    MovementAnimations[0] = (AnimationClip) EditorGUILayout.ObjectField("Walk Forward",
//                        MovementAnimations[0], typeof(AnimationClip), false);
//                    MovementAnimations[1] = (AnimationClip) EditorGUILayout.ObjectField("Walk Backward",
//                        MovementAnimations[1], typeof(AnimationClip), false);
//                    MovementAnimations[2] = (AnimationClip) EditorGUILayout.ObjectField("Walk Left",
//                        MovementAnimations[2], typeof(AnimationClip), false);
//                    MovementAnimations[3] = (AnimationClip) EditorGUILayout.ObjectField("Walk Right",
//                        MovementAnimations[3], typeof(AnimationClip), false);
//                    MovementAnimations[4] = (AnimationClip) EditorGUILayout.ObjectField("Walk Forward Left",
//                        MovementAnimations[4], typeof(AnimationClip), false);
//                    MovementAnimations[5] = (AnimationClip) EditorGUILayout.ObjectField("Walk Forward Right",
//                        MovementAnimations[5], typeof(AnimationClip), false);
//                    MovementAnimations[6] = (AnimationClip) EditorGUILayout.ObjectField("Walk Backward Left",
//                        MovementAnimations[6], typeof(AnimationClip), false);
//                    MovementAnimations[7] = (AnimationClip) EditorGUILayout.ObjectField("Walk Backward Right",
//                        MovementAnimations[7], typeof(AnimationClip), false);
//                    
//                    EditorGUILayout.EndVertical();
//
//                    if (parameters.activeSprint)
//                    {
//                        EditorGUILayout.Space();
//                        EditorGUILayout.LabelField("Run Animations", EditorStyles.boldLabel);
//                        EditorGUILayout.BeginVertical("box");
//                        MovementAnimations[9] = (AnimationClip) EditorGUILayout.ObjectField("Run Forward",
//                            MovementAnimations[9], typeof(AnimationClip), false);
//                        MovementAnimations[10] = (AnimationClip) EditorGUILayout.ObjectField("Run Backward",
//                            MovementAnimations[10], typeof(AnimationClip), false);
//                        MovementAnimations[11] = (AnimationClip) EditorGUILayout.ObjectField("Run Left",
//                            MovementAnimations[11], typeof(AnimationClip), false);
//                        MovementAnimations[12] = (AnimationClip) EditorGUILayout.ObjectField("Run Right",
//                            MovementAnimations[12], typeof(AnimationClip), false);
//                        MovementAnimations[13] = (AnimationClip) EditorGUILayout.ObjectField("Run Forward Left",
//                            MovementAnimations[13], typeof(AnimationClip), false);
//                        MovementAnimations[14] = (AnimationClip) EditorGUILayout.ObjectField("Run Forward Right",
//                            MovementAnimations[14], typeof(AnimationClip), false);
//                        MovementAnimations[15] = (AnimationClip) EditorGUILayout.ObjectField("Run Backward Left",
//                            MovementAnimations[15], typeof(AnimationClip), false);
//                        MovementAnimations[16] = (AnimationClip) EditorGUILayout.ObjectField("Run Backward Right",
//                            MovementAnimations[16], typeof(AnimationClip), false);
//                        EditorGUILayout.EndVertical();
//                    }
//
//                    if (animationsError)
//                    {
//                        animationsError = false;
//                        CheckAnims();
//                    }
//                    else
//                    {
//                        CheckAnims();
//                    }
//
//                }

//                if (fontErrorGUI)
//                {
//                    EditorGUILayout.Space();
//                    EditorGUILayout.BeginVertical("box");
//
//                    if (fontError)
//                        EditorGUILayout.HelpBox(
//                            "Standard font for the UI was not found, please set some other.",
//                            MessageType.Warning);
//
//                    font = (Font) EditorGUILayout.ObjectField("Font", font, typeof(Font), false);
//                    EditorGUILayout.EndVertical();
//                    EditorGUILayout.Space();
//
//                    fontError = !font;
//                }

                if (startCreation)
                    EditorGUI.ProgressBar(new Rect(3, 160, position.width - 6, 20), progress / 1.5f, "Creation...");
            }


            EditorGUI.BeginDisabledGroup(ragdollError || animControllerError || !CharacterModel); // || !parameters);// || !CameraParameters);

//            if (!animationsError)
//            {
                if (!startCreation)
                    if (GUILayout.Button("Create"))
                    {
                        //tempParameters = parameters;
                        //tempCameraParameters = CameraParameters;
                        ResetErrors();
                        startVal = (float) EditorApplication.timeSinceStartup;
                        startCreation = true;
                    }

//            }
//            else
//            {
//                if (!startCreation)
//                {
//                    if (parameters != tempParameters || tempCameraParameters != CameraParameters)
//                    {
//                        ResetErrors();
//                        ResetVariables();
//                        tempParameters = parameters;
//                        tempCameraParameters = CameraParameters;
//                        return;
//                    }
//
//                    EditorGUILayout.Space();
//                    if (GUILayout.Button("Create anyway (Not recommended)"))
//                    {
//                        ResetErrors();
//                        startVal = (float) EditorApplication.timeSinceStartup;
//                        startCreation = true;
//                        createAnyway = true;
//                    }
//                }
//            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();

            progress = (float) (EditorApplication.timeSinceStartup - startVal);
        }

        void ResetErrors()
        {
//            animationsError = false;
            animControllerError = false;
            animControllerErrorGUI = false;
            ragdollError = false;
//            animationsErrorGUI = false;
            //fontError = false;
           //fontErrorGUI = false;
        }

//        void ResetVariables()
//        {
//            for (int i = 0; i < 20; i++)
//            {
//                MovementAnimations[i] = null;
//            }
//        }

//        void CheckAnims()
//        {
//            for (int i = 0; i < 20; i++)
//            {
//                if (i < 9 || i >= 17)
//                {
//                    if (!MovementAnimations[i])
//                        animationsError = true;
//                }
//                else if(i > 9 & i < 17)
//                {
//                    if (parameters.activeSprint)
//                        if (!MovementAnimations[i])
//                            animationsError = true;
//                }
//            }
//        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

//        void CheckFont()
//        {
//            if (!font)
//            {
//                fontError = true;
//                fontErrorGUI = true;
//                font = AssetDatabase.LoadAssetAtPath(
//                    "Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
//            }
//
//            if (font)
//            {
//                fontError = false;
//                fontErrorGUI = false;
//            }
//        }

        void CreateRagdoll()
        {
            if (Ragdoll.GetComponent<Controller>())
                DestroyImmediate(Ragdoll.GetComponent<Controller>());

            foreach (var comp in Ragdoll.GetComponents<Component>())
            {
                if (!(comp is Animator) & !(comp is Transform))
                {
                    DestroyImmediate(comp);
                }
            }

            var ragdollBuilderType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
            var windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);

            if (windows == null || windows.Length == 0)
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
                windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
            }

            if (windows != null && windows.Length > 0)
            {
                var ragdollWindow = windows[0] as ScriptableWizard;

                var animator = Ragdoll.GetComponent<Animator>();
                SetFieldValue(ragdollWindow, "pelvis", animator.GetBoneTransform(HumanBodyBones.Hips));
                SetFieldValue(ragdollWindow, "leftHips", animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
                SetFieldValue(ragdollWindow, "leftKnee", animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
                SetFieldValue(ragdollWindow, "leftFoot", animator.GetBoneTransform(HumanBodyBones.LeftFoot));
                SetFieldValue(ragdollWindow, "rightHips", animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
                SetFieldValue(ragdollWindow, "rightKnee", animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
                SetFieldValue(ragdollWindow, "rightFoot", animator.GetBoneTransform(HumanBodyBones.RightFoot));
                SetFieldValue(ragdollWindow, "leftArm", animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
                SetFieldValue(ragdollWindow, "leftElbow", animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
                SetFieldValue(ragdollWindow, "rightArm", animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
                SetFieldValue(ragdollWindow, "rightElbow", animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
                SetFieldValue(ragdollWindow, "middleSpine", animator.GetBoneTransform(HumanBodyBones.Spine));
                SetFieldValue(ragdollWindow, "head", animator.GetBoneTransform(HumanBodyBones.Head));

                var method = ragdollWindow.GetType().GetMethod("CheckConsistency",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (method != null)
                {
                    ragdollWindow.errorString = (string) method.Invoke(ragdollWindow, null);
                    ragdollWindow.isValid = string.IsNullOrEmpty(ragdollWindow.errorString);
                }

                saveRagdoll = false;
            }
        }

        private void SetFieldValue(ScriptableWizard obj, string name, object value)
        {
            if (value == null)
            {
                return;
            }

            var field = obj.GetType().GetField(name);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }

        void SaveRagdollToPrefab()
        {
            if (Ragdoll.GetComponent<Animator>())
                DestroyImmediate(Ragdoll.GetComponent<Animator>());

            Ragdoll.AddComponent<DestroyObject>().destroy_time = 7;

            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/_Ragdolls/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/_Ragdolls/");
            }
            
#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" + CharacterModel.name + "Ragdoll.prefab");
            PrefabUtility.ReplacePrefab(Ragdoll, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(Ragdoll, "Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" + CharacterModel.name + "Ragdoll.prefab");
#endif
            
            DestroyImmediate(Ragdoll);
            
            Ragdoll = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" + CharacterModel.name + "Ragdoll.prefab",
                typeof(GameObject)) as GameObject;

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Prefabs/_Ragdolls/" + CharacterModel.name + "Ragdoll.prefab",
                typeof(GameObject)) as GameObject);
        }

        void SaveCharacterToPrefab()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/_Characters/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/_Characters/");
            }

            var index = 0;
            while(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Characters/" + CharacterModel.name + " " + index + ".prefab", typeof(GameObject)) != null)
            {
                index++;
            }
            
#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/_Characters/" + CharacterModel.name + " " + index + ".prefab");
            PrefabUtility.ReplacePrefab(CharacterModel, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(CharacterModel, "Assets/Universal Shooter Kit/Prefabs/_Characters/" + CharacterModel.name + " " + index + ".prefab");
#endif

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Characters/" + CharacterModel.name + " " + index + ".prefab",
                typeof(GameObject)));

            DestroyImmediate(CharacterModel);

        }

        void AddScripts()
        {
            CharacterModel = Instantiate(CharacterModel, Vector3.zero, Quaternion.Euler(Vector3.zero));
            CharacterModel.SetActive(true);
            
            if (!CharacterModel.GetComponent<Controller>())
                CharacterModel.AddComponent<Controller>();

            if (!CharacterModel.GetComponent<InventoryManager>())
                CharacterModel.AddComponent<InventoryManager>();


            var controller = CharacterModel.GetComponent<Controller>();
            if (!controller.CharacterController)
            {
                controller.CharacterController = CharacterModel.AddComponent<CharacterController>();
//                controller.CharacterController.hideFlags = HideFlags.HideInInspector;
            }
            
            if (!controller.colliderRigidbody)
            {
                controller.colliderRigidbody = CharacterModel.AddComponent<Rigidbody>();
                controller.colliderRigidbody.useGravity = false;
                controller.colliderRigidbody.isKinematic = true;
//                controller.colliderRigidbody.hideFlags = HideFlags.HideInInspector;
            }
            
            if (!controller.characterCollider)
                controller.characterCollider = CharacterModel.AddComponent<CapsuleCollider>();

            if (!controller.DirectionObject)
            {
                controller.DirectionObject = new GameObject("Direction object").transform;
                controller.DirectionObject.parent = CharacterModel.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Spine);
                controller.DirectionObject.localPosition = Vector3.zero;
                controller.DirectionObject.localEulerAngles = Vector3.zero;
            }
        }

//        void GetDefaultParameters()
//        {
//            if (parameters == null)
//            {
//                parameters =
//                    AssetDatabase.LoadAssetAtPath(
//                        "Assets/Universal Shooter Kit/Presets/Characters/DefaultCharacterParameters.asset",
//                        typeof(CharacterParameters)) as CharacterParameters;
//            }
//        }

//        void GetDefaultCameraParameters()
//        {
//            if (CameraParameters == null)
//            {
//                CameraParameters =
//                    AssetDatabase.LoadAssetAtPath(
//                        "Assets/Universal Shooter Kit/Presets/Characters/DefaultCameraParameters.asset",
//                        typeof(CameraParameters)) as CameraParameters;
//            }
//        }

        void GetAnimatorController()
        {
            AnimatorController =
                AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Animations/Character/Character.controller", typeof(RuntimeAnimatorController)) as AnimatorController;

            //if (!(AnimatorController = parameters.AnimatorController))
            animControllerError = !AnimatorController;
            animControllerErrorGUI = !AnimatorController;
        }

        void SetAnimatorController()
        {
            CharacterModel.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;
        }

        void CreateUI()
        {
            //var controller = CharacterModel.GetComponent<Controller>();
            var manager = CharacterModel.GetComponent<InventoryManager>();

            if (!manager.canvas)
            {
                manager.canvas = Helper.NewCanvas("Canvas", new Vector2(1920, 1080), CharacterModel.transform);
            }

            if (!manager.inventoryWheel)
            {
                Helper.newInventory(manager);
            }
            
            if (!manager.aimTextureImage)
            {
                var obj = Helper.NewUIElement("Aim texture", manager.canvas.transform, Vector2.zero, new Vector2(1920, 1080), Vector3.one);
                manager.aimTextureImage = obj.AddComponent<RawImage>();
                obj.SetActive(false);
            }
        }

        void CreateCamera()
        {
            var controller = CharacterModel.GetComponent<Controller>();
            if (!controller.thisCamera)
            {
                var camera = new GameObject("MainCamera");
                camera.AddComponent<Camera>();
                camera.tag = "MainCamera";
                var cameraController = camera.AddComponent<CameraController>();
                

                camera.transform.parent = CharacterModel.transform;
                camera.transform.localPosition = new Vector3(0, 0, 0);
                camera.AddComponent<AudioListener>();

                controller.thisCamera = camera;

                var aimCamera = new GameObject("AimCamera") {tag = "MainCamera"};
                aimCamera.transform.parent = camera.transform;
                aimCamera.transform.localPosition = Vector3.zero;
                aimCamera.transform.localEulerAngles = Vector3.zero;
                cameraController.AimCamera = aimCamera.AddComponent<Camera>();
                Helper.CameraExtensions.LayerCullingHide(cameraController.AimCamera, 8);

                cameraController.CameraPosition =
                    Helper.NewObject(CharacterModel.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head), "FPS Camera Position and Rotation", PrimitiveType.Cube, Color.yellow);

                DestroyImmediate(cameraController.CameraPosition.GetComponent<BoxCollider>());
            }
        }

        void SetVariables()
        {
            var _animator = CharacterModel.GetComponent<Animator>();
            var controller = CharacterModel.GetComponent<Controller>();

            if (Ragdoll != null)
                controller.Ragdoll = Ragdoll.transform;
            
            controller.PlayerHealth = 100;
            
            controller.BodyObjects.RightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            controller.BodyObjects.LeftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
            controller.BodyObjects.Head = _animator.GetBoneTransform(HumanBodyBones.Head);
            controller.BodyObjects.TopBody = _animator.GetBoneTransform(HumanBodyBones.Spine);
            controller.BodyObjects.Hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            
            controller.leftAudioSource = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).gameObject.AddComponent<AudioSource>();
            controller.rightAudioSource = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).gameObject.AddComponent<AudioSource>();
            
            controller.inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(Inputs)) as Inputs;
        }
    }
}
