using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.Bullets
{
    public class BulletAnimationController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem sparksParticleSystem;

        public void PlaySparksEffect(Vector3 position, Vector2 direction)
        {
            if (sparksParticleSystem == null) return;

            Quaternion particleDirection = Quaternion.LookRotation(direction);

            ParticleSystem sparks = GameObject.Instantiate(sparksParticleSystem, position , particleDirection);
            sparks.Play();
            GameObject.Destroy(sparks.gameObject, 1f);
        }
    }
}
