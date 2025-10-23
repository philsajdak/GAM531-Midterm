using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GAM531_Midterm.Graphics;
using GAM531_Midterm.Game;

namespace GAM531_Midterm
{
    public class Engine : GameWindow
    {
        // constants
        private const float CLEAR_COLOR_R = 0.1f;
        private const float CLEAR_COLOR_G = 0.1f;
        private const float CLEAR_COLOR_B = 0.15f;
        private const float CLEAR_COLOR_A = 1.0f;
        private const float CAMERA_START_HEIGHT = 1.6f;
        private const float CAMERA_START_Z = 8f;
        private const float CAMERA_START_X = 0f;

        private Camera? camera;
        private Renderer renderer;
        private GameLogic gameLogic;
        private List<(Vector3 position, Vector3 scale, Vector3 color)> walls;

        // input state
        private KeyboardState? previousKeyboardState;

        public Engine(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                ClientSize = (width, height),
                Title = title,
            })
        {
            renderer = new Renderer();
            gameLogic = new GameLogic();
            walls = new List<(Vector3, Vector3, Vector3)>();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(CLEAR_COLOR_R, CLEAR_COLOR_G, CLEAR_COLOR_B, CLEAR_COLOR_A);
            GL.Enable(EnableCap.DepthTest);
            CursorState = CursorState.Grabbed;

            camera = new Camera(new Vector3(CAMERA_START_X, CAMERA_START_HEIGHT, CAMERA_START_Z), Vector3.UnitY);
            camera.PlayerHeight = CAMERA_START_HEIGHT;

            ResourceManager.LoadResources();
            walls = GameLogic.BuildRoom();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            ResourceManager.UnloadResources();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (camera == null) return;

            float deltaTime = (float)args.Time;
            var currentKeyboard = KeyboardState;

            // camera movement
            if (currentKeyboard.IsKeyDown(Keys.W))
                camera.ProcessKeyboard(CameraMovement.Forward, deltaTime);
            if (currentKeyboard.IsKeyDown(Keys.S))
                camera.ProcessKeyboard(CameraMovement.Backward, deltaTime);
            if (currentKeyboard.IsKeyDown(Keys.A))
                camera.ProcessKeyboard(CameraMovement.Left, deltaTime);
            if (currentKeyboard.IsKeyDown(Keys.D))
                camera.ProcessKeyboard(CameraMovement.Right, deltaTime);

            // jump (key press detection)
            if (IsKeyPressed(Keys.Space, currentKeyboard))
                camera.Jump();

            camera.UpdateVerticalMovement(deltaTime);

            // game logic (E key press detection)
            gameLogic.Update(camera, IsKeyPressed(Keys.E, currentKeyboard));

            if (currentKeyboard.IsKeyDown(Keys.Escape))
                Close();

            // update previous keyboard state at the end of the frame
            previousKeyboardState = currentKeyboard.GetSnapshot();
        }

        private bool IsKeyPressed(Keys key, KeyboardState currentState)
        {
            // key is pressed this frame if it's down now AND was not down last frame
            return currentState.IsKeyDown(key) && (previousKeyboardState == null || !previousKeyboardState.IsKeyDown(key));
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            camera?.ProcessMouseMovement(e.X, e.Y);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            camera?.ProcessMouseScroll(e.OffsetY);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (camera == null) return;

            float aspectRatio = (float)ClientSize.X / ClientSize.Y;
            renderer.RenderScene(camera, gameLogic.Buttons, gameLogic.Door, walls, aspectRatio);

            Title = gameLogic.GetStatusText();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }
    }
}