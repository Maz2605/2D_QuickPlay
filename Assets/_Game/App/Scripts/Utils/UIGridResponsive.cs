using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.App.Scripts.Utils
{
    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIGridResponsive : MonoBehaviour
    {
        [Header("Settings")]
        [Min(1)] public int columnCount = 2;
        [Range(0.5f, 2f)] public float aspectRatio = 0.8f;
        public bool includePadding = true;

        private GridLayoutGroup _grid;
        private RectTransform _rect;
    
        private bool _isDirty = false;

        private void Awake()
        {
            RefreshReferences();
        }

        private void OnEnable()
        {
            UpdateCellSizeImmediate();
#if UNITY_EDITOR
            EditorApplication.update += OnEditorUpdate;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= OnEditorUpdate;
#endif
        }

        private void OnValidate()
        {
            _isDirty = true;
        }

        private void OnRectTransformDimensionsChange()
        {
            _isDirty = true;
        }

        private void OnEditorUpdate()
        {
            if (_isDirty)
            {
                _isDirty = false;
                UpdateCellSizeImmediate();
            }
        }

        private void Update()
        {
            if (Application.isPlaying && _isDirty)
            {
                _isDirty = false;
                UpdateCellSizeImmediate();
            }
        }

        [ContextMenu("Force Update Now")]
        public void UpdateCellSizeImmediate()
        {
            if (_grid == null || _rect == null) RefreshReferences();
            if (_grid == null || _rect == null) return;
        
            if (_rect.rect.width <= 0) return;

            int safeColumns = Mathf.Max(1, columnCount);
            float safeRatio = Mathf.Max(0.1f, aspectRatio);
            float contentWidth = _rect.rect.width;
            float spaceAvailable = contentWidth;

            if (includePadding)
            {
                spaceAvailable -= (_grid.padding.left + _grid.padding.right);
            }

            float totalSpacing = _grid.spacing.x * (safeColumns - 1);
            spaceAvailable -= totalSpacing;

            if (spaceAvailable <= 10) return;

            float cellWidth = spaceAvailable / safeColumns;
            float cellHeight = cellWidth / safeRatio;
            Vector2 newSize = new Vector2(cellWidth, cellHeight);

            if (_grid.cellSize != newSize)
            {
                _grid.cellSize = newSize;
            }
        }

        private void RefreshReferences()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _rect = GetComponent<RectTransform>();
        }
    }
}