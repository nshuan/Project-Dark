using System.IO;
using NeatBullets.Core.Scripts.CustomEditor.MinMaxRangeAttribute;
using NeatBullets.Core.Scripts.Interfaces;
using NeatBullets.Core.Scripts.WeaponSystem;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using FileMode = System.IO.FileMode;

namespace NeatBullets.Core.Scripts.SODefinitions {
    [CreateAssetMenu(menuName = "ScriptableObjects/WeaponParamsSO", fileName = "WP_new")]
    public class WeaponParamsSO : ScriptableObject, IWeaponParams {
        
        public TextAsset GenomeXml;
        public TextAsset WeaponParamsJson;

        [field: SerializeField] public WeaponMode WeaponMode { get; set; }
        [field: SerializeField] public BurstMode BurstMode { get; set; }
        [field: SerializeField] [field: Range(0.02f, 0.3f)] public float BurstRate { get; set; }
        
        [field: SerializeField] [field: Range(0.1f, 1f)] public float FireRate { get; set; }
        [field: SerializeField] [field: Range(1, 30)] public int ProjectilesInOneShot { get; set; }
        
        [field: SerializeField] [field: Range(-120f, 120f)] public float RotationSpeed { get; set; }
        [field: SerializeField] [field: Range(0f, 20f)] public float MoveSpeed { get; set; }
        
        [field: SerializeField] public NetworkControlMode NetworkControlMode { get; set; }
        [field: SerializeField] public ReadMode ReadMode { get; set; }
        [field: SerializeField] public Vector2 Size { get; set; }
        [field: SerializeField] public bool HasLifespan { get; set; }
        [field: SerializeField] [field: Range(2f, 10f)] public float Lifespan { get; set; }
        [field: SerializeField] public bool HasTrail { get; set; }
        
        [field: SerializeField] [field: MinMaxSlider(0f, 1f)] public Vector2 HueRange { get; set; }
        [field: SerializeField] [field: Range(0f, 1f)] public float Saturation { get; set; }
        [field: SerializeField] [field: Range(0f, 1f)] public float Brightness { get; set; }
        
        [field: SerializeField] [field: MinMaxSlider(1f, 8f)] public Vector2 SpeedRange { get; set; }
        [field: SerializeField] [field: MinMaxSlider(0.5f, 8f)] public Vector2 ForceRange { get; set; }
        [field: SerializeField] [field: Range(1f, 8f)] public float NNControlDistance { get; set; }
        [field: SerializeField] [field: Range(-1f, 1f)] public float SignX { get; set; }
        [field: SerializeField] [field: Range(-1f, 1f)] public float SignY { get; set; }
        [field: SerializeField] public bool ForwardForce { get; set; }
        
        [field: SerializeField] [field:Range(0.05f, 0.5f)] public float InitialFlightRadius { get; set; }
        [field: SerializeField] [field:Range(0.5f, 5f)] public float InitialSpeed { get; set; }
        [field: SerializeField] [field:Range(1f, 179.9f)] public float Angle { get; set; }
        
        [field: SerializeField] public bool FlipXOnReflect { get; set; }
        [field: SerializeField] public bool FlipYOnReflect { get; set; }
        [field: SerializeField] public ReflectionMode Mode { get; set; }
        
        [field: SerializeField] [field:Range(math.SQRT2, 2f)] public float ReflectiveCircleRadius { get; set; }
        [field: SerializeField] public Vector2 RectDimensions { get; set; }
        [field: SerializeField] [field: Range(5f, 179.9f)] public float MaxPolarAngleDeg { get; set; }
        
        public delegate void Event();
        public Event DestroyProjectilesEvent;
        public Event UpdateParamsEvent;
        public Event LaunchCoordinateSystemsEvent;
        
        public delegate void BoolEvent(bool value);
        public BoolEvent EnableCoroutinesEvent;
        
        // Default params
        private void InitializeParams() {
            WeaponMode = WeaponMode.MultiShot;
            BurstMode = BurstMode.Alternate;
            BurstRate = 0.05f;
            
            FireRate = 0.9f;
            ProjectilesInOneShot = 14;
            
            RotationSpeed = 0f;
            MoveSpeed = 10f;
            
            NetworkControlMode = NetworkControlMode.ForceSum;
            ReadMode = ReadMode.Default;
            Size = new Vector2(0.04f, 0.08f);
            HasLifespan = true;
            Lifespan = 8f;
            HasTrail = true;
            
            HueRange = new Vector2(0.4f, 0.8f);
            Saturation = 0.9f;
            Brightness = 0.9f;
            
            SpeedRange = new Vector2(3f, 6f);
            ForceRange = new Vector2(1f, 3f);
            NNControlDistance = 3f;
            SignX = 1f;
            SignY = 1f;
            ForwardForce = false;
            
            InitialFlightRadius = 0.1f;
            InitialSpeed = 2f;
            Angle = 60f;

            FlipXOnReflect = true;
            FlipYOnReflect = true;
            Mode = ReflectionMode.CircleReflection;
            
            ReflectiveCircleRadius = math.SQRT2;
            RectDimensions = new Vector2(1f, 1f);
            MaxPolarAngleDeg = 65f;
        }
        
        public void ResetFunc() {
            InitializeParams();
            UpdateParamsEvent?.Invoke();
        }

        private void OnValidate() {
            UpdateParamsEvent?.Invoke();
        }
        
        public void LoadParamsFromJson() {
            if (WeaponParamsJson == null){
                Debug.LogWarning("Could not load Params. Json is null.");
                return;
            }
        
            WeaponParams weaponParams = JsonUtility.FromJson(WeaponParamsJson.text, typeof(WeaponParams)) as WeaponParams;
            WeaponParams.Copy(weaponParams, this);
            UpdateParamsEvent?.Invoke();
        }

        
        
        #if UNITY_EDITOR
        public void LoadGenomeAndParamsFromFolder(bool rename = false) {
            string folderName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            string[] genomeGuid = AssetDatabase.FindAssets("Genome_ t:Object", new[] {folderName});
            string[] paramsGuid = AssetDatabase.FindAssets("Params_ t:Object", new[] {folderName});
            
            if (genomeGuid.Length != 1 || paramsGuid.Length != 1) {
                Debug.LogWarning($"Could not load. There must be exactly one genome_file and one params_file in {folderName}.");
                return;
            }
            
            GenomeXml = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(genomeGuid[0]));
            WeaponParamsJson = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(paramsGuid[0]));
            
            if (rename) {
                string fileName = "WP_" + GenomeXml.name.Split("_")[1];
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), fileName);
            }
        }
        
        
        public void UpdateJsonFileFunc() {
            
            string folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            string[] paramsGuid = AssetDatabase.FindAssets("Params_ t:Object", new[] {folderPath});
            
            if (folderPath == null) {
                Debug.LogWarning("Json was not updated. FolderPath is null!");
                return;
            }
            
            if (paramsGuid.Length != 1) {
                Debug.LogWarning($"Json was not updated. There must be exactly one Params_<UniqueHash>.json in {folderPath}.");
                return;
            }
            
            WeaponParamsJson = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(paramsGuid[0]));
            
            string fileName = "Params_" + WeaponParamsJson.name.Split("_")[1] + ".json";;
            string _savePath = Path.Combine(folderPath, fileName);
            
            WeaponParams newWPs = new WeaponParams(this);
            string newParamsJson = JsonUtility.ToJson(newWPs, true);
            
            using (FileStream fs = new FileStream(_savePath, FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(newParamsJson);
                }
            }
            
            Debug.Log($"Json was updated in {folderPath}!");
            
            
            AssetDatabase.Refresh();
        }
        #endif

        
    }
    
    
}
