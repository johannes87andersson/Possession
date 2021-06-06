using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public class WormController : EnemyAIController, IPossesable
    {

        public bool possesed = false;

        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        protected override void Start()
        {

        }

        // Update is called once per frame
        protected override void Update()
        {
            if (!possesed)
            {
                base.Update();
                return;
            }
            //velocity.y += (Physics2D.gravity.y * 1.5f) * Time.deltaTime;

            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

            float moveInput = Input.GetAxisRaw("Horizontal");

            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);

            float acceleration = grounded ? walkAcceleration : airAcceleration;
            float deceleration = grounded ? groundDeceleration : 0;

            if (moveInput != 0)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
            }

            grounded = false;
            foreach (Collider2D hit in hits)
            {
                if (hit == boxCollider)
                    continue;

                ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

                if (hit.isTrigger)
                {
                    // fall through

                }
                else
                {
                    if (colliderDistance.isOverlapped)
                    {
                        transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                        if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                        {
                            grounded = true;
                        }
                    }
                }
            }

            _transform.Translate(velocity * Time.deltaTime);
        }

        public bool OnPosses(PlayerController playerController)
        {
            possesed = true;
            playerController.GetComponent<SpriteRenderer>().enabled = false;
            playerController.GetComponent<BoxCollider2D>().enabled = true;
            return true;
        }

        public bool OnUnPosses(PlayerController playerController)
        {
            possesed = false;
            playerController.GetComponent<SpriteRenderer>().enabled = true;
            playerController.GetComponent<BoxCollider2D>().enabled = true;
            return false;
        }
    }
}
