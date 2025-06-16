using System;
using System.Collections.Generic;
using System.Linq;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using UnityEngine;
using Random = UnityEngine.Random;


namespace NeatBullets.Core.Scripts.WeaponSystem.NEAT {
    public class EvolutionAlgorithm : MonoBehaviour
    {
        public IList<NeatGenome> GenomeList;
        
        public event Action NewGenEvent;
        public NeatGenomeDecoder Decoder;
        public CppnGenomeFactory CppnGenomeFactory;
        private NetworkActivationScheme _activationScheme;
        private IActivationFunctionLibrary _activationFunctionLib;

        [field: SerializeField] [field: HideInInspector] public int PopulationSize { private set; get; }
        [field: SerializeField] [field: HideInInspector] public uint Generation { private set; get; }
        [field: SerializeField] [field: HideInInspector] public int InputCount { private set; get; }
        [field: SerializeField] [field: HideInInspector] public int OutputCount { private set; get; }
        [field: SerializeField] [field: HideInInspector] public int CloneOffspringCount { private set; get; }
        [field: SerializeField] [field: HideInInspector] public int SexualOffspringCount { private set; get; }
    
        public static EvolutionAlgorithm Instance;
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }
        
            InitializeEvolutionAlgorithmParams();
            InitializeEvolutionAlgorithm();
        }
    
        private void InitializeEvolutionAlgorithmParams() {
            _activationScheme = NetworkActivationScheme.CreateAcyclicScheme();
            
            PopulationSize = 6;
            CloneOffspringCount = 2;
            SexualOffspringCount = PopulationSize - CloneOffspringCount;
            
            // Do not change this
            Generation = 0;
            InputCount = 3;
            OutputCount = 5;
        }
        
        private void InitializeEvolutionAlgorithm() {
            Decoder = new NeatGenomeDecoder(_activationScheme);
        
            List<ActivationFunctionInfo> fnList = new List<ActivationFunctionInfo>(10);
            fnList.Add(new ActivationFunctionInfo(0, 0.1, Linear.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(1, 0.1, Sine.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(2, 0.1, ArcTan.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(3, 0.1, BipolarGaussian.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(4, 0.1, BipolarSigmoid.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(5, 0.1, LogisticFunction.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(6, 0.1, QuadraticSigmoid.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(7, 0.1, TanH.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(8, 0.1, ArcSinH.__DefaultInstance));
            fnList.Add(new ActivationFunctionInfo(9, 0.1, ReLU.__DefaultInstance));
        
            _activationFunctionLib = new DefaultActivationFunctionLibrary(fnList);;
            NeatGenomeParameters neatGenomeParams = new NeatGenomeParameters {
                FeedforwardOnly = true,
                InitialInterconnectionsProportion = 0.8,
                DisjointExcessGenesRecombinedProbability = 0.3,
                ConnectionWeightMutationProbability = 0.8,
                AddNodeMutationProbability = 0.6,
                AddConnectionMutationProbability = 0.6,
                NodeAuxStateMutationProbability = 0.1,
                DeleteConnectionMutationProbability = 0.1
            };

            CppnGenomeFactory = new CppnGenomeFactory(InputCount, OutputCount, _activationFunctionLib, neatGenomeParams);
            GenomeList = CppnGenomeFactory.CreateGenomeList(PopulationSize, Generation);
        }

    
        public void CreateNewGeneration() {
            Debug.Log("Created new generation!");
        
            Generation++;
            List<NeatGenome> selectedGenomes = GenomeList.Where(genome => genome.EvaluationInfo.IsEvaluated).ToList();
            List<NeatGenome> rejectedGenomes = GenomeList.Where(genome => !genome.EvaluationInfo.IsEvaluated).ToList();
            
            // if no genomes were selected => select two randomly
            if (selectedGenomes.Count == 0) {
                int randInt1, randInt2;
                do {
                    randInt1 = Random.Range(0, GenomeList.Count);
                    randInt2 = Random.Range(0, GenomeList.Count);
                } while (randInt1 == randInt2);
                
                selectedGenomes.Add(GenomeList[randInt1]);
                selectedGenomes.Add(GenomeList[randInt2]);
            }
            
            int count = selectedGenomes.Count;
            // if one genome was selected => pair it up with rejected genomes for sexual reproduction
            if (count == 1) {
                for (int i = 0; i < SexualOffspringCount; i++) 
                    GenomeList[i] = rejectedGenomes[i].CreateOffspring(selectedGenomes[0], Generation);
            } 
            // if number of selected genomes is sufficient => breed them with each other
            else {
                for (int i = 0; i < SexualOffspringCount; i++) {
                    int indexA, indexB = i % count; 
                    do {
                        indexA = Random.Range(0, count);
                    } while (indexA == indexB);
                    
                    GenomeList[i] = selectedGenomes[indexA].CreateOffspring(selectedGenomes[indexB], Generation);
                }
            }
            
            // asexual reproduction is the same for both cases
            for (int i = SexualOffspringCount; i < PopulationSize; i++)
                GenomeList[i] = selectedGenomes[i % count].CreateOffspring(Generation);
            
            
            NewGenEvent?.Invoke();
        }
        
        public void CreateRandomPopulation() {
            Debug.Log("Created random population!");

            Generation = 0;
            CppnGenomeFactory.GenomeIdGenerator.Reset();
            for (int i = 0; i < PopulationSize; i++) {
                CppnGenomeFactory.InnovationIdGenerator.Reset();
                GenomeList[i] = CppnGenomeFactory.CreateGenome(Generation);
            }
            
            NewGenEvent?.Invoke();
        }
    
    
    
    
    }
}
