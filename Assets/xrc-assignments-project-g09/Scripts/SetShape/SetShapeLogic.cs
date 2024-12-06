using UnityEngine;
using System;
using System.Collections.Generic;

namespace XRC.Assignments.Project.G09
{
    namespace SetShape
    {
        [RequireComponent(typeof(SetShapeInput))]
        [RequireComponent(typeof(SetShapeUI))]
        [RequireComponent(typeof(SetShapeFeedback))]
        public class SetShapeLogic : MonoBehaviour
        {
            public enum ShapeType
            {
                Cube,
                Sphere,
                Cylinder,
                Capsule,
                Torus,
                Cone,
                Plane,
                Text
            }

            [System.Serializable]
            public class ShapeData
            {
                public ShapeType type;
                public GameObject prefab;
            }

            [SerializeField] private List<ShapeData> shapeList = new List<ShapeData>();
            private ShapeType currentShapeType = ShapeType.Cube; // Default shape

            // Event triggered when the current shape changes
            public delegate void ShapeChangedEventHandler(ShapeType newShape);

            public event ShapeChangedEventHandler OnShapeChanged;

            public ShapeType CurrentShapeType => currentShapeType;

            private void Start()
            {
                ValidateShapeList();
            }

            public GameObject GetCurrentShapePrefab()
            {
                return shapeList.Find(s => s.type == currentShapeType)?.prefab;
            }

            public void SetCurrentShape(ShapeType shapeType)
            {
                if (currentShapeType != shapeType)
                {
                    currentShapeType = shapeType;
                    OnShapeChanged?.Invoke(currentShapeType);
                    Debug.Log($"Shape changed to: {currentShapeType}");
                }
            }

            private void ValidateShapeList()
            {
                if (shapeList.Count != Enum.GetValues(typeof(ShapeType)).Length)
                {
                    Debug.LogError("Shape list is not complete!");
                    return;
                }

                foreach (var shapeData in shapeList)
                {
                    if (shapeData.prefab == null)
                    {
                        Debug.LogError($"Prefab for shape {shapeData.type} is not assigned!");
                    }
                }
            }
        }
    }
}

