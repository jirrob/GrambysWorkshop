using Godot;
using System;

public class OrbitTarget : Spatial
{
    private const float MaxDistance = 50.0f;
    private const float MinDistance = 5.0f;
    private const float SensitivityConstant = 0.01f;

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
                camera.Translation = new Vector3(0, 0, value);
            }
        }
    }
    [Export]
    private float Sensitivity = 1.0f;
    [Export]
    private bool InvertX = false;
    [Export]
    private bool InvertY = false;
    [Export]
    private bool InvertedScrolling = false;
    [Export]
    private float ScrollSpeed = 0.3f;

    public Camera camera;
    private bool MovingCamera = false;

    public override void _Ready()
    {
        camera = GetNode<Camera>(@"Camera");
        if (Distance >= 0.0)
            Distance = MinDistance;
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton e:
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
                if (!MovingCamera)
                    break;
                // moving x => rotate global y
                var horizontalAngle = e.Relative.x * Sensitivity * SensitivityConstant * -1.0f * (InvertX ? -1.0f : 1.0f);
                // moving y => rotate local x
                var verticalAngle = e.Relative.y * Sensitivity * SensitivityConstant * -1.0f * (InvertY ? -1.0f : 1.0f);
                var rightVector = new Basis(Rotation).x;
                Rotate(rightVector, verticalAngle);
                Rotate(Vector3.Up, horizontalAngle);
                break;
        }
    }
}
