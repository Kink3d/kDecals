using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditorInternal;

namespace kTools.Decals.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(DecalData)), CanEditMultipleObjects]
    sealed class DecalDataEditor : Editor
    {
#region Structs
        struct Styles
        {
            // Foldouts
            public static readonly GUIContent PoolingOptions = new GUIContent("Pooling Options");
            public static readonly GUIContent ProjectionOptions = new GUIContent("Projection Options");
            public static readonly GUIContent DeferredOptions = new GUIContent("Deferred Options");

            // Properties
            public static readonly GUIContent PoolingEnabled = new GUIContent("Enabled",
                "If enabled all Decals created via DecalSystem will be pre-generated and taken from a Pool.");

            public static readonly GUIContent InstanceCount = new GUIContent("Instance Count",
                "How many instances of this Decal will be created in the Pool.");

            public static readonly GUIContent Depth = new GUIContent("Depth",
                "How far the Decal projection draw on Z axis.");

            public static readonly GUIContent DepthFalloff = new GUIContent("Depth Falloff",
                "How much the Decal should blend transparency towards its Depth value.");

            public static readonly GUIContent Angle = new GUIContent("Angle",
                "Maximum angle between surface and Decal forward vector.");

            public static readonly GUIContent AngleFalloff = new GUIContent("Angle Falloff",
                "How much the Decal should blend transparency towards its Angle value.");

            public static readonly GUIContent LayerMask = new GUIContent("Layer Mask",
                "Which layers the Decal is applied to.");

            public static readonly GUIContent SortingOrder = new GUIContent("Sorting Order",
                "Decals with higher values are drawn on top of ones with lower values.");

            // Deferred
            public static readonly GUIContent ForceForward = new GUIContent("Force Forward",
                "Should this Decal be rendered in Forward?");
            
            public static readonly GUIContent AffectAlbedo = new GUIContent("Affect Albedo",
                "Should Decals write to Abledo? (Deferred mode only)");

            public static readonly GUIContent AffectSpecular = new GUIContent("Affect Specular",
                "Should Decals write to Specular? (Deferred mode only)");

            public static readonly GUIContent AffectSmoothness = new GUIContent("Affect Smoothness",
                "Should Decals write to Smoothness? (Deferred mode only)");

            public static readonly GUIContent AffectNormal = new GUIContent("Affect Normal",
                "Should Decals write to Normal? (Deferred mode only)");

            public static readonly GUIContent AffectOcclusion = new GUIContent("Affect Occlusion",
                "Should Decals write to Occlusion? (Deferred mode only)");

            public static readonly GUIContent UpdateGBuffers = new GUIContent("Update GBuffers",
                "Should GBuffer copies update after this Decal is drawn?");
        }

        struct PropertyNames
        {
            public static readonly string PoolingEnabled = "m_PoolingEnabled";
            public static readonly string InstanceCount = "m_InstanceCount";
            public static readonly string Depth = "m_Depth";
            public static readonly string DepthFalloff = "m_DepthFalloff";
            public static readonly string Angle = "m_Angle";
            public static readonly string AngleFalloff = "m_AngleFalloff";
            public static readonly string LayerMask = "m_LayerMask";
            public static readonly string SortingOrder = "m_SortingOrder";
            public static readonly string ForceForward = "m_ForceForward";
            public static readonly string AffectAlbedo = "m_AffectAlbedo";
            public static readonly string AffectSpecular = "m_AffectSpecular";
            public static readonly string AffectSmoothness = "m_AffectSmoothness";
            public static readonly string AffectNormal = "m_AffectNormal";
            public static readonly string AffectOcclusion = "m_AffectOcclusion";
            public static readonly string UpdateGBuffers = "m_UpdateGBuffers";
        }
#endregion

#region Fields
        const string kEditorPrefKey = "kDecals:DecalData:";
        DecalData m_Target;
        MaterialEditor m_MaterialEditor;

        // Foldouts
        bool m_PoolingOptionsFoldout;
        bool m_ProjectionOptionsFoldout;
        bool m_DeferredOptionsFoldout;

        // Properties
        SerializedProperty m_PoolingEnabledProp;
        SerializedProperty m_InstanceCountProp;
        SerializedProperty m_DepthProp;
        SerializedProperty m_DepthFalloffProp;
        SerializedProperty m_AngleProp;
        SerializedProperty m_AngleFalloffProp;
        SerializedProperty m_LayerMaskProp;
        SerializedProperty m_SortingOrderProp;
        SerializedProperty m_ForceForwardProp;
        SerializedProperty m_AffectAlbedoProp;
        SerializedProperty m_AffectSpecularProp;
        SerializedProperty m_AffectSmoothnessProp;
        SerializedProperty m_AffectNormalProp;
        SerializedProperty m_AffectOcclusionProp;
        SerializedProperty m_UpdateGBuffersProp;
#endregion

#region State
        void OnEnable()
        {
            // Set data
            m_Target = target as DecalData;
            if(!m_Target.isImported)
                return;

            // Get Properties
            m_PoolingEnabledProp = serializedObject.FindProperty(PropertyNames.PoolingEnabled);
            m_InstanceCountProp = serializedObject.FindProperty(PropertyNames.InstanceCount);
            m_DepthProp = serializedObject.FindProperty(PropertyNames.Depth);
            m_DepthFalloffProp = serializedObject.FindProperty(PropertyNames.DepthFalloff);
            m_AngleProp = serializedObject.FindProperty(PropertyNames.Angle);
            m_AngleFalloffProp = serializedObject.FindProperty(PropertyNames.AngleFalloff);
            m_LayerMaskProp = serializedObject.FindProperty(PropertyNames.LayerMask);
            m_SortingOrderProp = serializedObject.FindProperty(PropertyNames.SortingOrder);
            m_ForceForwardProp = serializedObject.FindProperty(PropertyNames.ForceForward);
            m_AffectAlbedoProp = serializedObject.FindProperty(PropertyNames.AffectAlbedo);
            m_AffectSpecularProp = serializedObject.FindProperty(PropertyNames.AffectSpecular);
            m_AffectSmoothnessProp = serializedObject.FindProperty(PropertyNames.AffectSmoothness);
            m_AffectNormalProp = serializedObject.FindProperty(PropertyNames.AffectNormal);
            m_AffectOcclusionProp = serializedObject.FindProperty(PropertyNames.AffectOcclusion);
            m_UpdateGBuffersProp = serializedObject.FindProperty(PropertyNames.UpdateGBuffers);

            // Create Editors
            m_MaterialEditor = Editor.CreateEditor(m_Target.material) as MaterialEditor;
        }

        void OnDisable ()
        {
            // Cleanup
            if(m_MaterialEditor != null)
            {
                DestroyImmediate(m_MaterialEditor);
            }
        }
#endregion

#region GUI
        public override void OnInspectorGUI()
        {
            // Test for target imported
            if(!m_Target.isImported)
            {
                EditorGUILayout.HelpBox("Waiting for Asset import...", MessageType.Info);
                return;
            }

            // Get foldouts from EditorPrefs
            m_PoolingOptionsFoldout = GetFoldoutState("PoolingOptions");
            m_ProjectionOptionsFoldout = GetFoldoutState("ProjectionOptions");
            m_DeferredOptionsFoldout = GetFoldoutState("DeferredOptions");
            
            // Setup
            serializedObject.Update();

            // Pooling Options
            var poolingOptions = EditorGUILayout.BeginFoldoutHeaderGroup(m_PoolingOptionsFoldout, Styles.PoolingOptions);
            if(poolingOptions)
            {
                DrawPoolingOptions();
                EditorGUILayout.Space();
            }
            SetFoldoutState("PoolingOptions", m_PoolingOptionsFoldout, poolingOptions);
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Projection Options
            var projectionOptions = EditorGUILayout.BeginFoldoutHeaderGroup(m_ProjectionOptionsFoldout, Styles.ProjectionOptions);
            if(projectionOptions)
            {
                DrawProjectionOptions();
                EditorGUILayout.Space();
            }
            SetFoldoutState("ProjectionOptions", m_ProjectionOptionsFoldout, projectionOptions);
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Deferred Options
            var deferredOptions = EditorGUILayout.BeginFoldoutHeaderGroup(m_DeferredOptionsFoldout, Styles.DeferredOptions);
            if(deferredOptions)
            {
                DrawDeferredOptions();
                EditorGUILayout.Space();
            }
            SetFoldoutState("DeferredOptions", m_DeferredOptionsFoldout, deferredOptions);
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Material Editor
            DrawMaterialEditor();

            // Finalize
            serializedObject.ApplyModifiedProperties();
        }

        void DrawPoolingOptions()
        {
            // Enabled
            EditorGUILayout.PropertyField(m_PoolingEnabledProp, Styles.PoolingEnabled);
            
            // Instance Count
            using(var disabledScope = new EditorGUI.DisabledScope(!m_PoolingEnabledProp.boolValue))
            {
                EditorGUILayout.PropertyField(m_InstanceCountProp, Styles.InstanceCount);
            }
        }

        void DrawProjectionOptions()
        {
            // Depth
            EditorGUILayout.PropertyField(m_DepthProp, Styles.Depth);
            EditorGUILayout.Slider(m_DepthFalloffProp, 0.0f, 1.0f, Styles.DepthFalloff);

            // Angle
            EditorGUILayout.Slider(m_AngleProp, 0.0f, 180.0f, Styles.Angle);
            EditorGUILayout.Slider(m_AngleFalloffProp, 0.0f, 1.0f, Styles.AngleFalloff);

            // Layer Mask
            EditorGUI.BeginChangeCheck();
            LayerMask tempMask = EditorGUILayout.MaskField(Styles.LayerMask, InternalEditorUtility.LayerMaskToConcatenatedLayersMask((LayerMask)m_LayerMaskProp.intValue), InternalEditorUtility.layers);
            tempMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            if(EditorGUI.EndChangeCheck())
            {
                m_LayerMaskProp.intValue = (int)tempMask;
            }

            // Sorting Order
            EditorGUILayout.PropertyField(m_SortingOrderProp, Styles.SortingOrder);
        }

        void DrawDeferredOptions()
        {
            var settings = DecalSettings.GetOrCreateSettings();
            var rendererAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            var renderer = rendererAsset.scriptableRenderer as UniversalRenderer;
            var field = typeof(UniversalRenderer).GetField("m_RenderingMode", BindingFlags.Instance | BindingFlags.NonPublic);
            var renderingMode = (RenderingMode)field.GetValue(renderer);

            var isDeferred = renderingMode == RenderingMode.Deferred;
            var isForceForward = m_ForceForwardProp.boolValue;
            var isEnablePerChannelDecals = isDeferred && settings.enablePerChannelDecals;
            var isAllowedToUpdateGBuffers = isDeferred && settings.enablePerChannelDecals && settings.gBufferUpdateFrequency != UpdateFrequency.Never;

            if(!isDeferred)
            {
                EditorGUILayout.HelpBox("Enable deferred rendering on the active UniversalRenderer to use these features.", MessageType.Info);
            }
            else if(!isForceForward && !isEnablePerChannelDecals)
            {
                EditorGUILayout.HelpBox("Enable per-channel Decals in Project Settings/kDecals to use these features.", MessageType.Info);
            }

            using (var disabledScope = new EditorGUI.DisabledGroupScope(!isDeferred))
            {
                EditorGUILayout.PropertyField(m_ForceForwardProp, Styles.ForceForward);
            }

            using (var disabledScope = new EditorGUI.DisabledGroupScope(!isDeferred || isForceForward || !isEnablePerChannelDecals))
            {
                EditorGUILayout.PropertyField(m_AffectAlbedoProp, Styles.AffectAlbedo);
                EditorGUILayout.PropertyField(m_AffectSpecularProp, Styles.AffectSpecular);
                EditorGUILayout.PropertyField(m_AffectSmoothnessProp, Styles.AffectSmoothness);
                EditorGUILayout.PropertyField(m_AffectNormalProp, Styles.AffectNormal);
                EditorGUILayout.PropertyField(m_AffectOcclusionProp, Styles.AffectOcclusion);
            }

            if(isDeferred && !isForceForward && isEnablePerChannelDecals && !isAllowedToUpdateGBuffers)
            {
                EditorGUILayout.HelpBox("Set GBuffer Update Frequency to something other than Never in Project Settings/kDecals to use this feature.", MessageType.Info);
            }

            using (var disabledScope = new EditorGUI.DisabledGroupScope(!isDeferred || isForceForward || !isAllowedToUpdateGBuffers))
            {
                EditorGUILayout.PropertyField(m_UpdateGBuffersProp, Styles.UpdateGBuffers);
            }
        }

        void DrawMaterialEditor()
        {
            // Multiple selection cant be handled for Material inspector...
            var selectionCount = Selection.objects.Length;
            if(selectionCount > 1)
            {
                EditorGUILayout.HelpBox("Multiple Object editing is not supported for Material Options.", MessageType.Warning);
                return;
            }

            // Default Material Editor
            EditorGUILayout.Space();
            m_MaterialEditor.DrawHeader();
            m_MaterialEditor.OnInspectorGUI();
        }
#endregion

#region EditorPrefs
        bool GetFoldoutState(string name)
        {
            // Get value from EditorPrefs
            return EditorPrefs.GetBool($"{kEditorPrefKey}.{name}");
        }

        void SetFoldoutState(string name, bool field, bool value)
        {
            if(field == value)
                return;

            // Set value to EditorPrefs and field
            EditorPrefs.SetBool($"{kEditorPrefKey}.{name}", value);
            field = value;
        }
#endregion
    }
}
