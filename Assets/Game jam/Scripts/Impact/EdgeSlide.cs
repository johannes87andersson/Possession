using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Impact
{
    public class EdgeSlide : MonoBehaviour
    {

        [SerializeField] public bool right = false;
        [SerializeField] float powerX = 4f;
        [SerializeField] float powerY = 8f;

        public void Jump(Vector2 velocity)
        {

            velocity.x = right ? -powerX : powerX;
            velocity.y = powerY;
        }
    }
}