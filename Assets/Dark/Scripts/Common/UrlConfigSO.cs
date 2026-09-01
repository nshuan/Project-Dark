using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dark.Scripts.Common
{
    [CreateAssetMenu(fileName = "UrlConfigSO", menuName = "Another World/Url Config SO")]
    public sealed class UrlConfigSO : ScriptableObject
    {
        [SerializeField] private List<UrlEntry> listUrl;

        public string GetUrl(UrlType urlType)
        {
            if (listUrl == null) return string.Empty;
            
            return listUrl.FirstOrDefault((entry) => entry.urlType == urlType)?.url;
        }
    }

    [Serializable]
    public class UrlEntry
    {
        public UrlType urlType;
        public string url;
    }

    public enum UrlType
    {
        None,
        WishlistOtherGame,
        Discord
    }
}