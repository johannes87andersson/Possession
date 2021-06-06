using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public class FlyAIController : EnemyAIController, IPossesable
    {
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] public float waypointTolerance = 1f;
        [SerializeField] public float waypointDwellTime = 3f;
        [SerializeField] int currentWaypointIndex = 0;

        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private bool nextIndexCycled = false;
        private Vector2 lastPos;

        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            //base.Update();

            _transform.Translate(velocity * Time.deltaTime);

            transform.localPosition = transform.localPosition + new Vector3(0f, 2f * Mathf.Sin(Time.time * 2f) * Time.deltaTime, 0f);
            if (Vector2.Distance(transform.position, patrolPath.GetWaypoint(patrolPath.getCurrentIndex())) < 0.5f)
            {
                patrolPath.GetNextIndex(patrolPath.getCurrentIndex());
            }

            float moveInput = movementDirection;
            //velocity.x = Mathf.MoveTowards(velocity.x, waypoint.GetCurrentWaypoint().position.x, walkAcceleration * Time.deltaTime);

            Vector2 moveTo = Vector2.MoveTowards(transform.position, patrolPath.GetWaypoint(currentWaypointIndex), 2f * Time.deltaTime);
            transform.localPosition = new Vector3(moveTo.x, transform.localPosition.y, transform.localPosition.z);
            if (transform.position.x < lastPos.x)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }

            PatrolBehaviour();

            UpdateTimers();
            lastPos = transform.position;
        }

        protected void UpdateTimers()
        {
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        protected void PatrolBehaviour()
        {
            Vector3 nextPosition = transform.position;

            if (patrolPath != null)
            {
                if (AtWaypoint() && !nextIndexCycled)
                {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                nextIndexCycled = false;
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint <= waypointTolerance;
        }

        private void CycleWaypoint()
        {
            timeSinceArrivedAtWaypoint = 0f;
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
            nextIndexCycled = true;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        public bool OnPosses(PlayerController playerController)
        {
            playerController.GetComponent<SpriteRenderer>().enabled = false;
            playerController.GetComponent<BoxCollider2D>().enabled = false;
            return true;
        }

        public bool OnUnPosses(PlayerController playerController)
        {
            playerController.GetComponent<SpriteRenderer>().enabled = true;
            playerController.GetComponent<BoxCollider2D>().enabled = true;
            return false;
        }
    }
}