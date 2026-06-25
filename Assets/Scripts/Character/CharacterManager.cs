using Unity.Netcode;
using UnityEngine;

namespace SG 
{
    public class CharacterManager : NetworkBehaviour
    {
        public CharacterController characterController;

        CharacterNetworkManager characterNetworkManager;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            
            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        protected virtual void Update()
        {
            // Если этот персонаж управляется с нашей стороны, то присвоить его сетевой позиции значение позиции нашего трансформа (компонента Transform)
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            // Если этот персонаж управляется откуда-то еще (другим игроком), то присвоить его позицию здесь локально на основе позиции из его сетевого трансформа
            else
            {
                // Позиция
                transform.position = Vector3.SmoothDamp
                (transform.position, characterNetworkManager.networkPosition.Value, 
                ref characterNetworkManager.networkPositionVelocity, 
                characterNetworkManager.networkPositionSmoothTime
                );
                // Поворот
                transform.rotation = Quaternion.Slerp
                (transform.rotation, 
                characterNetworkManager.networkRotation.Value, 
                characterNetworkManager.networkRotationSmoothTime);
            }
        }

        protected virtual void LateUpdate()
        {
            
        }
    }
}