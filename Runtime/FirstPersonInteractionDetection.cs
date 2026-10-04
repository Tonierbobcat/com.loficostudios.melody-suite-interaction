using System.Linq;
using UnityEngine;

namespace InteractionSystem.Runtime
{
    public class FirstPersonInteractionDetection : AbstractInteractionDetection
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private bool m_requireLineOfSight;

        public override Interactable Best()
        {
            // return null;
            var ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            if (Physics.Raycast(ray, out var hitInfo, 100, LayerMask))
            {
                if (hitInfo.collider)
                { 
                    if (hitInfo.collider.GetComponent<Collider>().TryGetComponent<Interactable>(out var obj))
                    {
                        if ((obj.transform.position - transform.position).sqrMagnitude <=
                            obj.InteractionSettings.minInteractionDistance *
                            obj.InteractionSettings.minInteractionDistance)
                        {
                            return obj;
                        }
                    }
                }
            }
            
            return null;
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
                var start = m_camera.transform.position;
                var target = best.GetComponent<Collider>().bounds.center;

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(start, target);
            }
        }
    }
}