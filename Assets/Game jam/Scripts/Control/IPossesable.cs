using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam2021.Control
{
    public interface IPossesable
    {
        bool OnPosses(PlayerController playerController);

        bool OnUnPosses(PlayerController playerController);
    }
}
