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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 TargetPosition = GetNode<Node2D>("../Player").Transform.Origin;
		path = nav.GetPointPath(Transform.Origin, TargetPosition);
		index = 1;
		
		if (index >= path.Length)
		{
			has_target = false;
			return ;
		}

		Vector2 next = path[index] - Transform.Origin;
		if (next.Length() < 100.0f && index < path.Length - 1)
			index++;
		next = (path[index] - Transform.Origin).Normalized();

		if (next.X > 0.0f)
		{
			Move_Right = next.X > 0f;
			Move_Left = next.X < 0f;
		}
		else if (next.X < 0.0f)
		{
			Move_Right = next.X > 0f;
			Move_Left = next.X < 0f;
		}
		if (next.Y < -0.5f && (Velocity.Y > 0.05f || IsOnFloor()))
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
