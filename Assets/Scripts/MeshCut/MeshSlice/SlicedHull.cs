using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice
{

    public sealed class SlicedHull
    {
        private Mesh upper_hull;
        private Mesh lower_hull;

        public SlicedHull(Mesh upperHull, Mesh lowerHull)
        {
            this.upper_hull = upperHull;
            this.lower_hull = lowerHull;
        }

        public GameObject CreateUpperHull(GameObject original)
        {
            return CreateUpperHull(original, null);
        }

        public GameObject CreateUpperHull(GameObject original, Material crossSectionMat)
        {
            GameObject newObject = CreateUpperHull();

            if (newObject != null)
            {
                newObject.transform.localPosition = original.transform.localPosition;
                newObject.transform.localRotation = original.transform.localRotation;
                newObject.transform.localScale = original.transform.localScale;

                Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
                Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

                if (mesh.subMeshCount == upper_hull.subMeshCount)
                {
                    newObject.GetComponent<Renderer>().sharedMaterials = shared;

                    return newObject;
                }

                Material[] newShared = new Material[shared.Length + 1];

                System.Array.Copy(shared, newShared, shared.Length);
                newShared[shared.Length] = crossSectionMat;

                newObject.GetComponent<Renderer>().sharedMaterials = newShared;
            }

            return newObject;
        }

        public GameObject CreateLowerHull(GameObject original)
        {
            return CreateLowerHull(original, null);
        }

        public GameObject CreateLowerHull(GameObject original, Material crossSectionMat)
        {
            GameObject newObject = CreateLowerHull();

            if (newObject != null)
            {
                newObject.transform.localPosition = original.transform.localPosition;
                newObject.transform.localRotation = original.transform.localRotation;
                newObject.transform.localScale = original.transform.localScale;

                Material[] shared = original.GetComponent<MeshRenderer>().sharedMaterials;
                Mesh mesh = original.GetComponent<MeshFilter>().sharedMesh;

                if (mesh.subMeshCount == lower_hull.subMeshCount)
                {
                    newObject.GetComponent<Renderer>().sharedMaterials = shared;

                    return newObject;
                }

                Material[] newShared = new Material[shared.Length + 1];

                System.Array.Copy(shared, newShared, shared.Length);
                newShared[shared.Length] = crossSectionMat;

                newObject.GetComponent<Renderer>().sharedMaterials = newShared;
            }

            return newObject;
        }

        public GameObject CreateUpperHull()
        {
            return CreateEmptyObject("Upper_Hull", upper_hull);
        }

        public GameObject CreateLowerHull()
        {
            return CreateEmptyObject("Lower_Hull", lower_hull);
        }

        public Mesh upperHull
        {
            get { return this.upper_hull; }
        }

        public Mesh lowerHull
        {
            get { return this.lower_hull; }
        }

        private static GameObject CreateEmptyObject(string name, Mesh hull)
        {
            if (hull == null)
            {
                return null;
            }

            GameObject newObject = new GameObject(name);

            newObject.AddComponent<MeshRenderer>();
            MeshFilter filter = newObject.AddComponent<MeshFilter>();

            filter.mesh = hull;

            return newObject;
        }
    }
}