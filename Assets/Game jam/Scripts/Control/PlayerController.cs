using System.Collections;
using UnityEngine;

using GameJam2021.Impact;
using UnityEngine.SceneManagement;
using Cinemachine;
using GameJam2021.UI;

namespace GameJam2021.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player physics settings")]
        [SerializeField] float speed = 5f;
        [SerializeField] float speedFraction = 1f;
        [SerializeField] float walkAcceleration = 0.02f;
        [SerializeField] float groundDeceleration = 0.06f;
        [SerializeField] float jumpHeight = 8f;
        [SerializeField] float airAcceleration = 0.01f;
        [SerializeField] LayerMask raycastLayerMask;
        [Header("Cinemachine virtual camera")]
        [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] Vector3 manipulateRayPos;
        [Header("On jump and land effects")]
        [SerializeField] GameObject jumpDust = null;
        [SerializeField] GameObject landDust = null;
        [Header("Transforms for raycast positions")]
        [SerializeField] Transform frontCheck = null;
        [SerializeField] Transform ledgeGrabCheck = null;
        [SerializeField] Transform ledgeGrabHeightCheck = null;
        [SerializeField] Transform groundCheck = null;
        [Header("Wallslide settings")]
        [SerializeField] float groundedRadius = 0.1f;
        [SerializeField] float checkRadius = 0.2f;
        [SerializeField] float wallSlidingSpeed = 1f;
        [SerializeField] LayerMask wallSlideLayer;
        [SerializeField] float xWallForce = 3f;
        [SerializeField] float YWallForce = 5f;
        [SerializeField] float wallJumpTime = 0.3f;
        [SerializeField] float ledgeGrabDistance = 0.3f;
        [SerializeField] LayerMask tilemapMask;
        [Header("Audio Settings")]
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip jumpClip = null;
        [SerializeField] AudioClip pickupCoinClip = null;
        [SerializeField] AudioClip possessClip = null;
        [SerializeField] AudioClip unPossessClip = null;
        [Header("Game Settings")]
        [SerializeField] GameManager gameManager = null;

        Transform _transform = null;
        Vector3 velocity;
        Rigidbody2D rbody = null;
        BoxCollider2D boxCollider;
        SpriteRenderer spriteRenderer = null;
        Animator animator = null;


        bool grounded;
        int animRunning, animJumping, animGrounded;
        bool isPossesing = false;
        GameObject possesedObject = null;
        bool canPlayLandDusct = false;
        bool isTouchingFront = false;
        bool isTouchingLedgeGrab = false;
        bool isTouchingLedgeHeightGrab = false;
        bool wallSliding = false;
        bool walljumping = false;
        bool edgeJumping = false;
        int collectedCoins = 0;

        private void Awake()
        {
            _transform = transform;
            velocity = new Vector3(0f, 0f, 0f);
            rbody = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            animRunning = Animator.StringToHash("Running");
            animJumping = Animator.StringToHash("Jumping");
            animGrounded = Animator.StringToHash("Grounded");
            lineRenderer.transform.position = lineRenderer.transform.position + manipulateRayPos;
        }

        // Update is called once per frame
        void Update()
        {
            bool isGrounded = Physics2D.Raycast(transform.position, -transform.up, 0.1f, tilemapMask);

            if (grounded)
            {
                velocity.y = 0;

                Jump();
            }

            if (!isPossesing)
            {
                velocity.y += (Physics2D.gravity.y * 1.5f) * Time.deltaTime;
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

            float moveInput = Input.GetAxisRaw("Horizontal");
            animator.SetFloat(animRunning, Mathf.Abs(moveInput));
            animator.SetFloat(animJumping, velocity.y);

            Flip(moveInput);

            velocity.x = Mathf.MoveTowards(velocity.x, (speed * moveInput) * speedFraction, walkAcceleration * Time.deltaTime);

            float acceleration = grounded ? walkAcceleration : airAcceleration;
            float deceleration = grounded ? groundDeceleration : 0;

            // Update the velocity assignment statements to use our selected
            // acceleration and deceleration values.
            if (moveInput != 0)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, (speed * moveInput) * speedFraction, acceleration * Time.deltaTime);
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
                            if (canPlayLandDusct)
                            {
                                AfterJumpDust();
                                canPlayLandDusct = false;
                            }
                            if (!animator.GetBool(animGrounded))
                            {
                                animator.SetBool(animGrounded, true);
                            }
                        }
                    }
                }
            }


            isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, wallSlideLayer);
            isTouchingLedgeGrab = Physics2D.Raycast(ledgeGrabCheck.position, transform.localScale.x * transform.right, ledgeGrabDistance, tilemapMask);
            isTouchingLedgeHeightGrab = Physics2D.Raycast(ledgeGrabHeightCheck.position, transform.localScale.x * transform.right, ledgeGrabDistance, tilemapMask);

            Debug.DrawLine(ledgeGrabCheck.position, ledgeGrabCheck.position + ((transform.localScale.x * transform.right) * ledgeGrabDistance), Color.yellow);
            Debug.DrawLine(ledgeGrabHeightCheck.position, ledgeGrabHeightCheck.position + ((transform.localScale.x * transform.right) * ledgeGrabDistance), Color.red);

            if (grounded == false && isTouchingLedgeGrab && !isTouchingLedgeHeightGrab)
            {
                if (!edgeJumping)
                {
                    velocity.y = 0;
                }
                if (Input.GetButtonDown("Jump"))
                {
                    edgeJumping = true;
                    velocity.y = 6f;
                    velocity.x = 0f;
                    Invoke("DisableEdgeJump", 0.3f);
                }
            }


            // Check if can wall slide
            if (isTouchingFront && !grounded && moveInput != 0)
            {
                wallSliding = true;
                velocity.x = 0;
                velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -wallSlidingSpeed, float.MaxValue));
            }
            else
            {
                wallSliding = false;
            }

            // Check if can wall jump
            if (!isGrounded && Input.GetButtonDown("Jump") && wallSliding)
            {
                walljumping = true;
            }

            if (!isGrounded && walljumping && !edgeJumping)
            {
                walljumping = false;
                velocity = new Vector3(xWallForce * -moveInput, YWallForce);
                PlaySound(jumpClip);
                Flip(moveInput);
            }

            _transform.Translate(velocity * Time.deltaTime);

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position + manipulateRayPos, transform.localScale.x * transform.right, 10f, raycastLayerMask);
            if (raycastHit2D)
            {
                //Debug.Log(raycastHit2D.collider.name);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    cinemachineVirtualCamera.Follow = raycastHit2D.collider.transform;
                    speedFraction = 0f;

                    IPossesable[] possesables = raycastHit2D.collider.GetComponents<IPossesable>();
                    foreach (IPossesable possesable in possesables)
                    {
                        possesable.OnPosses(this);
                        PlaySound(possessClip);
                    }

                    isPossesing = true;
                    possesedObject = raycastHit2D.collider.gameObject;
                    lineRenderer.SetPosition(1, new Vector3(10f, 0f, 0f));
                    lineRenderer.gameObject.SetActive(true);
                    StopCoroutine("HideLineRenderer");
                    StartCoroutine("HideLineRenderer", 0.2f);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    lineRenderer.SetPosition(1, new Vector3(10f, 0f, 0f));
                    lineRenderer.gameObject.SetActive(true);
                    StopCoroutine("HideLineRenderer");
                    StartCoroutine("HideLineRenderer", 0.2f);
                }
            }

            if (isPossesing && possesedObject != null && Input.GetKeyDown(KeyCode.Q))
            {
                IPossesable[] possesables = possesedObject.GetComponents<IPossesable>();
                foreach (IPossesable possesable in possesables)
                {
                    if (possesable.OnUnPosses(this))
                    {
                        PlaySound(unPossessClip);
                        speedFraction = 1f;
                        isPossesing = false;
                        possesedObject = null;
                        cinemachineVirtualCamera.Follow = transform;
                    }
                }
            }

            if (transform.position.y < -50f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }


        }

        private void Flip(float moveInput)
        {
            if (moveInput > 0)
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            else if (moveInput < 0)
                transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }

        private void DisableEdgeJump()
        {
            edgeJumping = false;
            walljumping = false;
        }

        private void SetWallJumpingToFalse()
        {
            walljumping = false;
        }

        private void EdgeSlideHandler(Collider2D hit)
        {
            EdgeSlide edgeSlide = hit.GetComponent<EdgeSlide>();
            hit.GetComponent<BoxCollider2D>().enabled = false;

            velocity.x = edgeSlide.right ? 4f : -4f;
            velocity.y = 6f;
            StopCoroutine("EnableColliderAfterTime");
            StartCoroutine(EnableColliderAfterTime(1f, hit));
        }

        void Jump()
        {
            // Jumping code we implemented earlier—no changes were made here.
            if (Input.GetButtonDown("Jump"))
            {
                BeforeJumpDust();
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                PlaySound(jumpClip);
                animator.SetBool(animGrounded, false);
                canPlayLandDusct = true;
            }
        }

        private void BeforeJumpDust()
        {
            //StopCoroutine("DestroyAfterUse");
            GameObject dust = Instantiate(jumpDust);
            dust.transform.position = transform.position;
            StartCoroutine("DestroyAfterUse", dust);
        }

        private void AfterJumpDust()
        {
            //StopCoroutine("DestroyAfterUse");
            GameObject dust = Instantiate(landDust);
            dust.transform.position = transform.position;
            StartCoroutine("DestroyAfterUse", dust);
        }


        IEnumerator DestroyAfterUse(GameObject go)
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(go);
        }

        IEnumerator HideLineRenderer(float time)
        {
            yield return new WaitForSeconds(time);
            lineRenderer.gameObject.SetActive(false);
        }

        IEnumerator EnableColliderAfterTime(float amountOfTime, Collider2D collider)
        {
            yield return new WaitForSeconds(amountOfTime);
            collider.GetComponent<BoxCollider2D>().enabled = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isPossesing && collision.collider.CompareTag("Trap"))
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
            if (collision.collider.CompareTag("SnesController"))
            {
                collision.collider.gameObject.SetActive(false);
                //gameManager.completeStageText.gameObject.SetActive(true);
                GetComponent<HUD>().ShowLevelComplete();
                GetComponent<HUD>().UpdateCoins(collectedCoins, GameObject.FindGameObjectsWithTag("Coin").Length);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Mushroom Jump"))
            {
                velocity.y = 12.5f;
                collision.transform.parent.GetComponent<EnemyAIController>().CrushMushroom();
                PlaySound(jumpClip);
            }
            if (collision.CompareTag("Coin") && collision.gameObject.GetComponent<SpriteRenderer>().enabled)
            {
                collectedCoins += 1;
                GetComponent<HUD>().OnPickupCoin(collectedCoins);
                PlaySound(pickupCoinClip, 0.5f);
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        private void PlaySound(AudioClip clip, float volume = 1.0f)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        void OnPickup(int pickupCoin)
        {
            Debug.Log(pickupCoin + " player");
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(frontCheck.position, checkRadius);
        }
    }
}
