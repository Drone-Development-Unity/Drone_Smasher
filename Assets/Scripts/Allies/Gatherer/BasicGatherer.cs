using DG.Tweening;
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
        private bool stateEntered = false;

        private Vector2 basePosition = new Vector2(-4.1f, -6.6f);

        private Wreck currentWreck;
        private Tween currentTween;
        private bool isHovering = false;

        private WreckManager _wreckManager;

        private void Start()
        {
            _wreckManager = WreckManager.Instance;
        }

        private void Update()
        {
            StateMachineUpdate();

            if (isHovering && currentWreck != null)
            {
                Vector3 offset = Vector3.up * flyOverHeightOffset;
                transform.position = currentWreck.transform.position + offset;
            }
        }


        // set or update state machine
        private void StateMachineUpdate()
        {
            if (!stateEntered)
            {
                EnterState(currentState);
                stateEntered = true;
            }

            UpdateState(currentState);
        }

        // enter state
        private void EnterState(State state)
        {
            switch (state)
            {
                case State.FindingWrecks: break;
                case State.FlyingTowardWreck: Enter_FlyingTowardWreck(); break;
                case State.CollectingPartsFromWreck: Enter_CollectingPartsFromWreck(); break;
                case State.ReturningToBase: Enter_ReturningToBase(); break;
            }
        }

        // update state - only FindingWreks
        private void UpdateState(State state)
        {
            switch (state)
            {
                case State.FindingWrecks: Update_FindingWrecks(); break;
                case State.FlyingTowardWreck: break;
                case State.CollectingPartsFromWreck: break;
                case State.ReturningToBase: break;
            }
        }

        private void ChangeState(State newState)
        {
            currentState = newState;
            stateEntered = false;
        }

        // ------------------------
        // FINDING WRECKS
        private void Update_FindingWrecks()
        {
            if (currentWreck != null) return;

            currentWreck = _wreckManager.ReserveClosest(transform.position);

            if (currentWreck != null)
                ChangeState(State.FlyingTowardWreck);
        }

        // ------------------------
        // FLY TOWARD WRECK
        private void Enter_FlyingTowardWreck()
        {
            if (currentWreck == null)
            {
                ChangeState(State.ReturningToBase);
                return;
            }

            Vector3 predictedPosition = GathererHelpers.PredictFuturePosition(
                currentWreck, transform, speed
            );

            currentTween = GathererHelpers.StartMovementTween(predictedPosition, currentTween, transform, speed, () =>
            {
                ChangeState(State.CollectingPartsFromWreck);
            });
        }

        // ------------------------
        // COLLECT
        private void Enter_CollectingPartsFromWreck()
        {
            if (currentWreck == null)
            {
                ChangeState(State.ReturningToBase);
                return;
            }

            Vector3 predictedPosition = GathererHelpers.PredictFuturePosition(
                currentWreck, transform, speed
            );

            Vector3 target = predictedPosition + Vector3.up * flyOverHeightOffset;

            // fly over wreck and hover
            currentTween = GathererHelpers.StartMovementTween(target, currentTween, transform, speed, () =>
            {
                isHovering = true;

                // wait collection time
                DOVirtual.DelayedCall(collectionTime, () =>
                {
                    // finish collection logic
                    isHovering = false;

                    if (currentWreck == null)
                    {
                        ChangeState(State.ReturningToBase);
                        return;
                    }

                    var drop = currentWreck.GetComponent<DropCurrency>();
                    if (drop != null) drop.TryGetCurrency();

                    currentWreck.IsTracked = false;
                    _wreckManager.RemoveWreck(currentWreck);
                    currentWreck = null;

                    ChangeState(State.ReturningToBase);

                });
            });
        }

        // ------------------------
        // RETURN TO BASE
        private void Enter_ReturningToBase()
        {
            currentTween = GathererHelpers.StartMovementTween(basePosition, currentTween, transform, speed, () =>
            {
                ChangeState(State.FindingWrecks);
            });
        }
    }
}
