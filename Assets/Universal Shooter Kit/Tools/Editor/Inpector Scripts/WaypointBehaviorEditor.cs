using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
	[CustomEditor(typeof(WaypointBehavior))]
	public class WaypointBehaviorEditor : Editor
	{
		private WaypointBehavior script;

		private ReorderableList pointsList;
		
		public void Awake()
		{
			script = (WaypointBehavior) target;
		}
		
		private void OnEnable()
		{

			pointsList = new ReorderableList(serializedObject, serializedObject.FindProperty("points"), true, true,
				true, true)
			{
				drawHeaderCallback = (Rect rect) =>
				{
					EditorGUI.LabelField(new Rect(rect.x + 11, rect.y, 100, EditorGUIUtility.singleLineHeight),
						"Point");
					EditorGUI.LabelField(
						new Rect(rect.x + rect.width / 2.5f + rect.width / 10 + 10, rect.y, rect.width / 4,
							EditorGUIUtility.singleLineHeight), "Next Action");
					EditorGUI.LabelField(
						new Rect(rect.x + rect.width * 0.9f - 7, rect.y, rect.width / 4,
							EditorGUIUtility.singleLineHeight), "Wait");
				},
				onAddCallback = (ReorderableList items) =>
				{
					if (script.points == null)
						script.points = new List<WaypointBehavior.Behavior>();


					var point = Helper.CreateWayPoint();
					point.transform.parent = script.transform;
					var wayPoint = new WaypointBehavior.Behavior
					{
						point = point,
						action = Helper.NextPointAction.NextPoint,
						curAction = Helper.NextPointAction.NextPoint,
						isLookAround = false,
						waitTime = 0
					};

					script.points.Add(wayPoint);
				},
				onRemoveCallback = (ReorderableList items) =>
				{
					DestroyImmediate(script.points[items.index].point);
					script.points.RemoveAt(items.index);
				},
				onChangedCallback = (ReorderableList items) =>
				{
					foreach (var point in script.points)
					{
						switch (point.action)
						{
							case Helper.NextPointAction.NextPoint:
								Helper.AddObjectIcon(point.point,
									script.points.IndexOf(point) == 0 ? "WaypointNextGreen" : "WaypointNextYellow");
								break;
							case Helper.NextPointAction.RandomPoint:
								Helper.AddObjectIcon(point.point,
									script.points.IndexOf(point) == 0 ? "WaypointRandomGreen" : "WaypointRandomYellow");
								break;
							case Helper.NextPointAction.ClosestPoint:
								Helper.AddObjectIcon(point.point,
									script.points.IndexOf(point) == 0
										? "WaypointClosestGreen"
										: "WaypointClosestYellow");
								break;
							case Helper.NextPointAction.Stop:
								Helper.AddObjectIcon(point.point, "StopWaypoint");
								break;
						}

//					if (point.action == Helper.NextPointAction.InOrder || point.action == Helper.NextPointAction.Random 
//					    || point.action == Helper.NextPointAction.Nearest)
//					{
//						Helper.AddObjectIcon(point.point,
//							script.points.IndexOf(point) == 0 ? "WaypointIconGreen" : "WaypointIconYellow");
//					}
					}
				},
				drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
				{
					script.points[index].point = (GameObject) EditorGUI.ObjectField(
						new Rect(rect.x, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight),
						script.points[index].point,
						typeof(GameObject), true);


					if (script.points[index].action != Helper.NextPointAction.Stop)
					{
						EditorGUI.LabelField(
							new Rect(rect.x + rect.width / 2.5f + 6, rect.y, rect.width / 10,
								EditorGUIUtility.singleLineHeight), "Go to ");


						script.points[index].action = (Helper.NextPointAction) EditorGUI.EnumPopup(
							new Rect(rect.x + rect.width / 2.5f + rect.width / 10 + 6, rect.y, rect.width / 3,
								EditorGUIUtility.singleLineHeight), script.points[index].action);
					}
					else
					{
						script.points[index].action = (Helper.NextPointAction) EditorGUI.EnumPopup(
							new Rect(rect.x + rect.width / 2.5f + 6, rect.y, rect.width / 3 + rect.width / 10,
								EditorGUIUtility.singleLineHeight), script.points[index].action);
					}

					script.points[index].waitTime = EditorGUI.FloatField(
						new Rect(rect.x + rect.width * 0.9f - 7, rect.y, rect.width / 10 + 7,
							EditorGUIUtility.singleLineHeight),
						script.points[index].waitTime);
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
			foreach (var point in script.points)
			{
				if (point.curAction != point.action)
				{
					point.curAction = point.action;
					script.ChangeIcon(point);
				}
			}
			
			ActiveEditorTracker.sharedTracker.isLocked = script.asjustment;
			
//			for (var i = 0; i < script.points.Count; i++)
//			{
//				var point = script.points[i].point;
//				if (script.points.Contains(point))
//					counts.Add(i);
//			}
//
//			if (counts.Count > 1)
//			{
//				script.points[script.counts.Count - 1]
//			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();

			if (!script.asjustment)
			{
				if (GUILayout.Button("Adjust"))
				{
					script.asjustment = true;
					
					foreach (var point in script.points)
					{
						if (!point.point) continue;
						point.point.SetActive(true);
						point.point.hideFlags = HideFlags.None;
					}
				}
			}
			else
			{
				if (GUILayout.Button("Exit"))
				{
					foreach (var point in script.points)
					{
						if (!point.point) continue;
						point.point.SetActive(false);
						point.point.hideFlags = HideFlags.HideInHierarchy;
						EditorApplication.RepaintHierarchyWindow();
						EditorApplication.DirtyHierarchyWindowSorting();
					}
					
					script.asjustment = false;
					Selection.activeObject = script.gameObject;
				}
			}


			EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(!script.asjustment);
			pointsList.DoLayoutList();
			EditorGUI.EndDisabledGroup();
			
			
			
//				for (var i = 0; i < script.Waypoints.Count; i++)
//				{
//					EditorGUILayout.BeginVertical("box");
//					if (script.Waypoints[i] != null)
//						if (!script.Waypoints[i].GetComponent<Waypoint>())
//						{
//							EditorGUILayout.HelpBox("Your waypoint should has [Waypoint] script", MessageType.Warning);
//						}
//
//					EditorGUILayout.BeginHorizontal();
//					EditorGUILayout.PropertyField(
//						serializedObject.FindProperty("Waypoints").GetArrayElementAtIndex(i),
//						new GUIContent("Point " + (i + 1)));
//
//					if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(20)))
//					{
//						script.Waypoints.Remove(script.Waypoints[i]);
//						break;
//					}
//					
//					EditorGUILayout.EndHorizontal();
//
//					if (GUILayout.Button("Create point"))
//					{
//						if (!script.Waypoints[i])
//						{
//							script.Waypoints[i] = Waypoint.CreateWayPoint();
//							break;
//						}
//					}
//
//					EditorGUILayout.EndVertical();
//			

			EditorGUILayout.Space();

			//DrawDefaultInspector();
//			if (GUILayout.Button("Add item"))
//			{
//				if (script.Waypoints == null)
//					script.Waypoints = new List<GameObject>();
//
//				script.Waypoints.Add(null);
//			}

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty(script);
		}
	}
}
