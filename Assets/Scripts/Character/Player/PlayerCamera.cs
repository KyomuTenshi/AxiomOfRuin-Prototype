using UnityEngine;

namespace SG {
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTrasform;

        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1; // ЧЕМ БОЛЬШЕ ЭТО ЧИСЛО, ТЕМ ДОЛЬШЕ КАМЕРА БУДЕТ ДОБИРАТЬСЯ ДО СВОЕЙ ПОЗИЦИИ ВО ВРЕМЯ ДВИЖЕНИЯ
        [SerializeField] private float leftAndRightRotaionSpeed= 220;
        [SerializeField] private float upAndDownRotaionSpeed = 220;
        [SerializeField] float minimumPivot = -30; // САМАЯ НИЗКАЯ ТОЧКА, В КОТОРУЮ ВЫ МОЖЕТЕ ПОСМОТРЕТЬ ВНИЗ
        [SerializeField] float maxmimumPivot = 60; // САМАЯ ВЫСОКАЯ ТОЧКА, В КОТОРУЮ ВЫ МОЖЕТЕ ПОСМОТРЕТЬ ВВЕР
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask collideWithLayers;

        [Header("Camera Value")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition; // ИСПОЛЬЗУЕТСЯ ДЛЯ СТОЛКНОВЕНИЙ КАМЕРЫ (ПЕРЕМЕЩАЕТ ОБЪЕКТ КАМЕРЫ В ЭТУ ПОЗИЦИЮ ПРИ СТОЛКНОВЕНИИ)
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition; // ЗНАЧЕНИЯ, ИСПОЛЬЗУЕМЫЕ ДЛЯ СТОЛКНОВЕНИЙ КАМЕРЫ (КОЛЛИЗИЙ)
        private float targetCameraZPosition; // ЗНАЧЕНИЯ, ИСПОЛЬЗУЕМЫЕ ДЛЯ СТОЛКНОВЕНИЙ КАМЕРЫ (КОЛЛИЗИЙ)
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
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraAction()
        {
            if (player != null)
            {
                HandleFollowTarget();
                HandleRotation();
                HandleCollision();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        public void HandleRotation()
        {
            // ЕСЛИ ЦЕЛЬ ЗАХВАЧЕНА, ПРИНУДИТЕЛЬНО ПОВЕРНУТЬ В СТОРОНУ ЦЕЛИ
            // ИНАЧЕ ВРАЩАТЬСЯ В ОБЫЧНОМ РЕЖИМЕ

            // ОБЫЧНЫЕ ПОВОРОТЫ (ВРАЩЕНИЯ)
            // ПОВОРОТ ВЛЕВО И ВПРАВО НА ОСНОВЕ ГОРИЗОНТАЛЬНОГО ДВИЖЕНИЯ ПРАВОГО ДЖОЙСТИКА
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotaionSpeed) * Time.deltaTime;
            // ПОВОРОТ ВВЕРХ И ВНИЗ НА ОСНОВЕ ВЕРТИКАЛЬНОГО ДВИЖЕНИЯ ПРАВОГО ДЖОЙСТИКА
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotaionSpeed) * Time.deltaTime;
            // ОГРАНИЧИТЬ УГОЛ ОБЗОРА ВВЕРХ И ВНИЗ МЕЖДУ МИНИМАЛЬНЫМ И МАКСИМАЛЬНЫМ ЗНАЧЕНИЕМ
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maxmimumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            // ПОВЕРНУТЬ ЭТОТ ИГРОВОЙ ОБЪЕКТ ВЛЕВО И ВПРАВО
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // ПОВЕРНУТЬ ПОВОРОТНЫЙ (PIVOT) ИГРОВОЙ ОБЪЕКТ ВВЕРХ И ВНИЗ
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTrasform.localRotation = targetRotation;
        }

        private void HandleCollision()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            // НАПРАВЛЕНИЕ ДЛЯ ПРОВЕРКИ СТОЛКНОВЕНИЙ
            Vector3 direction = cameraObject.transform.position - cameraPivotTrasform.position;
            direction.Normalize();

            // МЫ ПРОВЕРЯЕМ, ЕСТЬ ЛИ ОБЪЕКТ ПЕРЕД НАШИМ ЖЕЛАЕМЫМ НАПРАВЛЕНИЕМ ^ (СМ. ВЫШЕ)
            if (Physics.SphereCast(cameraPivotTrasform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                // ЕСЛИ ОБЪЕКТ ЕСТЬ, МЫ ПОЛУЧАЕМ НАШЕ РАССТОЯНИЕ ДО НЕГО
                float distanceFromHitObject = Vector3.Distance(cameraPivotTrasform.position, hit.point);
                // ЗАТЕМ МЫ ПРИРАВНИВАЕМ НАШУ ЦЕЛЕВУЮ ПОЗИЦИЮ Z К СЛЕДУЮЩЕМУ ЗНАЧЕНИЮ
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            // ЕСЛИ НАША ЦЕЛЕВАЯ ПОЗИЦИЯ МЕНЬШЕ, ЧЕМ НАШ РАДИУС СТОЛКНОВЕНИЯ, МЫ ВЫЧИТАЕМ НАШ РАДИУС СТОЛКНОВЕНИЯ (ПРИНУДИТЕЛЬНО ВОЗВРАЩАЯ ЕЕ НАЗАД)
            if (Mathf.Abs(targetCameraZPosition) < -cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            // ЗАТЕМ МЫ ПРИМЕНЯЕМ НАШУ ФИНАЛЬНУЮ ПОЗИЦИЮ С ПОМОЩЬЮ LERP (ЛИНЕЙНОЙ ИНТЕРПОЛЯЦИИ) СО ВРЕМЕНЕМ 0.2F
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
    
}