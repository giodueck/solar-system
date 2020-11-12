using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CelestialBody))]
public class BodyEditor : Editor
{
    CelestialBody body;
    Editor shapeEditor;
    Editor colorEditor;
    Editor physicsEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (check.changed)
            {
                body.GenerateBody();
            }
        }

        if (GUILayout.Button("Generate Body"))
        {
            body.GenerateBody();
        }

        DrawSettingsEditor(body.shapeSettings, body.OnShapeSettingsUpdated, ref body.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(body.colorSettings, body.OnColorSettingsUpdated, ref body.colorSettingsFoldout, ref colorEditor);
        DrawSettingsEditor(body.physicsSettings, body.OnPhysiscsSettingsUpdated, ref body.physicsSettingsFoldout, ref physicsEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable() {
        body = (CelestialBody) target;
    }
}