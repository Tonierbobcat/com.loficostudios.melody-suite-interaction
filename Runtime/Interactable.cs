using System;
using UnityEngine;
using UnityEngine.Events;

namespace MelodySuite.Interaction.Runtime
{
    public class Interactable : MonoBehaviour
    {
        public Collider Collider { get; private set;}
        [SerializeField] private PostInteractionAction m_PostInteractionAction;
        [SerializeField] private UnityEvent<Transform> m_onInteract = new();

        [SerializeField] private InteractionSettings m_Settings = new();
        
        public UnityEvent<Transform> onInteract => m_onInteract;
        
        public InteractionSettings InteractionSettings => m_Settings;

        protected virtual void Awake()
        {
            if (!TryGetComponent<Collider>(out var col))
            {
                Debug.LogError($"No Interaction Collider", gameObject);
                enabled = false;
                return;
            }
            
            Collider = col;
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
        public float minInteractionDistance = 1f;
    }

    public enum InteractionType
    {
        Press,
        Hold
    }
}