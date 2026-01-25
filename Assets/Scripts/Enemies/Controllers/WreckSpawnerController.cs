using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies.Controllers
{
    public class WreckSpawnerController : MonoBehaviour
    {
        public void SpawnWreck(Vector3 position, Quaternion rotation, SpriteRenderer enemySprite)
        {
            var wreckPrefabs = WreckManager.Instance.WreckPrefabs;

            if (wreckPrefabs == null || wreckPrefabs.Count == 0) return;

            GameObject randomPrefab = wreckPrefabs[UnityEngine.Random.Range(0, wreckPrefabs.Count)];

            if (randomPrefab == null) return;

            GameObject wreckObj = Instantiate(randomPrefab, position, rotation);
            // set parent and register in WreckManager
            wreckObj.transform.SetParent(WreckManager.Instance.wrecksContainer.transform, false);
            WreckManager.Instance.RegisterWreck(wreckObj.GetComponent<Wreck>());

            // copy sprite from enemy to wreck
            SpriteRenderer wreckRenderer = wreckObj.GetComponent<SpriteRenderer>();

            if (enemySprite != null && wreckRenderer != null)
            {
                //wreckRenderer.sprite = enemySprite.sprite;
                //wreckRenderer.color = Color.gray; // np. default wreck color
            }
        }
    }
}
