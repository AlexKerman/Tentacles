using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FourTentacles
{
	public class Material
	{
		public static void Init()
		{
			float[] material_Ka = { 0.1f, 0.0f, 0.0f, 1.0f };
			float[] material_Kd = { 0.4f, 0.4f, 0.5f, 1.0f };
			float[] material_Ks = { 0.7f, 0.7f, 0.7f, 0.7f };
			float[] material_Ke = { 0.1f, 0.0f, 0.0f, 0.0f };
			float material_Se = 40.0f;

			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, material_Ka);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, material_Kd);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, material_Ks);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, material_Ke);
			GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, material_Se);

			GL.Enable(EnableCap.ColorMaterial);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public static void SetMeshMaterial()
		{
			GL.Enable(EnableCap.Lighting);
			GL.Color3(Color.Chocolate);
			Shader.Phong.Use();
		}

		public static void SetLineMaterial(Color color)
		{
			GL.Disable(EnableCap.Lighting);
			GL.Enable(EnableCap.LineSmooth);
			GL.Color3(color);
			GL.LineWidth(1.0f);
			Shader.Simple.Use();
		}
	}
}
