using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace GAM531_Midterm.Graphics
{
    public class Renderer
    {
        // constants
        private const float FLOOR_Y_POSITION = -1.0f;
        private const float FLOOR_TEX_SCALE = 0.15f;
        private const float WALL_TEX_SCALE = 0.25f;
        private const float BUTTON_TEX_SCALE = 2.0f;
        private const float DOOR_TEX_SCALE = 0.4f;
        private const float DOOR_SCALE_X = 6.5f;
        private const float DOOR_SCALE_Y = 4.2f;
        private const float DOOR_SCALE_Z = 0.2f;
        private const float POINT_LIGHT_INTENSITY_BUTTON = 1.0f;
        private const float POINT_LIGHT_INTENSITY_DOOR = 6.0f;
        private const float DOOR_EMISSIVE_STRENGTH = 0.5f;
        private const float NO_EMISSIVE_STRENGTH = 0.0f;
        private const int TEXTURE_UNIT_0 = 0;

        private static readonly Vector3 LIGHT_POSITION = new Vector3(0.0f, 5.0f, 0.0f);
        private static readonly Vector3 LIGHT_COLOR = new Vector3(1.0f, 1.0f, 1.0f);
        private static readonly Vector3 FLOOR_COLOR = new Vector3(0.4f, 0.4f, 0.4f);
        private static readonly Vector3 POINT_LIGHT_COLOR_ORANGE = new Vector3(1.0f, 0.5f, 0.0f);
        private static readonly Vector3 DOOR_EMISSIVE_COLOR = new Vector3(1.0f, 0.5f, 0.0f);

        public void RenderScene(Camera camera, List<Button> buttons, Door door, List<(Vector3 position, Vector3 scale, Vector3 color)> walls, float aspectRatio)
        {
            var shader = ResourceManager.PhongShader;
            if (shader == null || camera == null) return;

            shader.Use();

            // setup camera and lighting
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix(aspectRatio);

            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
            shader.SetVector3("lightPos", LIGHT_POSITION);
            shader.SetVector3("viewPos", camera.Position);
            shader.SetVector3("lightColor", LIGHT_COLOR);

            // setup point lights
            SetupPointLights(shader, buttons, door);

            // render objects
            RenderFloor(shader);
            RenderWalls(shader, walls);
            RenderButtons(shader, buttons);
            RenderDoor(shader, door);
        }

        private void SetupPointLights(Shader shader, List<Button> buttons, Door door)
        {
            List<Vector3> pointLightPos = new List<Vector3>();
            List<Vector3> pointLightCol = new List<Vector3>();
            List<float> pointLightInt = new List<float>();

            foreach (var button in buttons)
            {
                if (button.IsPressed)
                {
                    pointLightPos.Add(button.Position);
                    pointLightCol.Add(POINT_LIGHT_COLOR_ORANGE);
                    pointLightInt.Add(POINT_LIGHT_INTENSITY_BUTTON);
                }
            }

            if (door.IsOpen)
            {
                pointLightPos.Add(door.Position);
                pointLightCol.Add(POINT_LIGHT_COLOR_ORANGE);
                pointLightInt.Add(POINT_LIGHT_INTENSITY_DOOR);
            }

            shader.SetInt("numPointLights", pointLightPos.Count);
            if (pointLightPos.Count > 0)
            {
                shader.SetVector3Array("pointLightPositions", pointLightPos.ToArray());
                shader.SetVector3Array("pointLightColors", pointLightCol.ToArray());
                shader.SetFloatArray("pointLightIntensities", pointLightInt.ToArray());
            }
        }

        private void RenderFloor(Shader shader)
        {
            ResourceManager.FloorTexture?.Use(TextureUnit.Texture0);
            shader.SetInt("textureSampler", TEXTURE_UNIT_0);
            shader.SetBool("useTexture", true);
            shader.SetFloat("texScale", FLOOR_TEX_SCALE);
            shader.SetMatrix4("model", Matrix4.CreateTranslation(0, FLOOR_Y_POSITION, 0));
            shader.SetVector3("objectColor", FLOOR_COLOR);
            shader.SetVector3("emissiveColor", Vector3.Zero);
            shader.SetFloat("emissiveStrength", NO_EMISSIVE_STRENGTH);

            ResourceManager.FloorMesh?.Bind();
            ResourceManager.FloorMesh?.Draw();
        }

        private void RenderWalls(Shader shader, List<(Vector3 position, Vector3 scale, Vector3 color)> walls)
        {
            ResourceManager.WallTexture?.Use(TextureUnit.Texture0);
            shader.SetBool("useTexture", true);
            shader.SetFloat("texScale", WALL_TEX_SCALE);

            foreach (var wall in walls)
            {
                Matrix4 wallModel = Matrix4.CreateScale(wall.scale) * Matrix4.CreateTranslation(wall.position);
                shader.SetMatrix4("model", wallModel);
                shader.SetVector3("objectColor", wall.color);
                shader.SetVector3("emissiveColor", Vector3.Zero);
                shader.SetFloat("emissiveStrength", NO_EMISSIVE_STRENGTH);

                ResourceManager.CubeMesh?.Bind();
                ResourceManager.CubeMesh?.Draw();
            }
        }

        private void RenderButtons(Shader shader, List<Button> buttons)
        {
            ResourceManager.ButtonTexture?.Use(TextureUnit.Texture0);
            shader.SetBool("useTexture", true);
            shader.SetFloat("texScale", BUTTON_TEX_SCALE);

            foreach (var button in buttons)
            {
                Matrix4 buttonModel = Matrix4.CreateScale(button.Size) * Matrix4.CreateTranslation(button.Position);
                shader.SetMatrix4("model", buttonModel);
                shader.SetVector3("objectColor", button.Color);
                shader.SetVector3("emissiveColor", button.EmissiveColor);
                shader.SetFloat("emissiveStrength", button.EmissiveStrength);

                ResourceManager.CubeMesh?.Bind();
                ResourceManager.CubeMesh?.Draw();
            }
        }

        private void RenderDoor(Shader shader, Door door)
        {
            ResourceManager.DoorTexture?.Use(TextureUnit.Texture0);
            shader.SetBool("useTexture", true);
            shader.SetFloat("texScale", DOOR_TEX_SCALE);

            Matrix4 doorModel = Matrix4.CreateScale(DOOR_SCALE_X, DOOR_SCALE_Y, DOOR_SCALE_Z) * Matrix4.CreateTranslation(door.Position);
            shader.SetMatrix4("model", doorModel);
            shader.SetVector3("objectColor", door.Color);

            if (door.IsOpen)
            {
                shader.SetVector3("emissiveColor", DOOR_EMISSIVE_COLOR);
                shader.SetVector3("emissiveColor", DOOR_EMISSIVE_COLOR);
                shader.SetFloat("emissiveStrength", DOOR_EMISSIVE_STRENGTH);
            }
            else
            {
                shader.SetVector3("emissiveColor", Vector3.Zero);
                shader.SetFloat("emissiveStrength", NO_EMISSIVE_STRENGTH);
            }

            ResourceManager.CubeMesh?.Bind();
            ResourceManager.CubeMesh?.Draw();
        }
    }
}