using System;
using System.Collections.Generic;
using System.Security.Policy;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(InventoryManager))]
    public class InventoryManagerEditor : Editor
    {
//        private Helper.GrenadeInfo defaultGrenadeFingers = new Helper.GrenadeInfo();

        private Vector3 defaultSize;

        private InventoryManager script;

        private Animator animator;

        private float startVal;
        private float progress;
        
        private ReorderableList[] weaponsList = new ReorderableList[8];

        private ReorderableList grenadesList;

        private bool weaponPrefabWarning;

        private bool greandePrefabWarning;
        
        
        public void Awake()
        {
            script = (InventoryManager) target;
        }

        private void OnEnable()
        {
            if(!script.inventoryWheel)
                return;
            
            for (var i = 0; i < 8; i++)
            {
                var i1 = i;
                weaponsList[i] = new ReorderableList(serializedObject, serializedObject.FindProperty("slots").GetArrayElementAtIndex(i)
                        .FindPropertyRelative("weaponSlots"), true, true,
                    true, true)
                {
                    drawHeaderCallback = rect =>
                    {
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Weapons");
                        
//                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 + 15, rect.y, rect.width / 4 - 7,
//                                EditorGUIUtility.singleLineHeight), "FP hands");
//                        
//                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 4 - 7,
//                            EditorGUIUtility.singleLineHeight), "TP hands");
//                        
//                        EditorGUI.LabelField(new Rect(rect.x +  3 * rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
//                            EditorGUIUtility.singleLineHeight), "TD hands");
                    },
                    onAddCallback = items =>
                    {
                        if (script.slots[i1] == null)
                            script.slots[i1] = new InventoryManager.InventorySlot();

                        script.slots[i1].weaponsCount++;
                        script.slots[i1].weaponSlots.Add(null);
                        
                    },
                    
                    onRemoveCallback = items =>
                    {
                        script.slots[i1].weaponsCount--;
                        script.slots[i1].weaponSlots.Remove(script.slots[i1].weaponSlots[items.index]);
                    },
                    
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        script.slots[i1].weaponSlots[index].weapon = (GameObject) EditorGUI.ObjectField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                            script.slots[i1].weaponSlots[index].weapon,typeof(GameObject), false);

//                        if (script.slots[i1].weaponSlots[index].weapon != null && script.slots[i1].weaponSlots[index].weapon.GetComponent<WeaponController>() &&
//                             script.slots[i1].weaponSlots[index].weapon.GetComponent<WeaponController>().enumNames != null)
//                        {
//                            script.slots[i1].weaponSlots[index].fpSlotIndex = EditorGUI.Popup(
//                                new Rect(rect.x + rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
//                                    EditorGUIUtility.singleLineHeight), script.slots[i1].weaponSlots[index].fpSlotIndex,
//                                script.slots[i1].weaponSlots[index].weapon.GetComponent<WeaponController>().enumNames.ToArray());
//                            
//                            script.slots[i1].weaponSlots[index].tpSlotIndex = EditorGUI.Popup(
//                                new Rect(rect.x + rect.width / 2 + 7, rect.y, rect.width / 4 - 7,
//                                    EditorGUIUtility.singleLineHeight), script.slots[i1].weaponSlots[index].tpSlotIndex,
//                                script.slots[i1].weaponSlots[index].weapon.GetComponent<WeaponController>().enumNames.ToArray());
//                            
//                            script.slots[i1].weaponSlots[index].tdSlotIndex = EditorGUI.Popup(
//                                new Rect(rect.x + 3 * rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
//                                    EditorGUIUtility.singleLineHeight), script.slots[i1].weaponSlots[index].tdSlotIndex,
//                                script.slots[i1].weaponSlots[index].weapon.GetComponent<WeaponController>().enumNames.ToArray());
//                        }
//                        else
//                        {
//                            EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2 - 11,
//                                    EditorGUIUtility.singleLineHeight), "Add prefab of weapon");
//                        }
                    }
                };
            }

            grenadesList = new ReorderableList(serializedObject, serializedObject.FindProperty("Grenades"),
                false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Grenade prefab");
                    
//                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2 - 11, 
//                        EditorGUIUtility.singleLineHeight), "Settings slot");
                },

                onAddCallback = items =>
                {
                    if (script.Grenades.Count > 0)
                        return;

                    script.Grenades.Add(null);
                },
                
                onRemoveCallback = items =>
                {
                    script.Grenades.Remove(script.Grenades[items.index]);
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.Grenades[index].Grenade = (GameObject) EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                        script.Grenades[index].Grenade, typeof(GameObject), false);
                    
//                    if (script.Grenades[index].Grenade != null && script.Grenades[index].Grenade.GetComponent<WeaponController>() &&
//                        script.Grenades[index].Grenade.GetComponent<WeaponController>().enumNames != null)
//                    {
//                        script.Grenades[index].saveSlotIndex = EditorGUI.Popup(
//                            new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2 - 11,
//                                EditorGUIUtility.singleLineHeight), script.Grenades[index].saveSlotIndex,
//                            script.Grenades[index].Grenade.GetComponent<WeaponController>().enumNames.ToArray());
//                    }
//                    else
//                    {
//                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 11, rect.y, rect.width / 2 - 11,
//                            EditorGUIUtility.singleLineHeight), "Add prefab of grenade");
//                    }
                }
            };

            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        public void Update()
        {
            for (var i = 0; i < 8; i++)
            {
                var WeaponCount = script.slots[i].weaponsCount;
                for (var j = 0; j < WeaponCount; j++)
                {
                    if (script.slots[i].weaponSlots[j] != null)
                    {
                        if (script.slots[i].weaponSlots[j].weapon)
                        {
                            if (!script.slots[i].weaponSlots[j].weapon.GetComponent<WeaponController>())
                            {
                                weaponPrefabWarning = true;
                                script.slots[i].weaponSlots[j].weapon = null;
                            }
                            else
                            {
                                weaponPrefabWarning = false;
                            }
                        }
                    }
                }
            }

            if (Application.isPlaying)
            {
                if (!animator)
                    animator = script.GetComponent<Animator>();

//                if (script.grenadeDebug)
//                {
//                    progress = (float) (EditorApplication.timeSinceStartup - startVal);
//                    if (progress >= script.GrenadeThrow.length / 3)
//                        animator.speed = 0;
//                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

//            EditorGUILayout.Space();
//            script.inspectorTab =
//                GUILayout.Toolbar(script.inspectorTab, new string[] {"Inventory", "Game UI"});

//            switch (script.inspectorTab)
//            {
//                case 0:
//                    script.currentTab = "Inventory";
//                    break;
//                case 1:
//                    script.currentTab = "Game UI";
//                    break;
//            }

            EditorGUILayout.Space();
//            switch (script.currentTab)
//            {
//                case "Inventory":
            if (!script.inventoryWheel)
            {
                if (GUILayout.Button("Create Inventory"))
                {
                    Helper.newInventory(script);
                }
            }
            else
            {
//                EditorGUILayout.LabelField("Inventory slots:", EditorStyles.boldLabel);
                script.inventoryTabUp = GUILayout.Toolbar(script.inventoryTabUp,
                    new [] {"Slot 1", "Slot 2", "Slot 3", "Slot 4"});

                switch (script.inventoryTabUp)
                {
                    case 0:
                        script.inventoryTabDown = 3;
                        script.inventoryTabMiddle = 4;
                        script.currentInventorySlot = 0;
                        break;
                    case 1:
                        script.inventoryTabDown = 3;
                        script.inventoryTabMiddle = 4;
                        script.currentInventorySlot = 1;
                        break;
                    case 2:
                        script.inventoryTabDown = 3;
                        script.inventoryTabMiddle = 4;
                        script.currentInventorySlot = 2;
                        break;
                    case 3:
                        script.inventoryTabDown = 3;
                        script.inventoryTabMiddle = 4;
                        script.currentInventorySlot = 3;
                        break;
                }

                script.inventoryTabMiddle = GUILayout.Toolbar(script.inventoryTabMiddle,
                    new [] {"Slot 5", "Slot 6", "Slot 7", "Slot 8"});

                switch (script.inventoryTabMiddle)
                {
                    case 0:
                        script.inventoryTabDown = 3;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 4;
                        break;
                    case 1:
                        script.inventoryTabDown = 3;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 5;
                        break;
                    case 2:
                        script.inventoryTabDown = 3;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 6;
                        break;
                    case 3:
                        script.inventoryTabDown = 3;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 7;
                        break;
                }

                script.inventoryTabDown = GUILayout.Toolbar(script.inventoryTabDown,
                    new[] {"Grenades Slot", "Ammo Slot", "Health slot"});

                switch (script.inventoryTabDown)
                {
                    case 0:
                        script.inventoryTabMiddle = 4;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 8;
                        break;
                    case 1:
                        script.inventoryTabMiddle = 4;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 9;
                        break;
                    case 2:
                        script.inventoryTabMiddle = 4;
                        script.inventoryTabUp = 4;
                        script.currentInventorySlot = 10;
                        break;
                }

                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");
                for (var i = 0; i < 8; i++)
                {
                    if (script.currentInventorySlot == i)
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("slots").GetArrayElementAtIndex(i)
                            .FindPropertyRelative("SlotButton"), new GUIContent("Slot UI"));
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.HelpBox("You can change the texture, text, color and other graphic parameters of this slot.", MessageType.Info);

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();

                        if (!Application.isPlaying)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("slots").GetArrayElementAtIndex(i)
                                .FindPropertyRelative("hideAllWeaponsSlot"), new GUIContent("Hide all weapons slot"));

                            if (script.slots[i].hideAllWeaponsSlot)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("HideAllWeaponImage"), new GUIContent("Image"));
                            }

                            if (!script.slots[i].hideAllWeaponsSlot)
                            {
                                weaponsList[script.currentInventorySlot].DoLayoutList();

//                                if (script.slots[i].weaponSlots.Count > 0 && script.slots[i].weaponSlots[0].weapon)
//                                    EditorGUILayout.HelpBox("FP Hands - IK position and rotation of the character's hands in the FP view." + "\n" +
//                                                            "TP and TD Hands - the same for other views of the camera." + "\n" +
//                                                            "If you use only one type of camera, set only one hands slot." + "\n\n" +
//                                                            "You can add new slots and adjust it in the [Adjustment scene]." + "\n\n" +
//                                                            "Also you can create and set the own slot for different characters (like character1_fps, character2_fps, e.t.c)",
//                                        MessageType.Info);
                            }
                        }
                        else if (Application.isPlaying && script.controller && !script.controller.AdjustmentScene)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("slots").GetArrayElementAtIndex(i)
                                .FindPropertyRelative("hideAllWeaponsSlot"), new GUIContent("Hide all weapons slot"));
                            
                            if (script.slots[i].hideAllWeaponsSlot)
                            {
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("HideAllWeaponImage"), new GUIContent("Image"));
                            }

                            EditorGUILayout.Space();

                            if (!script.slots[i].hideAllWeaponsSlot)
                            {
                                weaponsList[script.currentInventorySlot].DoLayoutList();

                            }

//                            if (!script.slots[i].hideAllWeaponsSlot)
//                            {
//                                weaponsList[script.currentInventorySlot].DoLayoutList();
//                                if (script.slots[i].weaponSlots.Count > 0 && script.slots[i].weaponSlots[0].weapon)
//                                    EditorGUILayout.HelpBox("FP Hands - IK position and rotation of the character's hands in the FP view." + "\n" +
//                                                            "TP and TD Hands - the same for other views of the camera." + "\n" +
//                                                            "If you use only one type of camera, set only one hands slot." + "\n\n" +
//                                                            "You can add new slots and adjust it in the [Adjustment scene]." + "\n\n" +
//                                                            "Als you can create and set the own slot for different characters (like character1_fps, character2_fps, e.t.c)",
//                                        MessageType.Info);
//                            }
                        }
                        else if (Application.isPlaying && script.controller && script.controller.AdjustmentScene)
                        {
                            EditorGUILayout.LabelField("You are in the [Adjustment scene]. " + "\n" + "To choose a weapon use [Adjustment] script here:");
                           
                            EditorGUILayout.Space();
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.ObjectField("Adjustment object",FindObjectOfType<Adjustment>(), typeof(Adjustment), true);
                            EditorGUI.EndDisabledGroup();
                        }

                        if (weaponPrefabWarning)
                            EditorGUILayout.HelpBox("Your weapon should has [WeaponController] script", MessageType.Warning);

//                                        EditorGUILayout.BeginHorizontal();
//                                        EditorGUILayout.PropertyField(
//                                            serializedObject.FindProperty("slots").GetArrayElementAtIndex(i)
//                                                .FindPropertyRelative("weaponSlots").GetArrayElementAtIndex(j)
//                                                .FindPropertyRelative("weapon"),
//                                            new GUIContent("Weapon " + (j + 1)));
//                                        if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(17)))
//                                        {
//                                            script.slots[i].weaponsCount--;
//                                            script.slots[i].weaponSlots.Remove(script.slots[i].weaponSlots[j]);
//                                            break;
//                                        }

//                                        EditorGUILayout.EndHorizontal();

//                                        if (script.slots[i].weaponSlots[j].saveSlotsNames != null &
//                                            script.slots[i].weaponSlots[j].weapon != null)
//                                            script.slots[i].weaponSlots[j].saveSlotIndex = EditorGUILayout.Popup(
//                                                "Weapon settings slot",
//                                                script.slots[i].weaponSlots[j].saveSlotIndex,
//                                                script.slots[i].weaponSlots[j].saveSlotsNames.ToArray());

//                                    }

                        // EditorGUILayout.EndVertical();
//                                }
//                                if (GUILayout.Button("Add weapon"))
//                                {
//                                    if (script.slots[i] == null)
//                                        script.slots[i] = new InventoryManager.InventorySlot();
//                                    
//
//                                    script.slots[i].weaponSlots.Add(null);
//                                    
//                                    script.slots[i].weaponsCount++;
//                                }

                    }
                }

                switch (script.currentInventorySlot)
                {
                    case 8:
                        grenadesList.DoLayoutList();
//                        if (script.Grenades.Count > 0 && script.Grenades[0].Grenade) 
//                            EditorGUILayout.HelpBox("You can add a new [Settings Slot] slot and adjust it in the [Adjustment scene]." + "\n\n" +
//                                                    "These slots are needed in order to use a grenade with different characters and enemies" + "\n" +
//                                                    "You can set the own slot for each character/enemy.", MessageType.Info);
                        break;
                    case 9:
                        EditorGUILayout.HelpBox("You can change the texture, text, color and other graphic parameters of this slot.", MessageType.Info);

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("AmmoButton"),
                            new GUIContent("Slot UI"), true);
                        EditorGUI.EndDisabledGroup();
                        break;
                    case 10:
                        EditorGUILayout.HelpBox(
                            "You can change the texture, text, color and other graphic parameters of this slot.",
                            MessageType.Info);
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("HealthButton"),
                            new GUIContent("Slot UI"), true);
                        EditorGUI.EndDisabledGroup();
                        break;
                }

                EditorGUILayout.EndVertical();

//                if (script.currentInventorySlot == 8)
//                {
//                     EditorGUILayout.LabelField("Different grenades in the 1.5 version (soon).",
//                                                new GUIStyle {normal = new GUIStyleState {textColor = Color.white}, fontStyle = FontStyle.Italic});
//                }
                EditorGUILayout.Space();
            }

//            DrawDefaultInspector();
            
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(script);
        }
    }
}


