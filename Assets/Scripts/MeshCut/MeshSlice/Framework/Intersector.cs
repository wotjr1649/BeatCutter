using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice
{
    public sealed class Intersector
    {

        public static bool Intersect(Plane pl, Line ln, out Vector3 q)
        {
            return Intersector.Intersect(pl, ln.positionA, ln.positionB, out q);
        }

        public static bool Intersect(Plane pl, Vector3 a, Vector3 b, out Vector3 q)
        {
            Vector3 normal = pl.normal;
            Vector3 ab = b - a;

            float t = (pl.dist - Vector3.Dot(normal, a)) / Vector3.Dot(normal, ab);

            if (t >= -float.Epsilon && t <= (1 + float.Epsilon))
            {
                q = a + t * ab;

                return true;
            }

            q = Vector3.zero;

            return false;
        }

        public static float TriArea2D(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);
        }

        public static void Intersect(Plane pl, Triangle tri, IntersectionResult result)
        {
            result.Clear();

            Vector3 a = tri.positionA;
            Vector3 b = tri.positionB;
            Vector3 c = tri.positionC;

            SideOfPlane sa = pl.SideOf(a);
            SideOfPlane sb = pl.SideOf(b);
            SideOfPlane sc = pl.SideOf(c);

            if (sa == sb && sb == sc)
            {
                return;
            }

            else if ((sa == SideOfPlane.ON && sa == sb) ||
                     (sa == SideOfPlane.ON && sa == sc) ||
                     (sb == SideOfPlane.ON && sb == sc))
            {
                return;
            }

            Vector3 qa;
            Vector3 qb;

            if (sa == SideOfPlane.ON)
            {
                if (Intersector.Intersect(pl, b, c, out qa))
                {
                    result.AddIntersectionPoint(qa);
                    result.AddIntersectionPoint(a);

                    Triangle ta = new Triangle(a, b, qa);
                    Triangle tb = new Triangle(a, qa, c);

                    if (tri.hasUV)
                    {
                        Vector2 pq = tri.GenerateUV(qa);
                        Vector2 pa = tri.uvA;
                        Vector2 pb = tri.uvB;
                        Vector2 pc = tri.uvC;

                        ta.SetUV(pa, pb, pq);
                        tb.SetUV(pa, pq, pc);
                    }

                    if (tri.hasNormal)
                    {
                        Vector3 pq = tri.GenerateNormal(qa);
                        Vector3 pa = tri.normalA;
                        Vector3 pb = tri.normalB;
                        Vector3 pc = tri.normalC;

                        ta.SetNormal(pa, pb, pq);
                        tb.SetNormal(pa, pq, pc);
                    }

                    if (tri.hasTangent)
                    {
                        Vector4 pq = tri.GenerateTangent(qa);
                        Vector4 pa = tri.tangentA;
                        Vector4 pb = tri.tangentB;
                        Vector4 pc = tri.tangentC;

                        ta.SetTangent(pa, pb, pq);
                        tb.SetTangent(pa, pq, pc);
                    }

                    if (sb == SideOfPlane.UP)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb);
                    }

                    else if (sb == SideOfPlane.DOWN)
                    {
                        result.AddUpperHull(tb).AddLowerHull(ta);
                    }
                }
            }

            else if (sb == SideOfPlane.ON)
            {
                if (Intersector.Intersect(pl, a, c, out qa))
                {
                    result.AddIntersectionPoint(qa);
                    result.AddIntersectionPoint(b);

                    Triangle ta = new Triangle(a, b, qa);
                    Triangle tb = new Triangle(qa, b, c);

                    if (tri.hasUV)
                    {
                        Vector2 pq = tri.GenerateUV(qa);
                        Vector2 pa = tri.uvA;
                        Vector2 pb = tri.uvB;
                        Vector2 pc = tri.uvC;

                        ta.SetUV(pa, pb, pq);
                        tb.SetUV(pq, pb, pc);
                    }

                    if (tri.hasNormal)
                    {
                        Vector3 pq = tri.GenerateNormal(qa);
                        Vector3 pa = tri.normalA;
                        Vector3 pb = tri.normalB;
                        Vector3 pc = tri.normalC;

                        ta.SetNormal(pa, pb, pq);
                        tb.SetNormal(pq, pb, pc);
                    }

                    if (tri.hasTangent)
                    {
                        Vector4 pq = tri.GenerateTangent(qa);
                        Vector4 pa = tri.tangentA;
                        Vector4 pb = tri.tangentB;
                        Vector4 pc = tri.tangentC;

                        ta.SetTangent(pa, pb, pq);
                        tb.SetTangent(pq, pb, pc);
                    }

                    if (sa == SideOfPlane.UP)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb);
                    }

                    else if (sa == SideOfPlane.DOWN)
                    {
                        result.AddUpperHull(tb).AddLowerHull(ta);
                    }
                }
            }

            else if (sc == SideOfPlane.ON)
            {
                if (Intersector.Intersect(pl, a, b, out qa))
                {
                    result.AddIntersectionPoint(qa);
                    result.AddIntersectionPoint(c);

                    Triangle ta = new Triangle(a, qa, c);
                    Triangle tb = new Triangle(qa, b, c);

                    if (tri.hasUV)
                    {
                        Vector2 pq = tri.GenerateUV(qa);
                        Vector2 pa = tri.uvA;
                        Vector2 pb = tri.uvB;
                        Vector2 pc = tri.uvC;

                        ta.SetUV(pa, pq, pc);
                        tb.SetUV(pq, pb, pc);
                    }

                    if (tri.hasNormal)
                    {
                        Vector3 pq = tri.GenerateNormal(qa);
                        Vector3 pa = tri.normalA;
                        Vector3 pb = tri.normalB;
                        Vector3 pc = tri.normalC;

                        ta.SetNormal(pa, pq, pc);
                        tb.SetNormal(pq, pb, pc);
                    }

                    if (tri.hasTangent)
                    {
                        Vector4 pq = tri.GenerateTangent(qa);
                        Vector4 pa = tri.tangentA;
                        Vector4 pb = tri.tangentB;
                        Vector4 pc = tri.tangentC;

                        ta.SetTangent(pa, pq, pc);
                        tb.SetTangent(pq, pb, pc);
                    }

                    if (sa == SideOfPlane.UP)
                    {
                        result.AddUpperHull(ta).AddLowerHull(tb);
                    }

                    else if (sa == SideOfPlane.DOWN)
                    {
                        result.AddUpperHull(tb).AddLowerHull(ta);
                    }
                }
            }

            else if (sa != sb && Intersector.Intersect(pl, a, b, out qa))
            {
                result.AddIntersectionPoint(qa);

                if (sa == sc)
                {

                    if (Intersector.Intersect(pl, b, c, out qb))
                    {
                        result.AddIntersectionPoint(qb);

                        Triangle ta = new Triangle(qa, b, qb);
                        Triangle tb = new Triangle(a, qa, qb);
                        Triangle tc = new Triangle(a, qb, c);

                        if (tri.hasUV)
                        {
                            Vector2 pqa = tri.GenerateUV(qa);
                            Vector2 pqb = tri.GenerateUV(qb);
                            Vector2 pa = tri.uvA;
                            Vector2 pb = tri.uvB;
                            Vector2 pc = tri.uvC;

                            ta.SetUV(pqa, pb, pqb);
                            tb.SetUV(pa, pqa, pqb);
                            tc.SetUV(pa, pqb, pc);
                        }

                        if (tri.hasNormal)
                        {
                            Vector3 pqa = tri.GenerateNormal(qa);
                            Vector3 pqb = tri.GenerateNormal(qb);
                            Vector3 pa = tri.normalA;
                            Vector3 pb = tri.normalB;
                            Vector3 pc = tri.normalC;

                            ta.SetNormal(pqa, pb, pqb);
                            tb.SetNormal(pa, pqa, pqb);
                            tc.SetNormal(pa, pqb, pc);
                        }

                        if (tri.hasTangent)
                        {
                            Vector4 pqa = tri.GenerateTangent(qa);
                            Vector4 pqb = tri.GenerateTangent(qb);
                            Vector4 pa = tri.tangentA;
                            Vector4 pb = tri.tangentB;
                            Vector4 pc = tri.tangentC;

                            ta.SetTangent(pqa, pb, pqb);
                            tb.SetTangent(pa, pqa, pqb);
                            tc.SetTangent(pa, pqb, pc);
                        }

                        if (sa == SideOfPlane.UP)
                        {
                            result.AddUpperHull(tb).AddUpperHull(tc).AddLowerHull(ta);
                        }
                        else
                        {
                            result.AddLowerHull(tb).AddLowerHull(tc).AddUpperHull(ta);
                        }
                    }
                }
                else
                {
                    if (Intersector.Intersect(pl, a, c, out qb))
                    {
                        result.AddIntersectionPoint(qb);

                        Triangle ta = new Triangle(a, qa, qb);
                        Triangle tb = new Triangle(qa, b, c);
                        Triangle tc = new Triangle(qb, qa, c);

                        if (tri.hasUV)
                        {

                            Vector2 pqa = tri.GenerateUV(qa);
                            Vector2 pqb = tri.GenerateUV(qb);
                            Vector2 pa = tri.uvA;
                            Vector2 pb = tri.uvB;
                            Vector2 pc = tri.uvC;

                            ta.SetUV(pa, pqa, pqb);
                            tb.SetUV(pqa, pb, pc);
                            tc.SetUV(pqb, pqa, pc);
                        }

                        if (tri.hasNormal)
                        {
                            Vector3 pqa = tri.GenerateNormal(qa);
                            Vector3 pqb = tri.GenerateNormal(qb);
                            Vector3 pa = tri.normalA;
                            Vector3 pb = tri.normalB;
                            Vector3 pc = tri.normalC;

                            ta.SetNormal(pa, pqa, pqb);
                            tb.SetNormal(pqa, pb, pc);
                            tc.SetNormal(pqb, pqa, pc);
                        }

                        if (tri.hasTangent)
                        {

                            Vector4 pqa = tri.GenerateTangent(qa);
                            Vector4 pqb = tri.GenerateTangent(qb);
                            Vector4 pa = tri.tangentA;
                            Vector4 pb = tri.tangentB;
                            Vector4 pc = tri.tangentC;

                            ta.SetTangent(pa, pqa, pqb);
                            tb.SetTangent(pqa, pb, pc);
                            tc.SetTangent(pqb, pqa, pc);
                        }

                        if (sa == SideOfPlane.UP)
                        {
                            result.AddUpperHull(ta).AddLowerHull(tb).AddLowerHull(tc);
                        }
                        else
                        {
                            result.AddLowerHull(ta).AddUpperHull(tb).AddUpperHull(tc);
                        }
                    }
                }
            }

            else if (Intersector.Intersect(pl, c, a, out qa) && Intersector.Intersect(pl, c, b, out qb))
            {

                result.AddIntersectionPoint(qa);
                result.AddIntersectionPoint(qb);

                Triangle ta = new Triangle(qa, qb, c);
                Triangle tb = new Triangle(a, qb, qa);
                Triangle tc = new Triangle(a, b, qb);

                if (tri.hasUV)
                {
                    Vector2 pqa = tri.GenerateUV(qa);
                    Vector2 pqb = tri.GenerateUV(qb);
                    Vector2 pa = tri.uvA;
                    Vector2 pb = tri.uvB;
                    Vector2 pc = tri.uvC;

                    ta.SetUV(pqa, pqb, pc);
                    tb.SetUV(pa, pqb, pqa);
                    tc.SetUV(pa, pb, pqb);
                }

                if (tri.hasNormal)
                {
                    Vector3 pqa = tri.GenerateNormal(qa);
                    Vector3 pqb = tri.GenerateNormal(qb);
                    Vector3 pa = tri.normalA;
                    Vector3 pb = tri.normalB;
                    Vector3 pc = tri.normalC;

                    ta.SetNormal(pqa, pqb, pc);
                    tb.SetNormal(pa, pqb, pqa);
                    tc.SetNormal(pa, pb, pqb);
                }

                if (tri.hasTangent)
                {
                    Vector4 pqa = tri.GenerateTangent(qa);
                    Vector4 pqb = tri.GenerateTangent(qb);
                    Vector4 pa = tri.tangentA;
                    Vector4 pb = tri.tangentB;
                    Vector4 pc = tri.tangentC;

                    ta.SetTangent(pqa, pqb, pc);
                    tb.SetTangent(pa, pqb, pqa);
                    tc.SetTangent(pa, pb, pqb);
                }

                if (sa == SideOfPlane.UP)
                {
                    result.AddUpperHull(tb).AddUpperHull(tc).AddLowerHull(ta);
                }
                else
                {
                    result.AddLowerHull(tb).AddLowerHull(tc).AddUpperHull(ta);
                }
            }
        }
    }
}