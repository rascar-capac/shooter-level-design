using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GercStudio.USK.Scripts
{
    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : Editor
    {

        public Controller script;
        private Animator _animator;

        private bool setBones;
        private bool WalkAnimations;
        private bool JumpAnimations;
        private bool RunAnimations;

        private float xRotationOffset;
        private float yRotationOffset;
        private float zRotationOffset;
        private float defaultCharacterHeight;

        private string curName;

        private bool delete;
        private bool rename;
        private bool renameError;
        
        private CameraController camera;
        private InventoryManager manager;
        
        private int tab;
        private int tab2;
        
        private bool selectKeyboardButton;
        private bool selectGamepadButton;
        
        private List<string> animationErrorNames;
        
        public void Awake()
        {
            script = (Controller) target;
            _animator = script.gameObject.GetComponent<Animator>();
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

            if(script)
                if (!manager)
                     manager = script.GetComponent<InventoryManager>();
            
//            if (script.colliderRigidbody)
//                script.colliderRigidbody.hideFlags = HideFlags.HideInInspector;
//
//            if (script.CharacterController)
//                script.CharacterController.hideFlags = HideFlags.HideInInspector;
            
            if (Application.isPlaying)
            {

                if (!camera & script.thisCamera)
                    camera = script.thisCameraScript;
                
                if (script.DebugMode || manager.grenadeDebug)
                {
                    ActiveEditorTracker.sharedTracker.isLocked = true;
                }
                else
                {
                    ActiveEditorTracker.sharedTracker.isLocked = false;
                }

                if (script.gameObject.activeInHierarchy)
                {
                    
                }
            }
            else
            {
                if (!script.inputs)
                    script.inputs = AssetDatabase.LoadAssetAtPath(
                        "Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(Inputs)) as Inputs;

                if (script.characterTag > script.inputs.CharacterTags.Count - 1)
                {
                    script.characterTag = script.inputs.CharacterTags.Count - 1;
                }
                
                if (_animator)
                {
                    if (_animator.isHuman)
                    {
                        if (!script.BodyObjects.RightHand)
                        {
                            script.BodyObjects.RightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.LeftHand)
                        {
                            script.BodyObjects.LeftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.Head)
                        {
                            script.BodyObjects.Head = _animator.GetBoneTransform(HumanBodyBones.Head);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.TopBody)
                        {
                            script.BodyObjects.TopBody = _animator.GetBoneTransform(HumanBodyBones.Spine);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        if (!script.BodyObjects.Hips)
                        {
                            script.BodyObjects.Hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                        
                        if (!script.BodyObjects.Chest)
                        {
                            script.BodyObjects.Chest = _animator.GetBoneTransform(HumanBodyBones.Chest);
                            EditorUtility.SetDirty(script);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            if(!script.CameraParameters.ActiveFP)
                script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop, new [] {"Camera", "Health", "Tag"});
            else script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop, new [] {"Camera", "Health", "Tag", "FPS Movement"});

            switch (script.inspectorTabTop)
            {
                case 0:

                    EditorGUILayout.Space();
                    script.cameraInspectorTab = GUILayout.Toolbar(script.cameraInspectorTab, new[] {"Third person", "First person", "Top down"});

                    EditorGUILayout.BeginVertical("box");

                    switch (script.cameraInspectorTab)
                    {
                        case 0:
                           
                            if (!Application.isPlaying)
                                script.TypeOfCamera = CharacterHelper.CameraType.ThirdPerson;
                            
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.ActiveTP"), new GUIContent("Active"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.X_MouseSensitivity"), new GUIContent("X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.Y_MouseSensitivity"), new GUIContent("Y Sensitivity"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.TPAimDepth"),
                                new GUIContent("Aim depth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.AimX_MouseSensitivity"),
                                new GUIContent("(Aim) X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.AimY_MouseSensitivity"),
                                new GUIContent("(Aim) Y Sensitivity"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.Y_MinLimit"),
                                new GUIContent("Min X"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.Y_MaxLimit"),
                                new GUIContent("Max X"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpSmoothX"), new GUIContent("X Smooth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tpSmoothY"), new GUIContent("Y Smooth"));
                            EditorGUILayout.Space();
                            script.SmoothCameraWhenMoving = EditorGUILayout.ToggleLeft("Smooth camera moving while character walking", script.SmoothCameraWhenMoving);
                            break;

                        case 1:

                            if (!Application.isPlaying)
                                script.TypeOfCamera = CharacterHelper.CameraType.FirstPerson;
                            
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.ActiveFP"), new GUIContent("Active"));

                            EditorGUILayout.Space();

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_X_MouseSensitivity"),
                                new GUIContent("X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_Y_MouseSensitivity"),
                                new GUIContent("Y Sensitivity"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.FPAimDepth"),
                                new GUIContent("Aim depth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_AimX_MouseSensitivity"),
                                new GUIContent("(Aim) X Sensitivity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_AimY_MouseSensitivity"),
                                new GUIContent("(Aim) Y Sensitivity"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_MinRotationX"),
                                new GUIContent("Min X"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_MaxRotationX"),
                                new GUIContent("Max X"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_X_Smooth"),
                                new GUIContent("X Smooth"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.fps_Y_Smooth"),
                                new GUIContent("Y Smooth"));
                            break;

                        case 2:

                            if (!Application.isPlaying)
                                script.TypeOfCamera = CharacterHelper.CameraType.TopDown;
                            
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.ActiveTD"), new GUIContent("Active"));
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.TD_X_MouseSensitivity"), new GUIContent("Sensitivity"));
                            
                            var angle = script.thisCamera.GetComponent<CameraController>().CameraOffset.TopDownAngle;
                            angle = EditorGUILayout.Slider("Angle", angle, 60, 90);
                            script.thisCamera.GetComponent<CameraController>().CameraOffset.TopDownAngle = angle;
                            
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraParameters.tdSmoothX"), new GUIContent("Smooth"));
                            
                            break;
                    }
                    
                    EditorGUILayout.EndVertical();

                    break;
                case 1:
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerHealth"), new GUIContent("Health Value"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Ragdoll"), new GUIContent("Ragdoll"));
                    EditorGUILayout.EndVertical();

                    break;

                case 2:

                    EditorGUILayout.Space();
                    //!!!// EditorGUILayout.HelpBox("This name already exist", MessageType.Warning);
                    EditorGUILayout.BeginVertical("box");
                    script.characterTag = EditorGUILayout.Popup("Character Tags", script.characterTag, script.inputs.CharacterTags.ToArray());

                    if (!rename)
                    {
                        if (GUILayout.Button("Rename"))
                        {
                            rename = true;
                            curName = "";
                        }
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical("box");
                        curName = EditorGUILayout.TextField("New name", curName);

                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("Cancel"))
                        {
                            rename = false;
                            curName = "";
                            renameError = false;
                        }
                        
                        if (GUILayout.Button("Save"))
                        {
                            if (!script.inputs.CharacterTags.Contains(curName))
                            {
                                rename = false;
                                script.inputs.CharacterTags[script.characterTag] = curName;
                                curName = "";
                                renameError = false;
                            }
                            else
                            {
                                renameError = true;
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        if (renameError)
                            EditorGUILayout.HelpBox("This name already exist.", MessageType.Warning);

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUI.BeginDisabledGroup(script.inputs.CharacterTags.Count <= 1);
                    if (!delete)
                    {
                        if (GUILayout.Button("Delete"))
                        {
                            delete = true;
                        }
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField("Are you sure?");
                        EditorGUILayout.BeginHorizontal();
                        

                        if (GUILayout.Button("No"))
                        {
                            delete = false;
                        }
                        
                        if (GUILayout.Button("Yes"))
                        {
                            script.inputs.CharacterTags.Remove(script.inputs.CharacterTags[script.characterTag]);
                            script.characterTag = script.inputs.CharacterTags.Count - 1;
                            delete = false;
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndVertical();
                    if (GUILayout.Button("Add new tag"))
                    {
                        if (!script.inputs.CharacterTags.Contains("Character " + script.inputs.CharacterTags.Count))
                            script.inputs.CharacterTags.Add("Character " + script.inputs.CharacterTags.Count);
                        else script.inputs.CharacterTags.Add("Character " + Random.Range(10, 100));

                        script.characterTag = script.inputs.CharacterTags.Count - 1;

                    }

//                    EditorGUILayout.Space();
//                    EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
//                    EditorGUILayout.BeginVertical("box");
//                    EditorGUILayout.HelpBox("If you want to use your motion animations, change them here:",
//                        MessageType.Info);
//                    EditorGUILayout.Space();
//                    
//                    EditorGUILayout.BeginVertical("box");
//                    EditorGUILayout.PropertyField(
//                        serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(8),
//                        new GUIContent("Idle"));
//                    EditorGUILayout.PropertyField(
//                        serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(17),
//                        new GUIContent("Turn right"));
//                    EditorGUILayout.PropertyField(
//                        serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(18),
//                        new GUIContent("Turn left"));
//                    EditorGUILayout.PropertyField(
//                        serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(19),
//                        new GUIContent("Flying in air"));
//                    EditorGUILayout.EndVertical();
//                    
//                    EditorGUILayout.Space();
//                    EditorGUILayout.BeginVertical("box");
//                    WalkAnimations = EditorGUILayout.Foldout(WalkAnimations, "Walk");
//                    if (WalkAnimations)
//                    {
//                        EditorGUILayout.HelpBox(
//                            "All animations must be of [Humanoid] type.", MessageType.Info);
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(0),
//                            new GUIContent("WalkForward"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(1),
//                            new GUIContent("WalkBackward"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(2),
//                            new GUIContent("WalkLeft"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(3),
//                            new GUIContent("WalkRight"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(4),
//                            new GUIContent("WalkForwardLeft"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(5),
//                            new GUIContent("WalkForwardRight"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(6),
//                            new GUIContent("WalkBackwardLeft"));
//
//                        EditorGUILayout.PropertyField(
//                            serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(7),
//                            new GUIContent("WalkBackwardRight"));
//                    }

//                    EditorGUILayout.EndVertical();

//                    EditorGUILayout.Space();
//
//                    if (script.activeSprint)
//                    {
//                        EditorGUILayout.BeginVertical("box");
//                        RunAnimations = EditorGUILayout.Foldout(RunAnimations, "Run");
//                        if (RunAnimations)
//                        {
//                            EditorGUILayout.HelpBox(
//                                "All animations must be of [Humanoid] type.", MessageType.Info);
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(9),
//                                new GUIContent("Run Forward"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(10),
//                                new GUIContent("Run Backward"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(11),
//                                new GUIContent("Run Left"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(12),
//                                new GUIContent("Run Right"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(13),
//                                new GUIContent("Run Forward Left"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(14),
//                                new GUIContent("Run Forward Right"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(15),
//                                new GUIContent("Run Backward Left"));
//
//                            EditorGUILayout.PropertyField(
//                                serializedObject.FindProperty("MovementAnimations").GetArrayElementAtIndex(16),
//                                new GUIContent("Run Backward Right"));
//                        }
//
//                        EditorGUILayout.EndVertical();
//                    }
//
//                    EditorGUILayout.EndVertical();
//                    EditorGUILayout.Space();
                    break;
                
                case 3:
                    EditorGUILayout.Space();
                     EditorGUILayout.LabelField("Movement types", EditorStyles.boldLabel);
                    script.inspectorSettingsTab = GUILayout.Toolbar (script.inspectorSettingsTab, new [] {"Walk", "Run", "Crouch", "Jump"});
                    switch (script.inspectorSettingsTab)
                    {
                        case 0:
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("NormForwardSpeed"),
                                new GUIContent("Forward speed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("NormBackwardSpeed"),
                                new GUIContent("Backward speed"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("NormLateralSpeed"),
                                new GUIContent("Lateral speed"));
                            EditorGUILayout.EndVertical();
//                            EditorGUILayout.Space();
//                            EditorGUILayout.LabelField("Noise radius", EditorStyles.boldLabel);
//                            EditorGUILayout.BeginVertical("box");
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("IdleNoise"),
//                                new GUIContent("Idle"));
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("MovementNoise"),
//                                new GUIContent("Movement"));
//                            EditorGUILayout.EndVertical();
                            break;
                        case 1:
                            EditorGUILayout.Space();
//                            script.activeSprint = EditorGUILayout.Toggle("Enabled", script.activeSprint);
//                            EditorGUILayout.Space();
//                            if (script.activeSprint)
//                            {
                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("RunForwardSpeed"),
                                    new GUIContent("Forward speed"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("RunBackwardSpeed"),
                                    new GUIContent("Backward speed"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("RunLateralSpeed"),
                                    new GUIContent("Lateral speed"));
                                EditorGUILayout.EndVertical();
//                                EditorGUILayout.Space();
//                                EditorGUILayout.LabelField("Noise radius", EditorStyles.boldLabel);
//                                EditorGUILayout.BeginVertical("box");
//                                EditorGUILayout.PropertyField(serializedObject.FindProperty("SprintMovementNoise"),
//                                    new GUIContent("Movement"));
//                                EditorGUILayout.EndVertical();
//                            }
                            
//                            EditorGUILayout.Space();
                            break;
                        case 2:
                            EditorGUILayout.Space();
//                            script.activeCrouch = EditorGUILayout.Toggle("Enabled", script.activeCrouch);
//                            EditorGUILayout.Space();
//                            if (script.activeCrouch)
//                            {

                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchForwardSpeed"),
                                    new GUIContent("Forward speed"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchBackwardSpeed"),
                                    new GUIContent("Backward speed"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchLateralSpeed"),
                                    new GUIContent("Lateral speed"));
                                EditorGUILayout.Space();
                            
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchHeight"),
                                    new GUIContent("Crouch depth"));
                                EditorGUILayout.EndVertical();
//                                EditorGUILayout.Space();
//                                EditorGUILayout.LabelField("Noise radius", EditorStyles.boldLabel);
//                                EditorGUILayout.BeginVertical("box");
//                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchIdleNoise"),
//                                    new GUIContent("Idle"));
//                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchMovementNoise"),
//                                    new GUIContent("Movement"));
//                                EditorGUILayout.EndVertical();
                               
//                            }
//                            EditorGUILayout.Space();
                            break;
                        case 3:
                            EditorGUILayout.Space();
//                            EditorGUILayout.PropertyField(serializedObject.FindProperty("activeJump"),
//                                new GUIContent("Enabled"));
//                            EditorGUILayout.Space();
//                            if (script.activeJump)
//                            {
                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpHeight"),
                                    new GUIContent("Height"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchHeightBeforeJump"),
                                    new GUIContent("Crouch depth before jump"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpSpeed"),
                                    new GUIContent("Speed"));
                                EditorGUILayout.EndVertical();
//                            }
                            
//                            EditorGUILayout.Space();
                            break;
                    }
                    
                    EditorGUILayout.Space();
                    break;
            }

            serializedObject.ApplyModifiedProperties();

//            DrawDefaultInspector();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
                if(!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

        }
    }
}

