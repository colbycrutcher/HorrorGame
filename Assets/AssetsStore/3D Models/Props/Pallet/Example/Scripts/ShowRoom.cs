using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Voo.ShowRoom
{
    [System.Serializable]
    public class ObjectData
    {
        public string Name;
        public GameObject Prefab;

        [Space]

        [Range(1f, 20f)]
        public float Speed = 15f;

        [Space]

        public Vector3 CanvasPosition = new Vector3(0f, 1f, -4f);

        [Space]

        public Vector3 Rotation = Vector3.zero;
        public Vector3 Position = Vector3.zero;
        public Vector3 Scale = Vector3.one;
    }

    public class ShowRoom : MonoBehaviour
    {
        public ObjectData[] Objects;

        [Space]

        public int PreviewIndex = -1;
        public Transform RootSpawnObjects;
        public Transform PoolRoot;
        public TextMeshProUGUI CanvasLabel;
        public Transform CanvasRoot;

        [Space]

        private Transform _currentObject;
        private readonly Dictionary<int, Transform> _pool = new ();
        private int _currentIndex = -1;

        public int CurrentIndex => _currentIndex;
        public Transform CurrentObject => _currentObject;

        private void Start()
        {
            SetObject(0);
        }

#if UNITY_EDITOR

        [ContextMenu("Next")]
        public void Next()
        {
            ButtonClicked(1);
        }

        [ContextMenu("Prev")]
        public void Prev()
        {
            ButtonClicked(-1);
        }

        [ContextMenu("Preview")]
        public void Preview()
        {
            SetObject(PreviewIndex);
        }

        [ContextMenu("Recreate")]
        public void Recreate()
        {
            if (_pool.Count != 0)
            {
                foreach (var poolObject in _pool)
                    if (poolObject.Value != null)
                        GameObject.DestroyImmediate(poolObject.Value.gameObject);
                _pool.Clear();
            }

            _currentIndex = -1;
            SetObject(0);
        }

        [ContextMenu("Save Object Settings")]
        public void SaveCurrentObjectSetting()
        {
            if (_currentObject == null || _currentIndex < 0 || _currentIndex >= Objects.Length)
                return;

            Objects[_currentIndex].CanvasPosition = CanvasRoot.localPosition;
            Objects[_currentIndex].Rotation = _currentObject.localEulerAngles;
            Objects[_currentIndex].Position = _currentObject.localPosition;
            Objects[_currentIndex].Scale = _currentObject.localScale;
        }
#endif

        private void Update()
        {
            if (_currentObject != null)
                _currentObject.Rotate(0, Objects[_currentIndex].Speed * Time.unscaledDeltaTime, 0);
        }

        public void ButtonClicked(int direction)
        {
            var currentIndex = _currentIndex + direction;
            if (currentIndex >= Objects.Length)
                currentIndex = 0;
            else if (currentIndex < 0)
                currentIndex = Objects.Length - 1;
            SetObject(currentIndex);
        }

        private void SetObject(int index)
        {
            if (index == _currentIndex || index < 0)
                return;

            if (_currentObject != null)
            {
                _pool[_currentIndex] = _currentObject;
                _currentObject.SetParent(PoolRoot);
                _currentObject = null;
            }

            _currentIndex = index;
            CanvasLabel.SetText(Objects[_currentIndex].Name);
            if (_pool.ContainsKey(_currentIndex))
            {
                _currentObject = _pool[_currentIndex];
                _currentObject.SetParent(RootSpawnObjects);
                _pool.Remove(_currentIndex);
                PreepareObject();
                return;
            }

            _currentObject = GameObject.Instantiate(Objects[_currentIndex].Prefab, RootSpawnObjects).transform;
            PreepareObject();

            void PreepareObject()
            {
                _currentObject.localPosition = Objects[_currentIndex].Position;
                _currentObject.localEulerAngles = Objects[_currentIndex].Rotation;
                _currentObject.localScale = Objects[_currentIndex].Scale;
                CanvasRoot.localPosition = Objects[_currentIndex].CanvasPosition;
            }
        }
    }
}