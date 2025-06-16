using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NeatBullets.Core.Scripts.WeaponSystem.NEAT;
using SharpNeat.Genomes.Neat;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem {
    public class EvoWeapon : AbstractWeapon
    {
        
        private void Awake() {
            base.OnAwakeFunc();
        }
        
        private void OnDestroy() {
            base.OnDestroyFunc();
        }
        
        private void Start() {
            base.InitializeParams();
            if (FireCoroutine == null)
                FireCoroutine = StartCoroutine(Fire());
        }
        
        
        
        private static string GenerateHash() {
            return DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();
        }
        
        private string _savePath;
        private string _hash;
        public void SaveFunc(bool generateName, string fileName) {
            if (!Application.isPlaying) {
                Debug.Log("Play mode only");
                return;
            }
            
            XmlWriterSettings xwSettings = new XmlWriterSettings {
                Indent = true
            };
            
            _hash = GenerateHash();
            _savePath = Path.Combine(Application.dataPath, @"Resources\ProjectileGenomes\");
            _savePath += generateName ? _hash : fileName;
            Directory.CreateDirectory(_savePath);
            
            string genomeFileName = "Genome_" + (generateName ? _hash : fileName) + ".xml";
            string paramsFileName = "Params_" + (generateName ? _hash : fileName) + ".json";
            
            using(XmlWriter xw = XmlWriter.Create(Path.Combine(_savePath, genomeFileName), xwSettings)) {
                NeatGenomeXmlIO.WriteComplete(xw, GenomeStats.Genome, true);
            }
            
            string paramsJson = JsonUtility.ToJson(_weaponParamsLocal, true);
            using (FileStream fs = new FileStream(Path.Combine(_savePath, paramsFileName), FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(paramsJson);
                }
            }
            
            Debug.Log($"Genome and weapon params are saved to path {_savePath}!");
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        
        public void LoadFunc(TextAsset genomeTextAsset) {
            if (!Application.isPlaying) {
                Debug.Log("Play mode only");
                return;
            }
            
            XmlDocument genomeXml = new XmlDocument();
            genomeXml.LoadXml(genomeTextAsset.text);
            
            List<NeatGenome> genomeList = NeatGenomeXmlIO.LoadCompleteGenomeList(genomeXml, true, EvolutionAlgorithm.Instance.CppnGenomeFactory);
            
            GenomeStats.UpdateGenomeStats(genomeList[0]);
            
            Debug.Log($"{genomeTextAsset.name} is loaded");
        }
        
        
        public void EvaluateGenome() {
            if (!Application.isPlaying) {
                Debug.Log("Play mode only");
                return;
            }
            GenomeStats.Genome.EvaluationInfo.SetFitness(10);
        }
        
        
   
    }
}
