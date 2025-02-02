﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.Core;
using VRC.SDK3.Avatars.Components;

namespace io.github.azukimochi
{
    [CustomEditor(typeof(LightLimitChangerSettings))]
    internal sealed class LightLimitChangerSettingsEditor : Editor
    {
        private SerializedProperty AssetContainer;
        private SerializedProperty IsDefaultUse;
        private SerializedProperty IsValueSave;
        private SerializedProperty OverwriteDefaultLightMinMax;
        private SerializedProperty DefaultLightValue;
        private SerializedProperty MaxLightValue;
        private SerializedProperty MinLightValue;
        private SerializedProperty TargetShader;
        private SerializedProperty AllowColorTempControl;
        private SerializedProperty AllowSaturationControl;
        private SerializedProperty AllowUnlitControl;
        private SerializedProperty AllowOverridePoiyomiAnimTag;
        private SerializedProperty AddResetButton;
        private SerializedProperty GenerateAtBuild;
        private SerializedProperty ExcludeEditorOnly;

        private static bool _isOptionFoldoutOpen = true;

        private void OnEnable()
        {
            AssetContainer =                            serializedObject.FindProperty  (nameof(LightLimitChangerSettings.AssetContainer));
            var parameters =                serializedObject.FindProperty  (nameof(LightLimitChangerSettings.Parameters));
            IsDefaultUse =                  parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.IsDefaultUse));
            IsValueSave =                   parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.IsValueSave));
            OverwriteDefaultLightMinMax =   parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.OverwriteDefaultLightMinMax));
            DefaultLightValue =             parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.DefaultLightValue));
            MaxLightValue =                 parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.MaxLightValue));
            MinLightValue =                 parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.MinLightValue));
            TargetShader =                  parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.TargetShader));
            AllowColorTempControl =        parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.AllowColorTempControl));
            AllowSaturationControl =        parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.AllowSaturationControl));
            AllowUnlitControl =             parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.AllowUnlitControl));
            AllowOverridePoiyomiAnimTag =
                parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.AllowOverridePoiyomiAnimTag));
            AddResetButton =                parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.AddResetButton));
            GenerateAtBuild =               parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.GenerateAtBuild));
            ExcludeEditorOnly =             parameters.FindPropertyRelative(nameof(LightLimitChangerParameters.ExcludeEditorOnly));
        }

        public override void OnInspectorGUI()
        {
            Utils.ShowVersionInfo();

            EditorGUILayout.Separator();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(IsDefaultUse, Localization.G("label.use_default"));
            EditorGUILayout.PropertyField(IsValueSave, Localization.G("label.save_value"));
            EditorGUILayout.PropertyField(OverwriteDefaultLightMinMax, Localization.G("label.override_min_max"));
            EditorGUILayout.PropertyField(MaxLightValue, Localization.G("label.light_max"));
            EditorGUILayout.PropertyField(MinLightValue, Localization.G("label.light_min"));
            EditorGUILayout.PropertyField(DefaultLightValue, Localization.G("label.light_default"));

            EditorGUILayout.PropertyField(AllowColorTempControl, Localization.G("label.allow_color_tmp"));
            EditorGUILayout.PropertyField(AllowSaturationControl, Localization.G("label.allow_saturation"));
            EditorGUILayout.PropertyField(AllowUnlitControl, Localization.G("label.allow_unlit"));
            EditorGUILayout.PropertyField(AddResetButton, Localization.G("label.allow_reset"));
            EditorGUILayout.PropertyField(AllowOverridePoiyomiAnimTag,
                Localization.G("label.allow_override_poiyomi"));

            using (var group = new Utils.FoldoutHeaderGroupScope(ref _isOptionFoldoutOpen, Localization.G("category.select_option")))
            {
                if (group.IsOpen)
                {
                    TargetShader.intValue = EditorGUILayout.MaskField(Localization.G("label.target_shader"), TargetShader.intValue, ShaderInfo.RegisteredShaderInfoNames);

                    EditorGUILayout.PropertyField(ExcludeEditorOnly, Localization.G("label.allow_editor_only"));
                    EditorGUILayout.Separator();
                    EditorGUILayout.PropertyField(GenerateAtBuild, Localization.G("label.allow_gen_playmode"));

                    EditorGUILayout.Separator();
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(AssetContainer);
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Separator();


            Localization.ShowLocalizationUI();
        }
    }
}
