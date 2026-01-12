using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Enemies
{
    public class EnemyAnimationController : MonoBehaviour
    {
        [Header("Hit flash animation")]
        private Material mainMaterial;
        [SerializeField] private Material flashMaterial;

        private float flashDuration = 0.1f;
        private Tween tweenAnim; //Tween object (for handling destroy)

        [Header("Explosion particles")]
        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private ParticleSystem fragParticles;
        [SerializeField] private float particleScale = 1.0f;
        private void Start()
        {
            mainMaterial = GetComponent<SpriteRenderer>().material;
        }

        public void OnHitAnim(SpriteRenderer sprite, bool isEnemyAlive)
        {
            if (sprite == null) return;

            tweenAnim= sprite.DOFade(0.1f, flashDuration)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject)
                .OnStart(() =>
                {
                    float halfTime = flashDuration / 2f;
                    DOVirtual.DelayedCall(halfTime, () =>
                    {
                        if (sprite == null) return;
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
                                if (sprite == null) return;
                                sprite.material = mainMaterial;
                                // re-enable additional effects after hit flash
                            }
                        });
                });
        }

        public void RunExplosionParticles(Vector2? position = null)
        {
            Vector2 pos = position ?? (Vector2)transform.position;

            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

            ParticleSystem explosion = Instantiate(explosionParticles, pos, rotation);
            explosion.transform.localScale = Vector3.one * particleScale;
            explosion.transform.parent = null;
            explosion.Play();
            Destroy(explosion.gameObject, 3f);

            ParticleSystem frag = Instantiate(fragParticles, pos, rotation);
            frag.transform.localScale = Vector3.one * particleScale;
            frag.transform.parent = null;
            frag.Play();
            Destroy(frag.gameObject, 5f);
        }

        void OnDestroy()
        {
            if (tweenAnim != null && tweenAnim.IsActive())
            {
                tweenAnim.Kill();
            }
        }
    }
}
