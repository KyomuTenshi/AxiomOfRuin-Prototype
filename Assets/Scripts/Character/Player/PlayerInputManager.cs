using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG {
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        // Думайте о целях поэтапно (разбивайте задачу на шаги):
        // 2. Двигать персонажа на основе этих значений
        PlayerControls playerControls;

        [SerializeField] Vector2 movementInput;

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

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            // Когда сцена меняется, выполнить эту логику
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Если мы загружаемся в сцену нашего игрового мира, включить управление игрока
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            // В противном случае мы, должно быть, находимся в главном меню, отключить управление игрока
            // Это нужно для того, чтобы наш игрок не мог двигаться, если мы заходим в такие интерфейсы, как меню создания персонажа и т.д.
            else
            {
                instance.enabled = false;
            }
        }
                
        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            }

        playerControls.Enable();
        }
        private void OnDestroy()
        {
            // Если мы уничтожаем этот объект, отписаться от этого события
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
    }
}