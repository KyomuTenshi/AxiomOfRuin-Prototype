using Unity.Netcode;
using UnityEngine;

namespace SG {
    public class PlayerUIManager : MonoBehaviour
    {
        public  static PlayerUIManager instance;
        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameClient;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        

        private void Update()
        {
            if (startGameClient)
            {
                startGameClient = false;
                // Сначала мы должны завершить работу, так как запустились в качестве хоста во время экрана главного меню (титульного экрана)
                NetworkManager.Singleton.Shutdown();
                // Затем мы перезапускаемся в качестве клиента
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}