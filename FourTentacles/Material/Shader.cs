using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	public class Shader
	{
		private static readonly List<Shader> loaded = new List<Shader>();

		public static Shader Simple = new Shader("simple");
		public static Shader Phong = new Shader("Phong");

		private int vertexObject;
		private int fragmentObject;
		private int program;

		public static void DisableAll()
		{
			GL.ShadeModel(ShadingModel.Flat);
			GL.Disable(EnableCap.Lighting);
		}

		public static void DeleteAll()
		{
			foreach (Shader s in loaded)
			{
				GL.DetachShader(s.program, s.vertexObject);
				GL.DetachShader(s.program, s.fragmentObject);
				GL.DeleteShader(s.vertexObject);
				GL.DeleteShader(s.fragmentObject);
				GL.DeleteProgram(s.program);
				s.program = 0;
			}
		}

		public Shader(string name)
		{
			vertexObject = Load(name, ShaderType.VertexShader);
			fragmentObject = Load(name, ShaderType.FragmentShader);

			program = GL.CreateProgram();
			GL.AttachShader(program, fragmentObject);
			GL.AttachShader(program, vertexObject);

			GL.LinkProgram(program);
			loaded.Add(this);
		}

		private static string ShadersDirectory
		{
			get 
			{
				var di = new DirectoryInfo(Application.StartupPath);
				di = di.Parent.Parent.CreateSubdirectory("Shaders");
				return di.FullName + "\\";
			}
		}

		private static int Load(string name, ShaderType shaderType)
		{
			int statusCode;
			string info;

			string extension = shaderType == ShaderType.VertexShader ? ".vert" : ".frag";
			string filename = ShadersDirectory + name + extension;

			var sr = new StreamReader(filename);
			string source = sr.ReadToEnd();
			sr.Close();

			int shaderId = GL.CreateShader(shaderType);
			GL.ShaderSource(shaderId, source);
			GL.CompileShader(shaderId);
			GL.GetShaderInfoLog(shaderId, out info);
			GL.GetShader(shaderId, ShaderParameter.CompileStatus, out statusCode);

			if (statusCode != 1) throw new Exception(info);
			return shaderId;
		}

		public void Use()
		{
			GL.UseProgram(program);
		}

		public void Bind(int texunit, string name)
		{
			var loc = GL.GetUniformLocation(program, name);
			GL.Uniform1(loc, texunit);
		}

		public void BindArrayFloat(float[] flo, string name)
		{
			var loc = GL.GetUniformLocation(program, name);
			GL.Uniform3(loc, flo.Length, flo);
		}
	}
}
