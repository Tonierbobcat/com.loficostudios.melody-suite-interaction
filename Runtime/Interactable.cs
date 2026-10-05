using System;
using MelodySuite.Core.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace MelodySuite.Interaction.Runtime
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] 
        private Collider collider = null;

        public Collider Collider => collider;
        
        [SerializeField] private PostInteractionAction m_PostInteractionAction;
        [SerializeField] private UEvent<Transform> m_onInteract = new();

        [SerializeField] private InteractionSettings m_Settings = new();
        
        public UEvent<Transform> onInteract => m_onInteract;
        
        public InteractionSettings InteractionSettings => m_Settings;

        protected virtual void Awake()
        {
            if (collider == null)
            {
                if (!TryGetComponent<Collider>(out var col))
                {
                    Debug.LogError($"No Interaction Collider", gameObject);
                    enabled = false;
                    return;
                }
            
                collider = col;
            }

            if (collider == null)
            {
                Debug.LogError($"No Interaction Collider", gameObject);
                enabled = false;
            }
        }

        public void Interact(Transform tf)
        {
            m_onInteract?.Invoke(tf);
            switch (m_PostInteractionAction)
            {
                case PostInteractionAction.Destroy:
                    Destroy(gameObject);
                    break;
                case PostInteractionAction.DisableInteraction:
                    enabled = false;
                    break;
                case PostInteractionAction.None:
                default:
                    break;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_Settings.minInteractionDistance);
        }
    }

    public enum PostInteractionAction
    {
        None,
        Destroy,
        DisableInteraction
    }

    [Serializable]
    public class InteractionSettings
    {
        public InteractionType type = InteractionType.Press;
        public float timeToHold;
        public string text = "Interact";
        [Min(0.1f)]
        public float minInteractionDistance = 5f;
    }

    public enum InteractionType
    {
        Press,
        Hold
    }
}