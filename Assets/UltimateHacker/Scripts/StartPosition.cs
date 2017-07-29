using UnityEngine;

namespace UltimateHacker
{
    public class StartPosition : MonoBehaviour
    {
        public Color GizmoColor;
        public float Size = 2f;

        public void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.color = this.GizmoColor;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, this.Size);

            Gizmos.matrix = oldMatrix;
        }
    }
}