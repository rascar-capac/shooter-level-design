// GercStudio
// © 2018-2019

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    public static class Helper
    {
#if UNITY_EDITOR
        [InitializeOnLoad]
        public static class SelectObjectWhenLoadScene
        {

            // constructor
            static SelectObjectWhenLoadScene()
            {
                EditorSceneManager.sceneOpened += SceneOpenedCallback;
            }

            static void SceneOpenedCallback(Scene _scene, OpenSceneMode _mode)
            {
                if (_scene.name == "Adjustment")
                {
                    Selection.activeObject = Object.FindObjectOfType<Adjustment>();
                }
            }
        }
#endif
        
        
        public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
        {
            public AnimationClipOverrides(int capacity) : base(capacity)
            {
            }

            public AnimationClip this[string name]
            {
                get { return Find(x => x.Key.name.Equals(name)).Value; }
                set
                {
                    int index = FindIndex(x => x.Key.name.Equals(name));
                    if (index != -1)
                        this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
                }
            }
        }
        
        public static int layerMask()
        {
            var layerMask = ~ LayerMask.GetMask("Character");
            return layerMask;
        }
        
        public enum NextPointAction
        {
            NextPoint,RandomPoint,ClosestPoint,Stop
        }

        public enum Parent
        {
            Character, Enemy
        }

        [Serializable]
        public class EnemiesInGameManager
        {
            public GameObject enemyPrefab;
            public float Count;
            public float SpawnTimeout;
            public float CurrentTime;
            public int CurrentSpawnMethodIndex;
            public int CurrentSpawnCount;
            public GameObject SpawnArea;
            public WaypointBehavior WaypointBehavior;
        }
        
        
        public enum GamepadAxes
        {
            XAxis,
            YAxis,
            _3rdAxis, _4thAxis, _5thAxis, _6thAxis, _7thAxis, _8thAxis, _9thAxis, _10thAxis, _11thAxis, _12thAxis, 
            _13thAxis, _14thAxis, _15thAxis, _16thAxis, _17thAxis, _18thAxis, _19thAxis,
        }

        public enum KeyBoardCodes
        {
            LeftMouseButton,
            RightMouseButton,
            MiddleMouseButton, 
            Q, W, E, R, T, Y, U, I, O, P, A, S, D, F, G, H, J, K, L, Z, X, C, V, B, N, M, _1, _2, _3, _4, _5, _6, _7, _8, _9, _0,
            Space,
            Backspace,
            LeftShift,
            RightShift,
            LeftCtrl,
            RightCtrl,
            LeftAlt,
            RightAlt,
            Tab,
            Escape
        }

        public enum AxisButtonValue
        {
            Plus, Minus, Both
        }

        public enum GamepadCodes
        {
            JoystickButton0,
            JoystickButton1,
            JoystickButton2,
            JoystickButton3,
            JoystickButton4,
            JoystickButton5,
            JoystickButton6,
            JoystickButton7,
            JoystickButton8,
            JoystickButton9,
            JoystickButton10,
            JoystickButton11,
            JoystickButton12,
            JoystickButton13,
            JoystickButton14,
            JoystickButton15,
            JoystickButton16,
            JoystickButton17,
            JoystickButton18,
            JoystickButton19,
            XAxis,
            YAxis,
            _3rdAxis,
            _4thAxis,
            _5thAxis,
            _6thAxis,
            _7thAxis,
            _8thAxis,
            _9thAxis,
            _10thAxis,
            _11thAxis,
            _12thAxis,
            _13thAxis,
            _14thAxis,
            _15thAxis,
            _16thAxis,
            _17thAxis,
            _18thAxis,
            _19thAxis,
        }


        public static void ConvertGamepadCodes(ref KeyCode gamepadKeys, GamepadCodes gamepadCodes)
        {
            switch (gamepadCodes)
            {
                case GamepadCodes.JoystickButton0:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton0;
                    break;
                case GamepadCodes.JoystickButton1:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton1;
                    break;
                case GamepadCodes.JoystickButton2:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton2;
                    break;
                case GamepadCodes.JoystickButton3:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton3;
                    break;
                case GamepadCodes.JoystickButton4:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton4;
                    break;
                case GamepadCodes.JoystickButton5:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton5;
                    break;
                case GamepadCodes.JoystickButton6:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton6;
                    break;
                case GamepadCodes.JoystickButton7:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton7;
                    break;
                case GamepadCodes.JoystickButton8:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton8;
                    break;
                case GamepadCodes.JoystickButton9:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton9;
                    break;
                case GamepadCodes.JoystickButton10:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton10;
                    break;
                case GamepadCodes.JoystickButton11:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton11;
                    break;
                case GamepadCodes.JoystickButton12:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton12;
                    break;
                case GamepadCodes.JoystickButton13:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton14:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton15:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton16:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton17:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton18:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                case GamepadCodes.JoystickButton19:
                    gamepadKeys = UnityEngine.KeyCode.JoystickButton13;
                    break;
                
            }
        }

        public static void ConvertKeyCodes(ref KeyCode keyboardKeys, KeyBoardCodes keyBoardCodes)
        {
            switch (keyBoardCodes)
            {
                case KeyBoardCodes.RightMouseButton:
                    keyboardKeys = KeyCode.Mouse1;
                    break;
                case KeyBoardCodes.LeftMouseButton:
                    keyboardKeys = KeyCode.Mouse0;
                    break;
                case KeyBoardCodes.MiddleMouseButton:
                    keyboardKeys = KeyCode.Mouse2;
                    break;
                case KeyBoardCodes.Q:
                    keyboardKeys = KeyCode.Q;
                    break;
                case KeyBoardCodes.W:
                    keyboardKeys = KeyCode.W;
                    break;
                case KeyBoardCodes.E:
                    keyboardKeys = KeyCode.E;
                    break;
                case KeyBoardCodes.R:
                    keyboardKeys = KeyCode.R;
                    break;
                case KeyBoardCodes.T:
                    keyboardKeys = KeyCode.T;
                    break;
                case KeyBoardCodes.Y:
                    keyboardKeys = KeyCode.Y;
                    break;
                case KeyBoardCodes.U:
                    keyboardKeys = KeyCode.U;
                    break;
                case KeyBoardCodes.I:
                    keyboardKeys = KeyCode.I;
                    break;
                case KeyBoardCodes.O:
                    keyboardKeys = KeyCode.O;
                    break;
                case KeyBoardCodes.P:
                    keyboardKeys = KeyCode.P;
                    break;
                case KeyBoardCodes.A:
                    keyboardKeys = KeyCode.A;
                    break;
                case KeyBoardCodes.S:
                    keyboardKeys = KeyCode.S;
                    break;
                case KeyBoardCodes.D:
                    keyboardKeys = KeyCode.D;
                    break;
                case KeyBoardCodes.F:
                    keyboardKeys = KeyCode.F;
                    break;
                case KeyBoardCodes.G:
                    keyboardKeys = KeyCode.G;
                    break;
                case KeyBoardCodes.H:
                    keyboardKeys = KeyCode.H;
                    break;
                case KeyBoardCodes.J:
                    keyboardKeys = KeyCode.J;
                    break;
                case KeyBoardCodes.K:
                    keyboardKeys = KeyCode.K;
                    break;
                case KeyBoardCodes.L:
                    keyboardKeys = KeyCode.L;
                    break;
                case KeyBoardCodes.Z:
                    keyboardKeys = KeyCode.Z;
                    break;
                case KeyBoardCodes.X:
                    keyboardKeys = KeyCode.X;
                    break;
                case KeyBoardCodes.C:
                    keyboardKeys = KeyCode.C;
                    break;
                case KeyBoardCodes.V:
                    keyboardKeys = KeyCode.V;
                    break;
                case KeyBoardCodes.B:
                    keyboardKeys = KeyCode.B;
                    break;
                case KeyBoardCodes.N:
                    keyboardKeys = KeyCode.N;
                    break;
                case KeyBoardCodes.M:
                    keyboardKeys = KeyCode.M;
                    break;
                case KeyBoardCodes._0:
                    keyboardKeys = KeyCode.Alpha0;
                    break;
                case KeyBoardCodes._1:
                    keyboardKeys = KeyCode.Alpha1;
                    break;
                case KeyBoardCodes._2:
                    keyboardKeys = KeyCode.Alpha2;
                    break;
                case KeyBoardCodes._3:
                    keyboardKeys = KeyCode.Alpha3;
                    break;
                case KeyBoardCodes._4:
                    keyboardKeys = KeyCode.Alpha4;
                    break;
                case KeyBoardCodes._5:
                    keyboardKeys = KeyCode.Alpha5;
                    break;
                case KeyBoardCodes._6:
                    keyboardKeys = KeyCode.Alpha6;
                    break;
                case KeyBoardCodes._7:
                    keyboardKeys = KeyCode.Alpha7;
                    break;
                case KeyBoardCodes._8:
                    keyboardKeys = KeyCode.Alpha8;
                    break;
                case KeyBoardCodes._9:
                    keyboardKeys = KeyCode.Alpha9;
                    break;
                case KeyBoardCodes.Space:
                    keyboardKeys = KeyCode.Space;
                    break;
                case KeyBoardCodes.Backspace:
                    keyboardKeys = KeyCode.Backspace;
                    break;
                case KeyBoardCodes.LeftShift:
                    keyboardKeys = KeyCode.LeftShift;
                    break;
                case KeyBoardCodes.RightShift:
                    keyboardKeys = KeyCode.RightShift;
                    break;
                case KeyBoardCodes.LeftCtrl:
                    keyboardKeys = KeyCode.LeftControl;
                    break;
                case KeyBoardCodes.RightCtrl:
                    keyboardKeys = KeyCode.RightControl;
                    break;
                case KeyBoardCodes.LeftAlt:
                    keyboardKeys = KeyCode.LeftAlt;
                    break;
                case KeyBoardCodes.RightAlt:
                    keyboardKeys = KeyCode.RightAlt;
                    break;
                case KeyBoardCodes.Tab:
                    keyboardKeys = KeyCode.Tab;
                    break;
                case KeyBoardCodes.Escape:
                    keyboardKeys = KeyCode.Escape;
                    break;
            }
        }



        public static void ConvertAxes(ref string axis, GamepadAxes gamepadAxes)
        {
            switch (gamepadAxes)
                {
                    case GamepadAxes.XAxis:
                        axis = "Gamepad Horizontal";
                        break;
                    case GamepadAxes.YAxis:
                        axis = "Gamepad Vertical";
                        break;
                    case GamepadAxes._3rdAxis:
                        axis = "Gamepad 3rd axis";
                        break;
                    case GamepadAxes._4thAxis:
                        axis = "Gamepad 4th axis";
                        break;
                    case GamepadAxes._5thAxis:
                        axis = "Gamepad 5th axis";
                        break;
                    case GamepadAxes._6thAxis:
                        axis = "Gamepad 6th axis";
                        break;
                    case GamepadAxes._7thAxis:
                        axis = "Gamepad 7th axis";
                        break;
                    case GamepadAxes._8thAxis:
                        axis = "Gamepad 8th axis";
                        break;
                    case GamepadAxes._9thAxis:
                        axis = "Gamepad 9th axis";
                        break;
                    case GamepadAxes._10thAxis:
                        axis = "Gamepad 10th axis";
                        break;
                    case GamepadAxes._11thAxis:
                        axis = "Gamepad 11th axis";
                        break;
                    case GamepadAxes._12thAxis:
                        axis = "Gamepad 12th axis";
                        break;
                    case GamepadAxes._13thAxis:
                        axis = "Gamepad 13th axis";
                        break;
                    case GamepadAxes._14thAxis:
                        axis = "Gamepad 14th axis";
                        break;
                    case GamepadAxes._15thAxis:
                        axis = "Gamepad 15th axis";
                        break;
                    case GamepadAxes._16thAxis:
                        axis = "Gamepad 16th axis";
                        break;
                    case GamepadAxes._17thAxis:
                        axis = "Gamepad 17th axis";
                        break;
                    case GamepadAxes._18thAxis:
                        axis = "Gamepad 18th axis";
                        break;
                    case GamepadAxes._19thAxis:
                        axis = "Gamepad 19th axis";
                        break;
                }
        }

        public static void ConvertAxes(ref string axis, GamepadCodes gamepadCodes)
        {
            switch (gamepadCodes)
            {
                case GamepadCodes.XAxis:
                    axis = "Gamepad Horizontal";
                    break;
                case GamepadCodes.YAxis:
                    axis = "Gamepad Vertical";
                    break;
                case GamepadCodes._3rdAxis:
                    axis = "Gamepad 3rd axis";
                    break;
                case GamepadCodes._4thAxis:
                    axis = "Gamepad 4th axis";
                    break;
                case GamepadCodes._5thAxis:
                    axis = "Gamepad 5th axis";
                    break;
                case GamepadCodes._6thAxis:
                    axis = "Gamepad 6th axis";
                    break;
                case GamepadCodes._7thAxis:
                    axis = "Gamepad 7th axis";
                    break;
                case GamepadCodes._8thAxis:
                    axis = "Gamepad 8th axis";
                    break;
                case GamepadCodes._9thAxis:
                    axis = "Gamepad 9th axis";
                    break;
                case GamepadCodes._10thAxis:
                    axis = "Gamepad 10th axis";
                    break;
                case GamepadCodes._11thAxis:
                    axis = "Gamepad 11th axis";
                    break;
                case GamepadCodes._12thAxis:
                    axis = "Gamepad 12th axis";
                    break;
                case GamepadCodes._13thAxis:
                    axis = "Gamepad 13th axis";
                    break;
                case GamepadCodes._14thAxis:
                    axis = "Gamepad 14th axis";
                    break;
                case GamepadCodes._15thAxis:
                    axis = "Gamepad 15th axis";
                    break;
                case GamepadCodes._16thAxis:
                    axis = "Gamepad 16th axis";
                    break;
                case GamepadCodes._17thAxis:
                    axis = "Gamepad 17th axis";
                    break;
                case GamepadCodes._18thAxis:
                    axis = "Gamepad 18th axis";
                    break;
                case GamepadCodes._19thAxis:
                    axis = "Gamepad 19th axis";
                    break;
            }
        }
        

        public static Color32[] colors =
        {
            new Color32(255, 190, 0, 255),
            new Color32(188, 140, 0, 255),
            new Color32(0, 67, 255, 255)
        };
       

        public struct ClipPlanePoints
        {
            public Vector3 UpperRight;
            public Vector3 UpperLeft;
            public Vector3 LowerRight;
            public Vector3 LowerLeft;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;

                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        public static ClipPlanePoints NearPoints(Vector3 pos, Camera camera)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;

            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }

        public static bool HasParameter(string paramName, Animator animator)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName) return true;
            }

            return false;
        }

        public static Vector3 MoveObjInNewPosition(Vector3 position,Vector3 newPosition, float Speed)
        {
            var x = Mathf.Lerp(position.x, newPosition.x, Speed);
            var y = Mathf.Lerp(position.y, newPosition.y, Speed);
            var z = Mathf.Lerp(position.z, newPosition.z, Speed);
            
            var newPos = new Vector3(x, y, z);

            return newPos;
        }

        public static void CopyTransformsRecurse(Transform src, Transform dst)
        {
            dst.position = src.position;
            dst.rotation = src.rotation;

            foreach (Transform child in dst)
            {
                var curSrc = src.Find(child.name);
                if (curSrc)
                    CopyTransformsRecurse(curSrc, child);
            }
        }

        static void AddOutline(GameObject obj)
        {
            var outline = obj.AddComponent<Outline>();
            outline.effectColor = new Color32(0, 0, 0, 200);
            outline.effectDistance = new Vector2(3,-3);
        }

        public static void ChangeLayersRecursively(Transform trans, String name)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in trans)
            {
                child.gameObject.layer = LayerMask.NameToLayer(name);
                ChangeLayersRecursively(child, name);
            }
        }

        public static float AngleBetween(Vector3 direction1, Vector3 direction2)
        {
            if (direction1 == Vector3.zero || direction2 == Vector3.zero) return 0;
            
            var dir1 = Quaternion.LookRotation(direction1);
            var dir1Angle = dir1.eulerAngles.y;
            if (dir1Angle > 180)
                dir1Angle -= 360;

            var dir2 = Quaternion.LookRotation(direction2);
            var dir2Angle = dir2.eulerAngles.y;
            if (dir2Angle > 180)
                dir2Angle -= 360;

            var middleAngle = Mathf.DeltaAngle(dir1Angle, dir2Angle);
            
            return middleAngle;
        }

        public static Vector2 AngleBetween(Vector3 direction1, Transform obj)
        {
            var look1 = Quaternion.LookRotation(direction1);

            var dir1AngleY = look1.eulerAngles.y;
            if (dir1AngleY > 180)
                dir1AngleY -= 360;
            
            var dir2AngleY = obj.eulerAngles.y;
            if (dir2AngleY > 180)
                dir2AngleY -= 360;

            var middleAngleY = Mathf.DeltaAngle(dir1AngleY, dir2AngleY);

            var dir1AngleX = look1.eulerAngles.x;
            if (dir1AngleX > 180)
                dir1AngleX -= 360;

            var dir2AngleX = obj.eulerAngles.x;
            if (dir2AngleX > 180)
                dir2AngleX -= 360;

            var middleAngleX = Mathf.DeltaAngle(dir1AngleX, dir2AngleX);

            return new Vector2(middleAngleX, middleAngleY);
        }

        public static bool ReachedPositionAndRotation(Vector3 position1, Vector3 position2, Vector3 angles1, Vector3 angles2)
        {
            return Math.Abs(position1.x - position2.x) < 0.2f && Math.Abs(position1.y - position2.y) < 0.2f && Math.Abs(position1.z - position2.z) < 0.2f &&
                   Math.Abs(angles1.x - angles2.x) < 0.2f && Math.Abs(angles1.y - angles2.y) < 0.2f && Math.Abs(angles1.z - angles2.z) < 0.2f;
        }
        
        public static bool ReachedPositionAndRotation(Vector3 position1, Vector3 position2)
        {
            return Math.Abs(position1.x - position2.x) < 0.1f && Math.Abs(position1.y - position2.y) < 0.1f && Math.Abs(position1.z - position2.z) < 0.1f;
        }


        public static Camera NewCamera(string name, Transform parent, string type)
        {
            Camera camera = new GameObject(name).AddComponent<Camera>();

            if (type != "GameManager")
            {
                camera.cullingMask = 1 << 8;
                camera.depth = 1;
                camera.clearFlags = CameraClearFlags.Depth;
                camera.nearClipPlane = 0.01f;
            }

            camera.transform.parent = parent;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.Euler(0, 0, 0);

            return camera;
        }

        public static void ChangeColor(Button button, Color highlightColor, Sprite sprite)
        {
            switch (button.transition)
            {
                case Selectable.Transition.ColorTint:
                    var colors = button.colors;
                    colors.normalColor = highlightColor;
                    button.colors = colors;
                    break;
                case Selectable.Transition.SpriteSwap:
                    if (sprite)
                        button.GetComponent<Image>().sprite = sprite;
                    break;
            }
        }

        public static void ChangeButtonColor (InventoryManager manager, int slot, string type)
        {
            if (type != "norm")
            {
                ChangeColor(manager.slots[slot].SlotButton, manager.slots[slot].SlotButton.colors.highlightedColor , manager.slots[slot].SlotButton.spriteState.highlightedSprite);
            }
            else
            {
                ChangeColor(manager.slots[slot].SlotButton, manager.normButtonsColors[slot], manager.normButtonsSprites[slot]);
            }
        }

        public static void AddButtonsEvents(Button[] buttons, InventoryManager manager, Controller controller)
        {
            buttons[0].onClick.AddListener(manager.UIAim);
            buttons[1].onClick.AddListener(manager.UIReload);
            buttons[2].onClick.AddListener(controller.ChangeCameraType);
            buttons[3].onClick.AddListener(manager.LaunchGrenade);
            buttons[4].onClick.AddListener(manager.DropWeapon);
            buttons[8].onClick.AddListener(controller.Jump);
            buttons[11].onClick.AddListener(manager.UIPickUp);
            buttons[14].onClick.AddListener(manager.WeaponDown);
            buttons[15].onClick.AddListener(manager.WeaponUp);
            buttons[17].onClick.AddListener(manager.UIChangeAttackType);

            addEventTriger(buttons[5].gameObject, manager, controller, "Attack");

            if (controller.inputs.PressSprintButton)
                addEventTriger(buttons[6].gameObject, manager, controller, "Sprint");
            else
                buttons[6].onClick.AddListener(delegate { controller.Sprint(true, "click"); });


            if (controller.inputs.PressCrouchButton)
                addEventTriger(buttons[7].gameObject, manager, controller, "Crouch");
            else
                buttons[7].onClick.AddListener(delegate { controller.Crouch(true, "click"); });


            if (manager.pressInventoryButton)
                addEventTriger(buttons[10].gameObject, manager, controller, "Inventory");
            else
                buttons[10].onClick.AddListener(manager.UIInventory);
        }

        public static void addEventTriger(GameObject button, InventoryManager manager, Controller controller, string type)
        {
            var eventTrigger = button.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};

            switch (type)
            {
                case "Attack":
                    entry.callback.AddListener(data => { manager.UIAttack(); });
                    break;
                case "Sprint":
                    entry.callback.AddListener(data => { controller.Sprint(true, "press"); });
                    break;
                case "Crouch":
                    entry.callback.AddListener(data => { controller.Crouch(true, "press"); });
                    break;
                case "Inventory":
                    entry.callback.AddListener(data => { manager.UIActivateInventory(); });
                    break;
            }
            eventTrigger.triggers.Add(entry);
            entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerUp};

            switch (type)
            {
                case "Attack":
                    entry.callback.AddListener(data => { manager.UIEndAttack(); });
                    break;
                case "Sprint":
                    entry.callback.AddListener(data => { controller.Sprint(false, "press"); });
                    break;
                case "Crouch":
                    entry.callback.AddListener(data => { controller.Crouch(false, "press"); });
                    break;
                case "Inventory":
                    entry.callback.AddListener(data => { manager.UIDeactivateInventory(); });
                    break;
            }
            
            eventTrigger.triggers.Add(entry);
        }
        
        public static Transform NewObject(Transform parent, string name, PrimitiveType type, Color color)
        {
            var sourse = GameObject.CreatePrimitive(type).transform;
            sourse.name = name;
            sourse.hideFlags = HideFlags.HideInHierarchy;
            sourse.GetComponent<MeshRenderer>().enabled = false;
            sourse.GetComponent<MeshRenderer>().material = NewMaterial(color);
            sourse.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            sourse.parent = parent;
            sourse.localPosition = Vector3.zero;
            sourse.localRotation = Quaternion.Euler(Vector3.zero);
            ChangeLayersRecursively(sourse, "Character");
            return sourse;
        }
        public static Material NewMaterial(Color color)
        {
            var mat = new Material(Shader.Find("Standard")) {name = "Standard", color = color};
            return mat;
        }

        public static string GeneratePickUpId()
        {
            const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";

            var pickUpId = "";
            
            for (int i = 0; i < 20; i++)
            {
                pickUpId += glyphs[Random.Range(0, glyphs.Length)];
            }

            return pickUpId;
        }

      

        public static bool CheckGamepadAxisButton(int number, string[] _gamepadButtonsAxes, bool[] hasAxisButtonPressed,
            string type, AxisButtonValue value)
        {
            if (_gamepadButtonsAxes[number] != "")
            {
                if (type == "GetKeyDown")
                {
                    switch (value)
                    {
                        case AxisButtonValue.Both:
                        {
                            if ((Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f ||
                                 Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f) & !hasAxisButtonPressed[number])
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                        }

                        case AxisButtonValue.Plus:
                        {
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f & !hasAxisButtonPressed[number])
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                        }

                        case AxisButtonValue.Minus:
                        {
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f & !hasAxisButtonPressed[number])
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                        }
                    }

                    if (Input.GetAxis(_gamepadButtonsAxes[number]) < 0.1f && Input.GetAxis(_gamepadButtonsAxes[number]) > -0.1f && hasAxisButtonPressed[number])
                    {
                        hasAxisButtonPressed[number] = false;
                        return false;
                    }
                }
                else
                {
                    switch (value)
                    {
                        case AxisButtonValue.Both:
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f ||
                                Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f)
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;

                        case AxisButtonValue.Plus:
                            if (Input.GetAxis(_gamepadButtonsAxes[number]) > 0.5f)
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;

                        case AxisButtonValue.Minus:

                            if (Input.GetAxis(_gamepadButtonsAxes[number]) < -0.5f)
                            {
                                hasAxisButtonPressed[number] = true;
                                return true;
                            }

                            break;
                    }


                    if (Math.Abs(Input.GetAxis(_gamepadButtonsAxes[number])) < 0.2f &
                        Math.Abs(Input.GetAxis(_gamepadButtonsAxes[number])) > -0.2f)
                    {
                        hasAxisButtonPressed[number] = false;
                        return false;
                    }
                }
            }

            return false;
        }

        public static void CreateNoiseCollider(Transform parent, Controller script)
        {
            var noiseCollider = new GameObject("Noise Collider");
            noiseCollider.transform.parent = parent;
            noiseCollider.transform.localPosition= Vector3.zero;
            noiseCollider.transform.localEulerAngles = Vector3.zero;

            script.noiseCollider = noiseCollider.AddComponent<SphereCollider>();
            script.noiseCollider.isTrigger = true;
            var rb = noiseCollider.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            noiseCollider.hideFlags = HideFlags.HideInHierarchy;
        } 

#if UNITY_EDITOR


        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Vector3.one);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (_height - (_radius * 2)) / 2;
 
                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
 
            }
        }
        
        [MenuItem("Window/USK/Adjust")]
        public static void OpenAdjustmentScene()
        {
            if (SceneManager.GetActiveScene().name != "Adjustment")
            {
                if(EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] {SceneManager.GetActiveScene()}))
                    EditorSceneManager.OpenScene("Assets/Universal Shooter Kit/Tools/Editor Assets/Adjustment.unity", OpenSceneMode.Single);
            }
        }
        
        [MenuItem("Edit/USK Project Settings/Input")]
        public static void Inputs()
        {
            var inputs = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(Inputs)) as Inputs;
            Selection.objects = new Object[] {inputs};
            EditorGUIUtility.PingObject(inputs);
        }
        
        [MenuItem("Edit/USK Project Settings/UI")]
        public static void UI()
        {
            var ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI.asset", typeof(UI)) as UI;
            Selection.objects = new Object[] {ui};
            EditorGUIUtility.PingObject(ui);
        }
        
         //[MenuItem("GameObject/USK/WayPoint Behavior", false, 10)]
        public static void CreateWaypointBehavior()
        {
            var behavior = new GameObject("Movement Behaviour");
            behavior.AddComponent<WaypointBehavior>();
            Selection.activeObject = behavior;
        }

        [MenuItem("GameObject/USK/Room Manager", false, 10)]
        public static void CreateRoomManager()
        {
            var manager = new GameObject("Room Manager");
            var script = manager.AddComponent<RoomManager>();
            
            script.Ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI.asset", typeof(UI)) as UI;
            
            script.DefaultCamera = NewCamera("Default camera", manager.transform, "GameManager").gameObject;
            Object.DestroyImmediate(script.DefaultCamera.GetComponent<AudioListener>());
            
            var font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
            
            var newCanvas = NewCanvas("Canvas", new Vector2(1920, 1080), manager.transform);
            
            var startMenu = NewUIElement("Start Menu", newCanvas.transform, new Vector2(0, 0), new Vector2(1100, 660), Vector3.one);

            var image = startMenu.AddComponent<Image>();
            image.color = new Color32(0, 152, 255, 200);

            var scrollRect = startMenu.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.inertia = false;
            scrollRect.scrollSensitivity = 4;

            var viewPort = NewUIElement("Viewport", startMenu.transform, new Vector2(0, 0), new Vector2(0, 0), Vector3.one);
            viewPort.AddComponent<Mask>().showMaskGraphic = false;
            viewPort.AddComponent<Image>();
            viewPort.GetComponent<RectTransform>().anchorMax = Vector2.one;
            viewPort.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            viewPort.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

            scrollRect.viewport = viewPort.GetComponent<RectTransform>();
            
            var playersContent = NewUIElement("Players Content", viewPort.transform, new Vector2(0, 295), new Vector2(0, 0), Vector3.one);
            playersContent.GetComponent<RectTransform>().anchorMax = Vector2.one;
            playersContent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            playersContent.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            
            var verticalLayout = playersContent.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 3;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;

            var sizeFilter = playersContent.AddComponent<ContentSizeFitter>();
            sizeFilter.verticalFit = ContentSizeFitter.FitMode.MinSize;

            scrollRect.content = playersContent.GetComponent<RectTransform>();

            var Start = NewButton("Start Button", new Vector2(356, -443), new Vector2(300, 50), Vector3.one, colors, startMenu.transform);
            NewText("Text", Start.transform, Vector2.zero, new Vector2(300, 50), "Start", font, 25, TextAnchor.MiddleCenter, Color.black, false);
            AddOutline(Start.gameObject);
            Start.gameObject.AddComponent<Image>().color = Color.white;
            
            var Resume = NewButton("Resume Button", new Vector2(356, -443), new Vector2(300, 50), Vector3.one, colors, startMenu.transform);
            NewText("Text", Resume.transform, Vector2.zero, new Vector2(300, 50), "Resume", font, 25, TextAnchor.MiddleCenter, Color.black, false);
            AddOutline(Resume.gameObject);
            Resume.gameObject.AddComponent<Image>().color = Color.white;
            Resume.gameObject.SetActive(false);
            
            var Exit = NewButton("Exit Button", new Vector2(-356, -443), new Vector2(300, 50), Vector3.one, colors, startMenu.transform);
            NewText("Text", Exit.transform, Vector2.zero, new Vector2(300, 50), "Exit", font, 25, TextAnchor.MiddleCenter, Color.black, false);
            AddOutline(Exit.gameObject);
            Exit.gameObject.AddComponent<Image>().color = Color.white;

            script.StartMenu = startMenu;
            script.PlayersPanel = playersContent.transform;

            script.StartButton = Start;
            script.ResumeButton = Resume;
            script.BackButton = Exit;
            
            script.PlayerListingPrefab = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Photon Multiplayer/Prefabs/UI/PlayerListing.prefab", typeof(GameObject)) as GameObject;

            script.TimerText = NewText("Timer text", newCanvas.transform, new Vector2(0, 473), new Vector2(300, 100), "Left Time: 10:23", font, 30,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();
            
            script.RestartTimer = NewText("Restart time text", newCanvas.transform, new Vector2(0, 473), new Vector2(300, 100), "Left Time: 10:23", font, 30,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();
            
            
            var statsBar = NewUIElement("Stats bar", newCanvas.transform, new Vector2(685, 435), new Vector2(556, 206), Vector3.one);

            scrollRect = statsBar.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;

            viewPort = NewUIElement("Viewport", statsBar.transform, new Vector2(0, 0), new Vector2(0, 0), Vector3.one);
            viewPort.AddComponent<Mask>().showMaskGraphic = false;
            viewPort.AddComponent<Image>();
            viewPort.GetComponent<RectTransform>().anchorMax = Vector2.one;
            viewPort.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            viewPort.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

            scrollRect.viewport = viewPort.GetComponent<RectTransform>();
            
            playersContent = NewUIElement("Stats Content", viewPort.transform, new Vector2(0, 0), new Vector2(0, 0), Vector3.one);
            playersContent.GetComponent<RectTransform>().anchorMax = Vector2.one;
            playersContent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            playersContent.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

            verticalLayout = playersContent.AddComponent<VerticalLayoutGroup>();
            verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.spacing = 1;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;

            sizeFilter = playersContent.AddComponent<ContentSizeFitter>();
            sizeFilter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            
            var scrollbar = NewUIElement("Scrollbar", statsBar.transform, new Vector2(260,0), new Vector2(20, 189), Vector3.one).AddComponent<Scrollbar>();
            
            var slideArea = NewUIElement("Sliding Area", scrollbar.transform, new Vector2(0,0), new Vector2(0, 168), Vector3.one);
            
            var slider = NewUIElement("Handle", slideArea.transform, new Vector2(0,0), new Vector2(0, 188), Vector3.one);

            scrollbar.handleRect = slider.GetComponent<RectTransform>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scrollbar.value = 1;
            scrollbar.size = 1;
            scrollbar.gameObject.AddComponent<Image>().enabled = false;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.content = playersContent.GetComponent<RectTransform>();

            script.StatsContent = statsBar.transform;
            script.StatsText = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Photon Multiplayer/Prefabs/UI/StatsText.prefab", typeof(Text)) as Text;

            script.Canvas = newCanvas.transform;
        }

        [MenuItem("GameObject/USK/Game Manger", false, 10)]
        public static void CreateGameManger()
        {
            var manager = new GameObject("GameManger");
            var script = manager.AddComponent<GameManager>();
            
            script.Ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/UI.asset", typeof(UI)) as UI;

            NewCamera("Default camera", manager.transform, "GameManager");

            var font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;

            var newCanvas = NewCanvas("Canvas", new Vector2(1920, 1080), manager.transform);
//            
//            var crosshairUI = NewUIElement("Crosshair UI", newCanvas.transform, new Vector2(0, 0), new Vector2(100, 100), Vector3.one);
//            var characterUI = NewUIElement("Character UI", newCanvas.transform, new Vector2(0, 0), new Vector2(100, 100), Vector3.one);
            
            var pauseUI = NewUIElement("Pause UI", newCanvas.transform, new Vector2(0, 0), new Vector2(100, 100), Vector3.one);

//            script.defaultCrosshair = CharacterHelper.CreateCrosshair(crosshairUI.transform, "crosshair");
//            script.pickUpIcon = CharacterHelper.CreateCrosshair(crosshairUI.transform, "pickup").GetComponent<Image>();
//
//            script.defaultCrosshair.SetActive(false);
//            script.pickUpIcon.gameObject.SetActive(false);
//
//            var healthInt = NewText("Health(Int)", characterUI.transform, new Vector2(-600, 471),
//                new Vector2(200, 50), "100", font, 40, TextAnchor.UpperLeft, Color.green, true);
//            healthInt.SetActive(false);
//            NewText("Health(Text)", healthInt.transform, new Vector2(-216, 0), new Vector2(200, 50), "Health",
//                font, 40, TextAnchor.UpperRight, Color.white, true);
//            script.Health = healthInt.GetComponent<Text>();
//
//            var AmmoInt = NewText("Ammo(Int)", characterUI.transform, new Vector2(-527, 400),
//                new Vector2(345, 50), "25/100", font, 40, TextAnchor.UpperLeft, Color.green, true);
//            AmmoInt.SetActive(false);
//            NewText("Ammo(Text)", AmmoInt.transform, new Vector2(-288, 0), new Vector2(200, 50), "Ammo",
//                font, 40, TextAnchor.UpperRight, Color.white, true);
//            script.WeaponAmmo = AmmoInt.GetComponent<Text>();
//
//            var GrenadeInt = NewText("Grenade(Int)", characterUI.transform, new Vector2(-600, 330),
//                new Vector2(200, 50), "10", font, 40, TextAnchor.UpperLeft, Color.green, true);
//            GrenadeInt.SetActive(false);
//            NewText("Grenade(Text)", GrenadeInt.transform, new Vector2(-265, 0), new Vector2(300, 50),
//                "Grens", font, 40, TextAnchor.UpperRight, Color.white, true);
//            script.GrenadeAmmo = GrenadeInt.GetComponent<Text>();
            
            
            script.UsePause = true;

            script.pauseBackground = NewUIElement("Background", pauseUI.transform, new Vector2(0, 12), new Vector2(326, 146), Vector3.one);
            script.pauseBackground.AddComponent<Image>().color = new Color32(0, 152, 255, 200);
            script.pauseBackground.SetActive(false);

            script.Resume = NewButton("Resume", new Vector2(0, 44), new Vector2(300, 50), Vector3.one, colors, pauseUI.transform);
            NewText("Text", script.Resume.transform, Vector2.zero, new Vector2(300, 50), "Resume", font, 25,
                TextAnchor.MiddleCenter, Color.black, false);
            AddOutline(script.Resume.gameObject);
            script.Resume.gameObject.SetActive(false);
            script.Resume.gameObject.AddComponent<Image>().color = Color.white;

            script.Restart = NewButton("Restart", new Vector2(0, 44), new Vector2(300, 50), Vector3.one, colors, pauseUI.transform);
            NewText("Text", script.Restart.transform, Vector2.zero, new Vector2(300, 50), "Restart", font, 25,
                TextAnchor.MiddleCenter, Color.black, false);
            AddOutline(script.Restart.gameObject);
            script.Restart.gameObject.SetActive(false);
            script.Restart.gameObject.AddComponent<Image>().color = Color.white;

            script.Exit = NewButton("Exit", new Vector2(0, -23), new Vector2(300, 50), Vector3.one, colors, pauseUI.transform);
            NewText("Text", script.Exit.transform, Vector2.zero, new Vector2(300, 50), "Exit", font, 25,
                TextAnchor.MiddleCenter, Color.black, false);
            AddOutline(script.Exit.gameObject);
            script.Exit.gameObject.SetActive(false);
            script.Exit.gameObject.AddComponent<Image>().color = Color.white;

            script.Canvas = newCanvas.transform;

            Selection.activeObject = manager;
        }

        public static GameObject CreateUI(UI script)
        {
            var font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
            
            var newCanvas = NewCanvas("Canvas", new Vector2(1920, 1080));
//            var crosshairUI = NewUIElement("Crosshair UI", newCanvas.transform, new Vector2(0, 0), new Vector2(100, 100), Vector3.one);
//            var characterUI = NewUIElement("Character UI", newCanvas.transform, new Vector2(0, 0), new Vector2(100, 100), Vector3.one);
            
            script.defaultCrosshair = CharacterHelper.CreateCrosshair(newCanvas.transform, "crosshair");
            script.pickUpIcon = CharacterHelper.CreateCrosshair(newCanvas.transform, "pickup").GetComponent<Image>();

            script.defaultCrosshair.SetActive(false);
            script.pickUpIcon.gameObject.SetActive(false);

            var healthInt = NewText("Health(Int)", newCanvas.transform, new Vector2(-600, 471),
                new Vector2(200, 50), "100", font, 40, TextAnchor.UpperLeft, Color.green, true);
            healthInt.SetActive(false);
            NewText("Health(Text)", healthInt.transform, new Vector2(-216, 0), new Vector2(200, 50), "Health",
                font, 40, TextAnchor.UpperRight, Color.white, true);
            script.Health = healthInt.GetComponent<Text>();

            var AmmoInt = NewText("Ammo(Int)", newCanvas.transform, new Vector2(-527, 400),
                new Vector2(345, 50), "25/100", font, 40, TextAnchor.UpperLeft, Color.green, true);
            AmmoInt.SetActive(false);
            NewText("Ammo(Text)", AmmoInt.transform, new Vector2(-288, 0), new Vector2(200, 50), "Ammo",
                font, 40, TextAnchor.UpperRight, Color.white, true);
            script.WeaponAmmo = AmmoInt.GetComponent<Text>();

            var GrenadeInt = NewText("Grenade(Int)", newCanvas.transform, new Vector2(-600, 330),
                new Vector2(200, 50), "10", font, 40, TextAnchor.UpperLeft, Color.green, true);
            GrenadeInt.SetActive(false);
            NewText("Grenade(Text)", GrenadeInt.transform, new Vector2(-265, 0), new Vector2(300, 50),
                "Grens", font, 40, TextAnchor.UpperRight, Color.white, true);
            script.GrenadeAmmo = GrenadeInt.GetComponent<Text>();
            
            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/_UI/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/_UI/");
            }

#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/_UI/Character UI.prefab");
            PrefabUtility.ReplacePrefab(newCanvas.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(newCanvas.gameObject, "Assets/Universal Shooter Kit/Prefabs/_UI/Character UI.prefab");
#endif
            
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_UI/Character UI.prefab", typeof(GameObject)) as GameObject);

            var ui = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_UI/Character UI.prefab", typeof(GameObject)) as GameObject;
            
            script.defaultCrosshair = ui.transform.GetChild(0).gameObject;
            script.pickUpIcon = ui.transform.GetChild(1).GetComponent<Image>();
            script.Health = ui.transform.GetChild(2).GetComponent<Text>();
            script.WeaponAmmo = ui.transform.GetChild(3).GetComponent<Text>();
            script.GrenadeAmmo = ui.transform.GetChild(4).GetComponent<Text>();

            script.UIPrefab = ui.gameObject;

            return newCanvas.gameObject;
        }
        
        public static void AddObjectIcon(GameObject obj, string icon)
        {
            var image = (Texture2D)Resources.Load(icon);
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { obj, image };
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
        
        public static GameObject CreateWayPoint()
        {
            var foundObjects = GameObject.FindGameObjectsWithTag("WayPoint");
            var curPointNumber = 0;

            foreach (var obj in foundObjects)
            {
                obj.name = "Waypoint " + curPointNumber;
                curPointNumber++;
            }

            var waypoint = new GameObject("Waypoint " + curPointNumber);
            waypoint.tag = "WayPoint";
            AddObjectIcon(waypoint, "DefaultWaypoint");

            if (SceneView.lastActiveSceneView)
            {
                var transform = SceneView.lastActiveSceneView.camera.transform;
                waypoint.transform.position = transform.position + transform.forward * 7;
            }
            
            
            Selection.activeObject = waypoint;
            EditorGUIUtility.PingObject(waypoint);
            
            return waypoint;
        }

        public static GameObject CreateButtons(Inputs inputs)
        {
            Canvas canvas = NewCanvas("UI Buttons", new Vector2(1280, 720), null);

            var aim = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Aim.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[0] = NewButton("Aim", new Vector2(405, 141), new Vector2(100, 100), Vector3.one,  aim, canvas.transform).gameObject;

            var reload = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Reload.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[1] = NewButton("Reload", new Vector2(-536, 141), new Vector2(100, 100),Vector3.one, reload, canvas.transform).gameObject;
            
            var camera = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Camera.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[2] = NewButton("Camera", new Vector2(-425, 141), new Vector2(100, 100),Vector3.one, camera, canvas.transform).gameObject;
            
            var grenades = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Grenades.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[3] = NewButton("Grenades", new Vector2(290, 141), new Vector2(100, 100), Vector3.one,grenades, canvas.transform).gameObject;
            
            var dropWeapon = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Drop weapon.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[4] = NewButton("DropWeapon", new Vector2(-425, 40), new Vector2(100, 100),Vector3.one, dropWeapon, canvas.transform).gameObject;
            
            var attack = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Attack.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[5] = NewButton("Attack", new Vector2(518, 141), new Vector2(100, 100),Vector3.one, attack, canvas.transform).gameObject;
            
            var sprint = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Sprint.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[6] = NewButton("Sprint", new Vector2(518, 40), new Vector2(100, 100),Vector3.one, sprint, canvas.transform).gameObject;
            
            var crouch = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Crouch.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[7] = NewButton("Crouch", new Vector2(405, 40), new Vector2(100, 100),Vector3.one, crouch, canvas.transform).gameObject;
            
            var jump = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Jump.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[8] = NewButton("Jump", new Vector2(290, 40), new Vector2(100, 100),Vector3.one, jump, canvas.transform).gameObject;
            
            var pause = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Pause.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[9] = NewButton("Pause", new Vector2(595, 295), new Vector2(50, 50),Vector3.one, pause, canvas.transform).gameObject;
            
            var inventory = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Inventory.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[10] = NewButton("Inventory", new Vector2(-536, 40), new Vector2(100, 100),Vector3.one, inventory, canvas.transform).gameObject;

            var pickUp = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Inventory/HandIcon.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[11] = NewButton("PickUp", new Vector2(0, 0), new Vector2(60, 60),Vector3.one, pickUp, canvas.transform).gameObject;

            inputs.uiButtons[13] = NewUIElement("Move Stick Outline", canvas.transform, new Vector2(-454, -193), new Vector2(250, 250), Vector3.one);
            var outline = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Move stick outline.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[13].AddComponent<Image>().sprite = outline;

            inputs.uiButtons[12] = NewUIElement("Move Stick", canvas.transform, new Vector2(-454, -193), new Vector2(130, 130), Vector3.one);
            var stick = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/Move stick.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[12].AddComponent<Image>().sprite = stick;
            

            var downArrow = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Inventory/Left.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[14] = NewButton("Down Weapon", new Vector2(-130, 300), new Vector2(100, 100), Vector3.one, downArrow, canvas.transform).gameObject;
            
            var upArrow = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Inventory/Right.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[15] = NewButton("Up Weapon", new Vector2(130, 300), new Vector2(100, 100), Vector3.one, upArrow, canvas.transform).gameObject;
            
            var changeCharacter = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/ChangeCharacter.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[16] = NewButton("Change Character", new Vector2(-314, 141), new Vector2(100, 100), Vector3.one, changeCharacter, canvas.transform).gameObject;
            
            var attackType = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Mobile inputs/ChangeAttackType.png", typeof(Sprite)) as Sprite;
            inputs.uiButtons[17] = NewButton("Change Attack Type", new Vector2(-314, 40), new Vector2(100, 100), Vector3.one, attackType, canvas.transform).gameObject;
            

            if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/_Mobile Inputs/"))
            {
                Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/_Mobile Inputs/");
            }
            
#if !UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/_Mobile Inputs/Mobile Buttons.prefab");
            PrefabUtility.ReplacePrefab(canvas.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
            PrefabUtility.SaveAsPrefabAsset(canvas.gameObject, "Assets/Universal Shooter Kit/Prefabs/_Mobile Inputs/Mobile Buttons.prefab");
#endif

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Mobile Inputs/Mobile Buttons.prefab", typeof(GameObject)) as GameObject);

            var buttons = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/_Mobile Inputs/Mobile Buttons.prefab", typeof(GameObject)) as GameObject;

            for (var i = 0; i < 18; i++)
            {
                inputs.uiButtons[i] = buttons.transform.GetChild(i).gameObject;
            }
            
            inputs.MobileInputs = buttons;

            return canvas.gameObject;
        }
        
        public static Transform NewPoint(GameObject parent, string name)
        {
            var point = new GameObject(name).transform;
            point.parent = parent.transform;
            point.localPosition = Vector3.zero;
            point.localRotation = Quaternion.Euler(Vector3.zero);
            point.localScale = Vector3.one;
            EditorUtility.SetDirty(parent.GetComponent<WeaponController>());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            return point;
        }

        public static BoxCollider NewCollider(string name, string tag, Transform parent)
        {
            var collider = new GameObject {name = name, tag = tag};
            var boxCollider = collider.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
				
            var rigidbody = collider.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            collider.transform.parent = parent;
            collider.transform.localPosition = Vector3.one;
            collider.transform.localScale = Vector3.one;
            return boxCollider;

        }
        

        public static void newInventory(InventoryManager manager)
        {
            var inventory = NewUIElement("Inventory Wheel", manager.canvas.transform,
                new Vector2(0, 0), new Vector2(700, 700), new Vector3(1.5f, 1.5f, 1.5f));

            var font = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;

            manager.inventoryWheel = inventory;

            var sprite = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Inventory/wheel part.png", typeof(Sprite)) as Sprite;
            
            manager.slots = new InventoryManager.InventorySlot[8];

            manager.slots[0] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 1", "wheel", manager.inventoryWheel.transform,
                    new Vector2(-167, 174), new Vector2(200, 200), new Vector3(0, 0, 0), sprite),
            };
            manager.slots[0].curAmmoText = NewText("Text", manager.slots[0].SlotButton.transform, new Vector2(-35, -52),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();


            manager.slots[1] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 2", "wheel", manager.inventoryWheel.transform,
                    new Vector2(5, 245), new Vector2(200, 200), new Vector3(0, 0, -45), sprite),
            };
            manager.slots[1].curAmmoText = NewText("Text", manager.slots[1].SlotButton.transform, new Vector2(28, -28),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.slots[2] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 3", "wheel", manager.inventoryWheel.transform,
                    new Vector2(176, 174), new Vector2(200, 200), new Vector3(0, 0, -90), sprite),
            };
            manager.slots[2].curAmmoText = NewText("Text", manager.slots[2].SlotButton.transform, new Vector2(52, 22),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.slots[3] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 4", "wheel", manager.inventoryWheel.transform,
                    new Vector2(247.5f, 2.5f), new Vector2(200, 200), new Vector3(0, 0, 225), sprite),
            };
            manager.slots[3].curAmmoText = NewText("Text", manager.slots[3].SlotButton.transform, new Vector2(54, 42),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.slots[4] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 5", "wheel", manager.inventoryWheel.transform,
                    new Vector2(177, -169), new Vector2(200, 200), new Vector3(0, 0, 180), sprite),
            };
            manager.slots[4].curAmmoText = NewText("Text", manager.slots[4].SlotButton.transform, new Vector2(27, 45),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.slots[5] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 6", "wheel", manager.inventoryWheel.transform,
                    new Vector2(5, -239), new Vector2(200, 200), new Vector3(0, 0, 135), sprite),
            };
            manager.slots[5].curAmmoText = NewText("Text", manager.slots[5].SlotButton.transform, new Vector2(-21, 21),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.slots[6] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 7", "wheel", manager.inventoryWheel.transform,
                    new Vector2(-167, -169), new Vector2(200, 200), new Vector3(0, 0, 90), sprite)
            };
            manager.slots[6].curAmmoText = NewText("Text", manager.slots[6].SlotButton.transform, new Vector2(-45, -28),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.slots[7] = new InventoryManager.InventorySlot
            {
                SlotButton = NewInventoryPart("Slot 8", "wheel", manager.inventoryWheel.transform,
                    new Vector2(-237, 2.5f), new Vector2(200, 200), new Vector3(0, 0, 45), sprite)
            };
            manager.slots[7].curAmmoText = NewText("Text", manager.slots[7].SlotButton.transform, new Vector2(-45, -51),
                new Vector2(160, 30), "100/200", font, 14,
                TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            sprite = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Textures & Materials/Inventory/inventory Button.png",
                typeof(Sprite)) as Sprite;


            manager.HealthButton = NewInventoryPart("Health Slot", "Health", manager.inventoryWheel.transform,
                new Vector2(-437, -219), new Vector2(150, 150), Vector3.zero, sprite);


            manager.AmmoButton = NewInventoryPart("Ammo Slot", "Ammo", manager.inventoryWheel.transform,
                new Vector2(437, -219), new Vector2(150, 150), Vector3.zero, sprite);


            sprite = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Textures & Materials/Inventory/Right.png",
                typeof(Sprite)) as Sprite;

            manager.UpWeaponButton = NewButton("UpWeaponButton", new Vector2(88, 0), new Vector2(100, 100),
                new Vector3(0.7f, 0.7f, 0.7f), sprite, manager.inventoryWheel.transform);

            manager.UpAmmoButton = NewButton("UpAmmoButton", new Vector2(547, -219), new Vector2(100, 100),
                new Vector3(0.7f, 0.7f, 0.7f), sprite, manager.inventoryWheel.transform);

            manager.UpHealthButton = NewButton("UpHealthButton", new Vector2(-327, -219), new Vector2(100, 100),
                new Vector3(0.7f, 0.7f, 0.7f), sprite, manager.inventoryWheel.transform);

            sprite = AssetDatabase.LoadAssetAtPath(
                "Assets/Universal Shooter Kit/Textures & Materials/Inventory/Left.png",
                typeof(Sprite)) as Sprite;

            manager.DownWeaponButton = NewButton("DownWeaponButton", new Vector2(-78, 0), new Vector2(100, 100),
                new Vector3(0.7f, 0.7f, 0.7f), sprite, manager.inventoryWheel.transform);

            manager.DownAmmoButton = NewButton("DownAmmoButton", new Vector2(327, -219), new Vector2(100, 100),
                new Vector3(0.7f, 0.7f, 0.7f), sprite, manager.inventoryWheel.transform);

            manager.DownHealthButton = NewButton("DownHealthButton", new Vector2(-547, -219), new Vector2(100, 100),
                new Vector3(0.7f, 0.7f, 0.7f), sprite, manager.inventoryWheel.transform);



            manager.CurrWeaponText = NewText("CurWeaponText", manager.inventoryWheel.transform, new Vector2(5.2f, 0),
                new Vector2(160, 30), "1/3", font, 25, TextAnchor.MiddleCenter, Color.white,true).GetComponent<Text>();

            manager.currHealthKitCount = NewText("Current health kit Count", manager.HealthButton.transform,
                new Vector2(0, -48),
                new Vector2(160, 30), "1/20", font, 14, TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.currentHealthKitAddedValue = NewText("Current health kit Added Value",
                manager.HealthButton.transform, new Vector2(0, 48),
                new Vector2(160, 30), "+20", font, 14, TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.currAmmoKitCount = NewText("Current ammo kit Count", manager.AmmoButton.transform,
                new Vector2(0, -48),
                new Vector2(160, 30), "1/20", font, 14, TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            manager.currentAmmoKitAddedValue = NewText("Current ammo kit Added Value", manager.AmmoButton.transform,
                new Vector2(0, 48),
                new Vector2(160, 30), "+20", font, 14, TextAnchor.MiddleCenter, Color.white, true).GetComponent<Text>();

            for (var i = 0; i < 8; i++)
            {
                manager.slots[i].SlotImage = NewImagePlace("Image place" + i, manager.slots[i].SlotButton.transform,
                    manager.inventoryWheel.transform, new Vector2(250, 250));
            }

            manager.HealthImage = NewImagePlace("Health image place", manager.HealthButton.transform,
                manager.HealthButton.transform, new Vector2(100, 100));
            
            manager.AmmoImage = NewImagePlace("Ammo image place", manager.AmmoButton.transform,
                manager.AmmoButton.transform, new Vector2(100, 100));
            
            EditorUtility.SetDirty(manager);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
           inventory.SetActive(false);


        }

        static RawImage NewImagePlace(string name, Transform parent, Transform parent2, Vector2 size)
        {
            var image = NewUIElement(name, parent, Vector2.zero, size, Vector3.one);

            var raw = image.AddComponent<RawImage>();
            raw.color = new Color(1, 1, 1, 0);

            raw.raycastTarget = false;

            image.transform.SetParent(parent2);

            return raw;
        }


        static Button NewInventoryPart(string name, string type, Transform parent, Vector2 position, Vector2 size,
            Vector3 rotation,
            Sprite image)
        {
            var part = NewUIElement(name, parent, position, size, Vector3.one);

            var img = part.AddComponent<Image>();
            img.sprite = image;
            img.color = new Color32(255, 255, 255, 1);

            part.AddComponent<Mask>();
            part.AddComponent<RaycastMask>();

            var button = NewButton("Button", new Vector2(0, 0), new Vector2(200, 200), Vector3.one, image,
                part.transform);

            var colors = button.colors;

            colors.normalColor = new Color32(0, 67, 255, 170);

            if (type == "wheel")
                colors.highlightedColor = new Color32(255, 190, 0, 190);
            else
                colors.normalColor = new Color32(0, 67, 255, 170);
            
            colors.pressedColor = new Color32(223, 166, 0, 190);

            button.colors = colors;

            part.GetComponent<RectTransform>().eulerAngles = rotation;

            return button;
        }

        public static GameObject NewUIElement(string name, Transform parent, Vector2 position, Vector2 size, Vector3 scale)
        {
            var element = new GameObject(name);
            element.transform.SetParent(parent);
            var rectTransform = element.AddComponent<RectTransform>();
            element.AddComponent<CanvasRenderer>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = size;

            element.transform.localScale = scale;
            element.layer = 5;
            
            return element;
        }

        public static GameObject NewText(string name, Transform parent, Vector2 position, Vector2 size,
            string textContent, Font font, int textSize, TextAnchor textAlignment, Color textColor, bool needOutline)
        {
            var textObject = NewUIElement(name, parent, position, size, Vector3.one);
            
            var text = textObject.AddComponent<Text>();
            text.text = textContent;
            
            if(font)
                text.font = font;
            
            text.fontSize = textSize;
            text.alignment = textAlignment;
            text.color = textColor;

            if (!needOutline) return textObject;
            
            var outline = textObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, -1);

            return textObject;
        }
        
        public static Button NewButton(string name, Vector2 position, Vector2 size, Vector3 scale, Sprite sprite, Transform parent)
        {
            var button = NewUIElement(name, parent, position, size, scale);
            var image = button.AddComponent<Image>();
            image.sprite = sprite;
            var _button = button.AddComponent<Button>();
            
            return _button;
        }
        
        public static Button NewButton(string name, Vector2 position, Vector2 size, Vector3 scale, Color32[] colors, Transform parent)
        {
            var button = NewUIElement(name, parent, position, size, scale);
           // var image = button.AddComponent<Image>();
            
            var _button = button.AddComponent<Button>();

            var buttonColors = _button.colors;
            buttonColors.normalColor = colors[0];
            buttonColors.highlightedColor = colors[1];
            buttonColors.pressedColor = colors[2];
            _button.colors = buttonColors;
            
            return _button;
        }

        public static Canvas NewCanvas(string name, Vector2 size, Transform parent)
        {
            GameObject canvas = new GameObject(name);
            canvas.AddComponent<RectTransform>();
            Canvas _canvas = canvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = size;
            canvas.AddComponent<GraphicRaycaster>();
            canvas.transform.SetParent(parent);

            return _canvas;
        }
        
        public static Canvas NewCanvas(string name, Vector2 size)
        {
            GameObject canvas = new GameObject(name);
            canvas.AddComponent<RectTransform>();
            Canvas _canvas = canvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = size;
            canvas.AddComponent<GraphicRaycaster>();

            return _canvas;
        }
        

        public static GameObject newCrosshairPart(string name, Vector2 positions, Vector2 size, GameObject parent)
        {
            GameObject crosshiarPart = NewUIElement(name, parent.transform, positions, size, Vector3.one);
            crosshiarPart.AddComponent<Image>().color = Color.white;
            Outline outline = crosshiarPart.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, -1);

            return crosshiarPart;
        }
#endif
        
        public static void HideIKObjects(bool value, HideFlags flag, Transform obj, Color color)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            renderer.enabled = !value;
            
            obj.hideFlags = flag;

            if (value)
            {
                if (obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(false);
                }
            }
            else
            {
                renderer.material = NewMaterial(color);
                    
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                }
            }
        }
        
        public static void HideAllObjects(WeaponsHelper.IKObjects IkObjects)
        {
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightObject, Color.red);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftObject, Color.red);

            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightAimObject, Color.blue);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftAimObject, Color.blue);

            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightWallObject, Color.yellow);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftWallObject, Color.yellow);

            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightElbowObject, Color.green);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftElbowObject, Color.green);
            
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.RightCrouchObject, Color.magenta);
            HideIKObjects(true, HideFlags.HideInHierarchy, IkObjects.LeftCrouchObject, Color.magenta);
        }
        
        public static void CreateObjects(WeaponsHelper.IKObjects IkObjects, Transform parent, bool adjusment, bool hide)
        {
            IkObjects.RightObject = NewObject(parent, "Right Hand Object", PrimitiveType.Cube, Color.red);
            Object.Destroy(IkObjects.RightObject.GetComponent<BoxCollider>());
            if(!adjusment && hide)
                Object.Destroy(IkObjects.RightObject.GetComponent<MeshRenderer>());
            

            IkObjects.LeftObject = NewObject(parent, "Left Hand Object", PrimitiveType.Cube, Color.red);
            Object.Destroy(IkObjects.LeftObject.GetComponent<BoxCollider>());
            if(!adjusment && hide)
                Object.Destroy(IkObjects.LeftObject.GetComponent<MeshRenderer>());

            IkObjects.RightAimObject = NewObject(parent, "Right Aim Object", PrimitiveType.Cube, Color.red);
            Object.Destroy(IkObjects.RightAimObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightAimObject.GetComponent<MeshRenderer>());

            IkObjects.LeftAimObject = NewObject(parent, "Left Aim Object", PrimitiveType.Cube, Color.red);
            Object.Destroy(IkObjects.LeftAimObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftAimObject.GetComponent<MeshRenderer>());
            
            IkObjects.RightCrouchObject = NewObject(parent, "Right Crouch Object", PrimitiveType.Cube, Color.black);
            Object.Destroy(IkObjects.RightCrouchObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightCrouchObject.GetComponent<MeshRenderer>());

            IkObjects.LeftCrouchObject = NewObject(parent, "Left Crouch Object", PrimitiveType.Cube, Color.black);
            Object.Destroy(IkObjects.LeftCrouchObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftCrouchObject.GetComponent<MeshRenderer>());


            IkObjects.RightWallObject = NewObject(parent, "Right Hand Wall Object", PrimitiveType.Cube, Color.yellow);
            Object.Destroy(IkObjects.RightWallObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightWallObject.GetComponent<MeshRenderer>());


            IkObjects.LeftWallObject = NewObject(parent, "Left Hand Wall Object", PrimitiveType.Cube, Color.yellow);
            Object.Destroy(IkObjects.LeftWallObject.GetComponent<BoxCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftWallObject.GetComponent<MeshRenderer>());


            IkObjects.RightElbowObject = NewObject(parent, "Right Elbow Object", PrimitiveType.Sphere, Color.green);
            Object.Destroy(IkObjects.RightElbowObject.GetComponent<SphereCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.RightElbowObject.GetComponent<MeshRenderer>());


            IkObjects.LeftElbowObject = NewObject(parent, "Left Elbow Object", PrimitiveType.Sphere, Color.green);
            Object.Destroy(IkObjects.LeftElbowObject.GetComponent<SphereCollider>());
            if(!adjusment)
                Object.Destroy(IkObjects.LeftElbowObject.GetComponent<MeshRenderer>());

        }

        public static void HandIK(Controller controller, WeaponController weaponController, InventoryManager weaponManager, Transform LeftIKObject,
            Transform RightIKObject, Transform leftParent, Transform rightParent, float value)
        {
            var L_ikObj = LeftIKObject;
            var R_ikObj = RightIKObject;

            if (weaponController.CanUseElbowIK)
            {
                controller.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, value);
                controller.anim.SetIKHintPosition(AvatarIKHint.LeftElbow, weaponController.IkObjects.LeftElbowObject.position);

                controller.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, value);
                controller.anim.SetIKHintPosition(AvatarIKHint.RightElbow, weaponController.IkObjects.RightElbowObject.position);
            }


            if (weaponController.TakeWeaponInAimMode && weaponController.TakeWeaponInWallMode && weaponController.TakeWeaponInCrouchlMode && value >= 1)
            {
                R_ikObj.parent = rightParent;
                L_ikObj.parent = leftParent;
            }
           
            if (value >= 0)
            {
                controller.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, value);
                controller.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, value);
            
                controller.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, value);
                controller.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, value);
                
                controller.anim.SetIKPosition(AvatarIKGoal.RightHand, R_ikObj.position);
                controller.anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(R_ikObj.eulerAngles));

                controller.anim.SetIKPosition(AvatarIKGoal.LeftHand, L_ikObj.position);
                controller.anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(L_ikObj.eulerAngles));
            }
        }
        
        public static void HandIK(AIController aiController, WeaponController weaponController, Transform LeftIKObject,
            Transform RightIKObject, Transform leftParent, Transform rightParent, float value)
        {
            Transform L_ikObj = LeftIKObject;
            Transform R_ikObj = RightIKObject;

            if (weaponController.CanUseElbowIK)
            {
                aiController.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, value);
                aiController.anim.SetIKHintPosition(AvatarIKHint.LeftElbow,
                    weaponController.IkObjects.LeftElbowObject.position);

                aiController.anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, value);
                aiController.anim.SetIKHintPosition(AvatarIKHint.RightElbow,
                    weaponController.IkObjects.RightElbowObject.position);
            }


            if (weaponController.TakeWeaponInAimMode && weaponController.TakeWeaponInWallMode && weaponController.TakeWeaponInCrouchlMode && value >= 1)
            {
                R_ikObj.parent = rightParent;
                L_ikObj.parent = leftParent;
            }
           
            if (value >= 0)
            {
                aiController.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, value);
                aiController.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, value);
            
                aiController.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, value);
                aiController.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, value);
                
                aiController.anim.SetIKPosition(AvatarIKGoal.RightHand, R_ikObj.position);
                aiController.anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(R_ikObj.eulerAngles));

                aiController.anim.SetIKPosition(AvatarIKGoal.LeftHand, L_ikObj.position);
                aiController.anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(L_ikObj.eulerAngles));
            }
        }

        private static void FingersRotation(Animator anim, float angle, HumanBodyBones finger, Vector3 axis)
        {
            anim.SetBoneLocalRotation(finger, anim.GetBoneTransform(finger).localRotation *= Quaternion.AngleAxis(angle, axis));
        }
        
        public static void FingersRotate(WeaponsHelper.WeaponInfo weaponInfo, Animator anim, string type)
        {
            if (type == "Weapon")
            {
                var leftAngleX = weaponInfo.FingersLeftX;
                var rightAngleX = weaponInfo.FingersRightX;

                var leftAngleY = weaponInfo.FingersLeftY;
                var rightAngleY = weaponInfo.FingersRightY;

                var leftAngleZ = weaponInfo.FingersLeftZ;
                var rightAngleZ = weaponInfo.FingersRightZ;

                var leftThumbAngleX = weaponInfo.ThumbLeftX;
                var rightThumbAngleX = weaponInfo.ThumbRightX;

                var leftThumbAngleY = weaponInfo.ThumbLeftY;
                var rightThumbAngleY = weaponInfo.ThumbRightY;

                var leftThumbAngleZ = weaponInfo.ThumbLeftZ;
                var rightThumbAngleZ = weaponInfo.ThumbRightZ;
                
                RotateFingersByAxis("X", leftAngleX, rightAngleX, leftThumbAngleX, rightThumbAngleX, anim, "Weapon");
                RotateFingersByAxis("Y", leftAngleY, rightAngleY, leftThumbAngleY, rightThumbAngleY, anim, "Weapon");
                RotateFingersByAxis("Z", leftAngleZ, rightAngleZ, leftThumbAngleZ, rightThumbAngleZ, anim, "Weapon");
            }
            else if (type == "Null")
            {
                RotateFingersByAxis("X", 0, 0, 0, 0, anim, "Reload");
                RotateFingersByAxis("Y", 0, 0, 0, 0, anim, "Reload");
                RotateFingersByAxis("Z", 0, 0, 0, 0, anim, "Reload");
            }
            else if  (type == "Grenade")
            {
                var leftAngleX = weaponInfo.FingersLeftX;
                var leftAngleY = weaponInfo.FingersLeftY;
                var leftAngleZ = weaponInfo.FingersLeftZ;
                
                var leftThumbAngleX = weaponInfo.ThumbLeftX;
                var leftThumbAngleY = weaponInfo.ThumbLeftY;
                var leftThumbAngleZ = weaponInfo.ThumbLeftY;

                RotateFingersByAxis("X", leftAngleX, 0, leftThumbAngleX, 0, anim, "Grenade");
                RotateFingersByAxis("Y", leftAngleY, 0, leftThumbAngleY, 0, anim, "Grenade");
                RotateFingersByAxis("Z", leftAngleZ, 0, leftThumbAngleZ, 0, anim, "Grenade");
            }
        }

        private static void RotateFingersByAxis(string axis, float leftAngle, float rightAngle, float leftThumbAngle,
            float rightThumbAngle, Animator anim, string type)
        {

            var axs = new Vector3();

            switch (axis)
            {
                case "X":
                    axs = Vector3.right;
                    break;
                case "Y":
                    axs = Vector3.up;
                    break;
                case "Z":
                    axs = Vector3.forward;
                    break;
            }

            if (type != "Grenade")
            {
                if (anim.GetBoneTransform(HumanBodyBones.RightIndexProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightIndexProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightIndexIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightIndexDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightIndexDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightRingProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightRingProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightRingIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightRingDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightRingDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightMiddleProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightMiddleIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightMiddleDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightLittleProximal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightLittleProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightLittleIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightLittleDistal))
                    FingersRotation(anim, rightAngle, HumanBodyBones.RightLittleDistal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightThumbProximal))
                    FingersRotation(anim, rightThumbAngle, HumanBodyBones.RightThumbProximal, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate))
                    FingersRotation(anim, rightThumbAngle, HumanBodyBones.RightThumbIntermediate, axs);

                if (anim.GetBoneTransform(HumanBodyBones.RightThumbDistal))
                    FingersRotation(anim, rightThumbAngle, HumanBodyBones.RightThumbDistal, axs);

            }


            //left fingers

            if (anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftIndexProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftIndexIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftIndexDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftRingProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftRingProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftRingIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftRingDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftRingDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftMiddleProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftMiddleIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftMiddleDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftLittleProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftLittleIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal))
                FingersRotation(anim, leftAngle, HumanBodyBones.LeftLittleDistal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal))
                FingersRotation(anim, leftThumbAngle, HumanBodyBones.LeftThumbProximal, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate))
                FingersRotation(anim, leftThumbAngle, HumanBodyBones.LeftThumbIntermediate, axs);

            if (anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal))
                FingersRotation(anim, leftThumbAngle, HumanBodyBones.LeftThumbDistal, axs);

        }
        
        
        // Copyright 2014 Jarrah Technology (http://www.jarrahtechnology.com). All Rights Reserved. 
        public static class CameraExtensions {

            public static void LayerCullingShow(Camera cam, int layerMask) {
                cam.cullingMask |= layerMask;
            }

            public static void LayerCullingShow(Camera cam, string layer) {
                LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static void LayerCullingHide(Camera cam, int layerMask) {
                cam.cullingMask &= ~layerMask;
            }

            public static void LayerCullingHide(Camera cam, string layer) {
                LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static void LayerCullingToggle(Camera cam, int layerMask) {
                cam.cullingMask ^= layerMask;
            }

            public static void LayerCullingToggle(Camera cam, string layer) {
                LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static bool LayerCullingIncludes(Camera cam, int layerMask) {
                return (cam.cullingMask & layerMask) > 0;
            }

            public static bool LayerCullingIncludes(Camera cam, string layer) {
                return LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));
            }

            public static void LayerCullingToggle(Camera cam, int layerMask, bool isOn) {
                var included = LayerCullingIncludes(cam, layerMask);
                if (isOn && !included) {
                    LayerCullingShow(cam, layerMask);
                } else if (!isOn && included) {
                    LayerCullingHide(cam, layerMask);
                }
            }

            public static void LayerCullingToggle(Camera cam, string layer, bool isOn) {
                LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);
            }
        }
        //
    }
}


