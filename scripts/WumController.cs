using Godot;

public partial class WumController : CharacterBody2D
{
	[Export] public float Speed = 150.0f;
	[Export] public float JumpVelocity = -300.0f;
	[Export] public float SpeedResponsiveness = 0.25f;
	[Export] public int MaxJumpCount = 2;

	protected bool Move_Left = false;
	protected bool Move_Right = false;
	protected bool Move_Jump = false;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private Sprite2D sprite;

	private int jump_count = 0;

	Vector2 GetLocalUp()
	{
		if (IsOnFloor())
			return GetFloorNormal();
		return Vector2.Up;
	}

    public override void _Ready()
    {
		sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 local_up = GetLocalUp();

		float fa = Mathf.Atan2(local_up.Y, local_up.X) + Mathf.Pi / 2;
		if (!IsOnFloor())
			fa = 0.0f;
		sprite.Rotation += (fa - sprite.Rotation) * 0.25f;
		
		float dx = (Move_Left ? -1 : 0) + (Move_Right ? 1 : 0);
		velocity.X = Mathf.MoveToward(Velocity.X, dx * Speed, Speed * (float)delta / SpeedResponsiveness);

		if (IsOnFloor())
			jump_count = MaxJumpCount;
		if (Move_Jump && (IsOnFloor() || jump_count-- > 1))
			velocity.Y = JumpVelocity;
		velocity.Y += gravity * (float)delta;

		Velocity = velocity;
		MoveAndSlide();
		Move_Jump = false;
	}
}
