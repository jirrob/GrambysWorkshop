using Godot;
using System;

public class OrbitTarget : Spatial
{
    private const float MaxDistance = 50.0f;
    private const float MinDistance = 5.0f;
    private const float SensitivityConstant = 0.01f;
    private const float SpeedConstant = 0.001f;

    [Export]
    private float Distance
    {
        get
        {
            if (camera != null)
            {
                return camera.Translation.z;
            }
            else
            {
                return 0.0f;
            }
        }
        set
        {
            if (camera != null)
            {
                // No clamp function :(
                camera.Translation = new Vector3(0, 0, value > MaxDistance ? MaxDistance : value < MinDistance ? MinDistance : value);
            }
        }
    }
    [Export]
    private float Sensitivity = 1.0f;
    [Export]
    private float Speed = 1.0f;
    [Export]
    private bool InvertX = false;
    [Export]
    private bool InvertY = false;
    [Export]
    private bool InvertedScrolling = false;
    [Export]
    private float ScrollSpeed = 0.3f;

    public Camera camera;
    private bool OrbitingCamera = false;
    private bool MovingCamera = false;

    public override void _Ready()
    {
        camera = GetNode<Camera>(@"Camera");
        if (Distance < MinDistance)
            Distance = MinDistance;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton e:
                OrbitingCamera = Input.IsActionPressed("orbit_camera");
                MovingCamera = Input.IsActionPressed("move_camera");
                var multiplier = InvertedScrolling ? -1.0f : 1.0f;
                if (Input.IsActionJustPressed("zoom_in"))
                {
                    Distance -= multiplier * ScrollSpeed;
                }
                else if (Input.IsActionJustPressed("zoom_out"))
                {
                    Distance += multiplier * ScrollSpeed;
                }
                break;
            case InputEventMouseMotion e:
                if (OrbitingCamera)
                {
                    // moving x => rotate global y
                    var horizontalAngle = e.Relative.x * Sensitivity * SensitivityConstant * -1.0f * (InvertX ? -1.0f : 1.0f);
                    // moving y => rotate local x
                    var verticalAngle = e.Relative.y * Sensitivity * SensitivityConstant * -1.0f * (InvertY ? -1.0f : 1.0f);
                    var rightVector = new Basis(Rotation).x;
                    Rotate(rightVector, verticalAngle);
                    Rotate(Vector3.Up, horizontalAngle);
                }
                else if (MovingCamera)
                {
                    var xMovement = e.Relative.x * Speed * SpeedConstant * Distance * -1.0f;
                    var yMovement = e.Relative.y * Speed * SpeedConstant * Distance;
                    Translate(new Vector3(xMovement, yMovement, 0));
                }
                break;
        }
    }
}
