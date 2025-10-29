//==============================================================================
// Door.cs
//==============================================================================
// The exit door that opens when all buttons are pressed. Tracks whether it's
// open or closed and changes color when unlocked. The player needs to reach
// this door to win the game.
//==============================================================================

using OpenTK.Mathematics;

namespace GAM531_Midterm
{
    public class Door
    {
        // constants
        private const float DEFAULT_WIDTH = 2.0f;
        private const float DEFAULT_HEIGHT = 3.0f;
        private const float DEFAULT_INTERACTION_DISTANCE = 2.0f;

        private static readonly Vector3 COLOR_CLOSED = new Vector3(0.6f, 0.3f, 0.1f);
        private static readonly Vector3 COLOR_OPEN = new Vector3(1.0f, 0.5f, 0.0f);

        public Vector3 Position { get; set; }
        public bool IsOpen { get; set; }
        public Vector3 Color { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Door(Vector3 position, float width = DEFAULT_WIDTH, float height = DEFAULT_HEIGHT)
        {
            Position = position;
            IsOpen = false;
            Width = width;
            Height = height;
            Color = COLOR_CLOSED;
        }

        public void Open()
        {
            IsOpen = true;
            Color = COLOR_OPEN;
        }

        public bool IsPlayerNear(Vector3 playerPosition, float interactionDistance = DEFAULT_INTERACTION_DISTANCE)
        {
            return (Position - playerPosition).Length < interactionDistance;
        }
    }
}