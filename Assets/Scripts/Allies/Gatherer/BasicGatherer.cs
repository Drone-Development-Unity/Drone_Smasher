using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Allies.Gatherer
{
    public class BasicGatherer : MonoBehaviour
    {
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float collectionTime = 2.0f;
        [SerializeField] private float flyOverHeightOffset = 0.5f;
        private enum State
        {
            FindingWrecks,
            FlyingTowardWreck,
            CollectingPartsFromWreck,
            ReturningToBase
        }
        private State currentState = State.FindingWrecks;
        private Vector2 basePosition = new Vector2(-4.1f, -6.6f);

        private Wreck currentWreck;
        private Tween currentTween;

        // hover control
        private bool isHovering = false;


        private void Update()
        {
            UpdateStateMachine();

            // auto update hover position
            if (isHovering && currentWreck != null)
            {
                Vector3 offset = Vector3.up * flyOverHeightOffset * Time.deltaTime;
                Vector3 hoverPos = currentWreck.transform.position + offset;

                transform.position = hoverPos;
            }
        }

        private void UpdateStateMachine()
        {

            switch (currentState)
            {
                case State.FindingWrecks:
                    FindWrecks();
                    break;

                case State.FlyingTowardWreck:
                    FlyTowardWreck();
                    break;

                case State.CollectingPartsFromWreck:
                    CollectPartsFromWreck();
                    break;

                case State.ReturningToBase:
                    ReturnToBase();
                    break;
            }
        }

        // find wrecks in the scene that are not tracked
        private void FindWrecks()
        {
            var wrecks = FindObjectsByType<Wreck>(FindObjectsSortMode.None)
                .Where(w => !w.IsTracked)
                .ToList();

            if (wrecks.Count == 0) return;


            // choose the closest wreck
            currentWreck = wrecks
                .OrderBy(w => Vector2.Distance(transform.position, w.transform.position))
                .FirstOrDefault();

            if (currentWreck != null)
            {
                currentWreck.IsTracked = true;
                currentState = State.FlyingTowardWreck;
            }
        }

        // set target position to the wreck's position
        private void FlyTowardWreck()
        {
            if (currentWreck == null)
            {
                GoBackToBase();
                return;
            }

            // position prediction
            Vector3 predictedPos = GathererHelpers.PredictFuturePosition(currentWreck, transform, speed);

            GathererHelpers.StartMovementTween(predictedPos, currentTween, transform, speed, () =>
            {
                if (currentWreck == null)
                {
                    GoBackToBase();
                    return;
                }

                currentState = State.CollectingPartsFromWreck;
            });
        }

        // fly over wreck for some time
        private void CollectPartsFromWreck()
        {
            if (currentWreck == null)
            {
                GoBackToBase();
                return;
            }

            // celecting position above the wreck
            Vector3 flyOverPos = currentWreck.transform.position + Vector3.up * flyOverHeightOffset;

            currentTween?.Kill();

            GathererHelpers.StartMovementTween(flyOverPos, currentTween, transform, speed, () =>
            {
                if (currentWreck == null)
                {
                    GoBackToBase();
                    return;
                }
                isHovering = true;

                // delayed call after collectionTime
                currentTween = DOVirtual.DelayedCall(collectionTime, () =>
                {
                    isHovering = false;
                    if (currentWreck == null)
                    {
                        GoBackToBase();
                        return;
                    }
                    //collect wreck resources
                    var drop = currentWreck.GetComponent<DropCurrency>();
                    if(drop != null) drop.TryGetCurrency();
                    
                    
                    currentWreck.IsTracked = false;
                    // or replace with other texture
                    Destroy(currentWreck.gameObject);

                    GoBackToBase();

                }, false);
            });

            
        }

        // go back to base
        private void ReturnToBase()
        {
            GathererHelpers.StartMovementTween(basePosition, currentTween, transform, speed, () =>
            {
                currentState = State.FindingWrecks;
            });
        }

        private void GoBackToBase()
        {
            isHovering = false;

            currentWreck = null;

            currentState = State.ReturningToBase;

        }

    }
}
