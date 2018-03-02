﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace TrilleonAutomation {

	public class CommandConsoleView : SwatWindow {

		GUIStyle args, console, description, editorName, load, open, variables;
		List<string> VariableInputFields = new List<string>();
		List<Command> Commands = new List<Command>();
		string search = string.Empty;
		bool showCommandLog = false;
		Vector2 _scroll = new Vector2();
		//TODO: Fix Vector2 _scrollConsole = new Vector2();

		public override void Set() { 
		
			VariableInputFields = new List<string>().OfSpecificValues(10, string.Empty);
			Commands = ConsoleCommandsBase.RegisteredCommands;

		}

		public override void OnTabSelected() { }

		public override bool UpdateWhenNotInFocus() {

			return false;

		}

		public override void Render() {

			console = new GUIStyle(GUI.skin.box);
			console.normal.background = Swat.MakeTexture(1, 1, (Color)new Color32(100, 100, 100, 255));
			console.wordWrap = true;
			console.normal.textColor = Color.white;
			console.padding = new RectOffset(10, 20, 0, 0);
			console.fontSize = 15;
			console.alignment = TextAnchor.MiddleLeft;

			description = new GUIStyle(GUI.skin.label);
			description.wordWrap = true;
			description.margin = new RectOffset(10, 10, 0, 0);
			description.padding = new RectOffset(0, 0, 5, 5);
			description.normal.textColor = Swat.WindowDefaultTextColor;
			description.fontSize = 12;

			args = new GUIStyle(GUI.skin.label);
			args.wordWrap = true;
			args.margin = new RectOffset(10, 10, 0, 0);
			args.normal.textColor = Swat.WindowDefaultTextColor;
			args.fontSize = 11;

			editorName = new GUIStyle(GUI.skin.label);
			editorName.fontSize = 16;
			editorName.fixedHeight = 20;
			editorName.fontStyle = FontStyle.Bold;
			editorName.padding = new RectOffset(8, 0, 0, 0);
			editorName.normal.textColor = Swat.WindowDefaultTextColor;

			load = new GUIStyle(GUI.skin.box);
			load.normal.background = Swat.MakeTexture(1, 1, (Color)new Color32(75, 75, 75, 255));
			load.normal.textColor = Color.white;
			load.fontSize = 18;
			load.alignment = TextAnchor.MiddleCenter;

			open = new GUIStyle(GUI.skin.button);
			open.fontSize = 14;
			open.fixedHeight = 28;
			open.fixedWidth = 100;
			open.margin = new RectOffset(10, 10, 0, 0);
			open.normal.textColor = Swat.WindowDefaultTextColor;
			open.normal.background = open.active.background = open.focused.background = Swat.ToggleButtonBackgroundSelectedTexture;

			variables = new GUIStyle(GUI.skin.textField);
			variables.fontSize = 12;
			variables.margin = new RectOffset(10, 10, 0, 0);
			variables.normal.textColor = Swat.WindowDefaultTextColor;

			_scroll = GUILayout.BeginScrollView(_scroll);

			GUILayout.Space(!Application.isPlaying ? 15 : 45);
			EditorGUILayout.LabelField("Filter (Search for command name or description/purpose matching)", description);
			search = EditorGUILayout.TextField(search, variables, new GUILayoutOption[] { GUILayout.MaxWidth(400) });
			GUILayout.Space(25);

			for(int x = 0; x < Commands.Count; x++) {

				Command command = Commands[x];
				string longestAlias = string.Empty;
				bool filterActive = !string.IsNullOrEmpty(search.Trim());
				bool filterMatched = false;
				for(int l = 0; l < command.Aliases.Count; l++) {

					if(filterActive && (command.Aliases[l].ToLower().Contains(search) || command.Purpose.ToLower().Contains(search))) {

						filterMatched = true;

					}
					longestAlias = command.Aliases[l].Length > longestAlias.Length ? command.Aliases[l] : longestAlias;

				}

				if(filterActive && !filterMatched) {

					continue; //Don't render this information; no match found.

				}

				EditorGUILayout.LabelField(longestAlias, editorName);
				GUILayout.Space(4);
				EditorGUILayout.LabelField(command.Purpose, description);
				GUILayout.Space(6);

				if(command.Args.Count > 0) {

					for(int a = 0; a < command.Args.Count; a++) {

						EditorGUILayout.LabelField(string.Format("(Arg {0}) <{1}> {2}", a, command.Args[a].Key, command.Args[a].Value), description);
						GUILayout.Space(2);

					}

					GUILayout.Space(4);
					VariableInputFields[x] = EditorGUILayout.TextField(VariableInputFields[x], variables, new GUILayoutOption[] { GUILayout.MaxWidth(400) });
					GUILayout.Space(8);

				}
				if(GUILayout.Button("Launch", open)) {

					if(!EditorApplication.isPlaying) {

						SimpleAlert.Pop("Console commands can only be executed when Play Mode is active.", null);

					} else {
						
						ConsoleCommands.SendCommand(string.Format("{0} {1}", longestAlias, VariableInputFields[x]));
						showCommandLog = true;

					}

				}
				GUILayout.Space(20);

			}
			GUILayout.Space(40);
			GUILayout.EndScrollView();

			string arrow = showCommandLog ? "▲" : "▼";
			if(Application.isPlaying) {

				load.padding = showCommandLog ? new RectOffset(0, 0, -8, 0) : new RectOffset(0, 0, 0, 0);
				load.fontSize = 15;
				if(GUI.Button(new Rect(0, 24, Nexus.Self.position.width, 30), string.Format("{0}    Show Command Log    {0}", arrow), load)) {

					showCommandLog = !showCommandLog;

				}
				if(showCommandLog) {

					//_scrollConsole = GUI.BeginScrollView(new Rect(0, 30, Nexus.Self.position.width, 400), _scrollConsole, new Rect(0, 30, Nexus.Self.position.width, 400));
					GUI.TextArea(new Rect(0, 50, Nexus.Self.position.width, 400), ConsoleCommands.ConsoleLog.text, console);
					//GUILayout.EndScrollView();

				}

			}

		}

	}

}