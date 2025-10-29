//==============================================================================
// Camera.cs
//==============================================================================
// First-person camera that lets the player look around and move. Handles
// keyboard movement (WASD), mouse looking, jumping with gravity, and zoom.
// Creates the view matrix so we can see the game world from the player's eyes.
//==============================================================================

using OpenTK.Mathematics;

namespace GAM531_Midterm
{
    public class Camera
    {
        // constants
        private const float DEFAULT_YAW = -90.0f;
        private const float DEFAULT_PITCH = 0.0f;
        private const float DEFAULT_MOVEMENT_SPEED = 7f;
        private const float DEFAULT_MOUSE_SENSITIVITY = 0.3f;
        private const float DEFAULT_FOV = 55.0f;
        private const float MIN_FOV = 30.0f;
        private const float MAX_FOV = 90.0f;
        private const float MAX_PITCH = 89.0f;
        private const float MIN_PITCH = -89.0f;
        private const float JUMP_INITIAL_VELOCITY = 6.0f;
        private const float GRAVITY = -15.0f;
        private const float DEFAULT_PLAYER_HEIGHT = 1.6f;
        private const float SCROLL_SENSITIVITY = 2.0f;
        private const float DEFAULT_NEAR_PLANE = 0.1f;
        private const float DEFAULT_FAR_PLANE = 100.0f;
        private const float GROUND_Y_POSITION = 0.0f;

        // angles
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        // camera attributes
        public Vector3 Position { get; set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 WorldUp { get; private set; }

        public float PlayerHeight { get; set; }

        // jump mechanics
        public float VerticalVelocity { get; set; }
        public bool IsJumping { get; set; }

        // camera options
        public float MovementSpeed { get; set; }
        public float MouseSensitivity { get; set; }
        public float Fov { get; private set; }
        public float FovMin { get; set; }
        public float FovMax { get; set; }

        // mouse attributes
        private bool initMove = true;
        private Vector2 lastMousePos = Vector2.Zero;

        public Camera(Vector3 position, Vector3 up, float yaw = DEFAULT_YAW, float pitch = DEFAULT_PITCH)
        {
            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            Front = new Vector3(GROUND_Y_POSITION, GROUND_Y_POSITION, -1.0f);

            MovementSpeed = DEFAULT_MOVEMENT_SPEED;
            MouseSensitivity = DEFAULT_MOUSE_SENSITIVITY;
            Fov = DEFAULT_FOV;
            FovMin = MIN_FOV;
            FovMax = MAX_FOV;

            PlayerHeight = position.Y;
            VerticalVelocity = GROUND_Y_POSITION;
            IsJumping = false;

            UpdateCameraVectors();
        }

        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw = DEFAULT_YAW, float pitch = DEFAULT_PITCH)
            : this(new Vector3(posX, posY, posZ), new Vector3(upX, upY, upZ), yaw, pitch)
        {
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public Matrix4 GetProjectionMatrix(float aspectRatio, float nearPlane = DEFAULT_NEAR_PLANE, float farPlane = DEFAULT_FAR_PLANE)
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), aspectRatio, nearPlane, farPlane);
        }

        public void ProcessKeyboard(CameraMovement direction, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;

            Vector3 forwardXZ = Vector3.Normalize(new Vector3(Front.X, GROUND_Y_POSITION, Front.Z));
            Vector3 rightXZ = Vector3.Normalize(new Vector3(Right.X, GROUND_Y_POSITION, Right.Z));

            switch (direction)
            {
                case CameraMovement.Forward:
                    Position += forwardXZ * velocity;
                    break;
                case CameraMovement.Backward:
                    Position -= forwardXZ * velocity;
                    break;
                case CameraMovement.Left:
                    Position -= rightXZ * velocity;
                    break;
                case CameraMovement.Right:
                    Position += rightXZ * velocity;
                    break;
            }

            if (!IsJumping)
            {
                Position = new Vector3(Position.X, PlayerHeight, Position.Z);
            }
        }

        public void ProcessMouseMovement(float xPos, float yPos, bool constrainPitch = true)
        {
            if (initMove)
            {
                lastMousePos = new Vector2(xPos, yPos);
                initMove = false;
                return;
            }

            float xOffset = xPos - lastMousePos.X;
            float yOffset = lastMousePos.Y - yPos;
            lastMousePos = new Vector2(xPos, yPos);

            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw += xOffset;
            Pitch += yOffset;

            if (constrainPitch)
            {
                if (Pitch > MAX_PITCH)
                    Pitch = MAX_PITCH;
                if (Pitch < MIN_PITCH)
                    Pitch = MIN_PITCH;
            }

            UpdateCameraVectors();
        }

        public void ProcessMouseScroll(float yOffset)
        {
            Fov -= yOffset * SCROLL_SENSITIVITY;

            if (Fov < FovMin)
                Fov = FovMin;
            if (Fov > FovMax)
                Fov = FovMax;
        }

        public void ResetFirstMove()
        {
            initMove = true;
        }

        private void UpdateCameraVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            Front = Vector3.Normalize(front);
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public void Jump()
        {
            if (!IsJumping)
            {
                VerticalVelocity = JUMP_INITIAL_VELOCITY;
                IsJumping = true;
            }
        }

        public void UpdateVerticalMovement(float deltaTime)
        {
            if (IsJumping)
            {
                VerticalVelocity += GRAVITY * deltaTime;
                Position = new Vector3(Position.X, Position.Y + VerticalVelocity * deltaTime, Position.Z);

                if (Position.Y <= DEFAULT_PLAYER_HEIGHT)
                {
                    Position = new Vector3(Position.X, DEFAULT_PLAYER_HEIGHT, Position.Z);
                    VerticalVelocity = GROUND_Y_POSITION;
                    IsJumping = false;
                }
            }
        }
    }

    public enum CameraMovement
    {
        Forward,
        Backward,
        Left,
        Right
    }
}