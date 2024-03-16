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

	//Renvoie vrai ou faux si la cible actuelle est "atteinte"
	bool Has_Reached()
	{
		Vector2 to_next = path[index] - Transform.Origin;
		float dist = to_next.Length();

		if (dist < 16.0f)
			return true;
		return false;
	}

	//Met a jour le chemin grace a un AStar2D custom
	void Update_Path()
	{
		Vector2 TargetPosition = GetNode<Node2D>("../Target").Transform.Origin;
		path = nav.GetPointPath(Transform.Origin, TargetPosition);
		index = 1;
		has_target = true;
	}

	void _on_timer_timeout()
	{
		//Met a jour le chemin a chaque frame
		Update_Path();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		

		//Guardes
		if (!has_target || index >= path.Length)
		{
			has_target = false;
			return ;
		}

		//Avancement de l'index du chemin
		if (Has_Reached() && index < path.Length - 1)
			index++;
		Vector2 next = path[index] - Transform.Origin;

		//Mets les booléens de controls pour se deplacer
		Move_Right = next.X > 8f || (next.X > 0f && Velocity.X < 5f);
		Move_Left = next.X < -8f || (next.X < 0f && Velocity.X > -5f);
		if (next.Y < -17f && (Velocity.Y > 0.05f || IsOnFloor()))
			Move_Jump = true;

		base._PhysicsProcess(delta);
	}

	//Afficher pour le debug
    public override void _Process(double delta)
    {
		QueueRedraw();
        base._Process(delta);
    }

    public override void _Draw()
    {
		// if (path == null || path.Length < 2)
		// {
		// 	return ;
		// }
		// DrawLine(Vector2.Zero, path[index] - Transform.Origin, Colors.Green, 1.0f);
		// DrawPolylineColors(path.Select(x => x - Transform.Origin).ToArray(), Enumerable.Repeat(Colors.Red, path.Length).ToArray(), 2.0f);
		// base._Draw();
    }
}
