using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace MoreRealisticSleeping.Util
{
    public static class EmbeddedAssets
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Loads a sprite from embedded resources.
        /// </summary>
        /// <param name="category">The folder category (e.g., "EffectImages", "UIElements", or null for root)</param>
        /// <param name="fileName">The file name without extension (e.g., "Balding", "Settings")</param>
        /// <returns>The loaded sprite, or null if not found</returns>
        public static Sprite LoadSprite(string category, string fileName)
        {
            string cacheKey = $"{category ?? "root"}/{fileName}";

            if (_spriteCache.TryGetValue(cacheKey, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            Texture2D texture = LoadTexture(category, fileName);
            if (texture == null)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            _spriteCache[cacheKey] = sprite;
            return sprite;
        }

        /// <summary>
        /// Loads a texture from embedded resources.
        /// </summary>
        /// <param name="category">The folder category (e.g., "EffectImages", "UIElements", or null for root)</param>
        /// <param name="fileName">The file name without extension (e.g., "Balding", "Settings")</param>
        /// <returns>The loaded texture, or null if not found</returns>
        public static Texture2D LoadTexture(string category, string fileName)
        {
            string cacheKey = $"{category ?? "root"}/{fileName}";

            if (_textureCache.TryGetValue(cacheKey, out Texture2D cachedTexture))
            {
                return cachedTexture;
            }

            // Build the resource name
            // Format: MoreRealisticSleeping.Assets.{Category}.{FileName}.png
            string resourceName;
            if (string.IsNullOrEmpty(category))
            {
                resourceName = $"MoreRealisticSleeping.Assets.{fileName}.png";
            }
            else
            {
                resourceName = $"MoreRealisticSleeping.Assets.{category}.{fileName}.png";
            }

            // Handle spaces in file names - they become underscores in resource names
            resourceName = resourceName.Replace(" ", "_");

            try
            {
                using var stream = _assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    // Try alternative name patterns
                    string altResourceName = resourceName.Replace("_", " ");
                    stream?.Dispose();
                    using var altStream = _assembly.GetManifestResourceStream(altResourceName);
                    if (altStream == null)
                    {
                        MelonLogger.Warning($"[EmbeddedAssets] Resource not found: {resourceName}");
                        return null;
                    }
                    return LoadTextureFromStream(altStream, cacheKey);
                }

                return LoadTextureFromStream(stream, cacheKey);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[EmbeddedAssets] Failed to load resource {resourceName}: {ex.Message}");
                return null;
            }
        }

        private static Texture2D LoadTextureFromStream(Stream stream, string cacheKey)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();

            Texture2D texture = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(texture, bytes))
            {
                _textureCache[cacheKey] = texture;
                return texture;
            }

            MelonLogger.Warning($"[EmbeddedAssets] Failed to load image data for: {cacheKey}");
            return null;
        }

        /// <summary>
        /// Loads the app icon sprite.
        /// </summary>
        public static Sprite LoadAppIcon()
        {
            return LoadSprite(null, "SleepingAppIcon");
        }

        /// <summary>
        /// Loads an effect image sprite by effect name.
        /// </summary>
        /// <param name="effectName">The effect name (e.g., "Balding", "Anti Gravity")</param>
        public static Sprite LoadEffectSprite(string effectName)
        {
            return LoadSprite("EffectImages", effectName);
        }

        /// <summary>
        /// Loads a UI element sprite by name.
        /// </summary>
        /// <param name="elementName">The UI element name (e.g., "Settings", "SaveButton")</param>
        public static Sprite LoadUIElementSprite(string elementName)
        {
            return LoadSprite("UIElements", elementName);
        }

        /// <summary>
        /// Loads a UI element texture by name.
        /// </summary>
        /// <param name="elementName">The UI element name (e.g., "Settings", "SaveButton")</param>
        public static Texture2D LoadUIElementTexture(string elementName)
        {
            return LoadTexture("UIElements", elementName);
        }

        /// <summary>
        /// Clears all cached sprites and textures.
        /// </summary>
        public static void ClearCache()
        {
            _spriteCache.Clear();
            _textureCache.Clear();
        }

        /// <summary>
        /// Lists all embedded resources (for debugging).
        /// </summary>
        public static void LogAllResources()
        {
            var resources = _assembly.GetManifestResourceNames();
            MelonLogger.Msg($"[EmbeddedAssets] Found {resources.Length} embedded resources:");
            foreach (var resource in resources)
            {
                MelonLogger.Msg($"  - {resource}");
            }
        }
    }
}
