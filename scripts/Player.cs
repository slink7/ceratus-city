using Godot;
using System;
using System.Linq;

public partial class Player : WumController
{
    bool prev = false;
	
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
        if (Input.IsKeyPressed(Key.Ctrl))
        {
            if (!prev)
            {
                PackedScene wum = GD.Load<PackedScene>("res://scenes/AI_wum.tscn");
                for (int k = 0; k < 5; k++)
                {
                    GetNode("..").AddChild(wum.Instantiate());
                }
                prev = true;
            }
        }
        else
            prev = false;
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
