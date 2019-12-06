using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSlice {
    
    public enum SideOfPlane {
        UP,
        DOWN,
        ON
    }
    
    public struct Plane {
        private Vector3 m_normal;
        private float m_dist;
#if UNITY_EDITOR
        private Transform trans_ref;
#endif

        public Plane(Vector3 pos, Vector3 norm) {
            this.m_normal = norm;
            this.m_dist = Vector3.Dot(norm, pos);
            
#if UNITY_EDITOR
            trans_ref = null;
#endif
        }

        public Plane(Vector3 norm, float dot) {
            this.m_normal = norm;
            this.m_dist = dot;
            
#if UNITY_EDITOR
            trans_ref = null;
#endif
        }

        public void Compute(Vector3 pos, Vector3 norm) {
            this.m_normal = norm;
            this.m_dist = Vector3.Dot(norm, pos);
        }

        public void Compute(Transform trans) {
            Compute(trans.position, trans.up);
            
#if UNITY_EDITOR
            trans_ref = trans;
#endif
        }

        public void Compute(GameObject obj) {
            Compute(obj.transform);
        }

        public Vector3 normal {
            get { return this.m_normal; }
        }

        public float dist {
            get { return this.m_dist; }
        }
        public SideOfPlane SideOf(Vector3 pt) {
            float result = Vector3.Dot(m_normal, pt) - m_dist;

            if (result > float.Epsilon) {
                return SideOfPlane.UP;
            }

            if (result < -float.Epsilon) {
                return SideOfPlane.DOWN;
            }

            return SideOfPlane.ON;
        }

        public void OnDebugDraw() {
            OnDebugDraw(Color.white);
        }

        public void OnDebugDraw(Color drawColor) {

#if UNITY_EDITOR

            if (trans_ref == null) {
                return;
            }

            Color prevColor = Gizmos.color;
            Matrix4x4 prevMatrix = Gizmos.matrix;
            
            Gizmos.matrix = Matrix4x4.TRS(trans_ref.position, trans_ref.rotation, trans_ref.localScale);
            Gizmos.color = drawColor;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 0.0f, 1.0f));

            Gizmos.color = prevColor;
            Gizmos.matrix = prevMatrix;

#endif
        }
    }
}