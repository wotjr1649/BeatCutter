using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice
{


    public sealed class Slicer
    {

        internal class SlicedSubmesh
        {
            public readonly List<Triangle> upperHull = new List<Triangle>();
            public readonly List<Triangle> lowerHull = new List<Triangle>();

            public bool hasUV
            {
                get
                {
                    return upperHull.Count > 0 ? upperHull[0].hasUV : lowerHull.Count > 0 ? lowerHull[0].hasUV : false;
                }
            }

            public bool hasNormal
            {
                get
                {
                    return upperHull.Count > 0 ? upperHull[0].hasNormal :
                        lowerHull.Count > 0 ? lowerHull[0].hasNormal : false;
                }
            }

            public bool hasTangent
            {
                get
                {
                    return upperHull.Count > 0 ? upperHull[0].hasTangent :
                        lowerHull.Count > 0 ? lowerHull[0].hasTangent : false;
                }
            }

            public bool isValid
            {
                get { return upperHull.Count > 0 && lowerHull.Count > 0; }
            }
        }

        public static SlicedHull Slice(GameObject obj, Plane pl, TextureRegion crossRegion, Material crossMaterial)
        {
            MeshFilter filter = obj.GetComponent<MeshFilter>();
            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            Material[] materials = renderer.sharedMaterials;
            Mesh mesh = filter.sharedMesh;

            int submeshCount = mesh.subMeshCount;
            int crossIndex = materials.Length;

            if (crossMaterial != null)
            {
                for (int i = 0; i < crossIndex; i++)
                {
                    if (materials[i] == crossMaterial)
                    {
                        crossIndex = i;
                        break;
                    }
                }
            }

            return Slice(mesh, pl, crossRegion, crossIndex);
        }

        public static SlicedHull Slice(Mesh sharedMesh, Plane pl, TextureRegion region, int crossIndex)
        {
            if (sharedMesh == null)
            {
                return null;
            }

            Vector3[] verts = sharedMesh.vertices;
            Vector2[] uv = sharedMesh.uv;
            Vector3[] norm = sharedMesh.normals;
            Vector4[] tan = sharedMesh.tangents;

            int submeshCount = sharedMesh.subMeshCount;

            SlicedSubmesh[] slices = new SlicedSubmesh[submeshCount];
            List<Vector3> crossHull = new List<Vector3>();

            IntersectionResult result = new IntersectionResult();

            bool genUV = verts.Length == uv.Length;
            bool genNorm = verts.Length == norm.Length;
            bool genTan = verts.Length == tan.Length;

            for (int submesh = 0; submesh < submeshCount; submesh++)
            {
                int[] indices = sharedMesh.GetTriangles(submesh);
                int indicesCount = indices.Length;

                SlicedSubmesh mesh = new SlicedSubmesh();

                for (int index = 0; index < indicesCount; index += 3)
                {
                    int i0 = indices[index + 0];
                    int i1 = indices[index + 1];
                    int i2 = indices[index + 2];

                    Triangle newTri = new Triangle(verts[i0], verts[i1], verts[i2]);

                    if (genUV)
                    {
                        newTri.SetUV(uv[i0], uv[i1], uv[i2]);
                    }

                    if (genNorm)
                    {
                        newTri.SetNormal(norm[i0], norm[i1], norm[i2]);
                    }

                    if (genTan)
                    {
                        newTri.SetTangent(tan[i0], tan[i1], tan[i2]);
                    }

                    if (newTri.Split(pl, result))
                    {
                        int upperHullCount = result.upperHullCount;
                        int lowerHullCount = result.lowerHullCount;
                        int interHullCount = result.intersectionPointCount;

                        for (int i = 0; i < upperHullCount; i++)
                        {
                            mesh.upperHull.Add(result.upperHull[i]);
                        }

                        for (int i = 0; i < lowerHullCount; i++)
                        {
                            mesh.lowerHull.Add(result.lowerHull[i]);
                        }

                        for (int i = 0; i < interHullCount; i++)
                        {
                            crossHull.Add(result.intersectionPoints[i]);
                        }
                    }
                    else
                    {
                        SideOfPlane side = pl.SideOf(verts[i0]);

                        if (side == SideOfPlane.UP || side == SideOfPlane.ON)
                        {
                            mesh.upperHull.Add(newTri);
                        }
                        else
                        {
                            mesh.lowerHull.Add(newTri);
                        }
                    }
                }

                slices[submesh] = mesh;
            }

            for (int i = 0; i < slices.Length; i++)
            {
                if (slices[i] != null && slices[i].isValid)
                {
                    return CreateFrom(slices, CreateFrom(crossHull, pl.normal, region), crossIndex);
                }
            }

            return null;
        }

        private static SlicedHull CreateFrom(SlicedSubmesh[] meshes, List<Triangle> cross, int crossSectionIndex)
        {
            int submeshCount = meshes.Length;

            int upperHullCount = 0;
            int lowerHullCount = 0;

            for (int submesh = 0; submesh < submeshCount; submesh++)
            {
                upperHullCount += meshes[submesh].upperHull.Count;
                lowerHullCount += meshes[submesh].lowerHull.Count;
            }

            Mesh upperHull = CreateUpperHull(meshes, upperHullCount, cross, crossSectionIndex);
            Mesh lowerHull = CreateLowerHull(meshes, lowerHullCount, cross, crossSectionIndex);

            return new SlicedHull(upperHull, lowerHull);
        }

        private static Mesh CreateUpperHull(SlicedSubmesh[] mesh, int total, List<Triangle> crossSection,
            int crossSectionIndex)
        {
            return CreateHull(mesh, total, crossSection, crossSectionIndex, true);
        }

        private static Mesh CreateLowerHull(SlicedSubmesh[] mesh, int total, List<Triangle> crossSection,
            int crossSectionIndex)
        {
            return CreateHull(mesh, total, crossSection, crossSectionIndex, false);
        }

        private static Mesh CreateHull(SlicedSubmesh[] meshes, int total, List<Triangle> crossSection, int crossIndex,
            bool isUpper)
        {
            if (total <= 0)
            {
                return null;
            }

            int submeshCount = meshes.Length;
            int crossCount = crossSection != null ? crossSection.Count : 0;

            Mesh newMesh = new Mesh();

            int arrayLen = (total + crossCount) * 3;

            bool hasUV = meshes[0].hasUV;
            bool hasNormal = meshes[0].hasNormal;
            bool hasTangent = meshes[0].hasTangent;

            Vector3[] newVertices = new Vector3[arrayLen];
            Vector2[] newUvs = hasUV ? new Vector2[arrayLen] : null;
            Vector3[] newNormals = hasNormal ? new Vector3[arrayLen] : null;
            Vector4[] newTangents = hasTangent ? new Vector4[arrayLen] : null;

            List<int[]> triangles = new List<int[]>(submeshCount);

            int vIndex = 0;

            for (int submesh = 0; submesh < submeshCount; submesh++)
            {
                List<Triangle> hull = isUpper ? meshes[submesh].upperHull : meshes[submesh].lowerHull;
                int hullCount = hull.Count;

                int[] indices = new int[hullCount * 3];

                for (int i = 0, triIndex = 0; i < hullCount; i++, triIndex += 3)
                {
                    Triangle newTri = hull[i];

                    int i0 = vIndex + 0;
                    int i1 = vIndex + 1;
                    int i2 = vIndex + 2;

                    newVertices[i0] = newTri.positionA;
                    newVertices[i1] = newTri.positionB;
                    newVertices[i2] = newTri.positionC;

                    if (hasUV)
                    {
                        newUvs[i0] = newTri.uvA;
                        newUvs[i1] = newTri.uvB;
                        newUvs[i2] = newTri.uvC;
                    }

                    if (hasNormal)
                    {
                        newNormals[i0] = newTri.normalA;
                        newNormals[i1] = newTri.normalB;
                        newNormals[i2] = newTri.normalC;
                    }

                    if (hasTangent)
                    {
                        newTangents[i0] = newTri.tangentA;
                        newTangents[i1] = newTri.tangentB;
                        newTangents[i2] = newTri.tangentC;
                    }

                    indices[triIndex] = i0;
                    indices[triIndex + 1] = i1;
                    indices[triIndex + 2] = i2;

                    vIndex += 3;
                }

                triangles.Add(indices);
            }

            if (crossSection != null && crossCount > 0)
            {
                int[] crossIndices = new int[crossCount * 3];

                for (int i = 0, triIndex = 0; i < crossCount; i++, triIndex += 3)
                {
                    Triangle newTri = crossSection[i];

                    int i0 = vIndex + 0;
                    int i1 = vIndex + 1;
                    int i2 = vIndex + 2;

                    newVertices[i0] = newTri.positionA;
                    newVertices[i1] = newTri.positionB;
                    newVertices[i2] = newTri.positionC;

                    if (hasUV)
                    {
                        newUvs[i0] = newTri.uvA;
                        newUvs[i1] = newTri.uvB;
                        newUvs[i2] = newTri.uvC;
                    }

                    if (hasNormal)
                    {
                        if (isUpper)
                        {
                            newNormals[i0] = -newTri.normalA;
                            newNormals[i1] = -newTri.normalB;
                            newNormals[i2] = -newTri.normalC;
                        }
                        else
                        {
                            newNormals[i0] = newTri.normalA;
                            newNormals[i1] = newTri.normalB;
                            newNormals[i2] = newTri.normalC;
                        }
                    }

                    if (hasTangent)
                    {
                        newTangents[i0] = newTri.tangentA;
                        newTangents[i1] = newTri.tangentB;
                        newTangents[i2] = newTri.tangentC;
                    }

                    if (isUpper)
                    {
                        crossIndices[triIndex] = i0;
                        crossIndices[triIndex + 1] = i1;
                        crossIndices[triIndex + 2] = i2;
                    }
                    else
                    {
                        crossIndices[triIndex] = i0;
                        crossIndices[triIndex + 1] = i2;
                        crossIndices[triIndex + 2] = i1;
                    }

                    vIndex += 3;
                }

                if (triangles.Count <= crossIndex)
                {
                    triangles.Add(crossIndices);
                }
                else
                {
                    int[] prevTriangles = triangles[crossIndex];
                    int[] merged = new int[prevTriangles.Length + crossIndices.Length];

                    System.Array.Copy(prevTriangles, merged, prevTriangles.Length);
                    System.Array.Copy(crossIndices, 0, merged, prevTriangles.Length, crossIndices.Length);
                    triangles[crossIndex] = merged;
                }
            }

            int totalTriangles = triangles.Count;

            newMesh.subMeshCount = totalTriangles;
            newMesh.vertices = newVertices;

            if (hasUV)
            {
                newMesh.uv = newUvs;
            }

            if (hasNormal)
            {
                newMesh.normals = newNormals;
            }

            if (hasTangent)
            {
                newMesh.tangents = newTangents;
            }

            for (int i = 0; i < totalTriangles; i++)
            {
                newMesh.SetTriangles(triangles[i], i, false);
            }

            return newMesh;
        }

        private static List<Triangle> CreateFrom(List<Vector3> intPoints, Vector3 planeNormal, TextureRegion region)
        {
            List<Triangle> tris;

            if (Triangulator.MonotoneChain(intPoints, planeNormal, out tris, region))
            {
                return tris;
            }

            return null;
        }
    }
}