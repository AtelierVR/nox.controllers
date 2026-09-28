namespace Nox.Controllers {
	/// <summary>
	/// Keys accepted by <see cref="IController.GetAbilities"/> and <see cref="IController.SetAbilities"/>.
	/// A controller only exposes the subset it supports: a key outside its
	/// <see cref="IController.GetAbilities"/> dictionary is ignored by <see cref="IController.SetAbilities"/>.
	/// </summary>
	public static class AbilitiesConstants {
		public const string WalkForce           = "walk_force";
		public const string SprintForce         = "sprint_force";
		public const string JumpForce           = "jump_force";
		public const string GravityAcceleration = "gravity_acceleration";
		public const string Friction            = "friction";
		public const string Slipperiness        = "slipperiness";
		public const string Grounded            = "grounded";
		public const string Height              = "height";
		public const string Flying              = "flying";
		public const string Immobilized         = "immobilized";
		public const string MayFly              = "may_fly";

		/// <summary><see cref="float"/> — walk speed limit, in metres per second.</summary>
		public const string MaxMoveSpeed = "max_move_speed";

		/// <summary><see cref="float"/> — acceleration used to reach <see cref="MaxMoveSpeed"/>.</summary>
		public const string MoveAcceleration = "move_acceleration";

		/// <summary><see cref="float"/> — multiplier applied to <see cref="MaxMoveSpeed"/> while sprinting.</summary>
		public const string SprintMultiplier = "sprint_multiplier";

		/// <summary><see cref="float"/> — part of the movement kept while airborne (0-1).</summary>
		public const string AirControl = "air_control";

		/// <summary><see cref="float"/> — flying speed, in metres per second.</summary>
		public const string FlySpeed = "fly_speed";

		/// <summary><see cref="bool"/> — the entity is crouching.</summary>
		public const string Crouching = "crouching";

		/// <summary><see cref="bool"/> — the entity is sprinting.</summary>
		public const string Sprinting = "sprinting";
	}
}