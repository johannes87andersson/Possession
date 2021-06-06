using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public class RabitController : EnemyAIController, IPossesable
    {
        [SerializeField] float jumpHeight = 4f;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip jumpClip = null;

        public bool possesed = false;

        private int animJump = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            animJump = Animator.StringToHash("Jump");
        }

        protected override void Update()
        {
            if (!possesed)
            {
                return;
            }

            if (!grounded)
            {
                velocity.y += (Physics2D.gravity.y * 1.5f) * Time.deltaTime;
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

            float moveInput = Input.GetAxisRaw("Horizontal");

            Flip(moveInput);

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

            Jump();

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
                            velocity.x = 0;
                        }
                    }
                }
            }

            _transform.Translate(velocity * Time.deltaTime);
        }

        private void Jump()
        {
            if (!grounded)
                return;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                audioSource.Play();
                animator.SetTrigger(animJump);
                velocity.x = transform.localScale.x > 0 ? jumpHeight : -jumpHeight;
            }
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
            playerController.GetComponent<SpriteRenderer>().enabled = false;
            playerController.GetComponent<BoxCollider2D>().enabled = false;
            possesed = true;
            return true;
        }

        public bool OnUnPosses(PlayerController playerController)
        {
            playerController.GetComponent<SpriteRenderer>().enabled = true;
            playerController.GetComponent<BoxCollider2D>().enabled = true;
            possesed = false;
            return false;
        }
    }
}
