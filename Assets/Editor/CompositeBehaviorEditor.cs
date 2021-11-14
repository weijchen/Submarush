using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompositeBehavior))]
public class CompositeBehaviorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // setup
        CompositeBehavior cb = (CompositeBehavior) target;

        GUILayout.Space(10f);
        // Buttons to add and remove behaviors in our containers
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Add"))
        {
            AddBehavior(cb);
            EditorUtility.SetDirty(cb);
        }

        if (GUILayout.Button("Remove"))
        {
            RemoveBehavior(cb);
            EditorUtility.SetDirty(cb);
        }
        EditorGUILayout.EndVertical();
        
        // Check for behaviors
        if (cb.behaviors == null || cb.behaviors.Length == 0)
        {
            EditorGUILayout.HelpBox("No behaviors in array.", MessageType.Warning);
        } 
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            EditorGUILayout.LabelField("Number", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
            
            EditorGUILayout.LabelField("Behaviors", GUILayout.MinWidth(60f));
            EditorGUILayout.LabelField("Weights", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < cb.behaviors.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(15f));
                cb.behaviors[i] = (FlockBehavior) EditorGUILayout.ObjectField(cb.behaviors[i], typeof(FlockBehavior),
                    false, GUILayout.MinWidth(60f));
                cb.weights[i] =
                    EditorGUILayout.FloatField(cb.weights[i], GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f));
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(cb);
                }
            }
        }
    }

    void AddBehavior(CompositeBehavior cb)
    {
        int oldCount = (cb.behaviors != null) ? cb.behaviors.Length : 0;
        FlockBehavior[] newBehaviors = new FlockBehavior[oldCount + 1];
        float[] newWeights = new float[oldCount + 1];
        for (int i = 0; i < oldCount; i++)
        {
            newBehaviors[i] = cb.behaviors[i];
            newWeights[i] = cb.weights[i];
        }

        newWeights[oldCount] = 1f;
        cb.behaviors = newBehaviors;
        cb.weights = newWeights;
    }

    void RemoveBehavior(CompositeBehavior cb)
    {
        int oldCount = cb.behaviors.Length;
        if (oldCount == 1)
        {
            cb.behaviors = null;
            cb.weights = null;
            return;
        }
        FlockBehavior[] newBehaviors = new FlockBehavior[oldCount - 1];
        float[] newWeights = new float[oldCount - 1];
        for (int i = 0; i < oldCount - 1; i++)
        {
            newBehaviors[i] = cb.behaviors[i];
            newWeights[i] = cb.weights[i];
        }
        
        cb.behaviors = newBehaviors;
        cb.weights = newWeights;
    }
}
