using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces.Enemy
{
    public interface IShooter
    {
        void SpawnBullet(Transform firePoint);
    }
}
