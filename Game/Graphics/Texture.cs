//==============================================================================
// Texture.cs
//==============================================================================
// Loads image files (like the floor, wall, etc) so they can be wrapped onto 3D
// objects. Makes objects look detailed instead of just solid colors.
//==============================================================================

using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GAM531_Midterm
{
    public class Texture
    {
        // constants
        private const int FLIP_VERTICALLY_ON_LOAD = 1;

        public int Handle { get; private set; }

        public Texture(string path)
        {
            Handle = GL.GenTexture();
            Use();

            // load image
            StbImage.stbi_set_flip_vertically_on_load(FLIP_VERTICALLY_ON_LOAD);

            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            // set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}