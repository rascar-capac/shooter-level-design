using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

namespace GercStudio.USK.Scripts
{
    [CustomEditor(typeof(WeaponController))]
    public class WeaponControllerEditor : Editor
    {
        private WeaponController script;

//        private WeaponsHelper.WeaponInfo WeaponData = new WeaponsHelper.WeaponInfo();
       // private WeaponsHelper.WeaponInfo defaultValues;

//        private Vector3 defaultSize;

       // private Controller controller;

//        private string curName;
//
//        private bool delete;

        private InventoryManager manager;
        
        private string curName;

        private bool delete;
        private bool rename;
        private bool renameError;

        private ReorderableList tagsList;

//        private enum FingersRotationAxises
//        {
//            X, Y, Z
//        }
//
//        private FingersRotationAxises axises;

        private void Awake()
        {
            script = (WeaponController) target;
        }

        private void OnEnable()
        {

            tagsList = new ReorderableList(serializedObject, serializedObject.FindProperty("IkSlots"), false, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 4, EditorGUIUtility.singleLineHeight), "Tag");

                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 4 + 15, rect.y, rect.width / 4 - 7,
                                EditorGUIUtility.singleLineHeight), "FP hands");
                        
                        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), "TP hands");
                        
                        EditorGUI.LabelField(new Rect(rect.x +  3 * rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), "TD hands");
                },
                
                onAddCallback = items =>
                {
                    script.IkSlots.Add(new WeaponsHelper.IKSlot());
                },
                
                onRemoveCallback = items =>
                {
                    if(script.IkSlots.Count == 1)
                        return;

                    script.IkSlots.Remove(script.IkSlots[items.index]);
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    script.IkSlots[index].currentTag = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width/4, EditorGUIUtility.singleLineHeight),
                        script.IkSlots[index].currentTag, script.inputs.CharacterTags.ToArray());

                    script.IkSlots[index].fpsSettingsSlot = EditorGUI.Popup(new Rect(rect.x + rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), script.IkSlots[index].fpsSettingsSlot, script.enumNames.ToArray());
                    
                    script.IkSlots[index].tpsSettingsSlot = EditorGUI.Popup(new Rect(rect.x + rect.width / 2 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight), script.IkSlots[index].tpsSettingsSlot, script.enumNames.ToArray());

                    script.IkSlots[index].tdsSettingsSlot = EditorGUI.Popup(new Rect(rect.x + 3 * rect.width / 4 + 7, rect.y, rect.width / 4 - 7,
                            EditorGUIUtility.singleLineHeight),  script.IkSlots[index].tdsSettingsSlot, script.enumNames.ToArray());
//                        
                }
            };
                
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (!script.inputs)
                    script.inputs = AssetDatabase.LoadAssetAtPath(
                        "Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(Inputs)) as Inputs;
                
                if (ActiveEditorTracker.sharedTracker.isLocked)
                    ActiveEditorTracker.sharedTracker.isLocked = false;

                if (!script) return;

                if (!script.gameObject.GetComponent<Rigidbody>())
                    script.gameObject.AddComponent<Rigidbody>();

                if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                {
                    if (!script.gameObject.GetComponent<BoxCollider>())
                        script.gameObject.AddComponent<BoxCollider>();
                    else script.gameObject.GetComponent<BoxCollider>().enabled = true;

                    if (script.gameObject.GetComponent<CapsuleCollider>())
                        script.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                }
                else
                {
                    if (!script.gameObject.GetComponent<CapsuleCollider>())
                        script.gameObject.AddComponent<CapsuleCollider>();
                    else script.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    if (script.gameObject.GetComponent<BoxCollider>())
                        script.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
                
                
                script.PickUpWeapon = script.gameObject.activeInHierarchy;

                if (!script.PickUpWeapon)
                {
                    if (script.gameObject.GetComponent<PickUp>())
                    {
                        var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                        DestroyImmediate(tempWeapon.GetComponent<PickUp>());
#if !UNITY_2018_3_OR_NEWER
                        PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                        PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                        DestroyImmediate(tempWeapon);
                    }

                    script.enabled = false;
                }
                else
                {
                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        if (!script.gameObject.GetComponent<PickUp>())
                            script.gameObject.AddComponent<PickUp>();

                        script.gameObject.GetComponent<PickUp>().PickUpType = PickUp.TypeOfPickUp.Weapon;
                        if(script.Attacks.Any(attack => attack.AttackCollider))
                        {
                            var _attacks = script.Attacks.FindAll(attack => attack.AttackCollider);
                            foreach (var _attack in _attacks)
                            {
                                if(_attack.AttackCollider.enabled)
                                    _attack.AttackCollider.enabled = false;
                            }
                        }
                    }

                    script.enabled = false;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();


            EditorGUILayout.HelpBox(script.PickUpWeapon
                    ? "This weapon is a pickup item. Please adjust the size of the [Box Collider]."
                    : "To use this weapon as a pickup item, just place it in the scene.", MessageType.Info);

            EditorGUILayout.Space();

            if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
            {
                script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop,
                    new [] {"Attacks", "Weapon Settings", "Aim Settings"});


                switch (script.inspectorTabTop)
                {
                    case 0:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Attacks";
                        break;
                    case 1:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Weapon Settings";
                        break;
                    case 2:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Aim Settings";
                        break;
                }
            }
            else
            {
                script.inspectorTabTop = GUILayout.Toolbar(script.inspectorTabTop, new [] {"Attack", "Weapon Settings"});
                
                switch (script.inspectorTabTop)
                {
                    case 0:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Attacks";
                        break;
                    case 1:
                        script.inspectorTabBottom = 3;
                        script.currentTab = "Weapon Settings";
                        break;
                }
            }

            script.inspectorTabBottom = GUILayout.Toolbar(script.inspectorTabBottom,
                new [] {"Animations", "Sounds"});

            switch (script.inspectorTabBottom)
            {
                case 0:
                    script.inspectorTabTop = 3;
                    script.currentTab = "Animations";
                    break;
                case 1:
                    script.inspectorTabTop = 3;
                    script.currentTab = "Sounds";
                    break;
            }
            

            switch (script.currentTab)
            {
                case "Attacks":

                    EditorGUILayout.Space();

                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {

                        if (script.Attacks.Count > 0)
                        {
                            EditorGUILayout.BeginVertical("box");

                            script.currentAttack = EditorGUILayout.Popup("Weapon attacks", script.currentAttack, script.attacksNames.ToArray());
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
                                    if (!script.attacksNames.Contains(curName))
                                    {
                                        rename = false;
                                        script.attacksNames[script.currentAttack] = curName;
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

                            EditorGUI.BeginDisabledGroup(script.Attacks.Count <= 1);
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
                                    script.attacksNames.Remove(script.attacksNames[script.currentAttack]);
                                    script.Attacks.Remove(script.Attacks[script.currentAttack]);
                                    script.currentAttack = script.Attacks.Count - 1;
                                    delete = false;
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                            }

                            EditorGUI.EndDisabledGroup();
                            EditorGUILayout.EndVertical();
                        }


                        if (GUILayout.Button("Add new attack"))
                        {
                            script.Attacks.Add(new WeaponsHelper.Attack());
                            script.Attacks[script.Attacks.Count - 1].BulletsSettings.Add(new WeaponsHelper.BulletsSettings());
                            script.Attacks[script.Attacks.Count - 1].BulletsSettings.Add(new WeaponsHelper.BulletsSettings());
                            
                            if (!script.attacksNames.Contains("Attack " + script.Attacks.Count))
                                script.attacksNames.Add("Attack " + script.Attacks.Count);
                            else script.attacksNames.Add("Attack " + Random.Range(10, 100));

                            script.currentAttack = script.Attacks.Count - 1;

                            break;
                        }

                        EditorGUILayout.Space();
                    }


                    if (script.Attacks.Count > 0)
                    {
                        var _attack = script.Attacks[script.currentAttack];
                        var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack);
                        
                        EditorGUILayout.BeginVertical("box");

                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack).FindPropertyRelative("AttackType"), new GUIContent("Attack Type"));
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        
                            switch (_attack.AttackType)
                            {
                                case WeaponsHelper.TypeOfAttack.Rockets:
                                    if(_attack.Rocket && !_attack.Rocket.GetComponent<Rocket>())
                                        EditorGUILayout.HelpBox("Rocket must have the [Rocket] script.", MessageType.Warning);
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Rocket"), new GUIContent("Rocket (prefab)"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackSpawnPoint"), new GUIContent("Rockets spawn point"));
                                    CheckPoint(_attack, "attack");
                                    break;
                                case WeaponsHelper.TypeOfAttack.Flame:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Fire"), new GUIContent("Fire (prefab)"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackSpawnPoint"), new GUIContent("Fire spawn point"));
                                    CheckPoint(_attack, "attack");
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackCollider"), new GUIContent("Fire collider"));
                                    CheckCollider(_attack, "fire");
                                    break;
                                case WeaponsHelper.TypeOfAttack.Knife:
                                {
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackCollider"), new GUIContent("Attack collider"));
                                    CheckCollider(_attack, "knife");
                                    break;
                                }
                                case WeaponsHelper.TypeOfAttack.Bullets:
             
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Tracer"), new GUIContent("Bullet (prefab)"));
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("MuzzleFlash"), new GUIContent("Muzzle flash (prefab)"));
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackSpawnPoint"), new GUIContent("Bullets spawn point"));
                                CheckPoint(_attack, "attack");
                                     
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();
                                     EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Shell"), new GUIContent("Shell (prefab)"));
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ShellPoint"), new GUIContent("Shells spawn point"));
                                     CheckPoint(_attack, "shell");
    
                                    break;
                                case WeaponsHelper.TypeOfAttack.Grenade:
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeExplosion"), new GUIContent("Explosion"));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    script.GrenadeParameters.ExplodeWhenTouchGround =
                                        EditorGUILayout.ToggleLeft("Explode when touch ground", script.GrenadeParameters.ExplodeWhenTouchGround);
                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeSpeed"), new GUIContent("Speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeExplosionTime"), new GUIContent("Time before explosion"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("weapon_damage"), new GUIContent("Damage of attack"));

                                    break;
                               case WeaponsHelper.TypeOfAttack.GrenadeLauncher:

                                   if (_attack.Rocket && _attack.Rocket.GetComponent<WeaponController>() && _attack.Rocket.GetComponent<WeaponController>().Attacks
                                           .All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade) || _attack.Rocket && !_attack.Rocket.GetComponent<WeaponController>())
                                       EditorGUILayout.HelpBox("Grenade must have the [WeaponController] script with the [Grenade] attack.", MessageType.Warning);
                                   

                                   EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Rocket"), new GUIContent("Grenade (prefab)"));
                                   EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackSpawnPoint"), new GUIContent("Grenades spawn point"));
                                   
                                   break;
                                    
                            }


                            if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();

                                if (_attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets)
                                {
                                    EditorGUILayout.HelpBox("You can use single, automatic or both shooting types." + "\n" +
                                                            "In the game you can switch between shooting types as well as between types of attack.", MessageType.Info);

                                    script.bulletTypeInspectorTab = GUILayout.Toolbar(script.bulletTypeInspectorTab, new[] {"Single", "Auto"});
                                    switch (script.bulletTypeInspectorTab)
                                    {
                                        case 0:

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                .FindPropertyRelative("Active"), new GUIContent("Active"));

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                .FindPropertyRelative("weapon_damage"), new GUIContent("Damage of attack"));

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                .FindPropertyRelative("RateOfShoot"), new GUIContent("Rate of attack"));

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(0)
                                                .FindPropertyRelative("ScatterOfBullets"), new GUIContent("Scatter of bullets"));
                                            break;

                                        case 1:

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                .FindPropertyRelative("Active"), new GUIContent("Active"));

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                .FindPropertyRelative("weapon_damage"), new GUIContent("Damage of attack"));

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                .FindPropertyRelative("RateOfShoot"), new GUIContent("Rate of attack"));

                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("BulletsSettings").GetArrayElementAtIndex(1)
                                                .FindPropertyRelative("ScatterOfBullets"), new GUIContent("Scatter of bullets"));
                                            break;
                                    }
                                }
                                else
                                {

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("weapon_damage"),
                                        _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame
                                            ? new GUIContent("Damage of attack (per 1 sec.)")
                                            : new GUIContent("Damage of attack"));

                                    if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Flame)
                                    {
                                        EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("RateOfShoot"), new GUIContent("Rate of attack"));

                                        if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Knife && _attack.AttackType != WeaponsHelper.TypeOfAttack.GrenadeLauncher)
                                        {
                                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ScatterOfBullets"),
                                                _attack.AttackType != WeaponsHelper.TypeOfAttack.Rockets
                                                    ? new GUIContent("Scatter of bullets")
                                                    : new GUIContent("Scatter of rockets"));
                                        }
                                    }
                                }
                            }

                            if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Knife)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade)
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("maxAmmo"), new GUIContent("Count of ammo in magazine"));

                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("inventoryAmmo"),
                                _attack.AttackType == WeaponsHelper.TypeOfAttack.Grenade ? new GUIContent("Count in inventory") : new GUIContent("Count of ammo in inventory"));
                            

                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AmmoType"), new GUIContent("Ammo Name"));
                            EditorGUILayout.HelpBox("Write the same type in the PickUp script for ammo.", MessageType.Info);
                        }

                        EditorGUILayout.EndVertical();
                    }

                    break;

                case "Weapon Settings":
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.HelpBox("This image will be displayed in the inventory.", MessageType.Info);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponImage"), new GUIContent("Weapon Image"));
                    
                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("For different characters you can use different IK settings, " + 
                                                "just set the necessary settings for each tag." + "\n\n" +
                                                "You can change the character tag in the [Controller] script." + "\n\n" +
                                                "Also for each type of camera, you can set an own slot.", MessageType.Info);
                        tagsList.DoLayoutList();
                    }

                    EditorGUILayout.EndVertical();

                    break;

                case "Aim Settings":
                    EditorGUILayout.Space();
//                    if (script.Attacks.Any(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Knife))
//                    {
                        EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ActiveAimTPS"), new GUIContent("Active aim in TP"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ActiveAimFPS"), new GUIContent("Active aim in FP"));
                    
                    EditorGUILayout.EndHorizontal();

                        if (script.ActiveAimTPS || script.ActiveAimFPS)
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.HelpBox(
                                "If you are using a third-person view, the camera will switch to first-person view when aiming", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("SwitchToFpCamera"),
                                new GUIContent("Switch To FP View"));
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.HelpBox(
                                "Use this mode to aim with scope model and camera", MessageType.Info);

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseScope"),
                                new GUIContent("Active"));

                            if (script.UseScope)
                            {
                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("ScopeScreen"),
                                    new GUIContent("Scope Screen"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("scopeDepth"),
                                    new GUIContent("Aim depth"));
                                EditorGUILayout.EndVertical();

                            }

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Texture mode", EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.HelpBox("Use this mode to aim with the scope texture and camera.", MessageType.Info);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseAimTexture"),
                                new GUIContent("Active"));

                            if (script.UseAimTexture)
                            {
                                EditorGUILayout.BeginVertical("box");
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("AimCrosshairTexture"), new GUIContent("Aim texture"));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("aimTextureDepth"), new GUIContent("Aim depth"));
                                EditorGUILayout.EndVertical();
                            }

                            EditorGUILayout.EndVertical();
                        }
//                    }

                    break;

                case "Animations":
                    EditorGUILayout.Space();
                    if (script.Attacks.Count > 0)
                    {
                        EditorGUILayout.BeginVertical("box");
                        
                        if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.WeaponIdle"), new GUIContent("Idle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.WeaponWalk"), new GUIContent("Walk"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.WeaponRun"), new GUIContent("Run"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimations.TakeWeapon"), new GUIContent("Take from inventory"));
                            EditorGUILayout.Space();
                            for (var i = 0; i < script.Attacks.Count; i++)
                            {
                                var attack = script.Attacks[i];
                                var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(i);
                                
                                EditorGUILayout.LabelField("Attack: " + script.attacksNames[i], EditorStyles.boldLabel);
//                                EditorGUILayout.BeginVertical("box");
                                if (attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets)
                                {
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAutoShoot"), new GUIContent("Auto Shoot"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttack"), new GUIContent("Single Shoot"));
                                }
                                else
                                {
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponAttack"), new GUIContent("Attack"));
                                }


                                if (attack.AttackType != WeaponsHelper.TypeOfAttack.Knife)
                                {
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("WeaponReload"), new GUIContent("Reload"));
                                }

//                                EditorGUILayout.EndVertical();
                                if (i < script.Attacks.Count - 1)
                                    EditorGUILayout.Space();
                            }
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeThrow_FPS"), new GUIContent("Throw (FP)"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeThrow_TPS_TDS"), new GUIContent("Throw (TP/TD)"));
                        }

                        EditorGUILayout.EndVertical();
                    }

                    break;

                case "Sounds":
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical("box");
                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("PickUpWeaponAudio"), new GUIContent("Pickup"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("DropWeaponAudio"), new GUIContent("Drop"));
                        EditorGUILayout.Space();
                        for (var i = 0; i < script.Attacks.Count; i++)
                        {
                            var attack = script.Attacks[i];
                            var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(i);

                            EditorGUILayout.LabelField("Attack: " + script.attacksNames[i], EditorStyles.boldLabel);
                            //EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("AttackAudio"), new GUIContent("Attack"));

                            if (attack.AttackType != WeaponsHelper.TypeOfAttack.Knife)
                            {
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("ReloadAudio"), new GUIContent("Reload"));
                                EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("NoAmmoShotAudio"), new GUIContent("Attack without ammo"));
                            }

                            // EditorGUILayout.EndVertical();
                            if (i < script.Attacks.Count - 1)
                                EditorGUILayout.Space();
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.ThrowAudio"), new GUIContent("Throw"));
                    }
                    EditorGUILayout.EndVertical();

                    break;
            }

            serializedObject.ApplyModifiedProperties();

//            DrawDefaultInspector();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(script);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        void CheckPoint(WeaponsHelper.Attack _attack, string type)
        {
            if (type == "attack" && !_attack.AttackSpawnPoint || type != "attack" && !_attack.ShellPoint)
            {
                if (!script.gameObject.activeInHierarchy)
                {
                    if (GUILayout.Button("Create point"))
                    {
                        var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                        if (type == "attack") tempWeapon.GetComponent<WeaponController>().Attacks[script.currentAttack].AttackSpawnPoint = Helper.NewPoint(tempWeapon, "Attack Point");
                        else tempWeapon.GetComponent<WeaponController>().Attacks[script.currentAttack].ShellPoint = Helper.NewPoint(tempWeapon, "Shell Spawn Point");

#if !UNITY_2018_3_OR_NEWER
                        PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                        PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                        
                        DestroyImmediate(tempWeapon);
                    }
                }
                else
                {
                    if (GUILayout.Button("Create point"))
                    {
                        if (type == "attack") script.Attacks[script.currentAttack].AttackSpawnPoint = Helper.NewPoint(script.gameObject, "Attack Point");
                        else  script.Attacks[script.currentAttack].ShellPoint = Helper.NewPoint(script.gameObject, "Shell Spawn Point");
                    }
                }
            }
            else if (type == "attack" && _attack.AttackSpawnPoint || type != "attack" && _attack.ShellPoint)
            {
                if (type == "attack" && _attack.AttackSpawnPoint.localPosition == Vector3.zero)
                    EditorGUILayout.HelpBox("Please adjust the position of the [Attack Point]", MessageType.Warning);
                
                else if(type != "attack" && _attack.ShellPoint.localPosition == Vector3.zero)
                    EditorGUILayout.HelpBox("Please adjust the position of the [Shell Spawn Point]", MessageType.Warning);
            }
        }

        void CheckCollider(WeaponsHelper.Attack _attack, string type)
        {
            if (_attack.AttackCollider)
            {
                if (_attack.AttackCollider.transform.localScale == Vector3.one)
                {
                    EditorGUILayout.HelpBox("Please adjust the size of the" + type == "fire" ? " Fire Collider." : " Knife Collider." +
                                            " It's the area that will deal damage.", MessageType.Warning);
                }
            }
            else
            {
                if (!script.gameObject.activeInHierarchy)
                {
                    if (GUILayout.Button("Create collider"))
                    {
                        var tempWeapon = (GameObject) PrefabUtility.InstantiatePrefab(script.gameObject);
                        tempWeapon.GetComponent<WeaponController>().Attacks[script.currentAttack].AttackCollider = type == "fire"
                            ? Helper.NewCollider("Fire Collider", "Fire", tempWeapon.transform)
                            : Helper.NewCollider("Knife Collider", "KnifeCollider", tempWeapon.transform);
#if !UNITY_2018_3_OR_NEWER
                        PrefabUtility.ReplacePrefab(tempWeapon, PrefabUtility.GetPrefabParent(tempWeapon), ReplacePrefabOptions.ConnectToPrefab);
#else
                        PrefabUtility.SaveAsPrefabAssetAndConnect(tempWeapon, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tempWeapon), InteractionMode.AutomatedAction);
#endif
                        DestroyImmediate(tempWeapon);
                    }
                }
                else
                {
                    if (GUILayout.Button("Create collider"))
                    {
                        script.Attacks[script.currentAttack].AttackCollider = type == "fire"
                            ? Helper.NewCollider("Fire Collider", "Fire", script.transform)
                            : Helper.NewCollider("Knife Collider", "KnifeCollider", script.transform);
                    }
                }
            }
        }
    }
}


