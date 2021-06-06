using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] private PatrolIndex[] patrolIndices;

        protected const float waypointGizmoRadius = 0.3f;

        protected int nextIndex = 0;
        protected int currentIndex = 0;

        protected virtual void Start()
        {

        }

        protected virtual void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public virtual int getCurrentIndex()
        {
            return currentIndex;
        }

        public virtual int GetNextIndex(int i)
        {
            nextIndex = (i + 1) % transform.childCount;
            return nextIndex;
        }

        public virtual Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).transform.position;
        }
    }
}
