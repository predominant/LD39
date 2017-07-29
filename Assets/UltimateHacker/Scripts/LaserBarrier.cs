using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UltimateHacker
{
    public class LaserBarrier : MonoBehaviour
    {
        public bool Online;
        public GameObject LaserPrefab;

        public int Lines = 3;
        public float LineGap = 0.5f;

        private Transform _left;
        private Transform _right;
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            this._left = this.transform.Find("Left");
            this._right = this.transform.Find("Right");

            this._lineRenderer = this.GetComponent<LineRenderer>();
            this._lineRenderer.positionCount = this.Lines*2;
            Vector3[] positions = new Vector3[this._lineRenderer.positionCount];

            var y = (this.Lines - 1f)/2f*this.LineGap;

            for (var i = 0; i < this.Lines; i++)
            {
                positions[i*2 + 0] = new Vector3(
                    this._left.position.x,
                    this._left.position.y + y,
                    this._left.position.z);

                positions[i*2 + 1] = new Vector3(
                    this._right.position.x,
                    this._right.position.y + y,
                    this._right.position.z);

                y -= this.LineGap;
            }
            this._lineRenderer.SetPositions(positions);
        }

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.color = new Color(255, 0, 0, 25);
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.left, Vector3.right);
            Gizmos.DrawLine(Vector3.left + Vector3.up/2f, Vector3.right + Vector3.up/2f);
            Gizmos.DrawLine(Vector3.left + Vector3.down/2f, Vector3.right + Vector3.down/2f);

            Gizmos.matrix = oldMatrix;
        }
    }
}