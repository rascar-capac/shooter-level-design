using UnityEngine;
using UnityEditor;

namespace GercStudio.USK.Scripts
{

    [CustomEditor(typeof(EnemyAttack))]
    public class EnemyAttackEditor : Editor
    {

        public EnemyAttack script;

        public void Awake()
        {
            script = (EnemyAttack) target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField("Gun Attack", EditorStyles.boldLabel);
            script.Gun_Attack = EditorGUILayout.Toggle("Active", script.Gun_Attack);
            if (script.Gun_Attack)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BulletDamage"),
                    new GUIContent("Damage from Bullets"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ScatterOfBullets"),
                    new GUIContent("Scatter Of Bullets"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RateOfShoot_Bullet"),
                    new GUIContent("Rate of shoot"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MuzzleFlash"),
                    new GUIContent("Muzzle Flash (prefab)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BulletAttackAudio"),
                    new GUIContent("Attack audio"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BulletSpawn"),
                    new GUIContent("Spawn points of Bullets"), true);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField("Fire Attack", EditorStyles.boldLabel);
            script.Fire_Attack = EditorGUILayout.Toggle("Active", script.Fire_Attack);
            if (script.Fire_Attack)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FireDamage"),
                    new GUIContent("Damage from Fire"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Fire"), new GUIContent("Fire (prefab)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FireAttackAudio"),
                    new GUIContent("Attack audio"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FireSpawn"),
                    new GUIContent("Spawn points of Fire"), true);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField("Rocket Attack", EditorStyles.boldLabel);
            script.Rocket_Attack = EditorGUILayout.Toggle("Active", script.Rocket_Attack);
            if (script.Rocket_Attack)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RocketDamage"),
                    new GUIContent("Damage from Rockets"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RateOfShoot_Rocket"),
                    new GUIContent("Rate of shoot"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Rocket"),
                    new GUIContent("Rocket (prefab)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RocketAttackAudio"),
                    new GUIContent("Attack audio"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RocketSpawn"),
                    new GUIContent("Spawn points of Rockets"), true);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField("Melee Attack", EditorStyles.boldLabel);
            script.Melee_Attack = EditorGUILayout.Toggle("Active", script.Melee_Attack);
            if (script.Melee_Attack)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MeleeDamage"), new GUIContent("Damage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RateOfAttack"),
                    new GUIContent("Rate of attack"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MeleeAttackAudio"),
                    new GUIContent("Attack audio"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.HelpBox("If set [Melee Attack] other attacks are disable.", MessageType.Info);
            }

            if (script.Melee_Attack)
            {
                script.Rocket_Attack = false;
                script.Gun_Attack = false;
                script.Fire_Attack = false;
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(script.gameObject);
        }
    }

}


