using System;
using Cysharp.Threading.Tasks;
using Nox.CCK.Mods.Cores;
using Nox.CCK.Mods.Initializers;
using Nox.CCK.Utils;
using Nox.Controllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Logger = Nox.CCK.Utils.Logger;

namespace Nox.Controllers.Runtime {
	public class Main : IControllerAPI, IMainModInitializer {
		public static Main Instance { get; private set; }

		private IController     _current;
		private IMainModCoreAPI _coreAPI;
		private IController     _current1;

		public void OnInitializeMain(IMainModCoreAPI api) {
			Instance = this;
			_coreAPI = api;
			_current = null;
			CameraGuard.Install();
		}

		public async UniTask OnDisposeMainAsync() {
			await SetCurrent(null);
			CameraGuard.Uninstall();
			_coreAPI = null;
			Instance = null;
		}

		public IController Current
			=> _current;


		public UnityEvent<IController> OnCurrentChanged { get; } = new();

		private void NotifyCurrentChanged(IController controller) {
			_coreAPI?.EventAPI.Emit("controller_changed", controller);
			OnCurrentChanged?.Invoke(controller);
		}

		public async UniTask<bool> SetCurrent(IController controller) {
			if (_current == controller)
				return true;

			var canChange = true;
			_coreAPI.EventAPI.Emit("controller_request_change", controller, new Action<object[]>(OnRequest));
			if (!canChange) {
				Logger.LogWarning("Controller change request was denied");
				return false;
			}

			if (controller == null) {
				if (_current == null)
					return true;

				var oldCam = _current.GetCamera();
				CameraChop.UnregisterCamera(oldCam);
				CameraGuard.Clear();
				_current.Dispose();
				_current = null;
				NotifyCurrentChanged(null);
				return true;
			}

			if (_current != null) {
				var oldCam = _current.GetCamera();
				CameraChop.UnregisterCamera(oldCam);
				await controller.Restore(_current);
				_current.Dispose();
			}

			_current = controller;

			var cam = _current.GetCamera();
			CameraChop.RegisterCamera(cam);

			// Garde-fou caméra : seule la caméra du contrôleur courant (et celles de son rig, ex. mains)
			// peut rendre dans les yeux XR / à l'écran. Toutes les autres (monde/avatar/session) sont
			// neutralisées au runtime par CameraGuard.
			CameraGuard.Clear();
			CameraGuard.Allow(cam);

			// La caméra du contrôleur est protégée durablement : le garde-fou ne doit jamais la
			// « détaguer » ni la neutraliser, même quand un autre contrôleur devient courant, sinon le
			// retour XR/bureau casse le rendu (XRController/DesktopController).
			CameraGuard.Protect(cam);

			if (_current is Component component)
				CameraGuard.AddRoot(component.transform);

			Camera.SetupCurrent(cam);
			cam.tag = "MainCamera";
			foreach (var c in ComponentExtension.GetComponentsInChildren<Camera>())
				if (c != cam && c.CompareTag("MainCamera"))
					c.tag = "Untagged";

			var eventSystem = _current.GetEventSystem();
			EventSystem.current = eventSystem;
			foreach (var es in ComponentExtension.GetComponentsInChildren<EventSystem>())
				if (es != eventSystem)
					es.gameObject.SetActive(false);

			NotifyCurrentChanged(_current);
			return true;

			void OnRequest(object[] args) {
				if (args.Length > 0 && args[0] is false)
					canChange = false;
			}
		}
	}
}