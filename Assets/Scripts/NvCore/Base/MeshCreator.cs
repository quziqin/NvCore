using UnityEngine;
using System.Collections.Generic;

public class MeshCreator :  Singleton<MeshCreator>
{
	public Mesh CreateFanMesh(float radius, float angle_fov, float angle_lookat)
	{
		int quality = 30;
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[2 * quality + 1];   // Could be of size [2 * quality + 2] if circle segment is continuous
		mesh.triangles = new int[3 * quality];

		Vector3[] normals = new Vector3[2 * quality + 1];
		Vector2[] uv = new Vector2[2 * quality + 1];

		for (int i = 0; i < uv.Length; i++)
			uv[i] = new Vector2(0, 0);
		for (int i = 0; i < normals.Length; i++)
			normals[i] = new Vector3(0, 1, 0);

		mesh.uv = uv;
		mesh.normals = normals;

		float angle_start = angle_lookat - angle_fov;
		float angle_end = angle_lookat + angle_fov;
		float angle_delta = (angle_end - angle_start) / quality;

		float angle_curr = angle_start;
		float angle_next = angle_start + angle_delta;

		Vector3 pos_curr_max = Vector3.zero;
		Vector3 pos_next_max = Vector3.zero;

		Vector3[] vertices = new Vector3[2 * quality + 1];   // Could be of size [2 * quality + 2] if circle segment is continuous
		vertices[0] = Vector3.zero;

		int[] triangles = new int[3 * quality];
		for (int i = 0; i < quality; i++)
		{
			Vector3 sphere_curr = new Vector3(
			Mathf.Sin(Mathf.Deg2Rad * (angle_curr)), 0,   // Left handed CW
			Mathf.Cos(Mathf.Deg2Rad * (angle_curr)));

			Vector3 sphere_next = new Vector3(
			Mathf.Sin(Mathf.Deg2Rad * (angle_next)), 0,
			Mathf.Cos(Mathf.Deg2Rad * (angle_next)));

			pos_curr_max = sphere_curr * radius;
			pos_next_max = sphere_next * radius;

			int a = 2 * i + 1;
			int b = 2 * i + 2;

			vertices[a] = pos_curr_max;
			vertices[b] = pos_next_max;

			triangles[3 * i] = 0;
			triangles[3 * i + 1] = a;
			triangles[3 * i + 2] = b;

			angle_curr += angle_delta;
			angle_next += angle_delta;

		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		return mesh;
	}

	public Mesh CreateSqureMesh(float radius, float width)
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[4];   // Could be of size [2 * quality + 2] if circle segment is continuous
		mesh.triangles = new int[6];

		Vector3[] normals = new Vector3[4];
		Vector2[] uv = new Vector2[4];

		for (int i = 0; i < uv.Length; i++)
			uv[i] = new Vector2(0, 0);
		for (int i = 0; i < normals.Length; i++)
			normals[i] = new Vector3(0, 1, 0);

		mesh.uv = uv;
		mesh.normals = normals;

		Vector3[] vertices = new Vector3[4];   // Could be of size [2 * quality + 2] if circle segment is continuous
		int[] triangles = new int[6];
		float hw = width / 2f;
		vertices[0] = new Vector3(-hw, 0, 0);
		vertices[1] = new Vector3(-hw, 0, radius);
		vertices[2] = new Vector3(hw, 0, radius);
		vertices[3] = new Vector3(hw, 0, 0);
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 3;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		return mesh;
	}

}
