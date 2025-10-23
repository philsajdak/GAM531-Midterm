using OpenTK.Mathematics;

namespace GAM531_Midterm.Game
{
    public class GameLogic
    {
        // constants (partly overkill but it makes parameters easier to understand)
        private const float WALL_THICKNESS = 0.5f;
        private const float WALL_HEIGHT = 5.0f;
        private const float ROOM_SIZE = 10.0f;
        private const float BUTTON_SIZE = 0.4f;
        private const float BUTTON_Y_POSITION = 1.5f;
        private const float BUTTON_LEFT_X = -8.5f;
        private const float BUTTON_RIGHT_X = 8.5f;
        private const float BUTTON_BACK_Z = -9.5f;
        private const float BUTTON_BACK_X = 0f;
        private const float DOOR_Y_POSITION = 1.0f;
        private const float DOOR_Z_POSITION = 10.0f;
        private const float DOOR_X_POSITION = 0f;
        private const float DOOR_WIDTH = 2.0f;
        private const float DOOR_HEIGHT = 3.0f;
        private const float WALL_Y_POSITION = 1f;
        private const float WALL_TOP_Y_POSITION = 3.5f;
        private const float FRONT_WALL_GAP_X = 6f;
        private const float FRONT_WALL_GAP_WIDTH = 6f;
        private const float FRONT_WALL_TOP_WIDTH = 20f;
        private const float FRONT_WALL_TOP_HEIGHT = 1f;
        private const float BACK_WALL_WIDTH = 20f;
        private const float BACK_WALL_HEIGHT = 6f;
        private const float SIDE_WALL_OFFSET = 1f;
        private const float SIDE_WALL_WIDTH = 20f;
        private const float SIDE_WALL_HEIGHT = 6f;

        private static readonly Vector3 WALL_COLOR = new Vector3(0.6f, 0.6f, 0.7f);

        public List<Button> Buttons { get; private set; }
        public Door Door { get; private set; }
        public bool GameWon { get; private set; }
        public bool ShowInteractionPrompt { get; private set; }
        public string InteractionText { get; private set; } = "";

        public GameLogic()
        {
            Buttons = CreateButtons();
            Door = CreateDoor();
        }

        public void Update(Camera camera, bool eKeyPressed)
        {
            ShowInteractionPrompt = false;
            InteractionText = "";

            // check button interactions
            foreach (var button in Buttons)
            {
                if (button.IsPlayerNear(camera.Position) && !button.IsPressed)
                {
                    ShowInteractionPrompt = true;
                    InteractionText = "Press E to Activate Button";

                    if (eKeyPressed)
                    {
                        button.Press();
                        CheckAllButtonsPressed();
                    }
                }
            }

            // check door interaction
            if (Door.IsPlayerNear(camera.Position))
            {
                if (!Door.IsOpen)
                {
                    ShowInteractionPrompt = true;
                    int pressedCount = Buttons.Count(b => b.IsPressed);
                    InteractionText = $"Door Locked - Buttons: {pressedCount}/{Buttons.Count}";
                }
                else if (!GameWon)
                {
                    ShowInteractionPrompt = true;
                    InteractionText = "Press E to Escape";

                    if (eKeyPressed)
                    {
                        GameWon = true;
                    }
                }
            }
        }

        private void CheckAllButtonsPressed()
        {
            if (Buttons.All(b => b.IsPressed) && !Door.IsOpen)
            {
                Door.Open();
            }
        }

        public string GetStatusText()
        {
            if (GameWon)
                return "Escape Room - You Escaped - Press ESC to close";

            int pressedCount = Buttons.Count(b => b.IsPressed);
            string status = $"Buttons: {pressedCount}/{Buttons.Count}";

            return ShowInteractionPrompt
                ? $"Escape Room - {InteractionText}"
                : $"Escape Room - {status}";
        }

        // scene building methods
        public static List<(Vector3 position, Vector3 scale, Vector3 color)> BuildRoom()
        {
            var walls = new List<(Vector3, Vector3, Vector3)>();

            // front wall (with door gap)
            walls.Add((
                new Vector3(-FRONT_WALL_GAP_X, WALL_Y_POSITION, ROOM_SIZE),
                new Vector3(FRONT_WALL_GAP_WIDTH, WALL_HEIGHT, WALL_THICKNESS),
                WALL_COLOR
            ));
            walls.Add((
                new Vector3(FRONT_WALL_GAP_X, WALL_Y_POSITION, ROOM_SIZE),
                new Vector3(FRONT_WALL_GAP_WIDTH, WALL_HEIGHT, WALL_THICKNESS),
                WALL_COLOR
            ));
            walls.Add((
                new Vector3(DOOR_X_POSITION, WALL_TOP_Y_POSITION, ROOM_SIZE),
                new Vector3(FRONT_WALL_TOP_WIDTH, FRONT_WALL_TOP_HEIGHT, WALL_THICKNESS),
                WALL_COLOR
            ));

            // back wall
            walls.Add((
                new Vector3(DOOR_X_POSITION, WALL_Y_POSITION, -ROOM_SIZE),
                new Vector3(BACK_WALL_WIDTH, BACK_WALL_HEIGHT, WALL_THICKNESS),
                WALL_COLOR
            ));

            // left wall
            walls.Add((
                new Vector3(-ROOM_SIZE + SIDE_WALL_OFFSET, WALL_Y_POSITION, DOOR_X_POSITION),
                new Vector3(WALL_THICKNESS, SIDE_WALL_HEIGHT, SIDE_WALL_WIDTH),
                WALL_COLOR
            ));

            // right wall
            walls.Add((
                new Vector3(ROOM_SIZE - SIDE_WALL_OFFSET, WALL_Y_POSITION, DOOR_X_POSITION),
                new Vector3(WALL_THICKNESS, SIDE_WALL_HEIGHT, SIDE_WALL_WIDTH),
                WALL_COLOR
            ));

            return walls;
        }

        private static List<Button> CreateButtons()
        {
            return new List<Button>
            {
                new Button(new Vector3(BUTTON_LEFT_X, BUTTON_Y_POSITION, DOOR_X_POSITION), BUTTON_SIZE),
                new Button(new Vector3(BUTTON_RIGHT_X, BUTTON_Y_POSITION, DOOR_X_POSITION), BUTTON_SIZE),
                new Button(new Vector3(BUTTON_BACK_X, BUTTON_Y_POSITION, BUTTON_BACK_Z), BUTTON_SIZE)
            };
        }

        private static Door CreateDoor()
        {
            return new Door(new Vector3(DOOR_X_POSITION, DOOR_Y_POSITION, DOOR_Z_POSITION), DOOR_WIDTH, DOOR_HEIGHT);
        }
    }
}