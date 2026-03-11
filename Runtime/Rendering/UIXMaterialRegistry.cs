using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIX.Rendering
{
    /// <summary>
    /// Registry of UIX materials. Creates and caches materials from UIX shaders.
    /// Supports custom material overrides for games.
    /// </summary>
    public static class UIXMaterialRegistry
    {
        public enum MaterialType
        {
            Solid,
            Rounded,
            Image,
            RoundedImage,
            Shadow
        }

        private static readonly Dictionary<MaterialType, Material> _materials = new Dictionary<MaterialType, Material>();
        private static readonly Dictionary<string, Material> _customMaterials = new Dictionary<string, Material>();

        private const string ShaderSolid = "UIX/Solid";
        private const string ShaderRounded = "UIX/RoundedRect";
        private const string ShaderImage = "UIX/Image";
        private const string ShaderRoundedImage = "UIX/RoundedImage";
        private const string ShaderShadow = "UIX/Shadow";

        /// <summary>
        /// Gets a material for the given type. Materials are cached.
        /// </summary>
        public static Material GetMaterial(MaterialType type)
        {
            if (_materials.TryGetValue(type, out var mat) && mat != null)
                return mat;

            var shader = GetShader(type);
            if (shader == null)
            {
                Debug.LogWarning($"[UIX] Shader for {type} not found. Using default UI material.");
                return null;
            }

            mat = new Material(shader);
            mat.hideFlags = HideFlags.DontSave;
            _materials[type] = mat;
            return mat;
        }

        /// <summary>
        /// Gets a material instance for rounded rect with the given radius (normalized 0-0.5).
        /// Returns a new material instance to allow per-element radius.
        /// </summary>
        public static Material GetRoundedMaterial(float radius)
        {
            var baseMat = GetMaterial(MaterialType.Rounded);
            if (baseMat == null) return null;

            var mat = new Material(baseMat);
            mat.SetFloat("_Radius", Mathf.Clamp(radius, 0f, 0.5f));
            mat.hideFlags = HideFlags.DontSave;
            return mat;
        }

        /// <summary>
        /// Gets a material instance for rounded image with radius.
        /// </summary>
        public static Material GetRoundedImageMaterial(float radius)
        {
            var baseMat = GetMaterial(MaterialType.RoundedImage);
            if (baseMat == null) return null;

            var mat = new Material(baseMat);
            mat.SetFloat("_Radius", Mathf.Clamp(radius, 0f, 0.5f));
            mat.hideFlags = HideFlags.DontSave;
            return mat;
        }

        /// <summary>
        /// Gets a custom material by path (Resources path or theme variable).
        /// </summary>
        public static Material GetCustomMaterial(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            if (_customMaterials.TryGetValue(path, out var mat) && mat != null)
                return mat;

            mat = Resources.Load<Material>(path);
            if (mat != null)
            {
                _customMaterials[path] = mat;
                return mat;
            }

            return null;
        }

        /// <summary>
        /// Registers a custom material for a path (e.g. from theme).
        /// </summary>
        public static void RegisterCustomMaterial(string path, Material material)
        {
            if (!string.IsNullOrEmpty(path) && material != null)
                _customMaterials[path] = material;
        }

        /// <summary>
        /// Applies material to Image. Stencil/Mask support is handled by Image when maskable.
        /// </summary>
        public static Material ApplyMaterialToImage(Image image, MaterialType type, Material customMaterial = null)
        {
            Material mat = customMaterial;
            if (mat == null)
                mat = GetMaterial(type);

            if (mat == null) return null;

            image.material = mat;
            return mat;
        }

        /// <summary>
        /// Converts pixel radius to normalized (0-0.5) for shader.
        /// rectSize is the size of the RectTransform.
        /// </summary>
        public static float PixelRadiusToNormalized(float pixelRadius, Vector2 rectSize)
        {
            if (rectSize.x <= 0 || rectSize.y <= 0) return 0.1f;
            float minDim = Mathf.Min(rectSize.x, rectSize.y);
            return Mathf.Clamp((pixelRadius / minDim) * 0.5f, 0f, 0.5f);
        }

        private static Shader GetShader(MaterialType type)
        {
            return type switch
            {
                MaterialType.Solid => Shader.Find(ShaderSolid),
                MaterialType.Rounded => Shader.Find(ShaderRounded),
                MaterialType.Image => Shader.Find(ShaderImage),
                MaterialType.RoundedImage => Shader.Find(ShaderRoundedImage),
                MaterialType.Shadow => Shader.Find(ShaderShadow),
                _ => null
            };
        }

        /// <summary>
        /// Clears cached materials (e.g. on theme change or domain reload).
        /// </summary>
        public static void ClearCache()
        {
            foreach (var mat in _materials.Values)
            {
                if (mat != null && mat.hideFlags == HideFlags.DontSave)
                    Object.Destroy(mat);
            }
            _materials.Clear();
            _customMaterials.Clear();
        }
    }
}
