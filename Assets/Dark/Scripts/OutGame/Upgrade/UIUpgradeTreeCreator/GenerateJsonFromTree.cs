using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade.UIUpgradeTreeCreator
{
    public class GenerateJsonFromTree : MonoBehaviour
    {
        public void SaveJson(string treeName, TreeDataStruct treeData)
        {
            var path = Application.dataPath + "/Dark/JSON/" + treeName + ".json";
            string json = JsonConvert.SerializeObject(treeData, Formatting.Indented);
            File.WriteAllText(path, json);
            Debug.Log("Saved to: " + path);
        }

        public TreeDataStruct LoadJson(string treeName)
        {
            var path = Application.dataPath + "/Dark/JSON/" + treeName + ".json";
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<TreeDataStruct>(json);
        }

        public bool Exist(string treeName)
        {
            var path = Application.dataPath + "/Dark/JSON/" + treeName + ".json";
            return File.Exists(path);
        }
    }
}