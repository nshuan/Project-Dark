using System;
using System.Collections.Generic;
using System.Linq;
using InGame;
using InGame.Pause;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dark.Scripts.RuntimeCheat.CheatLevel
{
    public class CheatEnemyPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Button btnDropEnemy;
        
        [Header("Enemy Behaviours")]
        public List<EnemyBehaviour> enemyBehaviours = new List<EnemyBehaviour>();
        
        private List<Button> createdButtons = new List<Button>();
        private bool isPanelOpen = false;
        private EnemyEntity newEnemy;
        private Camera cam;
        
        private void Awake()
        {
            if (openButton != null)
            {
                openButton.onClick.RemoveAllListeners();
                openButton.onClick.AddListener(OpenPanel);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(ClosePanel);
            }
            
            if (panel != null)
            {
                panel.SetActive(isPanelOpen);
            }
            
            cam = Camera.main;
        }
        
        private void Start()
        {
            if (Application.isPlaying)
            {
                CreateEnemyButtons();
            }
        }
        
        /// <summary>
        /// Call this method to refresh the buttons if the enemyBehaviours list is modified at runtime.
        /// </summary>
        public void RefreshButtons()
        {
            if (Application.isPlaying)
            {
                CreateEnemyButtons();
            }
        }
        
        private void OpenPanel()
        {
            isPanelOpen = true;
            if (panel != null)
            {
                panel.SetActive(true);
                
                PauseGame.Instance.Pause();
            }
        }

        private void ClosePanel()
        {
            isPanelOpen = false;
            if (panel != null)
            {
                panel.SetActive(false);
                
                PauseGame.Instance.Resume();
            }
        }
        
        private void CreateEnemyButtons()
        {
            if (buttonContainer == null)
            {
                Debug.LogError("Button Container is not assigned!");
                return;
            }
            
            if (buttonPrefab == null)
            {
                Debug.LogError("Button Prefab is not assigned! Creating default button.");
                CreateDefaultButtons();
                return;
            }
            
            // Clear existing buttons
            ClearButtons();
            
            // Create buttons for each enemy behaviour
            foreach (var enemyBehaviour in enemyBehaviours)
            {
                if (enemyBehaviour == null) continue;
                
                GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
                Button button = buttonObj.GetComponent<Button>();
                
                if (button == null)
                {
                    button = buttonObj.AddComponent<Button>();
                }
                
                // Set button text
                TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text == null)
                {
                    // Try to find Text component if TMP is not available
                    Text textComponent = buttonObj.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = GetButtonText(enemyBehaviour);
                    }
                }
                else
                {
                    text.text = GetButtonText(enemyBehaviour);
                }
                
                // Set up button click
                int enemyId = enemyBehaviour.enemyId; // Capture for closure
                EnemyBehaviour behaviour = enemyBehaviour; // Capture for closure
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnEnemyButtonClicked(behaviour, enemyId));
                
                createdButtons.Add(button);
            }
        }
        
        private void CreateDefaultButtons()
        {
            // Clear existing buttons
            ClearButtons();
            
            // Create buttons for each enemy behaviour
            foreach (var enemyBehaviour in enemyBehaviours)
            {
                if (enemyBehaviour == null) continue;
                
                GameObject buttonObj = new GameObject($"Button_{enemyBehaviour.enemyId}");
                buttonObj.transform.SetParent(buttonContainer, false);
                
                // Add RectTransform
                RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(200, 50);
                
                // Add Image
                Image image = buttonObj.AddComponent<Image>();
                image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                
                // Add Button
                Button button = buttonObj.AddComponent<Button>();
                
                // Create text child
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                
                RectTransform textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
                
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = GetButtonText(enemyBehaviour);
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.white;
                text.fontSize = 24;
                
                // Set up button click
                int enemyId = enemyBehaviour.enemyId;
                EnemyBehaviour behaviour = enemyBehaviour;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnEnemyButtonClicked(behaviour, enemyId));
                
                createdButtons.Add(button);
            }
        }
        
        private string GetButtonText(EnemyBehaviour enemyBehaviour)
        {
            if (enemyBehaviour == null) return "Unknown";
            
            string name = enemyBehaviour.name;
            if (string.IsNullOrEmpty(name))
            {
                name = $"Enemy_{enemyBehaviour.enemyId}";
            }
            
            return name;
        }
        
        private void ClearButtons()
        {
            foreach (var button in createdButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            createdButtons.Clear();
        }
        
        /// <summary>
        /// Called when an enemy button is clicked. Override this method or modify it to define custom behavior.
        /// </summary>
        /// <param name="enemyBehaviour">The EnemyBehaviour associated with the clicked button</param>
        /// <param name="enemyId">The enemy ID</param>
        protected virtual void OnEnemyButtonClicked(EnemyBehaviour enemyBehaviour, int enemyId)
        {
            // Default implementation - user can override this or modify it
            Debug.Log($"Enemy Button Clicked: {enemyBehaviour.name} (ID: {enemyId})");
            
            // User can add their custom logic here
            OnEnemyButtonClickedCustom(enemyBehaviour, enemyId);
        }
        
        /// <summary>
        /// Override this method in a derived class or modify this method to define custom behavior for each button click.
        /// </summary>
        protected virtual void OnEnemyButtonClickedCustom(EnemyBehaviour enemyBehaviour, int enemyId)
        {
            // This method is intentionally empty - user should override or modify this
            var newEnemy =  EnemyPool.Instance.Get(enemyBehaviour.enemyPrefab, enemyId, null, false);
            this.newEnemy = newEnemy;
            
            btnDropEnemy.gameObject.SetActive(true);
            btnDropEnemy.onClick.RemoveAllListeners();
            btnDropEnemy.onClick.AddListener(() =>
            {
                btnDropEnemy.gameObject.SetActive(false);
                if (newEnemy)
                {
                    var nearestTower = LevelManager.Instance.Towers[0];
                    var minDistance = Vector2.Distance(LevelManager.Instance.Towers[0].transform.position,
                        newEnemy.transform.position);
                    if (Vector2.Distance(LevelManager.Instance.Towers[1].transform.position,
                            newEnemy.transform.position) < minDistance)
                    {
                        minDistance = Vector2.Distance(LevelManager.Instance.Towers[1].transform.position,
                            newEnemy.transform.position);
                        nearestTower = LevelManager.Instance.Towers[1];
                    }
                    if (Vector2.Distance(LevelManager.Instance.Towers[2].transform.position,
                            newEnemy.transform.position) < minDistance)
                    {
                        minDistance = Vector2.Distance(LevelManager.Instance.Towers[2].transform.position,
                            newEnemy.transform.position);
                        nearestTower = LevelManager.Instance.Towers[2];
                    }
                    
                    newEnemy.Init(enemyBehaviour,
                        nearestTower,
                        1f,
                        1f,
                        1f,
                        1f,
                        1);
                    
                    newEnemy.Activate();
                }

                newEnemy = null;
            });
        }

        private void Update()
        {
            if (newEnemy)
                newEnemy.transform.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}

