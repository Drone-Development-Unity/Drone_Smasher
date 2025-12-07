using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Allies.Gatherer
{
    public class BasicGatherer : MonoBehaviour
    {
        // configurable fields
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float collectionTime = 2.0f;
        [SerializeField] private float flyOverHeightOffset = 0.5f;
        private Vector2 basePosition;

        // states
        private enum State
        {
            FindingWrecks,
            FlyingTowardWreck,
            CollectingPartsFromWreck,
            ReturningToBase
        }

        private State currentState = State.FindingWrecks;
        private bool stateEntered = false;
        private bool isHovering = false;

        // references
        private Wreck currentWreck;
        private Tween currentTween;
        [SerializeField] private Transform visual;

        // wreck manager
        private WreckManager _wreckManager;

        //id
        private int id;

        // movement
        private float noiseOffsetX;
        private float noiseOffsetY;


        private void Start()
        {
            _wreckManager = WreckManager.Instance;
            basePosition = transform.position;
            id = GetInstanceID();

            noiseOffsetX = id * 13.37f;
            noiseOffsetY = id * 42.21f;
        }

        private void Update()
        {
            // shake visual
            float amplitude = (
                currentState == State.CollectingPartsFromWreck || currentState == State.FindingWrecks
                )  ? 0.08f : 0.04f;
            float noiseX = (Mathf.PerlinNoise(Time.time * 3f + noiseOffsetX, noiseOffsetY) - 0.5f) * amplitude;
            float noiseY = (Mathf.PerlinNoise(noiseOffsetX, Time.time * 2f + noiseOffsetY) - 0.5f) * (amplitude * 0.8f);

            visual.localPosition = new Vector3(noiseX, noiseY, 0f);

            // state machine update
            StateMachineUpdate();

            // hovering logic
            if (isHovering && currentWreck != null)
            {
                Vector3 offset = Vector3.up * flyOverHeightOffset;
                transform.position = currentWreck.transform.position + offset;
            }
        }

        public int GetID()
        {
            return id;
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
            if(state == State.FindingWrecks)
            {
                Update_FindingWrecks();
            }
        }

        private void ChangeState(State newState)
        {
            currentState = newState;
            stateEntered = false;

            // wreck lost check
            if (newState == State.FlyingTowardWreck || newState == State.CollectingPartsFromWreck)
            {
                StartCoroutine(CheckWreckLost());
            }
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

        // ------------------------
        // WRECK LOST 
        private IEnumerator CheckWreckLost()
        {
            while (
                currentState == State.FlyingTowardWreck ||
                currentState == State.CollectingPartsFromWreck
                )
            {
                if (currentWreck == null)
                {
                    // cancel tween
                    currentTween?.Kill();
                    currentTween = null;

                    ChangeState(State.ReturningToBase);
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
