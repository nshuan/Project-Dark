using System;
using NeatBullets.Core.Scripts.WeaponSystem;
using UnityEditor;
using UnityEngine;

namespace NeatBullets.Core.Scripts.CustomEditor.Editor {
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(EvoWeapon))]
    public class EvoWeaponEditor : UnityEditor.Editor {

        private TextAsset _genomeTextAsset;
        private string _fileName;
        private bool _generateUniqueName;
        private bool _selectedAsParent;

        private void OnEnable() {
            _genomeTextAsset = null;
            _fileName = "File name";
            _generateUniqueName = true;
            _selectedAsParent = false;
        }
        
        public override void OnInspectorGUI() {
            
            EvoWeapon weapon = target as EvoWeapon;
            if (weapon == null) {
                Debug.LogException(new Exception("Target is null!"));
                return;
            }
            
            EvoWeapon[] weapons = Array.ConvertAll(targets, item => (EvoWeapon)item);
            if (weapons.Length == 0) {
                Debug.LogException(new Exception("Zero targets!"));
                return;
            }
            
            base.OnInspectorGUI();
            
            GUILayout.Space(10);
            
            _selectedAsParent = weapon.GenomeStats.IsEvaluated;
            
            using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                
                GUILayout.Space(5);
                GUILayout.Label("Genome controls", EditorStyles.boldLabel);
                GUILayout.Space(3);
                
                if (!_selectedAsParent) {
                    if (GUILayout.Button("Select as parent for next gen")) {
                        foreach (EvoWeapon ew in weapons) {
                            ew.EvaluateGenome();
                            ew.GenomeStats.IsEvaluated = true;
                        }
                    }
                }
                else {
                    GUI.enabled = false;
                    GUILayout.Button("Already selected");
                    GUI.enabled = true;
                }

                GUILayout.Space(2);

                if (weapons.Length > 1) {
                    GUI.enabled = false;
                    _generateUniqueName = true;
                }
                _generateUniqueName = EditorGUILayout.ToggleLeft("Generate unique hash name", _generateUniqueName, GUILayout.ExpandWidth(false));
                GUI.enabled = true;
                
                using(new GUILayout.HorizontalScope()) {
                    if (GUILayout.Button("Save")) {
                        foreach (EvoWeapon ew in weapons) {
                            ew.SaveFunc(_generateUniqueName, _fileName);
                        }
                    }
                    if (!_generateUniqueName)
                        _fileName = EditorGUILayout.TextField(_fileName, GUILayout.MaxWidth(300));
                }
                
                GUILayout.Space(2);
                
                using(new GUILayout.HorizontalScope()) {
                    if (_genomeTextAsset == null)
                        GUI.enabled = false;
                    
                    if (GUILayout.Button("Load Genome")) {
                        foreach (EvoWeapon ew in weapons) {
                            ew.LoadFunc(_genomeTextAsset);
                        }
                    }
                    GUI.enabled = true;
                    _genomeTextAsset = EditorGUILayout.ObjectField(_genomeTextAsset, typeof(TextAsset), false, GUILayout.MaxWidth(300)) as TextAsset;
                }
                
                GUILayout.Space(10);
            }
            
            
            

        }

    }
}

