using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameManager.Instance.OnMultiPlayerGamePaused += KitchenGameManager_OnMultiplayerGamePaused;
        KitchenGameManager.Instance.OnMultiPlayerGameUnPaused += KitchenGameManager_OnMultiplayerGameUnPaused;

        Hide();
    }

    private void KitchenGameManager_OnMultiplayerGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    private void KitchenGameManager_OnMultiplayerGameUnPaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
