using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PccRenderer : MonoBehaviour
{
    public void LoadAndRenderPly(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("PLY file not found: " + path);
            return;
        }

        using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            int vertexCount = 0;
            string line;

            while (true)
            {
                line = ReadLine(br);

                if (line.StartsWith("element vertex"))
                    vertexCount = int.Parse(line.Split(' ')[2]);

                if (line == "end_header")
                    break;
            }

            Vector3[] vertices = new Vector3[vertexCount];
            Color[] colors = new Color[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();

                br.ReadSingle(); // opacity

                br.ReadSingle(); // scale0
                br.ReadSingle(); // scale1
                br.ReadSingle(); // scale2

                br.ReadSingle(); // rot0
                br.ReadSingle(); // rot1
                br.ReadSingle(); // rot2

                float r = br.ReadSingle(); // f_dc_0
                float g = br.ReadSingle(); // f_dc_1
                float b = br.ReadSingle(); // f_dc_2

                vertices[i] = new Vector3(x, y, z);

                colors[i] = new Color(
                    Mathf.Clamp01(r),
                    Mathf.Clamp01(g),
                    Mathf.Clamp01(b)
                );
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.vertices = vertices;
            mesh.colors = colors;

            int[] indices = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                indices[i] = i;

            mesh.SetIndices(indices, MeshTopology.Points, 0);

            GameObject go = new GameObject("PointCloud");

            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();

            mf.mesh = mesh;
            mr.material = new Material(Shader.Find("Sprites/Default"));

            Debug.Log("Rendered " + vertexCount + " points");
        }
    }

    string ReadLine(BinaryReader br)
    {
        List<byte> bytes = new List<byte>();
        byte b;

        while ((b = br.ReadByte()) != '\n')
            bytes.Add(b);

        return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
    }
}


