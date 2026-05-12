#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Nox.CCK.Mods.Cores;
using Nox.CCK.Mods.Initializers;
using Nox.Controllers;
using Nox.Editor.Panel;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nox.Controllers.Runtime {
	public class ControllerPanel : IEditorModInitializer, Nox.Editor.Panel.IPanel {
		internal IEditorModCoreAPI      API;
		internal ControllerPanelInstance Instance;

		public void OnInitializeEditor(IEditorModCoreAPI api) => API = api;
		public void OnDisposeEditor()  { Instance?.OnDestroy(); API = null; }
		public void OnUpdateEditor()   => Instance?.OnUpdate();

		public string[] GetPath()  => new[] { "controller" };
		public string   GetLabel() => "Controller";

		public IInstance[] GetInstances()
			=> Instance != null ? new IInstance[] { Instance } : Array.Empty<IInstance>();

		public IInstance Instantiate(IWindow window, Dictionary<string, object> data)
			=> Instance = new ControllerPanelInstance(this, window);
	}

	public class ControllerPanelInstance : IInstance {
		private readonly ControllerPanel _panel;
		private readonly IWindow         _window;
		private          VisualElement   _root;
		private          DateTime        _lastUpdate    = DateTime.MinValue;
		private          IController     _lastController;

		public ControllerPanelInstance(ControllerPanel panel, IWindow window) {
			_panel  = panel;
			_window = window;
		}

		public Nox.Editor.Panel.IPanel GetPanel()  => _panel;
		public IWindow                 GetWindow() => _window;
		public string                  GetTitle()  => "Controller";
		public void                    OnDestroy() => _panel.Instance = null;

		public VisualElement GetContent() {
			if (_root != null) return _root;
			_root = _panel.API.AssetAPI.GetAsset<VisualTreeAsset>("controller-panel.uxml").CloneTree();
			_root.style.flexGrow = 1;
			UpdateControllerInfo();
			return _root;
		}

		internal void OnUpdate() {
			if (DateTime.UtcNow - _lastUpdate < TimeSpan.FromSeconds(0.5)) return;
			_lastUpdate = DateTime.UtcNow;

			var api = Main.Instance as IControllerAPI;
			var currentController = api?.Current;

			if (_lastController != currentController) {
				_lastController = currentController;
				UpdateControllerInfo();
			}
		}

		private void UpdateControllerInfo() {
			if (_root == null) return;
			var noController  = _root.Q<VisualElement>("no-controller");
			var controllerInfo = _root.Q<VisualElement>("controller-info");

			if (_lastController == null) {
				noController?.EnableInClassList("hidden", false);
				controllerInfo?.EnableInClassList("hidden", true);
				return;
			}

			noController?.EnableInClassList("hidden", true);
			controllerInfo?.EnableInClassList("hidden", false);

			var idLabel       = _root.Q<Label>("controller-id");
			var priorityLabel = _root.Q<Label>("controller-priority");
			var cameraLabel   = _root.Q<Label>("controller-camera");
			var colliderLabel = _root.Q<Label>("controller-collider");
			var positionLabel = _root.Q<Label>("controller-position");

			if (idLabel != null)       idLabel.text       = _lastController.GetId() ?? "N/A";
			if (priorityLabel != null) priorityLabel.text = _lastController.GetPriority().ToString();

			var camera = _lastController.GetCamera();
			if (cameraLabel != null)   cameraLabel.text   = camera != null ? camera.name : "None";

			var collider = _lastController.GetCollider();
			if (colliderLabel != null) colliderLabel.text = collider != null ? collider.name : "None";

			if (positionLabel != null && camera != null) {
				var pos = camera.transform.position;
				positionLabel.text = $"({pos.x:F2}, {pos.y:F2}, {pos.z:F2})";
			} else if (positionLabel != null)
				positionLabel.text = "N/A";
		}

	}
}
#endif // UNITY_EDITOR
