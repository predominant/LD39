using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateHacker
{
    public class SecurityCamera : MonoBehaviour
    {
        public Color OnlineColor = Color.red;
        public Color PlaybackColor = Color.yellow;
        public Color OfflineColor = Color.black;

        public GameObject Indicator;

        private void Awake()
        {
            if (this.Indicator == null)
                return;

            this.Indicator.GetComponent<MeshRenderer>().material.color = this.OnlineColor;
        }

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, 40f, 60f, 0.1f, 1.4f);

            Gizmos.matrix = oldMatrix;
        }
    }
}