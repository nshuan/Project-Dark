using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InGame;
using InGame.Pause;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat.CheatLevel
{
    public class CheatGatePanel : MonoBehaviour
    {
        public CheatEnemyPanel cheatEnemyPanel;
        
        [Header("UI References")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private CheatGateItem btnGatePrefab;
        [SerializeField] private Button btnCreateGate;
        [SerializeField] private GameObject blockButton;

        [Header("Gate config")] 
        [SerializeField] private GameObject panelConfig;
        [SerializeField] private Button buttonAddEnemyType;
        [SerializeField] private Transform configContainer;
        [SerializeField] private GameObject prefabItem;
        
        private void Awake()
        {
            btnCreateGate.onClick.RemoveAllListeners();
            btnCreateGate.onClick.AddListener(CreateGate);
            PauseGame.Instance.onPause += OnPause;
        }

        private void OnPause(bool isPaused)
        {
            if (isPaused)
            {
                for (int i = buttonContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(buttonContainer.GetChild(i).gameObject);
                }
                
                for (int i = configContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(configContainer.GetChild(i).gameObject);
                }
                
                buttonAddEnemyType.onClick.RemoveAllListeners();
            }
        }
        
        private void CreateGate()
        {
            var newGate = Instantiate(btnGatePrefab, buttonContainer);
            newGate.SetDragging(true);
            newGate.ActionOpenConfigBoard = () =>
            {
                ShowConfigGate(newGate);
            };
            newGate.ActionStartMove = () => blockButton.SetActive(true);
            newGate.ActionEndMove = () => blockButton.SetActive(false);
            blockButton.SetActive(true);
        }

        private void ShowConfigGate(CheatGateItem item)
        {
            panelConfig.SetActive(true);
            // panelConfig.transform.position = item.transform.position;
            if (item.info == null) item.info = new List<CheatGateConfigInfo>();
            else
            {
                // Delete all exist item
                for (int i = configContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(configContainer.GetChild(i).gameObject);
                }

                for (var i = 0; i < item.info.Count; i++)
                {
                    var a = i;
                    var info = item.info[i];
                    var newObj = Instantiate(prefabItem, configContainer);
                    var inputFields = newObj.GetComponentsInChildren<TMP_InputField>();
                    inputFields[0].text = info.enemyConfig.enemyId.ToString();
                    inputFields[1].text = info.amount.ToString();
                    inputFields[2].text = info.interval.ToString(CultureInfo.InvariantCulture);
                    inputFields[3].text = info.targetTowerId.ToString();

                    inputFields[0].onValueChanged.RemoveAllListeners();
                    inputFields[0].onValueChanged.AddListener((value =>
                    {
                        if (int.TryParse(value, out var parsedValue) &&
                            cheatEnemyPanel.enemyBehaviours.Any((enemy) => enemy.enemyId == parsedValue))
                            item.info[a].enemyConfig =
                                cheatEnemyPanel.enemyBehaviours.FirstOrDefault((enemy) => enemy.enemyId == parsedValue);
                    }));
                    
                    inputFields[1].onValueChanged.RemoveAllListeners();
                    inputFields[1].onValueChanged.AddListener((value =>
                    {
                        if (int.TryParse(value, out var parsedValue)) item.info[a].amount = parsedValue;
                    }));
                    
                    inputFields[2].onValueChanged.RemoveAllListeners();
                    inputFields[2].onValueChanged.AddListener((value =>
                    {
                        if (float.TryParse(value, out var parsedValue)) item.info[a].interval = parsedValue;
                    }));
                    
                    inputFields[3].onValueChanged.RemoveAllListeners();
                    inputFields[3].onValueChanged.AddListener((value =>
                    {
                        if (int.TryParse(value, out var parsedValue)) item.info[a].targetTowerId = parsedValue;
                    }));
                }
            }
            
            buttonAddEnemyType.onClick.RemoveAllListeners();
            buttonAddEnemyType.onClick.AddListener(() =>
            {
                var newCheatInfo = new CheatGateConfigInfo();
                var newObj = Instantiate(prefabItem, configContainer);
                var inputFields = newObj.GetComponentsInChildren<TMP_InputField>();
                inputFields[0].text = "";
                inputFields[1].text = "";
                inputFields[2].text = "";
                inputFields[3].text = "";

                inputFields[0].onValueChanged.RemoveAllListeners();
                inputFields[0].onValueChanged.AddListener((value =>
                {
                    if (int.TryParse(value, out var parsedValue)) newCheatInfo.enemyConfig =
                        cheatEnemyPanel.enemyBehaviours.FirstOrDefault((enemy) => enemy.enemyId == parsedValue);
                }));
                    
                inputFields[1].onValueChanged.RemoveAllListeners();
                inputFields[1].onValueChanged.AddListener((value =>
                {
                    if (int.TryParse(value, out var parsedValue)) newCheatInfo.amount = parsedValue;
                }));
                    
                inputFields[2].onValueChanged.RemoveAllListeners();
                inputFields[2].onValueChanged.AddListener((value =>
                {
                    if (float.TryParse(value, out var parsedValue)) newCheatInfo.interval = parsedValue;
                }));
                    
                inputFields[3].onValueChanged.RemoveAllListeners();
                inputFields[3].onValueChanged.AddListener((value =>
                {
                    if (int.TryParse(value, out var parsedValue)) newCheatInfo.targetTowerId = parsedValue;
                }));

                item.info.Add(newCheatInfo);
                
                var btnDelete = newObj.GetComponentInChildren<Button>();
                btnDelete.onClick.RemoveAllListeners();
                btnDelete.onClick.AddListener(() =>
                {
                    item.info.Remove(newCheatInfo);
                    Destroy(newObj);
                });
            });
        }
    }

    [Serializable]
    public class CheatGateConfigInfo
    {
        public EnemyBehaviour enemyConfig;
        public int amount;
        public float interval;
        public int targetTowerId;
    }
}