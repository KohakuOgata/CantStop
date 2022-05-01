using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CantStop
{
    public class Switch : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;
        private Material RingMaterial;
        private Material CenterMaterial;
        [SerializeField, ColorUsage(true, true)]
        private Color emissionColor;
        [SerializeField]
        private float fadeTime = 0.3f;
        private Collider myCollider;
        private Animator animator;
        [SerializeField]
        public UnityEvent onPressed = new UnityEvent();
        [SerializeField]
        private UnityEvent onReleased = new UnityEvent();
        private bool isPressed = false;
        private int interactAnimationId;
        private EventTrigger eventTrigger;
        public bool active { get; private set; } = false;

        private void Awake()
        {
            RingMaterial = meshRenderer.materials[1];
            CenterMaterial = meshRenderer.materials[2];
            RingMaterial.EnableKeyword("_EMISSION");
            CenterMaterial.EnableKeyword("_EMISSION");
            myCollider = GetComponent<BoxCollider>();
            myCollider.enabled = false;
            animator = GetComponent<Animator>();
            interactAnimationId = Animator.StringToHash("Interacted");
            eventTrigger = GetComponent<EventTrigger>();
            AddListnerOnClicked((x) => InteractAnimation());
        }

        public void AddListnerOnClicked(UnityAction<BaseEventData> call)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(call);
            eventTrigger.triggers.Add(entry);
        }

        public void OnRing()
        {
            var nowR = RingMaterial.color.r;
            DOTween.To( 
                () => nowR,
                (x) => RingMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                emissionColor.r,
                fadeTime * (1.0f - nowR / emissionColor.r));
        }

        public void OffRing()
        {
            var nowR = RingMaterial.color.r;
            DOTween.To(
                () => nowR,
                (x) => RingMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                0,
                fadeTime).
                SetEase(Ease.InQuad);
        }

        public void OnCenter()
        {
            var nowR = RingMaterial.color.r;
            DOTween.To(
                () => nowR,
                (x) => CenterMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                emissionColor.r,
                fadeTime * (1.0f - nowR / emissionColor.r));
        }

        public void OffCenter()
        {
            var nowR = RingMaterial.color.r;
            DOTween.To(
                () => nowR,
                (x) => CenterMaterial.SetColor("_EmissionColor", new Color(x, x, x, 1)),
                0,
                fadeTime).
                SetEase(Ease.InQuad);
        }

        public void Activate()
        {
            OnRing();
            myCollider.enabled = true;
            active = true;
        }

        public void Deactivate()
        {
            OffRing();
            myCollider.enabled = false;
            active = false;
        }

        public void InteractAnimation()
        {
            Debug.Log("InteractAnimation Called");
            animator.SetTrigger(interactAnimationId);
            isPressed = !isPressed;
            if (isPressed)
            {
                OnCenter();
                return;
            }
            OffCenter();
        }

        public void OnPressed()
        {
            onPressed.Invoke();
        }

        public void OnReleased()
        {
            onReleased.Invoke();
        }
    }
}
