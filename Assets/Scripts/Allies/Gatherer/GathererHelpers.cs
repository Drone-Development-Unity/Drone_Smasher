using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Allies.Gatherer
{
    static public class GathererHelpers
    {
        static public void StartMovementTween(
            Vector3 target, Tween currentTween, Transform gathererPosition, float speed, 
            TweenCallback onComplete
        )
        {

            currentTween?.Kill();
            currentTween = null;

            float distance = Vector2.Distance(gathererPosition.position, target);
            float duration = distance / speed;

            currentTween = gathererPosition
                .DOMove(target, duration)
                .SetEase(Ease.Linear)
                .OnComplete(onComplete);
        }

        static public Vector3 PredictFuturePosition(
            Wreck wreck, Transform gathererPosition, float speed
        )
        {
            float distance = Vector2.Distance(gathererPosition.position, wreck.transform.position);
            float timeToReach = distance / speed;

            float fallSpeed = wreck.FallSpeed; // musisz udostępnić fallSpeed jako public/protected

            Vector3 predicted = wreck.transform.position;
            predicted.y -= fallSpeed * timeToReach;

            return predicted;
        }
    }
}
