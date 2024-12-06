using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace XRC.Assignments.Project.G09
{
    public class SphereSelectFeedback : MonoBehaviour
    {

        [SerializeField] private SphereSelectLogic sphereLogic;

        [Header("Visual Settings")] [SerializeField]
        private Material sphereMaterial;

        [SerializeField] private Color sphereNormalColor = new Color(0, 0.5f, 1f, 0.5f);
        [SerializeField] private Color sphereHoverColor = new Color(0, 1f, 0.5f, 0.5f);
        [SerializeField] private Color objectHoverColor = new Color(0, 1f, 0.5f, 1f);
        [SerializeField] private Color objectSelectColor = new Color(0, 0.5f, 1f, 1f);

        // Store original materials and their properties
        private Dictionary<MeshRenderer, Material[]> originalMaterials = new Dictionary<MeshRenderer, Material[]>();
        private List<MeshRenderer> selectedRenderers = new List<MeshRenderer>();
        private GameObject sphereIndicator;
        private MeshRenderer sphereRenderer;

        void Awake()
        {
            CreateSphereIndicator();
        }

        void Start()
        {
        }

        private void OnEnable()
        {
            sphereLogic.OnSphereUpdateEvent += UpdateSphereIndicator;
            sphereLogic.OnHoverEnteredEvent += OnHoverEntered;
            sphereLogic.OnHoverExitedEvent += OnHoverExited;
            sphereLogic.OnSelectEnteredEvent += OnSelectEntered;
            sphereLogic.OnSelectExitedEvent += OnSelectExited;
        }

        private void OnDisable()
        {
            sphereLogic.OnSphereUpdateEvent -= UpdateSphereIndicator;
            sphereLogic.OnHoverEnteredEvent -= OnHoverEntered;
            sphereLogic.OnHoverExitedEvent -= OnHoverExited;
            sphereLogic.OnSelectEnteredEvent -= OnSelectEntered;
            sphereLogic.OnSelectExitedEvent -= OnSelectExited;
            RestoreAllMaterials();
        }

        private void UpdateSphereIndicator(Vector3 center, float radius)
        {
            sphereIndicator.transform.localPosition = center;
            sphereIndicator.transform.localScale = Vector3.one * radius * 2;
        }

        private void CreateSphereIndicator()
        {
            // Create a sphere and set it as a child of the default center
            sphereIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereIndicator.transform.parent = sphereLogic.DefaultCenter;

            // Reset position
            sphereIndicator.transform.localPosition = Vector3.zero;

            // Remove collider
            Destroy(sphereIndicator.GetComponent<Collider>());

            // Set material color
            sphereMaterial.SetColor("_BaseColor", sphereNormalColor);
            sphereRenderer = sphereIndicator.GetComponent<MeshRenderer>();
            sphereRenderer.material = sphereMaterial;
        }

        private void StoreMaterials(MeshRenderer renderer)
        {
            if (!originalMaterials.ContainsKey(renderer))
            {
                // Store a copy of all materials
                Material[] materials = renderer.materials;
                Material[] materialsCopy = new Material[materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materialsCopy[i] = new Material(materials[i]);
                }

                originalMaterials[renderer] = materialsCopy;
            }
        }

        private void RestoreMaterials(MeshRenderer renderer)
        {
            if (originalMaterials.ContainsKey(renderer))
            {
                renderer.materials = originalMaterials[renderer];
                originalMaterials.Remove(renderer);
            }
        }

        private void RestoreAllMaterials()
        {
            foreach (var renderer in originalMaterials.Keys)
            {
                if (renderer != null)
                {
                    renderer.materials = originalMaterials[renderer];
                }
            }

            originalMaterials.Clear();
            selectedRenderers.Clear();
        }

        private void ApplyColor(MeshRenderer renderer, Color color)
        {
            StoreMaterials(renderer);
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                // Create a new material instance to avoid modifying the shared material
                Material newMaterial = new Material(materials[i]);

                // Set the new color
                if (newMaterial.HasProperty("_BaseColor"))
                {
                    newMaterial.SetColor("_BaseColor", color);
                }
                else if (newMaterial.HasProperty("_Color"))
                {
                    newMaterial.SetColor("_Color", color);
                }

                materials[i] = newMaterial;
            }

            renderer.materials = materials;
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            sphereMaterial.SetColor("_BaseColor", sphereHoverColor);

            var hoverInteractable =
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable)args.interactableObject;
            if (hoverInteractable.transform
                .TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(
                    out var grabInteractable))
            {
                var interactionLayers = grabInteractable.interactionLayers.value;
                bool hasSelectLayer =
                    (interactionLayers & InteractionLayerMask.GetMask("SphereSelectVertex")) != 0 ||
                    (interactionLayers & InteractionLayerMask.GetMask("SphereSelectEdge")) != 0 ||
                    (interactionLayers & InteractionLayerMask.GetMask("SphereSelectFace")) != 0;

                if (hasSelectLayer && hoverInteractable.transform.TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    ApplyColor(meshRenderer, objectHoverColor);
                }
            }
        }

        private void OnHoverExited(bool hasHover, HoverExitEventArgs args)
        {
            if (!hasHover)
            {
                sphereMaterial.SetColor("_BaseColor", sphereNormalColor);
            }

            var hoverInteractable =
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable)args.interactableObject;
            if (hoverInteractable.transform
                .TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(
                    out var grabInteractable))
            {
                var interactionLayers = grabInteractable.interactionLayers.value;
                bool hasSelectLayer =
                    (interactionLayers & InteractionLayerMask.GetMask("SphereSelectVertex")) != 0 ||
                    (interactionLayers & InteractionLayerMask.GetMask("SphereSelectEdge")) != 0 ||
                    (interactionLayers & InteractionLayerMask.GetMask("SphereSelectFace")) != 0;

                if (hasSelectLayer && hoverInteractable.transform.TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    if (!selectedRenderers.Contains(meshRenderer))
                    {
                        RestoreMaterials(meshRenderer);
                    }
                }
            }
        }

        private void OnSelectEntered(List<MeshRenderer> selectedRenderers)
        {
            sphereIndicator.SetActive(false);
            this.selectedRenderers.Clear();

            foreach (MeshRenderer meshRenderer in selectedRenderers)
            {
                if (meshRenderer.gameObject
                    .TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(
                        out var grabInteractable))
                {
                    var interactionLayers = grabInteractable.interactionLayers.value;
                    bool hasSelectLayer =
                        (interactionLayers & InteractionLayerMask.GetMask("SphereSelectVertex")) != 0 ||
                        (interactionLayers & InteractionLayerMask.GetMask("SphereSelectEdge")) != 0 ||
                        (interactionLayers & InteractionLayerMask.GetMask("SphereSelectFace")) != 0;

                    if (hasSelectLayer)
                    {
                        this.selectedRenderers.Add(meshRenderer);
                        ApplyColor(meshRenderer, objectSelectColor);
                    }
                }
            }
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            sphereIndicator.SetActive(true);
            sphereMaterial.SetColor("_BaseColor", sphereHoverColor);

            foreach (MeshRenderer meshRenderer in selectedRenderers)
            {
                if (meshRenderer != null)
                {
                    ApplyColor(meshRenderer, objectHoverColor);
                }
            }

            selectedRenderers.Clear();
        }
    }
}