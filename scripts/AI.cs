using Godot;
using System;

public partial class AI : WumController
{
	Random r;
	NavigationAgent2D nav_agent;
	bool has_target = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		r = new Random();
		Speed += r.NextSingle() * 20.0f - 10.0f;
		nav_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (!has_target)
			return ;
		
		Vector2 next = (nav_agent.GetNextPathPosition() - Transform.Origin).Normalized();
		if (next.X > 0.25f)
		{
			Move_Left = false;
			Move_Right = true;
		}
		else if (next.X < -0.25f)
		{
			Move_Left = true;
			Move_Right = false;
		}
		else if (next.Y < -0.1f && Velocity.Y >= -0.0f)
			Move_Jump = true;
		// Move_Right = r.Next() % 200 == 1 ^ Move_Right;
		// Move_Left = r.Next() % 200 == 1 ^ Move_Left;
		// Move_Jump = r.Next() % 50 == 1;
		base._PhysicsProcess(delta);
	}

	void _on_timer_timeout()
	{
		nav_agent.TargetPosition = GetNode<Node2D>("../Target").Transform.Origin;
		has_target = true;
	}
}
