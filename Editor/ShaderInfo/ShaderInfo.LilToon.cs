﻿using System;
using System.Linq;
using UnityEngine;

namespace io.github.azukimochi
{
    partial class ShaderInfo
    {
        public sealed class LilToon : ShaderInfo
        {
            public static LilToon Instance { get; } = new LilToon();

            public const string _LightMinLimit = nameof(_LightMinLimit);
            public const string _LightMaxLimit = nameof(_LightMaxLimit);
            public const string _AsUnlit = nameof(_AsUnlit);
            public const string _MainTexHSVG = nameof(_MainTexHSVG);
            public const string _Color = nameof(_Color);
            public const string _Color2nd = nameof(_Color2nd);
            public const string _Color3rd = nameof(_Color3rd);
            public const string _MainTex = nameof(_MainTex);
            public const string _Main2ndTex = nameof(_Main2ndTex);
            public const string _Main3rdTex = nameof(_Main3rdTex);
            public const string _MainGradationTex = nameof(_MainGradationTex);
            public const string _MainGradationStrength = nameof(_MainGradationStrength);
            public const string _MainColorAdjustMask = nameof(_MainColorAdjustMask);

            private static class PropertyIDs
            {
                public static readonly int LightMinLimit = Shader.PropertyToID(_LightMinLimit);
                public static readonly int LightMaxLimit = Shader.PropertyToID(_LightMaxLimit);
                public static readonly int AsUnlit = Shader.PropertyToID(_AsUnlit);
                public static readonly int MainTexHSVG = Shader.PropertyToID(_MainTexHSVG);
                public static readonly int Color = Shader.PropertyToID(_Color);
                public static readonly int Color2nd = Shader.PropertyToID(_Color2nd);
                public static readonly int Color3rd = Shader.PropertyToID(_Color3rd);
                public static readonly int MainTex = Shader.PropertyToID(_MainTex);
                public static readonly int Main2ndTex = Shader.PropertyToID(_Main2ndTex);
                public static readonly int Main3rdTex = Shader.PropertyToID(_Main3rdTex);
                public static readonly int MainGradationTex = Shader.PropertyToID(_MainGradationTex);
                public static readonly int MainGradationStrength = Shader.PropertyToID(_MainGradationStrength);
                public static readonly int MainColorAdjustMask = Shader.PropertyToID(_MainColorAdjustMask);
            }

            private static class DefaultParameters
            {
                public static readonly Color Color = Color.white;
                public static readonly Color Color2nd = Color.white;
                public static readonly Color Color3rd = Color.white;
                public static readonly Vector4 MainTexHSVG = new Vector4(0, 1, 1, 1);
                public static readonly float MainGradationStrength = 0;
            }

            public override bool TryNormalizeMaterial(Material material, UnityEngine.Object assetContainer)
            {
                bool result = false;
                var textureBaker = TextureBaker.GetInstance<DefaultTextureBaker>();

                // MainTexture
                {
                    bool bakeFlag = false;
                    bool isColorAdjusted = false;

                    var tex = material.GetTexture(PropertyIDs.MainTex);
                    if (tex != null)
                        textureBaker.Texture = tex;

                    // MainColor
                    if (material.GetColor(PropertyIDs.Color) != DefaultParameters.Color)
                    {
                        textureBaker.Color = material.GetColor(PropertyIDs.Color);
                        material.SetColor(PropertyIDs.Color, DefaultParameters.Color);
                        bakeFlag = true;
                    }

                    // HSV / Gamma
                    if (material.GetVector(PropertyIDs.MainTexHSVG) != DefaultParameters.MainTexHSVG)
                    {
                        textureBaker.HSVG = material.GetVector(PropertyIDs.MainTexHSVG);
                        material.SetVector(PropertyIDs.MainTexHSVG, DefaultParameters.MainTexHSVG);
                        bakeFlag = true;
                        isColorAdjusted = true;
                    }

                    // Gradation
                    if (material.GetTexture(PropertyIDs.MainGradationTex) != null && material.GetFloat(PropertyIDs.MainGradationStrength) != DefaultParameters.MainGradationStrength)
                    {
                        textureBaker.GradationMap = material.GetTexture(PropertyIDs.MainGradationTex);
                        textureBaker.GradationStrength = material.GetFloat(PropertyIDs.MainGradationStrength);
                        material.SetTexture(PropertyIDs.MainGradationTex, null);
                        material.SetFloat(PropertyIDs.MainGradationStrength, DefaultParameters.MainGradationStrength);
                        bakeFlag = true;
                        isColorAdjusted = true;
                    }

                    // Color Adujust Mask
                    if (isColorAdjusted && material.GetTexture(PropertyIDs.MainColorAdjustMask) != null)
                    {
                        textureBaker.Mask = material.GetTexture(PropertyIDs.MainColorAdjustMask);
                        material.SetTexture(PropertyIDs.MainColorAdjustMask, null);
                        bakeFlag = true;
                    }

                    // Run Bake
                    if (bakeFlag)
                    {
                        material.SetTexture(PropertyIDs.MainTex, textureBaker.Bake().AddTo(assetContainer));
                    }

                    result |= bakeFlag;
                }

                // 2nd Texture
                {
                    textureBaker.Reset();
                    bool bakeFlag = false;

                    var tex = material.GetTexture(PropertyIDs.Main2ndTex);
                    if (tex != null)
                        textureBaker.Texture = tex;

                    if (material.GetColor(PropertyIDs.Color2nd) != DefaultParameters.Color2nd)
                    {
                        textureBaker.Color = material.GetColor(PropertyIDs.Color2nd);
                        material.SetColor(PropertyIDs.Color2nd, DefaultParameters.Color2nd);
                        bakeFlag = true;
                    }

                    if (bakeFlag)
                    {
                        material.SetTexture(PropertyIDs.Main2ndTex, textureBaker.Bake().AddTo(assetContainer));
                    }

                    result |= bakeFlag;
                }

                // 3rd Texture
                {
                    textureBaker.Reset();
                    bool bakeFlag = false;

                    var tex = material.GetTexture(PropertyIDs.Main3rdTex);
                    if (tex != null)
                        textureBaker.Texture = tex;

                    if (material.GetColor(PropertyIDs.Color3rd) != DefaultParameters.Color3rd)
                    {
                        textureBaker.Color = material.GetColor(PropertyIDs.Color3rd);
                        material.SetColor(PropertyIDs.Color3rd, DefaultParameters.Color3rd);
                        bakeFlag = true;
                    }

                    if (bakeFlag)
                    {
                        material.SetTexture(PropertyIDs.Main3rdTex, textureBaker.Bake().AddTo(assetContainer));
                    }
                    result |= bakeFlag;
                }

                return result;
            }

            public override bool IsTargetShader(Shader shader)
            {
                if (shader.name.Contains("lilToon", StringComparison.OrdinalIgnoreCase))
                    return true;

                // カスタムシェーダーの名前にlilToonが入ってない時のことを考慮して、パラメーターが含まれるかどうかをチェックする
                if (_propertyIDsArrayCache == null)
                {
                    // 横着
                    _propertyIDsArrayCache = typeof(PropertyIDs).GetFields().Select(x => (int)x.GetValue(null)).ToArray();
                }
                return _propertyIDsArrayCache.Intersect(shader.EnumeratePropertyNameIDs()).Count() == _propertyIDsArrayCache.Length;
            }

            private static int[] _propertyIDsArrayCache;

            public override void SetControlAnimation(in ControlAnimationContainer container, in ControlAnimationParameters parameters)
            {
                switch(container.ControlType)
                {
                    case LightLimitControlType.Light:

                        container.Default.SetParameterAnimation(parameters, _LightMinLimit, parameters.MinLightValue);
                        container.Default.SetParameterAnimation(parameters, _LightMaxLimit, parameters.MaxLightValue);

                        container.Control.SetParameterAnimation(parameters, _LightMinLimit, parameters.MinLightValue, parameters.MaxLightValue);
                        container.Control.SetParameterAnimation(parameters, _LightMaxLimit, parameters.MinLightValue, parameters.MaxLightValue);

                        break;

                    case LightLimitControlType.Saturation:

                        container.Default.SetParameterAnimation(parameters, _MainTexHSVG, DefaultParameters.MainTexHSVG);

                        container.Control.SetParameterAnimation(parameters, _MainTexHSVG, DefaultParameters.MainTexHSVG, ~ShaderInfoUtility.IncludeField.Y);
                        container.Control.SetParameterAnimation(parameters, $"{_MainTexHSVG}.y", 0, 2);

                        break;

                    case LightLimitControlType.Unlit:

                        container.Default.SetParameterAnimation(parameters, _AsUnlit, 0);
                        container.Control.SetParameterAnimation(parameters, _AsUnlit, 0, 1);

                        break;

                    case LightLimitControlType.ColorTemperature:

                        container.Default.SetParameterAnimation(parameters, _Color   , DefaultParameters.Color);
                        container.Default.SetParameterAnimation(parameters, _Color2nd, DefaultParameters.Color2nd);
                        container.Default.SetParameterAnimation(parameters, _Color3rd, DefaultParameters.Color3rd);

                        container.Control.SetColorTempertureAnimation(parameters, _Color);
                        container.Control.SetColorTempertureAnimation(parameters, _Color2nd);
                        container.Control.SetColorTempertureAnimation(parameters, _Color3rd);

                        break;
                }
            }
        }
    }
}
