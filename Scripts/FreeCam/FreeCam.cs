using Godot;
using System;

public class FreeCam : Spatial
{
    private const float SensitivityConstant = 0.01f;

    [Export]
    private float Sensitivity = 1.0f;

    [Export]
    private float Speed = 7.0f;

    [Export]
    private bool InvertX = false;

    [Export]
    private bool InvertY = false;

    public Camera camera;

    private Vector2 InputVector;

    public override void _Ready()
    {
        camera = GetNode<Camera>(@"Camera");
    }

    public override void _Process(float delta)
    {
        // Input
        // TODO: HOW DO I GET THE CURRENTLY FOCUSED CONTROL???
        float xMovement = (Input.IsActionPressed("move_camera_right") ? 1.0f : 0.0f) +
            (Input.IsActionPressed("move_camera_left") ? -1.0f : 0.0f);
        float zMovement = (Input.IsActionPressed("move_camera_forward") ? -1.0f : 0.0f) +
            (Input.IsActionPressed("move_camera_backward") ? 1.0f : 0.0f);
        InputVector = new Vector2(xMovement, zMovement);
        InputVector = InputVector.Normalized();
        if (!Input.IsActionPressed("rotate_camera"))
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }

        // Movement
        var movementVector = new Vector3(InputVector.x, 0.0f, InputVector.y);
        movementVector = Transform.basis.Xform(movementVector);
        movementVector *= Speed * delta;
        GlobalTranslate(movementVector);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion e)
        {
            if (Input.IsActionPressed("rotate_camera"))
            {
                // moving x => rotate global y
                var horizontalAngle = e.Relative.x * Sensitivity * SensitivityConstant * -1.0f * (InvertX ? -1.0f : 1.0f);
                // moving y => rotate local x
                var verticalAngle = e.Relative.y * Sensitivity * SensitivityConstant * -1.0f * (InvertY ? -1.0f : 1.0f);
                var rightVector = new Basis(Rotation).x;
                Rotate(rightVector, verticalAngle);
                Rotate(Vector3.Up, horizontalAngle);
                if (horizontalAngle != 0 || verticalAngle != 0)
                {
                    Input.SetMouseMode(Input.MouseMode.Captured);
                }
            }
        }
    }
}
