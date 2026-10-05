using System.Collections.Generic;
using UnityEngine;

namespace MelodySuite.Interaction.Runtime
{
    public interface IInteractionDetection
    {
        public List<Interactable> Detected { get; }
        public Interactable Best();
    }
    public abstract class AbstractInteractionDetection : MonoBehaviour, IInteractionDetection
    {
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private LayerMask m_obstacleMask;
        
        protected LayerMask obstacleMask => m_obstacleMask;
        protected LayerMask LayerMask => layerMask;
        public List<Interactable> Detected { get; } = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                return;
            // Debug.Log("OnTriggerEnter: " + other.gameObject.name);
            // clean up
            Detected.RemoveAll(item => item == null);
            
            if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
            {
                if (TryGetInteractable(other, out var interactable))
                {
                    Detected.Add(interactable);
                }
            }
        }

        protected bool TryGetInteractable(Collider other, out Interactable interactable)
        {
            if (other.TryGetComponent<Interactable>(out interactable))
            {
     
                return true;
            }
                
            return interactable = other.GetComponentInParent<Interactable>();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                return;
            if (TryGetInteractable(other, out var interactable))
            {
                Detected.Remove(interactable);
            }
        }

        public abstract Interactable Best();
    }
}