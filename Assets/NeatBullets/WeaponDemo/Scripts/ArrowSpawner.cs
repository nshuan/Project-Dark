using System.Collections.Generic;
using UnityEngine;

namespace NeatBullets.WeaponDemo.Scripts {
    public class ArrowSpawner : MonoBehaviour
    {
        [Tooltip ("Cell dimensions in units, should be larger than arrow dimensions")]
        [SerializeField] public Vector2 CellDimensions;

        [Tooltip ("Prefab to generate grid with")]
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] public Vector2 ArrowDimensions;
        [SerializeField] private bool _showForceMagnitude = false;
        public List<GameObject> ArrowList = new List<GameObject>();
        
        [HideInInspector] [SerializeField] private Camera _camera;
        [HideInInspector] [SerializeField] private DemoWeapon _demoWeapon;
        private void Awake() {
            _camera = Camera.main;
            _demoWeapon = FindObjectOfType<DemoWeapon>();
        }
        
        
        [Header("Arrow spawn")] 
        [SerializeField] [Range(0f, 5f)] private float _xMargin;
        [SerializeField] [Range(0f, 5f)] private float _yMargin;
        public void CreateForceMap() {
            Vector2 p11 = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            Vector2 p00 = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));

            p00 += new Vector2(_xMargin, _yMargin);
            p11 -= new Vector2(_xMargin, _yMargin);

            float horizontalSpace = p11.x - p00.x;
            float verticalSpace = p11.y - p00.y;
            
            int columnsCount = Mathf.CeilToInt(horizontalSpace / CellDimensions.x);
            int rowsCount = Mathf.CeilToInt(verticalSpace / CellDimensions.y);
            
            for (int row = 0; row < rowsCount; row++) {
                for (int col = 0; col < columnsCount; col++) {
                    ForceArrow arrow = Instantiate(_arrowPrefab, _demoWeapon.ProjectileSpawnPoint.transform).GetComponent<ForceArrow>();
                    arrow.name = "Arrow" + row + col;
                    arrow.transform.position = new Vector3(
                        p00.x + col * CellDimensions.x + CellDimensions.x * 0.5f,
                        p00.y + row * CellDimensions.y + CellDimensions.y * 0.5f);
                    arrow.ArrowSpawner = this;
                    arrow.transform.localScale = ArrowDimensions;
                    arrow.ShowForceMagnitude = _showForceMagnitude;
                    ArrowList.Add(arrow.gameObject);
                }
            }
            
        }
        
        
        
        
    }
}
