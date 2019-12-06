using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice
{

    public sealed class Triangulator
    {

        internal struct Mapped2D
        {
            private readonly Vector3 original;
            private readonly Vector2 mapped;

            public Mapped2D(Vector3 newOriginal, Vector3 u, Vector3 v)
            {
                this.original = newOriginal;
                this.mapped = new Vector2(Vector3.Dot(newOriginal, u), Vector3.Dot(newOriginal, v));
            }

            public Vector2 mappedValue
            {
                get { return this.mapped; }
            }

            public Vector3 originalValue
            {
                get { return this.original; }
            }
        }

        public static bool MonotoneChain(List<Vector3> vertices, Vector3 normal, out List<Triangle> tri)
        {
            // default texture region is in coordinates 0,0 to 1,1
            return MonotoneChain(vertices, normal, out tri, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f));
        }

        public static bool MonotoneChain(List<Vector3> vertices, Vector3 normal, out List<Triangle> tri,
            TextureRegion texRegion)
        {
            int count = vertices.Count;

            if (count < 3)
            {
                tri = null;
                return false;
            }

            Vector3 u = Vector3.Normalize(Vector3.Cross(normal, Vector3.up));
            if (Vector3.zero == u)
            {
                u = Vector3.Normalize(Vector3.Cross(normal, Vector3.forward));
            }

            Vector3 v = Vector3.Cross(u, normal);

            Mapped2D[] mapped = new Mapped2D[count];

            float maxDivX = float.MinValue;
            float maxDivY = float.MinValue;
            float minDivX = float.MaxValue;
            float minDivY = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Vector3 vertToAdd = vertices[i];

                Mapped2D newMappedValue = new Mapped2D(vertToAdd, u, v);
                Vector2 mapVal = newMappedValue.mappedValue;

                maxDivX = Mathf.Max(maxDivX, mapVal.x);
                maxDivY = Mathf.Max(maxDivY, mapVal.y);
                minDivX = Mathf.Min(minDivX, mapVal.x);
                minDivY = Mathf.Min(minDivY, mapVal.y);

                mapped[i] = newMappedValue;
            }

            Array.Sort<Mapped2D>(mapped, (a, b) =>
            {
                Vector2 x = a.mappedValue;
                Vector2 p = b.mappedValue;

                return (x.x < p.x || (x.x == p.x && x.y < p.y)) ? -1 : 1;
            });

            Mapped2D[] hulls = new Mapped2D[count + 1];

            int k = 0;

            for (int i = 0; i < count; i++)
            {
                while (k >= 2)
                {
                    Vector2 mA = hulls[k - 2].mappedValue;
                    Vector2 mB = hulls[k - 1].mappedValue;
                    Vector2 mC = mapped[i].mappedValue;

                    if (Intersector.TriArea2D(mA.x, mA.y, mB.x, mB.y, mC.x, mC.y) > 0.0f)
                    {
                        break;
                    }

                    k--;
                }

                hulls[k++] = mapped[i];
            }

            for (int i = count - 2, t = k + 1; i >= 0; i--)
            {
                while (k >= t)
                {
                    Vector2 mA = hulls[k - 2].mappedValue;
                    Vector2 mB = hulls[k - 1].mappedValue;
                    Vector2 mC = mapped[i].mappedValue;

                    if (Intersector.TriArea2D(mA.x, mA.y, mB.x, mB.y, mC.x, mC.y) > 0.0f)
                    {
                        break;
                    }

                    k--;
                }

                hulls[k++] = mapped[i];
            }

            int vertCount = k - 1;
            int triCount = (vertCount - 2) * 3;

            if (vertCount < 3)
            {
                tri = null;
                return false;
            }

            tri = new List<Triangle>(triCount / 3);

            float width = maxDivX - minDivX;
            float height = maxDivY - minDivY;

            int indexCount = 1;

            for (int i = 0; i < triCount; i += 3)
            {
                Mapped2D posA = hulls[0];
                Mapped2D posB = hulls[indexCount];
                Mapped2D posC = hulls[indexCount + 1];

                Vector2 uvA = posA.mappedValue;
                Vector2 uvB = posB.mappedValue;
                Vector2 uvC = posC.mappedValue;

                uvA.x = (uvA.x - minDivX) / width;
                uvA.y = (uvA.y - minDivY) / height;

                uvB.x = (uvB.x - minDivX) / width;
                uvB.y = (uvB.y - minDivY) / height;

                uvC.x = (uvC.x - minDivX) / width;
                uvC.y = (uvC.y - minDivY) / height;

                Triangle newTriangle = new Triangle(posA.originalValue, posB.originalValue, posC.originalValue);

                newTriangle.SetUV(texRegion.Map(uvA), texRegion.Map(uvB), texRegion.Map(uvC));

                newTriangle.SetNormal(normal, normal, normal);
                newTriangle.ComputeTangents();

                tri.Add(newTriangle);

                indexCount++;
            }

            return true;
        }
    }
}