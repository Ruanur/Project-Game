using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footstepTImer;
    private float footstepTimerMax = .1f;
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepTImer -= Time.deltaTime;
        if (footstepTImer < 0f)
        {
            footstepTImer = footstepTimerMax;

            if (player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepSound(player.transform.position, volume);
            }
        }
    }
}
