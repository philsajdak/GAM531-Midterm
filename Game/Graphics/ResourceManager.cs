namespace GAM531_Midterm.Graphics
{
    public static class ResourceManager
    {
        // constants
        private const float FLOOR_SIZE = 15.0f;
        private const int VERTEX_STRIDE_FLOATS = 6; // 3 position + 3 normal

        public static Mesh? CubeMesh { get; private set; }
        public static Mesh? FloorMesh { get; private set; }

        public static Texture? FloorTexture { get; private set; }
        public static Texture? WallTexture { get; private set; }
        public static Texture? ButtonTexture { get; private set; }
        public static Texture? DoorTexture { get; private set; }

        public static Shader? PhongShader { get; private set; }

        public static void LoadResources()
        {
            // create meshes
            CubeMesh = CreateCubeMesh();
            FloorMesh = CreateFloorMesh();

            // load textures
            FloorTexture = new Texture("Textures/floor.jpg");
            WallTexture = new Texture("Textures/wall.jpg");
            ButtonTexture = new Texture("Textures/button.jpg");
            DoorTexture = new Texture("Textures/door.jpg");

            // load shader
            PhongShader = new Shader("Shader/phong.vert", "Shader/phong.frag");
        }

        private static Mesh CreateCubeMesh()
        {
            float[] vertices =
            {
                // front
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,
                // back
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 0.0f, -1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, -1.0f,
                // left
                -0.5f, -0.5f, -0.5f,  -1.0f, 0.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  -1.0f, 0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  -1.0f, 0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  -1.0f, 0.0f, 0.0f,
                // right
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f,
                // top
                -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f,
                // bottom
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f, 0.0f
            };

            uint[] indices =
            {
                0, 1, 2,  2, 3, 0,
                4, 5, 6,  6, 7, 4,
                8, 9, 10,  10, 11, 8,
                12, 13, 14,  14, 15, 12,
                16, 17, 18,  18, 19, 16,
                20, 21, 22,  22, 23, 20
            };

            return new Mesh(vertices, indices, VERTEX_STRIDE_FLOATS * sizeof(float));
        }

        private static Mesh CreateFloorMesh()
        {
            float[] vertices =
            {
                -FLOOR_SIZE, 0.0f, -FLOOR_SIZE,   0.0f, 1.0f, 0.0f,
                 FLOOR_SIZE, 0.0f, -FLOOR_SIZE,   0.0f, 1.0f, 0.0f,
                 FLOOR_SIZE, 0.0f,  FLOOR_SIZE,   0.0f, 1.0f, 0.0f,
                -FLOOR_SIZE, 0.0f,  FLOOR_SIZE,   0.0f, 1.0f, 0.0f
            };

            uint[] indices = { 0, 1, 2, 2, 3, 0 };

            return new Mesh(vertices, indices, VERTEX_STRIDE_FLOATS * sizeof(float));
        }

        public static void UnloadResources()
        {
            CubeMesh?.Dispose();
            FloorMesh?.Dispose();
            PhongShader?.Dispose();
            FloorTexture?.Dispose();
            WallTexture?.Dispose();
            ButtonTexture?.Dispose();
            DoorTexture?.Dispose();
        }
    }
}