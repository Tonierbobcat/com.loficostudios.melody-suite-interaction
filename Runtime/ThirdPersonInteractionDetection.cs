using System.Linq;
using UnityEngine;

namespace MelodySuite.Interaction.Runtime
{
    public class ThirdPersonInteractionDetection : AbstractInteractionDetection
    {
        [SerializeField] private Transform m_eyeHeight;
        [SerializeField] private Camera m_camera;
        
        [SerializeField] private bool m_requireLineOfSight;
        
        public override Interactable Best()
        {
            return Detected
                .Where(obj => obj && IsInLineOfSight(obj))
                .Where(obj =>
                    (obj.transform.position - transform.position).sqrMagnitude <=
                    obj.InteractionSettings.minInteractionDistance *
                    obj.InteractionSettings.minInteractionDistance)
                .OrderByDescending(obj =>
                    Vector3.Dot(
                        m_camera.transform.forward,
                        (obj.transform.position - m_camera.transform.position).normalized))
                .FirstOrDefault();
        }
        
        private bool IsInLineOfSight(Interactable other)
        {
            if (!m_requireLineOfSight)
                return true;
            
            var start = m_eyeHeight.position;
            
            var target = other.Collider.bounds.center;
            
            return !Physics.Linecast(start, target, out var hit, obstacleMask);
        }
        
        private void OnDrawGizmos()
        {
            
            var best = Best();
            for (var i = 0; i < Detected.Count; i++)
            {
                var col = Detected[i];
                if (!col) continue;
                Gizmos.color = col == best ? Color.green : Color.red;
                Gizmos.DrawSphere(col.transform.position, 0.1f);
            }

            if (best != null)
            {
                var start = m_eyeHeight.position;
                var target = best.GetComponent<Collider>().bounds.center;

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(start, target);
            }
        }
    }
}