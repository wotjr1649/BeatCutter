using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice {
    
    public sealed class IntersectionResult {
        
        private bool is_success;
        
        private readonly Triangle[] upper_hull;
        private readonly Triangle[] lower_hull;
        private readonly Vector3[] intersection_pt;
        
        private int upper_hull_count;
        private int lower_hull_count;
        private int intersection_pt_count;

        public IntersectionResult() {
            this.is_success = false;

            this.upper_hull = new Triangle[2];
            this.lower_hull = new Triangle[2];
            this.intersection_pt = new Vector3[2];

            this.upper_hull_count = 0;
            this.lower_hull_count = 0;
            this.intersection_pt_count = 0;
        }

        public Triangle[] upperHull {
            get { return upper_hull; }
        }

        public Triangle[] lowerHull {
            get { return lower_hull; }
        }

        public Vector3[] intersectionPoints {
            get { return intersection_pt; }
        }

        public int upperHullCount {
            get { return upper_hull_count; }
        }

        public int lowerHullCount {
            get { return lower_hull_count; }
        }

        public int intersectionPointCount {
            get { return intersection_pt_count; }
        }

        public bool isValid {
            get { return is_success; }
        }
        
        public IntersectionResult AddUpperHull(Triangle tri) {
            upper_hull[upper_hull_count++] = tri;

            is_success = true;

            return this;
        }
        
        public IntersectionResult AddLowerHull(Triangle tri) {
            lower_hull[lower_hull_count++] = tri;

            is_success = true;

            return this;
        }
        
        public void AddIntersectionPoint(Vector3 pt) {
            intersection_pt[intersection_pt_count++] = pt;
        }
        
        public void Clear() {
            is_success = false;
            upper_hull_count = 0;
            lower_hull_count = 0;
            intersection_pt_count = 0;
        }
        
        public void OnDebugDraw() {
            OnDebugDraw(Color.white);
        }

        public void OnDebugDraw(Color drawColor) {
#if UNITY_EDITOR

            if (!isValid) {
                return;
            }

            Color prevColor = Gizmos.color;

            Gizmos.color = drawColor;
            
            for (int i = 0; i < intersectionPointCount; i++) {
                Gizmos.DrawSphere(intersectionPoints[i], 0.1f);
            }
            
            for (int i = 0; i < upperHullCount; i++) {
                upperHull[i].OnDebugDraw(Color.red);
            }
            
            for (int i = 0; i < lowerHullCount; i++) {
                lowerHull[i].OnDebugDraw(Color.blue);
            }

            Gizmos.color = prevColor;

#endif
        }
    }
}