using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Dark.Scripts.Settings.Resolution
{
    public static class ResolutionSettings
    {
        public static bool IsInitialized { get; private set; }

        public static bool Fullscreen { get; private set; }
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        /// <summary>
        /// Which fullscreen mode to use when <see cref="Fullscreen"/> is true.
        /// Default is <see cref="FullScreenMode.FullScreenWindow"/> (borderless).
        /// </summary>
        public static FullScreenMode FullscreenModeWhenFullscreen { get; private set; } = FullScreenMode.FullScreenWindow;

        public static IReadOnlyList<ResolutionEntry> SupportedResolutions => _supportedResolutions;
        private static readonly List<ResolutionEntry> _supportedResolutions = new();
        
        public static void Initialize(IReadOnlyList<ResolutionEntry> supportedResolutions = null, bool applyNow = true)
        {
            Fullscreen = !GameSettings.WindowedMode;
            Width = GameSettings.ResolutionWidth;
            Height = GameSettings.ResolutionHeight;

            SetSupportedResolutions(supportedResolutions, keepSelection: true);

            // If saved settings are invalid (e.g., first run), default to current screen size.
            if (Width <= 1 || Height <= 1)
            {
                Width = Mathf.Max(1, Screen.width);
                Height = Mathf.Max(1, Screen.height);
            }

            // Snap to nearest supported resolution (if any).
            if (_supportedResolutions.Count > 0)
            {
                var best = FindBestMatchIndex(Width, Height);
                Width = _supportedResolutions[best].width;
                Height = _supportedResolutions[best].height;
            }

            IsInitialized = true;

            if (applyNow)
            {
                Apply();
            }
        }

        public static void SetSupportedResolutions(IReadOnlyList<ResolutionEntry> supportedResolutions, bool keepSelection = true)
        {
            _supportedResolutions.Clear();

            if (supportedResolutions != null && supportedResolutions.Count > 0)
            {
                AddUniqueResolutions(_supportedResolutions, supportedResolutions);
            }
            else
            {
                AddUniqueResolutionsFromSystem(_supportedResolutions);
            }

            _supportedResolutions.Sort(CompareResolutionEntry);

            if (!keepSelection || _supportedResolutions.Count == 0) return;

            // Keep the closest selection after list changes.
            var best = FindBestMatchIndex(Width > 0 ? Width : Screen.width, Height > 0 ? Height : Screen.height);
            Width = _supportedResolutions[best].width;
            Height = _supportedResolutions[best].height;
        }

        public static string[] GetResolutionLabels()
        {
            if (_supportedResolutions.Count == 0) return Array.Empty<string>();

            var labels = new string[_supportedResolutions.Count];
            for (var i = 0; i < _supportedResolutions.Count; i++)
            {
                labels[i] = _supportedResolutions[i].ToDisplayString();
            }
            return labels;
        }

        public static int GetSelectedResolutionIndex()
        {
            if (_supportedResolutions.Count == 0) return -1;

            for (var i = 0; i < _supportedResolutions.Count; i++)
            {
                if (_supportedResolutions[i].width == Width && _supportedResolutions[i].height == Height)
                    return i;
            }

            return FindBestMatchIndex(Width, Height);
        }

        public static void SetResolutionByIndex(int index, bool apply = true)
        {
            if (_supportedResolutions.Count == 0) return;

            index = Mathf.Clamp(index, 0, _supportedResolutions.Count - 1);
            var entry = _supportedResolutions[index];
            Width = entry.width;
            Height = entry.height;

            if (apply) Apply();
        }

        public static void SetResolution(int width, int height, bool apply = true)
        {
            Width = Mathf.Max(1, width);
            Height = Mathf.Max(1, height);

            if (_supportedResolutions.Count > 0)
            {
                var best = FindBestMatchIndex(Width, Height);
                Width = _supportedResolutions[best].width;
                Height = _supportedResolutions[best].height;
            }

            if (apply) Apply();
        }

        public static void SetFullscreen(bool fullscreen, bool apply = true)
        {
            Fullscreen = fullscreen;

            if (apply) Apply();
        }

        public static void ToggleFullscreen(bool apply = true)
        {
            SetFullscreen(!Fullscreen, apply);
        }

        public static void SetFullscreenModeWhenFullscreen(FullScreenMode fullscreenMode, bool apply = true)
        {
            FullscreenModeWhenFullscreen = fullscreenMode;

            if (apply) Apply();
        }

        public static void Apply()
        {
            var mode = Fullscreen ? FullscreenModeWhenFullscreen : FullScreenMode.Windowed;
            Screen.SetResolution(Width > 1 ? Width : 1920, Height > 1 ? Height : 1080, mode);
        }

        private static int FindBestMatchIndex(int width, int height)
        {
            if (_supportedResolutions.Count == 0) return -1;

            // Exact match first.
            for (var i = 0; i < _supportedResolutions.Count; i++)
            {
                if (_supportedResolutions[i].width == width && _supportedResolutions[i].height == height)
                    return i;
            }

            // Otherwise: closest by squared distance.
            var bestIndex = 0;
            var bestScore = long.MaxValue;
            for (var i = 0; i < _supportedResolutions.Count; i++)
            {
                var dx = (long)_supportedResolutions[i].width - width;
                var dy = (long)_supportedResolutions[i].height - height;
                var score = dx * dx + dy * dy;
                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        private static void AddUniqueResolutions(List<ResolutionEntry> dest, IReadOnlyList<ResolutionEntry> src)
        {
            var seen = new HashSet<long>();
            for (var i = 0; i < src.Count; i++)
            {
                var w = Mathf.Max(1, src[i].width);
                var h = Mathf.Max(1, src[i].height);
                var key = MakeKey(w, h);
                if (!seen.Add(key)) continue;

                dest.Add(new ResolutionEntry(w, h, src[i].label));
            }
        }

        private static void AddUniqueResolutionsFromSystem(List<ResolutionEntry> dest)
        {
            var seen = new HashSet<long>();
            var sys = Screen.resolutions;
            for (var i = 0; i < sys.Length; i++)
            {
                var w = Mathf.Max(1, sys[i].width);
                var h = Mathf.Max(1, sys[i].height);
                var key = MakeKey(w, h);
                if (!seen.Add(key)) continue;
                dest.Add(new ResolutionEntry(w, h));
            }

            // If Unity returns nothing (rare), at least include current.
            if (dest.Count == 0)
            {
                dest.Add(new ResolutionEntry(Mathf.Max(1, Screen.width), Mathf.Max(1, Screen.height)));
            }
        }

        private static long MakeKey(int width, int height) => ((long)width << 32) | (uint)height;

        private static int CompareResolutionEntry(ResolutionEntry a, ResolutionEntry b)
        {
            var w = a.width.CompareTo(b.width);
            if (w != 0) return w;
            return a.height.CompareTo(b.height);
        }
    }
}

