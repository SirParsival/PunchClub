using UnityEngine;

[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour 
{

	public string filename;
	public bool autoSnapping;

	public Color gridColor = Color.white;

	void Update() 
	{
		if (autoSnapping) 
		{
			SnapChildren();
		}
	}

	public void SnapChildren() 
	{
		foreach (Transform child in transform) 
		{
			Vector3 pos = child.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			child.localPosition = pos;
		}
	}

	public Mesh MakeMesh() 
	{
		Mesh mesh = new Mesh();

		int polygons = transform.childCount;

		Vector3[] vertices = new Vector3[polygons * 4];
		Vector2[] uvs = new Vector2[polygons * 4];
		int[] tris = new int[6 * polygons];

		for (int i = 0; i < polygons; i++) {
			SpriteRenderer spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();

			vertices[i * 4 + 0] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[3];
			vertices[i * 4 + 1] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[1];
			vertices[i * 4 + 2] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[0];
			vertices[i * 4 + 3] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[2];

			uvs[i * 4 + 0] = spriteRenderer.sprite.uv[3];
			uvs[i * 4 + 1] = spriteRenderer.sprite.uv[1];
			uvs[i * 4 + 2] = spriteRenderer.sprite.uv[0];
			uvs[i * 4 + 3] = spriteRenderer.sprite.uv[2];

			tris[i * 6 + 0] = (i * 4) + 0;
			tris[i * 6 + 1] = (i * 4) + 2;
			tris[i * 6 + 2] = (i * 4) + 1;
			tris[i * 6 + 3] = (i * 4) + 2;
			tris[i * 6 + 4] = (i * 4) + 3;
			tris[i * 6 + 5] = (i * 4) + 1;
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = tris;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}


	public void OnDrawGizmos() 
	{

		float scale = 1000;
		int columns = 100;
		int rows = 100;

		Vector2 offset = new Vector2(0.5f, 0.5f);

		Gizmos.color = gridColor;

		for (int j = -rows; j < rows; j++) 
		{
			Vector3 min = new Vector3(scale + offset.x, j + offset.y, 0);
			Vector3 max = new Vector3(-scale + offset.x, j + offset.y, 0);

			min = transform.TransformPoint(min);
			max = transform.TransformPoint(max);

			Gizmos.DrawLine(min, max);

		}

		for (int i = -columns; i < columns; i++) 
		{
			Vector3 min = new Vector3(i + offset.x, +scale + offset.y, 0);
			Vector3 max = new Vector3(i + offset.x, -scale + offset.y, 0);

			min = transform.TransformPoint(min);
			max = transform.TransformPoint(max);

			Gizmos.DrawLine(min, max);
		}
	}
}
