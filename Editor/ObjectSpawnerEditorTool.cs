using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Berke.ObjectSpawner.Editor
{
    [EditorTool("Object Spawner")]
    public class ObjectSpawnerEditorTool : EditorTool
    {
        
        [NonSerialized] GUIContent toolIcon;
        [NonSerialized]
        private Rect toolWindowRect = new Rect(10, 0, 260f, 0f);
        private Spawner selectedSpawner;
        private float spawnRadius = 2;
        [NonSerialized] private bool mouseDown;
        [NonSerialized] private Vector2 mouseDownPosition;
        public override GUIContent toolbarIcon
        {
            get
            {
                if(toolIcon is null)
                {
                    toolIcon = EditorGUIUtility.IconContent("GameObject On Icon", "Object Spawner");
                    
                }
                return toolIcon;
            }
        }

        public void DrawToolWindow(int id)
        {
            selectedSpawner = EditorGUILayout.
            ObjectField(selectedSpawner, typeof(Spawner), false) as Spawner;
            spawnRadius = EditorGUILayout.FloatField("Radius", spawnRadius);
        }
        public override void OnToolGUI(EditorWindow window)
        {
            toolWindowRect.y = window.position.height - toolWindowRect.height - 10;
            toolWindowRect = GUILayout.Window(45, toolWindowRect,
            DrawToolWindow, "Object Spawner");
            Handles.color = Color.red;

            var ray = HandleUtility.GUIPointToWorldRay(mouseDown ? mouseDownPosition : Event.current.mousePosition);
            bool hitGround = Physics.Raycast(ray, out var result, 100);
            if (hitGround)
            {
                Handles.DrawWireDisc(result.point, Vector3.up, spawnRadius,3f);
                
                
            }

            var controlId = EditorGUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && Event.current.
                    modifiers == EventModifiers.None)
                    {
                        GUIUtility.hotControl = controlId;
                        mouseDown = true;
                        mouseDownPosition = Event.current.mousePosition;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (mouseDown)
                    {
                        spawnRadius += EditorGUIUtility.
                        PixelsToPoints(Event.current.delta).x / 100;
                        window.Repaint();
                    }
                    break;
                case EventType.MouseMove:
                    window.Repaint();
                    break;
                case EventType.MouseLeaveWindow:
                case EventType.MouseUp:
                    if (mouseDown && hitGround && selectedSpawner)
                    {
                        selectedSpawner.SpawnObjects(result.point,spawnRadius);
                    }
                    mouseDown = false;
                    break;
            }
        }

    }
}
