using System;
using Heatmap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZoneGraph
{
	public class ZoneGraphViewer : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private float traceSize;
        private Mesh _mesh;
		private Vector3[] _vertices;
		private Color[] _colors;
		private int[] _indices;
		private HeatmapData _heatmap;

		private void Start()
		{
			_mesh = new Mesh();
			_mesh.MarkDynamic();

			_vertices = new Vector3[ZoneGraphManager.Instance.Nodes.Count * 4];
			_colors = new Color[ZoneGraphManager.Instance.Nodes.Count * 4];
			_indices = new int[ZoneGraphManager.Instance.Nodes.Count * 6];

			GenerateMeshData();

			_mesh.vertices = _vertices;
			_mesh.colors = _colors;
			_mesh.triangles = _indices;
			_mesh.MarkModified();

			_heatmap = new HeatmapData("Viewer");
		}

		private void Update()
		{
			
		}

		private void GenerateMeshData()
		{
			for (var i = 0; i < ZoneGraphManager.Instance.Nodes.Count; i++)
			{
				var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
				var forward = rotation * Vector3.forward;
				var right = rotation * Vector3.right;
				var position = ZoneGraphManager.Instance.Nodes[i].Position;

				_vertices[i * 4 + 0] = position + (forward + right) * traceSize;
				_vertices[i * 4 + 1] = position + (forward - right) * traceSize;
				_vertices[i * 4 + 2] = position - (forward - right) * traceSize;
				_vertices[i * 4 + 3] = position - (forward + right) * traceSize;
				
				_colors[i * 4 + 0] = Color.black;
				_colors[i * 4 + 1] = Color.black;
				_colors[i * 4 + 2] = Color.black;
				_colors[i * 4 + 3] = Color.black;

				_indices[i * 6 + 0] = i * 4 + 0;
				_indices[i * 6 + 1] = i * 4 + 1;
				_indices[i * 6 + 2] = i * 4 + 2;
				_indices[i * 6 + 3] = i * 4 + 2;
				_indices[i * 6 + 4] = i * 4 + 3;
				_indices[i * 6 + 5] = i * 4 + 0;
			}
		}
	}
}