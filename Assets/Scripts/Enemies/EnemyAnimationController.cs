using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class EnemyAnimationController : MonoBehaviour
    {
        private Material mainMaterial;
        [SerializeField] private Material flashMaterial;

        private float flashDuration = 0.1f;

        private void Start()
        {
            mainMaterial = GetComponent<SpriteRenderer>().material;
        }

        public void OnHitAnim(SpriteRenderer sprite, bool isEnemyAlive)
        {
            if (sprite == null) return;

            sprite.DOFade(0.1f, flashDuration)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject)
                .OnStart(() =>
                {
                    float halfTime = flashDuration / 2f;
                    DOVirtual.DelayedCall(halfTime, () =>
                    {
                        sprite.material = flashMaterial;
                        // disable additional effects during hit flash
                    });
                })
                .OnComplete(() =>
                {
                    sprite.DOFade(1f, flashDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLink(gameObject)
                        .OnComplete(() =>
                        {
                            if (isEnemyAlive)
                            {
                                sprite.material = mainMaterial;
                                // re-enable additional effects after hit flash
                            }
                        });
                });
        }
    }
}
