using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace com.SolePilgrim.DevConsole.Unity
{
	/// <summary>Unity Component to control a DevConsole.</summary>
	public class DevConsoleComponent : MonoBehaviour
	{
		/// <summary>The controlled DevConsole.</summary>
		public DevConsole DevConsole { get; private set; }
		/// <summary>If true, the Console is currently drawn on screen.</summary>
		public bool ConsoleActive { get; private set; }
		/// <summary>Callback whenever the console is toggled (in)active.</summary>
		[field: SerializeField]
		public UnityEvent<bool> OnConsoleToggled { get; private set; }
		[field: SerializeField]
		public UnityObjectInstanceMapperComponent InstanceMapper { get; private set; }

		[SerializeField, Tooltip("Keyboard key used to toggle the console.")]
		private KeyCode _toggleConsoleKey = KeyCode.Tilde;
		[SerializeField, Tooltip("TextAsset containing all the possible commands for the console.")]
		private TextAsset _consoleCommandsFile;

		/// <summary>String the Input field writes to.</summary>
		private string _inputLine;
		/// <summary>Index of the currently highlighted past entry, if any.</summary>
		private int _currentEntry = -1;

		private Rect _guiRect = new Rect(0, 0, Screen.width / 2, Screen.height / 2);
		private float _inputHeight = 25f, _resizeButtonSize = 25f;
		private Vector2 _scrollviewPosition;
		private Vector2 _mousePosDelta, _mousePosDeltaGuiSpace, _oldMousePos;
		private Vector2 _minimumConsoleSize = new Vector2(Screen.width / 2, Screen.height / 2);


		private void Awake()
		{
			var methodRegex		= new Regex(DevConsoleUtilities.CSharpMethodRegex, RegexOptions.IgnoreCase);
			var typeRegex		= new Regex($"^{DevConsoleUtilities.TypeRegex}$", RegexOptions.IgnoreCase);
			var nameRegex		= new Regex($"^{DevConsoleUtilities.NameRegex}$", RegexOptions.IgnoreCase);
			var parser = new DevConsoleParser(methodRegex, typeRegex, nameRegex, ',');
			DevConsole = new DevConsole(_consoleCommandsFile.text, parser, InstanceMapper.InstanceMapper);
			DevConsole.OnException += OnException;
		}

		private void Update()
		{
			if (Input.GetKeyDown(_toggleConsoleKey))
			{
				ToggleConsole();
			}

			if (!ConsoleActive)
			{
				return;
			}
			if (Input.GetMouseButtonUp(0)) //Click on GameObjects for selecting.
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out var hit))
				{
					var go = hit.collider.transform.root.gameObject;
					DevConsole.Entries.Add(new DevConsoleEntry($"Clicked {go}", $"InstanceID: {go.GetInstanceID()}"));
				}
			}
			_mousePosDelta			= (Vector2)Input.mousePosition - _oldMousePos;
			_mousePosDeltaGuiSpace	= new Vector2(_mousePosDelta.x, -_mousePosDelta.y);
			_oldMousePos			= Input.mousePosition;
		}

		private void OnGUI()
		{
			if (!ConsoleActive)
			{
				return;
			}
			var newRect			= GUILayout.Window(0, _guiRect, DrawConsoleWindow, "Console");
			_guiRect.position	= new Vector2(Mathf.Clamp(newRect.x, 0, Screen.width - newRect.width), Mathf.Clamp(newRect.y, 0, Screen.height - newRect.height)); //Reposition
			_guiRect.size		= new Vector2(Mathf.Max(newRect.size.x, _minimumConsoleSize.x), Mathf.Max(newRect.size.y, _minimumConsoleSize.y)); //Resize
		}

		private void DrawConsoleWindow(int windowId)
		{
			_scrollviewPosition = GUILayout.BeginScrollView(_scrollviewPosition); //Draw previous entries
			GUILayout.BeginVertical();
			for (int i = 0; i < DevConsole.Entries.Count; i++)
			{
				var entry = DevConsole.Entries[i];
				GUILayout.Label(entry.ToString());
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();

			var e = Event.current;
			if (e.type == EventType.KeyDown)
			{
				if (e.keyCode == _toggleConsoleKey) //TODO This works but feels a bit dirty. Without this, toggling the console off without unfocusing the input doesn't work. If we auto-focus the input, unfocusing becomes impossible...
				{
					ToggleConsole();
					e.Use();
				}
				else if (e.keyCode == KeyCode.Return && !string.IsNullOrEmpty(_inputLine)) //Handle command sending before any other input.
				{
					DevConsole.EnterCommand(_inputLine);
					_inputLine = string.Empty;
					_scrollviewPosition.y = Mathf.Infinity;
					e.Use();
				}
				else if (DevConsole.Entries.Count > 0) //Navigate previous entries
				{
					if (e.keyCode == KeyCode.UpArrow)
					{
						_currentEntry = _currentEntry == -1 ? DevConsole.Entries.Count - 1 : Mathf.Max(0, _currentEntry - 1);
						_inputLine = DevConsole.Entries[_currentEntry].Input;
						e.Use();
					}
					else if (e.keyCode == KeyCode.DownArrow)
					{
						_currentEntry = Mathf.Min(DevConsole.Entries.Count - 1, _currentEntry + 1);
						_inputLine = DevConsole.Entries[_currentEntry].Input;
						e.Use();
					}
				}
			}

			GUILayout.BeginHorizontal();
			GUI.SetNextControlName("ConsoleInput");
			_inputLine = GUILayout.TextField(_inputLine, GUILayout.Height(_inputHeight));
			GUI.FocusControl("ConsoleInput");
			if (GUILayout.RepeatButton("", GUILayout.Width(_resizeButtonSize), GUILayout.Height(_resizeButtonSize)))
			{
				_guiRect.size += _mousePosDeltaGuiSpace;
			}
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		private void ToggleConsole()
		{
			ConsoleActive = !ConsoleActive;
			OnConsoleToggled?.Invoke(ConsoleActive);
		}

		private void OnException(object sender, Exception e)
		{
			Debug.LogError(e, this);
		}
	}
}
