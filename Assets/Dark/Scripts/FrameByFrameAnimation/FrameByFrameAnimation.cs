using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.FrameByFrameAnimation
{
    [CreateAssetMenu(menuName = "Dark/Frame By Frame Animation", fileName = "Anim")]   
    public class FrameByFrameAnimation : ScriptableObject
    {
        public Sprite[] frames;
        
        /// <summary>
        /// Get sliced sprites from a given texture asset by index range.
        /// Works only in the editor.
        /// startIndex - included
        /// endIndex - included
        /// </summary>
        [Button]
        public void GetSprites(Texture2D texture, int startIndex, int number)
        {
            if (texture == null)
            {
                return;
            }

            if (startIndex < 0)
            {
                DebugUtility.LogWarning("Start index should not be less than zero!");
            }

            if (number < 0)
            {
                DebugUtility.LogWarning("Number of sprites cannot be less than zo!");
                return;
            }
            
            // Get asset path from texture reference
            var path = AssetDatabase.GetAssetPath(texture);

            // Load all sub-assets (sprites) from that path
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            var sprites = new List<Sprite>();
            foreach (var obj in subAssets)
            {
                if (obj is Sprite s)
                    sprites.Add(s);
            }

            // Sort by name if needed (Unity's slicing usually names them sequentially)
            sprites.Sort((a, b) =>
            {
                int na = ExtractNumber(a.name);
                int nb = ExtractNumber(b.name);
                int cmp = na.CompareTo(nb);
                return cmp != 0 ? cmp : string.Compare(a.name, b.name, System.StringComparison.Ordinal);
            });
            
            if (startIndex > sprites.Count)
            {
                DebugUtility.LogWarning("Start index cannot be greater than the number of sprites!");
                return;
            }
            
            if (startIndex + number - 1 > sprites.Count)
            {
                DebugUtility.LogWarning("End index should not be greater than the number of sprites!");
            }
                
            // Collect range
            frames = new Sprite[number];
            for (int i = startIndex; i <= startIndex + number - 1 && i < sprites.Count; i++)
            {
                if (i >= 0) 
                    frames[i - startIndex] = sprites[i];
            }
        }
        
        private int ExtractNumber(string text)
        {
            // find the last group of digits in the name
            var m = Regex.Match(text, @"(\d+)$");
            if (m.Success)
            {
                if (int.TryParse(m.Groups[1].Value, out int num))
                    return num;
            }
            return 0;
        }
    }
}