using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        [SerializeField]
        private int worldSceneIndex = 1;

        private void Awake()
        {
            // ОДНОВРЕМЕННО МОЖЕТ СУЩЕСТВОВАТЬ ТОЛЬКО ОДИН ЭКЗЕМПЛЯР ЭТОГО СКРИПТА
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            yield return loadOperation;
        }
    }
}