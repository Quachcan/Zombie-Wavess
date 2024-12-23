using UnityEngine;

namespace ExpSystem
{
    public class ExpSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject expPrefabs;

        public int expAmount = 1;
        public float spawnHeightOffSet = 1f;

        private void OnEnable()
        {
            EnemyScripts.Enemy.OnEnemyDeath += SpawnExp;
        }

        private void OnDisable()
        {
            EnemyScripts.Enemy.OnEnemyDeath -= SpawnExp;
        }
    

        private void SpawnExp(Vector3 position)
        {
            if (expPrefabs == null)
            {
                Debug.LogError("No exp prefabs assigned");
                return;
            }

            Vector3 spawnPosition = position + new Vector3(0, spawnHeightOffSet, 0);
            for (int i = 0; i < expAmount; i++)
            {
            
                GameObject expObj = Instantiate(expPrefabs, spawnPosition, Quaternion.identity);
                    
                ExpOrb expOrb = expObj.GetComponent<ExpOrb>();
                if (expOrb != null)
                {
                    expOrb.Initialize();
                }
            }
        }
    }
}
