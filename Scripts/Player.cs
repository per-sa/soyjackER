using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public float speed = 0.0f;
	public const float sprintSpeed = 10.0f;
	public const float walkSpeed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float SENSITIVITY = 0.005f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = 9.8f;

	// bob Variables
	public const float bobFreq = 2.0f;
	public const float bobAmp = 0.08f;
	public float tBob = 0.0f;

	// FOV Variables
	public const float baseFov = 75f;
	public const float fovChange = 1.5f;


	public override void _Ready()
	{
		var somet = GetNode<Node3D>("Head");
		GetNode<Node3D>("Head/Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			GetNode<Node3D>("Head").RotateY(-mouseMotion.Relative.X * SENSITIVITY);
			GetNode<Node3D>("Head/Camera3D").RotateX(-mouseMotion.Relative.Y * SENSITIVITY);
			GetNode<Node3D>("Head/Camera3D").RotationDegrees = new Vector3(
				Mathf.Clamp(GetNode<Node3D>("Head/Camera3D").RotationDegrees.X, -40f, 60f),
				GetNode<Node3D>("Head/Camera3D").RotationDegrees.Y,
				GetNode<Node3D>("Head/Camera3D").RotationDegrees.Z
			);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Handle sprinting.
		if (Input.IsActionPressed("sprint"))
			speed = sprintSpeed;
		else
			speed = walkSpeed;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (GetNode<Node3D>("Head").Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = 0.0f;
			velocity.Z = 0.0f;
		}

		// FOV
		var velocityClamped = Mathf.Clamp(velocity.Length(), 0.5f, sprintSpeed * 2);
		var targetFov = baseFov +  fovChange * velocityClamped;
		
		// TODO: Add a smooth transition to the FOV.
		// GetNode<Node3D>("Head/Camera3D").GetNode<Camera>("Camera").Fov = targetFov;		
		
		Velocity = velocity;
		MoveAndSlide();
	}


}
