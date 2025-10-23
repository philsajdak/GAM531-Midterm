using OpenTK.Mathematics;

namespace GAM531_Midterm
{
    public class Button
    {
        // constants
        private const float DEFAULT_SIZE = 0.3f;
        private const float DEFAULT_INTERACTION_DISTANCE = 2.0f;
        private const float EMISSIVE_STRENGTH_PRESSED = 0.5f;
        private const float EMISSIVE_STRENGTH_UNPRESSED = 0.0f;

        private static readonly Vector3 COLOR_UNPRESSED = new Vector3(1.0f, 0.0f, 0.0f);
        private static readonly Vector3 COLOR_PRESSED = new Vector3(1.0f, 0.5f, 0.0f);
        private static readonly Vector3 EMISSIVE_NONE = new Vector3(0.0f, 0.0f, 0.0f);
        private static readonly Vector3 EMISSIVE_PRESSED = new Vector3(1.0f, 0.5f, 0.0f);

        public Vector3 Position { get; set; }
        public bool IsPressed { get; set; }
        public Vector3 Color { get; set; }
        public float Size { get; set; }

        // properties for glow effect
        public Vector3 EmissiveColor { get; set; }
        public float EmissiveStrength { get; set; }

        public Button(Vector3 position, float size = DEFAULT_SIZE)
        {
            Position = position;
            IsPressed = false;
            Size = size;
            Color = COLOR_UNPRESSED;
            EmissiveColor = EMISSIVE_NONE;
            EmissiveStrength = EMISSIVE_STRENGTH_UNPRESSED;
        }

        public void Press()
        {
            if (!IsPressed)
            {
                IsPressed = true;
                Color = COLOR_PRESSED;

                // change emissive color when button is pressed
                EmissiveColor = EMISSIVE_PRESSED;
                EmissiveStrength = EMISSIVE_STRENGTH_PRESSED;
            }
        }

        public bool IsPlayerNear(Vector3 playerPosition, float interactionDistance = DEFAULT_INTERACTION_DISTANCE)
        {
            return (Position - playerPosition).Length < interactionDistance;
        }
    }
}