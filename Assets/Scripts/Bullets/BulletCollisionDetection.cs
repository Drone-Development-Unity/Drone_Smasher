using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Bullets
{
    public class BulletCollisionDetection : MonoBehaviour
    {
        private Vector3 bottomLeft;
        private Vector3 topRight;
        private Vector2 pos;
        private Vector2 direction;
        private float leftXClamp, rightXClamp, downYClamp, upYClamp;
        private float clampSize = 0.5f;


        // bullet properties
        private GameObject shooter;
        private string shooterTag;
        private int damage;

        private BulletAnimationController animController;

        public void Initialize(GameObject shooter, int damage, Vector2 direction)
        {
            this.shooter = shooter;
            this.shooterTag = shooter.tag;
            this.damage = damage;
            this.direction = direction;
        }

        void Start()
        {

            bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            //clamp borders
            leftXClamp = bottomLeft.x - clampSize;
            rightXClamp = topRight.x + clampSize;
            downYClamp = bottomLeft.y - clampSize;
            upYClamp = topRight.y + clampSize;

            animController = GetComponent<BulletAnimationController>();
        }

        void FixedUpdate()
        {
            pos = transform.position;
            if (pos.x < leftXClamp || pos.x > rightXClamp || pos.y < downYClamp || pos.y > upYClamp)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // ignore collision with shooter or allies
            if (collision.gameObject.tag == shooterTag) return;

            // damage enemy / player base
            if (collision.CompareTag("PlayerBase") || collision.CompareTag("Enemy"))
            {
                IVulnerable vulnerableTarget = collision.GetComponent<IVulnerable>();
                if (vulnerableTarget != null)
                {
                    vulnerableTarget.TakeDamage(damage);
                    animController.PlaySparksEffect(transform.position, direction);
                }
            }
        }
    }
}
