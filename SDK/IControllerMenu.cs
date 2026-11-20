using Nox.UI;

namespace Nox.Controllers {
	/// <summary>
	/// A controller that drives a menu, so callers that only hold an <see cref="IController"/> - the editor
	/// tools, a widget, another mod - can open it without knowing which rig built it.
	/// <para>
	/// Kept separate from <see cref="IController"/>: a controller without UI (the desktop rig, a proxy used
	/// by an editor tool) does not implement it, so a caller tests the cast instead of handling a null menu.
	/// </para>
	/// </summary>
	public interface IControllerMenu {
		/// <summary>Menu built for this controller, or <c>null</c> while the UI mod has not built it yet.</summary>
		IMenu Menu { get; }

		/// <summary>
		/// Whether the menu is currently shown. Setting it shows the menu or hides it; a menu that is not
		/// built yet (the UI mod has not run) ignores the write.
		/// </summary>
		bool IsOpen { get; set; }
	}
}
