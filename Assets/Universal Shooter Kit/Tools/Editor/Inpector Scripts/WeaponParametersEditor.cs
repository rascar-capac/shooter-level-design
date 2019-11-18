using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using GercStudio.USK.Scripts;

namespace GercStudio.USK.Scripts
{
    [CustomEditor(typeof(WeaponParameters))]
    public class WeaponParametersEditor : Editor
    {
        public WeaponParameters script;
        
        private string curName;

        private bool delete;
        private bool rename;
        private bool renameError;

        public void Awake()
        {
            script = (WeaponParameters) target;
        }
        
        private void OnEnable()
        {
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            
            if (script.Attacks.Count == 0)
            {
                script.Attacks.Add(new WeaponsHelper.Attack());
                script.Attacks[script.Attacks.Count - 1].BulletsSettings.Add(new WeaponsHelper.BulletsSettings());
                script.Attacks[script.Attacks.Count - 1].BulletsSettings.Add(new WeaponsHelper.BulletsSettings());
                            
                if (!script.attacksNames.Contains("Attack " + script.Attacks.Count))
                    script.attacksNames.Add("Attack " + script.Attacks.Count);
                else script.attacksNames.Add("Attack " + Random.Range(10, 100));

                script.currentAttack = script.Attacks.Count - 1;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
                
                
                script.inspectorTab = GUILayout.Toolbar(script.inspectorTab,
                    new [] {"Settings", "Animations", "Sounds"});
                EditorGUILayout.Space();
                
                switch (script.inspectorTab)
            {
                case 0:
                    
                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        if (script.Attacks.Count > 0)
                        {
                            EditorGUILayout.LabelField("Attacks settings", EditorStyles.boldLabel);
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

                    }

                    EditorGUILayout.Space();
                    

                    if (script.Attacks.Count > 0)
                    {
                        var _attack = script.Attacks[script.currentAttack];
                        var curAttackSerialized = serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack);
                        
                        EditorGUILayout.BeginVertical("box");

                        EditorGUILayout.PropertyField(
                            serializedObject.FindProperty("Attacks").GetArrayElementAtIndex(script.currentAttack).FindPropertyRelative("AttackType"),
                            new GUIContent("Attack Type"));
                        
                        if (_attack.AttackType != WeaponsHelper.TypeOfAttack.Knife)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            
                            switch (_attack.AttackType)
                            {
                                case WeaponsHelper.TypeOfAttack.Rockets:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Rocket"), new GUIContent("Rocket (prefab)"));
                                    break;

                                case WeaponsHelper.TypeOfAttack.GrenadeLauncher:
                                    if (_attack.Rocket && _attack.Rocket.GetComponent<WeaponController>() && _attack.Rocket.GetComponent<WeaponController>().Attacks
                                            .All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade) || _attack.Rocket && !_attack.Rocket.GetComponent<WeaponController>())
                                        EditorGUILayout.HelpBox("Grenade must have the [WeaponController] script with the [Grenade] attack.", MessageType.Warning);
                                    
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Rocket"), new GUIContent("Grenade (prefab)"));
                                    break;
                                case WeaponsHelper.TypeOfAttack.Flame:
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Fire"), new GUIContent("Fire (prefab)"));
                                    break;

                                case WeaponsHelper.TypeOfAttack.Bullets:

                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Tracer"), new GUIContent("Bullet (prefab)"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("MuzzleFlash"), new GUIContent("Muzzle flash (prefab)"));

                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("Shell"), new GUIContent("Shell (prefab)"));
                                    break;

                                case WeaponsHelper.TypeOfAttack.Grenade:
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeExplosion"), new GUIContent("Explosion"));
                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    script.GrenadeParameters.ExplodeWhenTouchGround = EditorGUILayout.ToggleLeft("Explode when touch ground", script.GrenadeParameters.ExplodeWhenTouchGround);
                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeSpeed"), new GUIContent("Speed"));
                                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeExplosionTime"),
                                        new GUIContent("Time before explosion"));
                                    EditorGUILayout.PropertyField(curAttackSerialized.FindPropertyRelative("weapon_damage"), new GUIContent("Damage of attack"));
                                    break;
                            }
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
                                    _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame ? new GUIContent("Damage of attack (per 1 sec.)") : new GUIContent("Damage of attack"));

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
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Weapon settings", EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box");
                    
                    if (script.Attacks.All(attack => attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("ActiveAim"), new GUIContent("Active aim"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("AimImage"), new GUIContent("Scope texture (optional)"));
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                    }
                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponImage"), new GUIContent("Weapon Image"));
                    EditorGUILayout.HelpBox("This image will be displayed in the inventory.", MessageType.Info);
                    EditorGUILayout.EndVertical();

                    break;
                
                case 1:

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
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeThrow_FPS"), new GUIContent("Throw (FPS)"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("GrenadeParameters.GrenadeThrow_TPS_TDS"), new GUIContent("Throw (TPS/TDS)"));
                        }

                        EditorGUILayout.EndVertical();
                    }

                    break;

                case 2:
                    
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
            }
        }
    }
}