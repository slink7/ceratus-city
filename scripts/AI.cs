using Godot;
using System;
using System.Linq;

public partial class AI : WumController
{
	Random r;
	AutoTilemap nav;
	bool has_target = false;

	Vector2[] path;
	int index = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		r = new Random();
		Speed += r.NextSingle() * 20.0f - 10.0f;
		nav = GetNode<AutoTilemap>("../TileMap");
		base._Ready();
	}

	bool Has_Same_Sign(float a, float b)
	{
		return Mathf.Sign(a) == Mathf.Sign(b);
	}

	bool Has_Same_Sign(Vector2 a, Vector2 b)
	{
		return a.Sign() == b.Sign();
	}

	bool Has_Reached()
	{
		Vector2 to_next = path[index] - Transform.Origin;
		float dist = to_next.Length();
		if (dist < 100.0f)
			return true;
		return false;
		if (index >= path.Length - 1)
			return true;
		Vector2 to_next_next = path[index + 1] - path[index];

		if (Has_Same_Sign(to_next, to_next_next) && dist < 100.0f)
			return true;
		if (dist < 12.0f)
			return true;
		return false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 TargetPosition = GetNode<Node2D>("../Target").Transform.Origin;
		path = nav.GetPointPath(Transform.Origin, TargetPosition);
		index = 1;
		has_target = true;

		if (!has_target)
			return ;

		if (index >= path.Length)
		{
			has_target = false;
			return ;
		}

		Vector2 next = path[index] - Transform.Origin;
		if (Has_Reached() && index < path.Length - 1)
			index++;
		next = (path[index] - Transform.Origin);

		Move_Right = next.X > 8f || (next.X > 0f && Velocity.X < 10f);
		Move_Left = next.X < -8f || (next.X < 0f && Velocity.X > -10f);
		// Move_Right = !Move_Left && (next.X > 16f || Velocity.X < 100.0f);
		// Move_Left = !Move_Right && (next.X < -16f || Velocity.X > -100.0f);
		if (next.Y < -17f && (Velocity.Y > 0.05f || IsOnFloor()))
		{
			// GD.Print("" + next + "\n" + path[index] + "\n" + Transform.Origin + "\n");
			Move_Jump = true;
		}
		// Move_Right = r.Next() % 200 == 1 ^ Move_Right;
		// Move_Left = r.Next() % 200 == 1 ^ Move_Left;
		// Move_Jump = r.Next() % 50 == 1;
		base._PhysicsProcess(delta);
	}

	void _on_timer_timeout()
	{
		
		
	}

    public override void _Process(double delta)
    {
		QueueRedraw();
        base._Process(delta);
    }

    public override void _Draw()
    {
		if (path == null || path.Length < 2)
		{
			return ;
		}
		DrawPolylineColors(path.Select(x => x - Transform.Origin).ToArray(), Enumerable.Repeat(Colors.Red, path.Length).ToArray());
		base._Draw();
    }
}
