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

        public float CameraDistance = 10f;
        public bool DebugEnabled = false;

        private GameController _gameController;
        private Coroutine _routine;
        private Camera _camera;
        private SecurityCameraState _state = SecurityCameraState.Online;
        private float _recording = 0f;
        private float _pollTime = 0.5f;
        private PlayerController _player;

        private float PollTime
        {
            get { return this._pollTime; }
            set
            {
                this._pollTime = value;
                this.UpdateCoroutine();
            }
        }

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
            this._player = GameObject.Find("Player").GetComponent<PlayerController>();
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

            this._routine = this.StartCoroutine(this.Scan());
        }

        private IEnumerator Scan()
        {
            while (true)
            {
                if (this.State == SecurityCameraState.Offline)
                {
                    this.UpdateIndicator(this.OfflineColor);
                    if (this.DebugEnabled)
                        Debug.Log("Not ONLINE, So skipping the main camera loop");
                    // Just wait for a second, and check again for scan/online.
                    yield return new WaitForSecondsRealtime(1f);
                    continue;
                }

                // OFF / Not looking for player
                if (this.DebugEnabled)
                    Debug.Log("Just turned the cam off");
                if (this.DebugEnabled)
                    Debug.Log("Poll Time: " + this.PollTime);
                yield return new WaitForSecondsRealtime(this.PollTime / 2f);

                // ON / Looking for player
                this.UpdateIndicator(this.OnlineColor);
                if (this.DebugEnabled)
                    Debug.Log("ONLINE");
                var waitTime = 0f;
                var pollCount = 0;
                while (waitTime < this.PollTime/2f)
                {
                    var wait = 0.5f;
                    yield return new WaitForSecondsRealtime(wait);
                    waitTime += wait;
                    pollCount++;
                    this.UpdateIndicator(pollCount % 2 == 0 ? this.OfflineColor : this.OnlineColor);
                    if (this.DebugEnabled)
                        Debug.Log("Updated indicator");

                    var player = this._player.gameObject;
                    if (player.layer != 10)
                        continue;

                    var planes = GeometryUtility.CalculateFrustumPlanes(this._camera);

                    if (!GeometryUtility.TestPlanesAABB(planes, player.GetComponent<Collider>().bounds))
                        continue;

                    RaycastHit hit;
                    if (
                        Physics.Raycast(
                            new Ray(this.transform.position, player.transform.position - this.transform.position),
                            this.CameraDistance, LayerMask.GetMask(new string[] {"Player"})))
                    {
                        this.PlayerDetected();
                    }
                }
            }
        }

        private void PlayerDetected()
        {
            if (this.State != SecurityCameraState.Online)
                return;

            this._gameController.PlayerDetected(this.gameObject);
        }

        private void UpdateIndicator()
        {
            this.UpdateIndicator(this.ColorForStatus(this.State));
        }

        private void UpdateIndicator(Color c)
        {
            if (this.Indicator == null)
                return;

            this.Indicator.GetComponent<MeshRenderer>().material.color = c;
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
            var playerPos = GameObject.Find("Player").transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, playerPos);

            Gizmos.color = Color.red;
            var direction = playerPos - this.transform.position;
            Gizmos.DrawRay(this.transform.position, direction.normalized * this.CameraDistance);

            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Gizmos.color = Color.white;
            Gizmos.DrawFrustum(Vector3.zero, 40f, 60f, 0.1f, 1.4f);

            Gizmos.matrix = oldMatrix;
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
                    if (command.Length == 1)
                    {
                        return this.CommandGetPollingTime();
                    }
                    else if (command.Length == 2)
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
            this.PollTime = 0.5f;
            this._recording = 0f;
            return output;
        }

        public List<string> CommandGetPollingTime()
        {
            var output = new List<string>();
            output.Add(string.Format(
                "Current polling time is <color=\"orange\">{0:0.##}s</color>",
                this.PollTime));
            return output;
        }

        public List<string> CommandSetPollingTime(float time)
        {
            var output = new List<string>();
            this.PollTime = time;
            output.Add(string.Format("Polling time updated to <color=\"orange\">{0:0.##}s</color>", time));
            return output;
        }

        public List<string> CommandRecord(float time)
        {
            this._recording = time;
            return new List<string>()
            {
                "Recording ...",
                string.Format("Recorded the last {0}s sensor data", time)
            };
        }

        public List<string> CommandGetRecording()
        {
            if (this._recording <= 1f)
            {
                return new List<string>()
                {
                    "<color=\"red\">ERR</color>: There is no recorded data on this device."
                };
            }

            this._player.HasRecording = true;
            this._player.Recording = this._recording;

            return new List<string>()
            {
                "Downloading ...",
                ". . . . . . . .",
                string.Format(
                    "Downloaded {0}s of sensor data from {1}",
                    this._recording,
                    this.gameObject.name)
            };
        }

        public List<string> CommandPutRecording(float time)
        {
            if (!this._player.HasRecording)
            {
                return new List<string>()
                {
                    "<color=\"red\">ERR</color>: No recording is available to upload."
                };
            }

            if (this._recording > 1f)
            {
                return new List<string>()
                {
                    "<color=\"red\">ERR</color>: This devices buffer already contains a recording."
                };
            }

            this._player.HasRecording = false;
            this._recording = this._player.Recording;
            this._player.Recording = 0f;

            return new List<string>()
            {
                "Modifying identification information ...",
                "Uploading ...",
                ". . . . . . . .",
                string.Format(
                    "Uploaded {0}s of sensor data to {1}",
                    this._recording,
                    this.gameObject.name)
            };
        }

        public List<string> CommandDeleteRecording()
        {
            this._recording = 0f;
            return new List<string>()
                {
                    "Deleting sensor buffer...",
                    "All buffered sensor data has been deleted"
                };
        }
    }
}