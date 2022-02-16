using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshFromLine {

    public static Quaternion GetQuaternionFromTo(Vector3 ptFrom, Vector3 ptTo) {
        return Quaternion.FromToRotation(Vector3.right, ptTo - ptFrom);
    }

    [System.Serializable]
    public class Settings {
        [Range(3,12)]
        public int sides = 8;
        public float radiusMax = 0.5f;
        public float radiusMin = 0.1f;
        public Vector2 scaleUV = new Vector2(1, 1);
        public AnimationCurve radiusCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });
    }
    private struct Output {
        public Settings settings { get; private set; }
        public float[] radii;
        public Vector3[,] rings;
        public Output(Settings settings, Vector3[] points) {

            this.settings = settings;

            radii = new float[points.Length];
            rings = new Vector3[points.Length, settings.sides + 1];

            // generate radii:

            for (int i = 0; i < points.Length; i++) {
                float percent = i / (float)(points.Length - 1);
                percent = settings.radiusCurve.Evaluate(percent);
                float taperedRadius = Mathf.Lerp(settings.radiusMin, settings.radiusMax, percent);

                radii[i] = taperedRadius;
            }

            // Generate rings

            // sides + 1 (UV seams!)
            float arc = 2 * Mathf.PI / settings.sides;
            Quaternion prevQuat = Quaternion.identity;
            for (int i = 0; i < points.Length; i++) { // loop through all of the points in the spline...

                bool isFirst = (i == 0);
                bool isLast = (i == points.Length - 1);

                Quaternion nextQuat = prevQuat;
                if (!isLast) {
                    // if the current point is NOT the end point...
                    // get the quaternion to the next point:
                    nextQuat = GetQuaternionFromTo(points[i], points[i + 1]);
                    // average the quaternion with the previous quaternion (if this point isn't the first point):
                    prevQuat = isFirst ? nextQuat : Quaternion.Slerp(prevQuat, nextQuat, 0.5f);
                }
                Quaternion rotation = prevQuat;
                float angle = 0;

                for (int j = 0; j < settings.sides; j++) {
                    // get the next point in the ring:
                    Vector3 v = radii[i] * new Vector3(0, Mathf.Sin(angle), Mathf.Cos(angle));
                    // rotate the point, translate the point, store the point:
                    rings[i, j] = rotation * v + points[i];
                    // spin the angle:
                    angle -= arc;
                }
                rings[i, settings.sides] = rings[i, 0]; // copy the first point (for UV seams!)

                prevQuat = nextQuat;
            }


        }
    }
    public static Mesh BuildMesh(Vector3[] points, Settings settings = null) {

        if (points.Length == 0) return new Mesh();
        if (settings == null) settings = new Settings();

        Output output = new Output(settings, points);


        Mesh mesh = new Mesh();
        mesh.vertices = GenerateVertList(output);
        mesh.triangles = GenerateTris(output, mesh.vertices);
        mesh.uv = GenerateUVs(output, mesh.vertices);
        mesh.normals = GenerateNormals(output, points, mesh.vertices);
        mesh.colors = GenerateColors(output, mesh.vertices);
        return mesh;
    }
    
    private static Vector3[] GenerateVertList(Output output) {

        int num1 = output.rings.GetLength(0);
        int num2 = output.rings.GetLength(1);
        Vector3[] vertices = new Vector3[num1 * num2];

        int k = 0;
        for (int i = 0; i < num1; i++) {
            for (int j = 0; j < num2; j++) {
                vertices[k] = output.rings[i, j];
                k++;
            }
        }
        return vertices;
    }
    private static int[] GenerateTris(Output output, Vector3[] verts) {


        int sides = output.settings.sides;
        int length = output.radii.Length;
        int[] tris = new int[sides * (length - 1) * 6];

        int triNum = 0;
        for (int i = 0; i < output.radii.Length - 1; i++) {
            for (int j = 0; j < sides; j++) {

                int n = i * (sides + 1) + j; // sides + 1 (UV seams!)

                int cornerBottomRight = n;
                int cornerBottomLeft = n + 1;
                int cornerTopLeft = n + sides + 2;
                int cornerTopRight = n + sides + 1;

                tris[triNum++] = cornerBottomRight;
                tris[triNum++] = cornerBottomLeft;
                tris[triNum++] = cornerTopRight;

                tris[triNum++] = cornerTopRight;
                tris[triNum++] = cornerBottomLeft;
                tris[triNum++] = cornerTopLeft;
            }
        }
        return tris;
    }
    private static Vector2[] GenerateUVs(Output output, Vector3[] verts) {

        int sides = output.settings.sides;
        int length = output.radii.Length;

        Vector2[] uvs = new Vector2[verts.Length];
        for (int i = 0; i <= length - 1; i++) {
            for (int j = 0; j <= sides; j++) {
                float u = output.settings.scaleUV.x * j / sides;
                float v = output.settings.scaleUV.y * i / (length - 1); // FIXME

                uvs[i * (sides + 1) + j] = new Vector2(-u, v);
            }
        }
        return uvs;
    }
    private static Vector3[] GenerateNormals(Output output, Vector3[] points, Vector3[] verts) {
        Vector3[] normals = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++) {
            int ringNum = i / (output.settings.sides + 1); // sides + 1 (UV seams!)
            normals[i] = (verts[i] - points[ringNum]).normalized; // FIXME: calculation is incorrect
        }
        return normals;
    }
    private static Color[] GenerateColors(Output output, Vector3[] verts) {
        Color[] colors = new Color[verts.Length];
        for (int i = 0; i < colors.Length; i++) {
            float radius = output.radii[i / (output.settings.sides + 1)];
            float percent = 1 - (radius - output.settings.radiusMin) / (output.settings.radiusMax - output.settings.radiusMin);
            colors[i] = Color.Lerp(Color.black, Color.white, percent);
        }
        return colors;
    }

}
