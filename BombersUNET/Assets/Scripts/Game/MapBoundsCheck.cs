﻿using UnityEngine;
using System.Collections;
using BrainCloudUNETExample.Game.PlayerInput;

namespace BrainCloudUNETExample.Game
{
    public class MapBoundsCheck : MonoBehaviour
    {
        public GameObject m_playerPlane;

        void Update()
        {
            if (m_playerPlane != null)
            {
                if (!GetComponent<Collider>().bounds.Contains(m_playerPlane.transform.position))
                {
                    GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().LeftBounds();
                }
                else
                {
                    GameObject.Find("PlayerController").GetComponent<BombersPlayerController>().EnteredBounds();
                }
            }
        }
    }
}