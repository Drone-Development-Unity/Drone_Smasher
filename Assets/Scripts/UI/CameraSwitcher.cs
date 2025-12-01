using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cele Kamery")] public Transform gameplayPosition;
    public Transform gatherersPosition;

    [Header("Ustawienia")] public float transitionSpeed = 2.0f; // Jak szybko kamera ma się przesuwać

    private bool isAtGatherers = false; //gatherers scene flag - may be in use later

    //Tween references
    private Tween moveTween;
    // Tę funkcję podepniesz pod przycisk
    public void ToggleCameraBasePosition()
    {
        if (isAtGatherers)
        {
            isAtGatherers = false;
            MoveCamera(gameplayPosition);
        }
    }

    public void ToggleCameraGatherersPosition()
    {
        if (!isAtGatherers)
        {
            isAtGatherers = true;
            MoveCamera(gatherersPosition);
        }
    }


    //Dotween animation
    public void MoveCamera(Transform target)
    {
        moveTween?.Kill();
        // Animacja pozycji
        moveTween = transform.DOMove(target.position, transitionSpeed)
            .SetEase(Ease.InOutQuad);

        /*// Opcjonalnie: animacja rotacji
        transform.DORotateQuaternion(target.rotation, transitionSpeed)
            .SetEase(Ease.InOutSine);*/
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
    }
}