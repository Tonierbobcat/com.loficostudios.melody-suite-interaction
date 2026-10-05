using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MelodySuite.Interaction.Runtime
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField]
        private AbstractInteractionDetection m_detection;
        
        public Interactable Selected { get; private set; }

        public UnityEvent<Interactable> OnSelected = new();
        public UnityEvent<Interactable> OnDeselected = new();
        
        public AbstractInteractionDetection Detection => m_detection;

        [SerializeField]
        private InputActionReference m_inputAction;

        private void Awake()
        {
            if (m_inputAction != null)
            {
                m_inputAction.action.performed += Interact;
                m_inputAction.action.canceled += Interact;
            }
        }

        private void Update()
        {
            if (!m_detection)
            {
                return; 
            }

            var interactable = m_detection.Best();
            
            if (!interactable || !interactable.isActiveAndEnabled)
            {
                ClearSelection();
                return;
            }
            
            if (interactable == Selected)
                return;

            var last = Selected;
            if (last)
            {
                OnDeselected.Invoke(last);
            }
            
            Selected = interactable;
            OnSelected.Invoke(Selected);
        }

        private void ClearSelection()
        {
            if (!Selected)
                return;

            var last = Selected;
            Selected = null;

            OnDeselected.Invoke(last);
        }
        
      
        public bool isHolding { get; private set; }

        public float HoldProgress { get; private set; }

        public UnityEvent<Interactable> OnHoldStarted = new();
        public UnityEvent<Interactable> OnHoldCompleted = new();
        public UnityEvent<Interactable> OnHoldCanceled = new();
        
        private Coroutine _holdCoroutine;

        private Interactable _holdInteractable;
        
        public void Interact(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                if (_holdCoroutine != null)
                {
                    StopCoroutine(_holdCoroutine);
                    _holdCoroutine = null;
                }

                if (_holdInteractable)
                {
                    OnHoldCanceled?.Invoke(_holdInteractable);
                    _holdInteractable = null;
                }

                isHolding = false;
                HoldProgress = 0f;
                return;
            }
            
            if (!Selected || !context.performed)
                return;
            
            switch (Selected.InteractionSettings.type)
            {
                case InteractionType.Press:
                    Selected.Interact(transform);
                    break;
                case InteractionType.Hold:
                    if (_holdCoroutine != null)
                        return;

                    _holdInteractable = Selected;
                    _holdCoroutine = StartCoroutine(
                        Hold(Selected.InteractionSettings.timeToHold, _holdInteractable)
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator Hold(float duration, Interactable interactable)
        {
            isHolding = true;
            OnHoldStarted?.Invoke(interactable);

            var holdDuration = 0f;
            while (holdDuration < duration)
            {
                holdDuration += Time.deltaTime;
                HoldProgress = Mathf.Clamp01(holdDuration / duration);
                yield return null;
            }

            isHolding = false;
            HoldProgress = 1f;

            OnHoldCompleted?.Invoke(interactable);

            if (interactable)
                interactable.Interact(transform);

            _holdInteractable = null;
            _holdCoroutine = null;

            HoldProgress = 0f;
            
            if (Selected == interactable)
            {
                ClearSelection();
            }
        }

        private bool IsSelected(Interactable other)
        {
            return Selected != null && Selected.Equals(other);
        }
    }
}