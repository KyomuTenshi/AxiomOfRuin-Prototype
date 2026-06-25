using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG {
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        
        PlayerControls playerControls;

        [Header("MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Camera MOVEMENT INPUT")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

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

            // Подписываемся на событие смены сцены
            SceneManager.activeSceneChanged += OnSceneChange;

            // Проверяем: если мы ЗАПУСТИЛИ игру сразу в рабочем мире (для тестов в редакторе)
            if (WorldSaveGameManager.instance != null && 
                SceneManager.GetActiveScene().buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false; // Выключаем, если мы в главном меню
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                
                // Подписка на выполнение движения
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
                // ОБЯЗАТЕЛЬНО: сброс ввода в ноль при отпускании клавиш (в гайде Себастьян это делает)
                playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;
            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnDestroy()
        {
            // Отписываемся от события при уничтожении объекта
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Включаем или выключаем скрипт при переходе между сценами
            if (WorldSaveGameManager.instance != null && 
                newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }

        // ИСПРАВЛЕНО: Теперь ввод корректно блокируется/разблокируется в зависимости от фокуса окна игры
        private void OnApplicationFocus(bool focus)
        {
            if (playerControls == null) return;

            if (enabled && focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }

        private void Update()
        {
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
        }

        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // Расчет силы движения (Math.Abs убран в пользу Mathf.Abs для Unity типов)
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            // Клампинг под анимации (0, 0.5, 1)
            if (moveAmount <= 0.5f && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1f;
            }
        }

        private  void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
    }
}