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

            // Обработать перемещение (управлять движением)
            playerLocomotionManager.HandleAllMovement();
        }   
    }
}