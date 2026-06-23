using UnityEngine;

namespace SG {
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;

        [SerializeField] float walkingSpeed = 2;

        [SerializeField] float runningSpeed = 5;
        [SerializeField] float rotatioSpeed = 15;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }
        public void HandleAllMovement()
        {
            // Если мы еще в меню, или камера/ввод не созданы — не двигаемся
            if (PlayerInputManager.instance == null || PlayerCamera.instance == null)
                return;

            HandleGroundedMovement();
            HandleRotation();
            // Перемещение в воздухе (воздушное движение)
        }

        private void GetVerticalAndHorizontalInputs()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;

            // Ограничить движения
        }

        private void HandleGroundedMovement()
        {
            GetVerticalAndHorizontalInputs();
            // Наше направление движения основано на направлении взгляда камеры и векторе ввода перемещения
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotatioSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
}