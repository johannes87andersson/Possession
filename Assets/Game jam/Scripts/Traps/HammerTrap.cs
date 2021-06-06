using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Trap
{
    public class HammerTrap : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        [SerializeField] float maxRotation = 35f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, maxRotation * Mathf.Sin(Time.time * speed));
        }
    }
}
