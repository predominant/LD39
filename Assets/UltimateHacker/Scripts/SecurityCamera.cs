using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace UltimateHacker
{
    public enum SecurityCameraState
    {
        Online,
        Offline,
        Playback,
    }

    public class SecurityCamera : MonoBehaviour, ICommandTarget, IRecorder
    {
        public Color OnlineColor = Color.red;
        public Color PlaybackColor = Color.yellow;
        public Color OfflineColor = Color.black;
        public Color UnknownColor = Color.blue;

        public GameObject Indicator;

        private GameController _gameController;
        private Coroutine _routine;
        private Camera _camera;
        private SecurityCameraState _state = SecurityCameraState.Online;
        private float _recording = 0f;
        private float _pollTime = 0.5f;

        public SecurityCameraState State
        {
            get { return this._state; }
            set
            {
                this._state = value;
                this.UpdateIndicator();
                this.UpdateCoroutine();
            }
        }

        private void Awake()
        {
            this._gameController = GameObject.Find("GameController").GetComponent<GameController>();
            this._camera = this.gameObject.AddComponent<Camera>();
            // Basically don't render this camera.
            this._camera.depth = -1;
            this._camera.fieldOfView = 40f;
            this._camera.nearClipPlane = 0.1f;
            this._camera.farClipPlane = 10f;
            this._camera.aspect = 1.4f;

            this.UpdateIndicator();
            this.UpdateCoroutine();
        }

        private void UpdateCoroutine()
        {
            if (this._routine != null)
            {
                this.StopAllCoroutines();
                this._routine = null;
            }

            this.StartCoroutine(this.Scan());
        }

        private IEnumerator Scan()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(this._pollTime);

                var player = GameObject.Find("Player");
                RaycastHit hit;
                var planes = GeometryUtility.CalculateFrustumPlanes(this._camera);

                if (GeometryUtility.TestPlanesAABB(planes, player.GetComponent<Collider>().bounds))
                    this.PlayerDetected();
            }
        }

        private void PlayerDetected()
        {
            if (this.State != SecurityCameraState.Online)
                return;

            //this._gameController.
        }

        private void UpdateIndicator()
        {
            if (this.Indicator == null)
                return;

            this.Indicator.GetComponent<MeshRenderer>().material.color = this.ColorForStatus(this.State);
        }

        private Color ColorForStatus(SecurityCameraState s)
        {
            switch (s)
            {
                case SecurityCameraState.Offline:
                    return this.OfflineColor;
                case SecurityCameraState.Online:
                    return this.OnlineColor;
                case SecurityCameraState.Playback:
                    return this.PlaybackColor;
                default:
                    return this.UnknownColor;
            }
        }

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Gizmos.color = Color.white;
            Gizmos.DrawFrustum(Vector3.zero, 40f, 60f, 0.1f, 1.4f);

            Gizmos.matrix = oldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, GameObject.Find("Player").transform.position);
        }

        public List<string> CommandStatusCheck()
        {
            var status = "unknown";
            switch (this.State)
            {
                case SecurityCameraState.Offline:
                    status = "offline";
                    break;
                case SecurityCameraState.Online:
                    status = "online";
                    break;
                case SecurityCameraState.Playback:
                    status = "performing playback";
                    break;
            }

            return new List<string>()
            {
                string.Format("Current status: <color=\"orange\">{0}</color>", status)
            };
        }

        public List<string> ProcessCommand(string commandStr)
        {
            var command = commandStr.Split(' ');

            switch (command[0])
            {
                case "status":
                    return this.CommandStatusCheck();
                case "poweroff":
                    return this.CommandPoweroff();
                case "polltime":
                    if (command.Length == 2)
                    {
                        var time = 1f;
                        float.TryParse(command[1], out time);
                        return this.CommandSetPollingTime(time);
                    }
                    return new List<string>() {
                        "<color=\"red\">ERR</color>: Invalid command format. Use: polltime <seconds>"
                    };
                case "record":
                    if (command.Length == 2)
                    {
                        var time = 1f;
                        float.TryParse(command[1], out time);
                        return this.CommandRecord(time);
                    }
                    return new List<string>() {
                        "<color=\"red\">ERR</color>: Invalid command format. Use: record <seconds>"
                    };
                default:
                    return this.CommandNotFound(command[0]);
            }
        }

        public List<string> CommandNotFound(string command)
        {
            var output = new List<string>();
            output.Add(string.Format("<color=\"red\">ERR</color>: Command not found: {0}", command));
            return output;
        }

        public List<string> CommandPoweroff()
        {
            var output = new List<string>();

            switch (this.State)
            {
                case SecurityCameraState.Offline:
                    output.Add("Camera is already off");
                    break;
                case SecurityCameraState.Online:
                    output.Add("Powering off camera...");
                    this.State = SecurityCameraState.Offline;
                    output.Add("Powered off");
                    break;
                case SecurityCameraState.Playback:
                    output.Add("Cannot power off during playback");
                    break;
            }
            return output;
        }

        public List<string> CommandRestart()
        {
            var output = new List<string>();
            output.Add("Restarting device, all current data will be lost...");
            this.State = SecurityCameraState.Online;
            this._recording = 0f;
            return output;
        }

        public List<string> CommandSetPollingTime(float time)
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandRecord(float time)
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandGetRecording()
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandPutRecording(float time)
        {
            throw new System.NotImplementedException();
        }

        public List<string> CommandDeleteRecording()
        {
            throw new System.NotImplementedException();
        }
    }
}