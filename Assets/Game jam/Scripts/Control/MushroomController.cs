using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public class MushroomController : EnemyAIController, IPossesable
    {

        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip jumpClip = null;

        public bool possesed = false;
        public bool freezePosition = false;

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
            if (!possesed && freezePosition)
            {
                return;
            }
            if (!possesed)
            {
                base.Update();
                return;
            }
            //velocity.y += (Physics2D.gravity.y * 1.5f) * Time.deltaTime;

            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

            float moveInput = Input.GetAxisRaw("Horizontal");

            Flip(moveInput);

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

            if (Input.GetKeyDown(KeyCode.Z))
            {
                freezePosition = true;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                freezePosition = false;
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

        private void Flip(float moveInput)
        {
            if (moveInput > 0)
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            else if (moveInput < 0)
                transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
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
