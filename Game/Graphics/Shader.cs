using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GAM531_Midterm
{
    public class Shader
    {
        int Handle;
        private bool _disposed = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int vertexSuccess);
            if (vertexSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine($"vertex shader compilation fail: {infoLog}");
            }

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int fragmentSuccess);
            if (fragmentSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine($"fragment shader compilation fail: {infoLog}");
            }

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkSuccess);
            if (linkSuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine($"program linking fail: {infoLog}");
            }

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(VertexShader);
            GL.DeleteShader(FragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, data);
        }

        public void SetFloat(string name, float data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, data);
        }

        public void SetInt(string name, int data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, data);
        }

        public void SetBool(string name, bool data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, data ? 1 : 0);
        }

        public void SetVector3Array(string name, Vector3[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int location = GL.GetUniformLocation(Handle, $"{name}[{i}]");
                GL.Uniform3(location, data[i]);
            }
        }

        public void SetFloatArray(string name, float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int location = GL.GetUniformLocation(Handle, $"{name}[{i}]");
                GL.Uniform1(location, data[i]);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                GL.DeleteProgram(Handle);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}