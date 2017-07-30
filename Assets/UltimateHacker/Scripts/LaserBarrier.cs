using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UltimateHacker.Extensions;
using UnityEngine;

namespace UltimateHacker
{
    public class LaserBarrier : MonoBehaviour, ICommandTarget
    {
        private bool _online = true;
        public bool Online
        {
            get { return this._online; }
            set
            {
                this._online = value;
                if (this._lineRenderer != null)
                {
                    this._lineRenderer.enabled = value;
                }
            }
        }
        public GameObject LaserPrefab;

        public int Lines = 3;
        public float LineGap = 0.5f;

        private Transform _left;
        private Transform _right;
        private LineRenderer _lineRenderer;
        private GameController _gameController;

        private void Awake()
        {
            this._gameController = GameObject.Find("GameController").GetComponent<GameController>();

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
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Gizmos.color = new Color(255, 0, 0, 25);
            Gizmos.DrawLine(Vector3.left, Vector3.right);
            Gizmos.DrawLine(Vector3.left + Vector3.up/2f, Vector3.right + Vector3.up/2f);
            Gizmos.DrawLine(Vector3.left + Vector3.down/2f, Vector3.right + Vector3.down/2f);

            Gizmos.matrix = oldMatrix;
        }

        private void OnDrawGizmosSelected()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Gizmos.color = new Color(255f, 0f, 0f, 0.4f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(1f, 1f, 0.2f));

            Gizmos.matrix = oldMatrix;
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!c.gameObject.IsPlayer())
                return;

            if (!this.Online)
                return;

            this._gameController.PlayerDetected(this.gameObject);
        }

        public List<string> CommandStatusCheck()
        {
            var status = this.Online ? "online" : "offline";
            return new List<string>()
            {
                string.Format("Current status: <color=\"orange\">{0}</color>", status)
            };
        }

        public List<string> ProcessCommand(string commandStr)
        {
            // TerminalInterface defines dynamic capabilities
            var termInterface = this.GetComponent<TerminalInterface>();
            var command = commandStr.Split(' ');

            switch (command[0])
            {
                case "status":
                    return this.CommandStatusCheck();
                case "poweroff":
                    return this.CommandPoweroff();
                //case "polltime":
                //    if (command.Length == 2)
                //    {
                //        var time = 1f;
                //        float.TryParse(command[1], out time);
                //        return this.CommandSetPollingTime(time);
                //    }
                //    return new List<string>() {
                //        "<color=\"red\">ERR</color>: Invalid command format. Use: polltime <seconds>"
                //    };
                default:
                    return this.CommandNotFound(command[0]);
            }
        }

        public List<string> CommandNotFound(string command)
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandPoweroff()
        {
            var output = new List<string>();

            if (this.Online)
            {
                output.Add("Laser Barrier is powering down...");
                output.Add("Laser Barrier is disabled.");
                this.Online = false;
            }
            else
            {
                output.Add("Laser Barrier is already disabled");
            }
            return output;
        }

        public List<string> CommandRestart()
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandSetPollingTime(float time)
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandGetPollingTime()
        {
            throw new System.NotImplementedException();
        }
    }
}