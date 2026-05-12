#if UNITY_EDITOR
using Nox.CCK.Mods.Cores;
using Nox.CCK.Mods.Initializers;

namespace Nox.Controllers.Runtime {
	public class Editor : IEditorModInitializer {
		internal static IEditorModCoreAPI CoreAPI;

		public void OnInitializeEditor(IEditorModCoreAPI api) => CoreAPI = api;
		public void OnDisposeEditor()                         => CoreAPI = null;
	}
}
#endif // UNITY_EDITOR
