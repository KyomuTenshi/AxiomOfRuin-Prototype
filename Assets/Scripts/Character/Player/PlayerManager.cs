using UnityEngine;

namespace SG {
    public class PlayerManager : CharacterManager
    {
        PlayerLocomotionManager playerLocomotionManager;

        protected override void Awake()
        {
            base.Awake();

            // Сделать больше вещей (выполнить дополнительные действия), только для игрока

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        }

        protected override void Update()
        {
            base.Update();
            
            // Если мы не являемся владельцем этого игрового объекта, мы не управляем им и не редактируем его
            if (!IsOwner)
            return;

            // Обработать перемещение (управлять движением)
            playerLocomotionManager.HandleAllMovement();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner)
                return;

            base.LateUpdate();
            
            PlayerCamera.instance.HandleAllCameraAction();
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkDespawn();

            // ЕСЛИ ЭТО ОБЪЕКТ ИГРОКА, КОТОРЫМ ВЛАДЕЕТ ЭТОТ КЛИЕНТ
            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
            }
        }
    }
}