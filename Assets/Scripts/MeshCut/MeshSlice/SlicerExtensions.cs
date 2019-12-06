using System.Collections;
using UnityEngine;

namespace MeshSlice
{
    public static class SlicerExtensions
    {

        public static SlicedHull Slice(this GameObject obj, Plane pl, Material crossSectionMaterial = null)
        {
            return Slice(obj, pl, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);
        }

        public static SlicedHull Slice(this GameObject obj, Vector3 position, Vector3 direction,
            Material crossSectionMaterial = null)
        {
            return Slice(obj, position, direction, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);
        }

        public static SlicedHull Slice(this GameObject obj, Vector3 position, Vector3 direction,
            TextureRegion textureRegion, Material crossSectionMaterial = null)
        {
            Plane cuttingPlane = new Plane();

            Vector3 refUp = obj.transform.InverseTransformDirection(direction);
            Vector3 refPt = obj.transform.InverseTransformPoint(position);

            cuttingPlane.Compute(refPt, refUp);

            return Slice(obj, cuttingPlane, textureRegion, crossSectionMaterial);
        }

        public static SlicedHull Slice(this GameObject obj, Plane pl, TextureRegion textureRegion,
            Material crossSectionMaterial = null)
        {
            return Slicer.Slice(obj, pl, textureRegion, crossSectionMaterial);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Plane pl)
        {
            return SliceInstantiate(obj, pl, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f));
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction)
        {
            return SliceInstantiate(obj, position, direction, null);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction,
            Material crossSectionMat)
        {
            return SliceInstantiate(obj, position, direction, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f),
                crossSectionMat);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Vector3 position, Vector3 direction,
            TextureRegion cuttingRegion, Material crossSectionMaterial = null)
        {
            Plane cuttingPlane = new Plane();

            Vector3 refUp = obj.transform.InverseTransformDirection(direction);
            Vector3 refPt = obj.transform.InverseTransformPoint(position);

            cuttingPlane.Compute(refPt, refUp);

            return SliceInstantiate(obj, cuttingPlane, cuttingRegion, crossSectionMaterial);
        }

        public static GameObject[] SliceInstantiate(this GameObject obj, Plane pl, TextureRegion cuttingRegion,
            Material crossSectionMaterial = null)
        {
            SlicedHull slice = Slicer.Slice(obj, pl, cuttingRegion, crossSectionMaterial);

            if (slice == null)
            {
                return null;
            }

            GameObject upperHull = slice.CreateUpperHull(obj, crossSectionMaterial);
            GameObject lowerHull = slice.CreateLowerHull(obj, crossSectionMaterial);

            if (upperHull != null && lowerHull != null)
            {
                return new GameObject[] {upperHull, lowerHull};
            }

            if (upperHull != null)
            {
                return new GameObject[] {upperHull};
            }

            if (lowerHull != null)
            {
                return new GameObject[] {lowerHull};
            }

            return null;
        }
    }
}