using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace GercStudio.USK.Scripts
{
	
	[CustomEditor(typeof(Surface))]
	public class SurfaceEditor : Editor
	{

		private Surface script;
		
		private void Awake()
		{
			script = (Surface) target;
			
			if(script.Shadow || !script.gameObject.GetComponent<MeshRenderer>())
				return;

			script.Shadow = Instantiate(script.gameObject, script.transform.position, script.transform.rotation,
				script.transform);
			script.Shadow.transform.localScale = Vector3.one;
			
			script.Shadow.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;

			script.Shadow.gameObject.layer = 8;
			
			foreach (var comp in script.Shadow.GetComponents<Component>())
			{
				if (!(comp is Transform) & !(comp is MeshRenderer) & !(comp is MeshFilter))
				{
					DestroyImmediate(comp);
				}
			}

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			serializedObject.ApplyModifiedProperties();

			DrawDefaultInspector();
           
			if (GUI.changed)
			{
				EditorUtility.SetDirty(script);
				if(!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}

	}
}
