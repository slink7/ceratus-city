using Godot;
using System;

public partial class Player : WumController
{

	
    public override void _PhysicsProcess(double delta)
    {
		Move_Jump = Input.IsActionJustPressed("Move_Jump");
		Move_Left = Input.IsActionPressed("Move_Left");
		Move_Right = Input.IsActionPressed("Move_Right");
        base._PhysicsProcess(delta);
    }
}
