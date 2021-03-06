﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateHacker.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateHacker
{
    [RequireComponent(typeof (BoxCollider))]
    public class Terminal : MonoBehaviour, IActionTarget
    {
        public GameObject TerminalUI;
        public Text TerminalText;
        public GameObject InteractionPrompt;
        public bool ControlEnabled = false;
        public float LineDelayTime = 0.2f;
        public string InputPrefix = "> ";
        public float CursorBlinkRate = 1.2f;
        public string Cursor = "_";
        public int MaxTextLines = 20;

        public List<GameObject> LinkedDevices;

        private GameObject _currentContext;

        private List<string> _outputBuffer = new List<string>();
        private string _inputBuffer = "";
        private List<string>  _output = new List<string>();
        private PlayerController PlayerController;
        private bool _doingOutput = false;

        private string _cursor = "_";

        private void Awake()
        {
            this.StartCoroutine(this.CursorBlink());
        }

        private IEnumerator CursorBlink()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(this.CursorBlinkRate);
                this._cursor = this._cursor.Equals(this.Cursor) ? "" : this.Cursor;
            }
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!c.gameObject.IsPlayer())
                return;

            this.PlayerController = c.gameObject.GetComponent<PlayerController>();
            this.PlayerController.ActionTarget = this;
            this.InteractionPrompt.SetActive(true);
        }

        private void OnTriggerExit(Collider c)
        {
            if (!c.gameObject.IsPlayer())
                return;

            this.PlayerController.ActionTarget = null;
            this.InteractionPrompt.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;

            var c = this.GetComponent<BoxCollider>();
            Gizmos.color = new Color(0, 255, 0, 255);
            Gizmos.DrawWireCube(c.center, c.size);

            Gizmos.matrix = oldMatrix;

            for (var i = 0; i < this.LinkedDevices.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(this.transform.position, this.LinkedDevices[i].transform.position);
                Gizmos.DrawWireSphere(this.LinkedDevices[i].transform.position, 0.25f);
            }
        }

        private void Update()
        {
            if (!this.ControlEnabled)
                return;

            this.ProcessControls();
            while (this._output.Count > this.MaxTextLines)
                this._output.RemoveAt(0);
            this.TerminalText.text = string.Join("\n", this._output.ToArray());
        }

        private void ProcessControls()
        {
            // Exit terminal mode
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                this.UndoAction();
                return;
            }

            if (!this._doingOutput)
            {
                if (this._outputBuffer.Count != 0)
                {
                    this._doingOutput = true;
                    this.StartCoroutine(this.ShowPendingOutput());
                    return;
                }

                // Catch all keypresses for the inputbuffer
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
                {
                    if (this._inputBuffer.Length != 0)
                    {
                        this._inputBuffer = this._inputBuffer.Substring(0, this._inputBuffer.Length - 1);
                    }
                }
                else
                {
                    var str = Input.inputString;
                    if (str.Length == 0)
                        return;

                    for (var i = 0; i < str.Length; i++)
                    {
                        if (str[i] == '\r' || str[i] == '\n')
                        {
                            var command = this._inputBuffer;
                            this._inputBuffer = "";
                            this.ProcessCommand(command);
                            return;
                        }
                        this._inputBuffer += str[i];
                    }
                }

                this._output.RemoveAt(this._output.Count - 1);
                var inputLine = this.InputPrefix + this._inputBuffer + this._cursor;
                this._output.Add(inputLine);
            }
        }

        private void ProcessCommand(string commandStr)
        {
            var command = commandStr.Trim().ToLower();
            if (command == "")
            {
                this.EndOutput();
                return;
            }

            // Check for device names
            foreach (var device in this.LinkedDevices)
                if (command == device.name.ToLower())
                {
                    this.CommandContext(device);
                    return;
                }

            var output = new List<string>();

            // Check for valid commands
            switch (command)
            {
                case "functions":
                    this.CommandFunctions();
                    break;
                case "help":
                    this.CommandHelp();
                    break;
                case "links":
                    this.CommandLinks();
                    break;
                case "clear":
                    this.CommandClear();
                    break;
                case "shutdown":
                case "exit":
                    this.CommandShutdown();
                    break;
                case "return":
                    this.CommandReturn();
                    break;
                default:
                    //if (!this._currentContext.GetComponent<ICommandTarget>())

                    try
                    {
                        var x = this._currentContext.GetComponent<ICommandTarget>();
                        output = x.ProcessCommand(command);
                    }
                    catch (Exception)
                    {
                        this.CommandNotFound(command);
                        throw;
                    }
                    break;
            }

            foreach (var line in output)
                this._outputBuffer.Add(line);
            this.EndOutput();
        }

#region Commands

        private void CommandNotFound(string command)
        {
            this._outputBuffer.Add(string.Format("<color=\"red\">ERR</color>: Command not found: {0}", command));
            this.EndOutput();
        }

        private void CommandFunctions()
        {
            if (this._currentContext == null)
            {
                this._outputBuffer.Add("<color=\"red\">ERR</color>: You have no current device context.");
                this.EndOutput();
                return;
            }

            try
            {
                var output = this._currentContext.GetComponent<TerminalInterface>().CommandFunctions();
                foreach (var str in output)
                    this._outputBuffer.Add(str);
            }
            catch (Exception e)
            {
                this._outputBuffer.Add("<color=\"red\">ERR</color>: TerminalInterface Adaptor is malfunctioning");
            }
            this.EndOutput();
        }

        private void CommandContext(GameObject device)
        {
            this._outputBuffer.Add(string.Format("Changing context to: <color=\"orange\">{0}</color>", device.name));
            this._currentContext = device;
            this.EndOutput();
        }

        private void CommandReturn()
        {
            if (this._currentContext == null)
            {
                this._outputBuffer.Add("<color=\"red\">ERR</color>: No context to return from.");
                this.EndOutput();
                return;
            }

            this._outputBuffer.Add(string.Format(
                "Exited the <color=\"orange\">{0}</color> context.",
                this._currentContext.name));
            this.EndOutput();
            this._currentContext = null;
        }

        private void CommandHelp()
        {
            this._outputBuffer.Add("Commands:");
            this._outputBuffer.Add("  functions - Show a list of available functions");
            //this._outputBuffer.Add("  script    - Open a script editor");
            this._outputBuffer.Add("  links     - Show a list of linked devices");
            this._outputBuffer.Add("  return    - Exit a device context");
            this._outputBuffer.Add("  clear     - Clear the screen");
            this._outputBuffer.Add("  shutdown  - Shutdown the terminal");
            this._outputBuffer.Add("");
            this._outputBuffer.Add("Entering the name of a device will switch the terminal context to that device and allow some remote interactions with the security device.");
            this.EndOutput();
        }

        private void CommandLinks()
        {
            if (this._currentContext != null)
            {
                this._outputBuffer.Add(string.Format(
                    "Device links cannot be used while in context <color=\"orange\">{0}</color>",
                    this._currentContext.name));
                this.EndOutput();
                return;
            }

            if (this.LinkedDevices == null || this.LinkedDevices.Count == 0)
            {
                this._outputBuffer.Add("0 Device links detected.");
                this.EndOutput();
                return;
            }

            if (this.LinkedDevices.Count == 1)
                this._outputBuffer.Add("1 Device linked");
            else
                this._outputBuffer.Add(this.LinkedDevices.Count + " Devices linked");

            foreach (var link in this.LinkedDevices)
                this._outputBuffer.Add(string.Format("  - Device '{0}'", link.name));
            this.EndOutput();
        }

        private void CommandClear()
        {
            this._output.Clear();
            this.EndOutput();
        }

        private void CommandShutdown()
        {
            this.UndoAction();
        }

#endregion

        private IEnumerator ShowPendingOutput()
        {
            while (this._outputBuffer.Count > 0)
            {
                this._output.Add(this._outputBuffer.First());
                this._outputBuffer.RemoveAt(0);
                yield return new WaitForSecondsRealtime(this.LineDelayTime);
            }
            this._doingOutput = false;
        }

        private void EndOutput()
        {
            this._outputBuffer.Add("");
            this._outputBuffer.Add(this.InputPrefix);
        }

        public void DoAction()
        {
            this.Reset();
            this.PlayerController.ControlEnabled = false;
            this.TerminalUI.SetActive(true);
            this.ControlEnabled = true;
        }

        // Not really "undo", but more "stop" doing the action.
        public void UndoAction()
        {
            this.ControlEnabled = false;
            this.TerminalUI.SetActive(false);
            this.PlayerController.ControlEnabled = true;
            this._currentContext = null;
        }

        private void Reset()
        {
            this.TerminalText.text = "";

            this._inputBuffer = "";
            this._outputBuffer = new List<string>();
            this._output = new List<string>();

            this._outputBuffer.Add("Powering up...");
            this._outputBuffer.Add("Accessing system...");
            this._outputBuffer.Add("Registering device links...");
            this._outputBuffer.Add("..................................");
            this._outputBuffer.Add("Ready!");
            this._outputBuffer.Add("");
            this._outputBuffer.Add("Type <color=\"orange\">help</color> to get assistance");
            this.EndOutput();
        }
    }
}