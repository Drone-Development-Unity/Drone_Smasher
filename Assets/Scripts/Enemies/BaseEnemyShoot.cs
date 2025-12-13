using Assets.Scripts.Interfaces;
using Assets.Scripts.Interfaces.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    public class BaseEnemyShoot : MonoBehaviour, IArrivable
    {
        protected bool waitingToShoot = true;
        protected float shootTimer = 0f;
        protected GameObject bulletsCointainer;

        [SerializeField] protected int damage;

        public virtual void Start()
        {
            bulletsCointainer = GameObject.Find("BulletsContainer");
        }

        public void OnArrive()
        {
            waitingToShoot = false;
        }

        public void SetDmgMultiplier(float multiplier)
        {
            damage = (int)(damage * multiplier);
        }
    }
}
