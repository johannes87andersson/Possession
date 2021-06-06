using GameJam2021.Impact;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public class EnemyAIController : MonoBehaviour
    {

        [SerializeField] protected float speedFraction = 1f;
        [SerializeField] protected float walkAcceleration = 0.02f;
        [SerializeField] protected float groundDeceleration = 0.06f;
        [SerializeField] protected float airAcceleration = 0.01f;

        protected bool isRight = true;

        protected Transform _transform;
        protected Vector2 velocity;
        protected bool grounded = false;
        protected BoxCollider2D boxCollider;
        protected SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected float speed = 1.5f;
        protected float movementDirection = 1f;
        private int animCrush;

        protected virtual void Awake()
        {
            _transform = transform;
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            animCrush = Animator.StringToHash("Crush");
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // COPY
            if (grounded)
            {
                velocity.y = 0;
            }
            velocity.y += (Physics2D.gravity.y * 1.5f) * Time.deltaTime;

            _transform.Translate(velocity * Time.deltaTime);

            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

            float moveInput = movementDirection;

            Flip(moveInput);

            velocity.x = Mathf.MoveTowards(velocity.x, (speed * moveInput) * speedFraction, walkAcceleration * Time.deltaTime);

            float acceleration = grounded ? walkAcceleration : airAcceleration;
            float deceleration = grounded ? groundDeceleration : 0;

            // Update the velocity assignment statements to use our selected
            // acceleration and deceleration values.
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
                    if (hit.CompareTag("Turn Around"))
                    {
                        movementDirection = hit.GetComponent<ChangeDirection>().turnRight ? 1f : -1f;
                    }
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
            // END COPY
        }

        private void Flip(float moveInput)
        {
            if (moveInput > 0)
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            else if (moveInput < 0)
                transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            grounded = true;
        }

        public void CrushMushroom()
        {
            animator.SetTrigger(animCrush);
            velocity = new Vector2(0, 0);
        }
    }
}
