using System;
using SharpNeat.Decoders.Neat;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using UnityEngine;

namespace NeatBullets.Core.Scripts.WeaponSystem.NEAT {
    [Serializable]
    public class GenomeStats {
        public NeatGenome Genome;
        
        public uint Id;
        public uint BirthGeneration;
        public double ConnectionCount;
        public int NodeCount;
        [HideInInspector] public bool IsEvaluated;
        
        public NeatGenomeDecoder Decoder;
        public CppnGenomeFactory Factory;

        public IBlackBox Box;
        
        public GenomeStats(NeatGenome genome, NeatGenomeDecoder decoder, CppnGenomeFactory factory) {
            Genome = genome;
            
            Id = genome.Id;
            BirthGeneration = genome.BirthGeneration;
            ConnectionCount = genome.Complexity;
            NodeCount = genome.NodeList.Count;
            IsEvaluated = genome.EvaluationInfo.IsEvaluated;
            
            Decoder = decoder;
            Factory = factory;
            
            Box = decoder.Decode(genome);
        }

        public void UpdateGenomeStats(NeatGenome genome) {
            Genome = genome;
            
            Id = genome.Id;
            BirthGeneration = genome.BirthGeneration;
            ConnectionCount = genome.Complexity;
            NodeCount = genome.NodeList.Count;
            IsEvaluated = genome.EvaluationInfo.IsEvaluated;
            
            Box = Decoder.Decode(genome);
        }
        
        
    }
}
