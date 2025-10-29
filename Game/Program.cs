//==============================================================================
// Program.cs
//==============================================================================
// The starting point of the program. Creates the game window and starts
// the game running.
//==============================================================================

namespace GAM531_Midterm
{
    class Program
    {
        // constants
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const string WINDOW_TITLE = "Escape Room - Loading...";

        static void Main(string[] args)
        {
            using (Engine engine = new Engine(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_TITLE))
            {
                engine.Run();
            }
        }
    }
}