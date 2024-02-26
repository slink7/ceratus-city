using Godot;
using System;
using System.Linq;

public partial class Player : WumController
{

	
    public override void _PhysicsProcess(double delta)
    {
		Move_Jump = Input.IsActionJustPressed("Move_Jump");
		Move_Left = Input.IsActionPressed("Move_Left");
		Move_Right = Input.IsActionPressed("Move_Right");
        if (Input.IsKeyPressed(Key.Alt))
        {
            Node2D n = GetNode<Node2D>("../Target");
            n.Translate(- n.Transform.Origin + Transform.Origin);
        }
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
		QueueRedraw();
        base._Process(delta);
    }

    public override void _Draw()
    {
        // AutoTilemap nav = GetNode<AutoTilemap>("../TileMap");
        // Vector2[] path = nav.GetPointPath(Transform.Origin, GetNode<Node2D>("../Target").Transform.Origin);
		// if (path == null || path.Length < 2)
		// {
		// 	return ;
		// }
		// DrawPolylineColors(path.Select(x => x - Transform.Origin).ToArray(), Enumerable.Repeat(Colors.Green, path.Length).ToArray());
		// base._Draw();
    }
}
