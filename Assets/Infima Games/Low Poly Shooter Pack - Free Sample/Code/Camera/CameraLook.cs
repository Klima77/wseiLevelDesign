// Copyright 2021, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Camera Look. Handles the rotation of the camera and crouching functionality.
    /// </summary>
    public class CameraLook : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Settings")]

        [Tooltip("Sensitivity when looking around.")]
        [SerializeField]
        private Vector2 sensitivity = new Vector2(1, 1);

        [Tooltip("Minimum and maximum up/down rotation angle the camera can have.")]
        [SerializeField]
        private Vector2 yClamp = new Vector2(-60, 60);

        [Tooltip("Should the look rotation be interpolated?")]
        [SerializeField]
        private bool smooth;

        [Tooltip("The speed at which the look rotation is interpolated.")]
        [SerializeField]
        private float interpolationSpeed = 25.0f;

        [Header("Crouch Settings")]

        [Tooltip("Height of the camera when crouching.")]
        [SerializeField]
        private float crouchCameraHeight = 0.5f;

        [Tooltip("Height of the camera when standing.")]
        [SerializeField]
        private float standCameraHeight = 1.6f;

        [Tooltip("Height of the collider when crouching.")]
        [SerializeField]
        private float crouchColliderHeight = 1.0f;

        [Tooltip("Speed of the crouch/stand transition.")]
        [SerializeField]
        private float crouchTransitionSpeed = 10.0f;

        #endregion

        #region FIELDS

        /// <summary>
        /// Player Character.
        /// </summary>
        private CharacterBehaviour playerCharacter;
        /// <summary>
        /// The player character's rigidbody component.
        /// </summary>
        private Rigidbody playerCharacterRigidbody;
        /// <summary>
        /// The player character's capsule collider.
        /// </summary>
        private CapsuleCollider playerCollider;

        /// <summary>
        /// The player character's rotation.
        /// </summary>
        private Quaternion rotationCharacter;
        /// <summary>
        /// The camera's rotation.
        /// </summary>
        private Quaternion rotationCamera;

        /// <summary>
        /// Default collider height.
        /// </summary>
        private float defaultColliderHeight;
        /// <summary>
        /// Default collider center.
        /// </summary>
        private Vector3 defaultColliderCenter;

        #endregion

        #region UNITY

        private void Awake()
        {
            //Get Player Character.
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
            //Cache the rigidbody.
            playerCharacterRigidbody = playerCharacter.GetComponent<Rigidbody>();
            //Cache the collider.
            playerCollider = playerCharacter.GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            //Cache the character's initial rotation.
            rotationCharacter = playerCharacter.transform.localRotation;
            //Cache the camera's initial rotation.
            rotationCamera = transform.localRotation;
            //Cache the default collider height and center.
            defaultColliderHeight = playerCollider.height;
            defaultColliderCenter = playerCollider.center;
        }

        private void LateUpdate()
        {
            //Frame Input. The Input to add this frame!
            Vector2 frameInput = playerCharacter.IsCursorLocked() ? playerCharacter.GetInputLook() : default;
            //Sensitivity.
            frameInput *= sensitivity;

            //Yaw.
            Quaternion rotationYaw = Quaternion.Euler(0.0f, frameInput.x, 0.0f);
            //Pitch.
            Quaternion rotationPitch = Quaternion.Euler(-frameInput.y, 0.0f, 0.0f);

            //Save rotation. We use this for smooth rotation.
            rotationCamera *= rotationPitch;
            rotationCharacter *= rotationYaw;

            //Local Rotation.
            Quaternion localRotation = transform.localRotation;

            //Smooth.
            if (smooth)
            {
                //Interpolate local rotation.
                localRotation = Quaternion.Slerp(localRotation, rotationCamera, Time.deltaTime * interpolationSpeed);
                //Interpolate character rotation.
                playerCharacterRigidbody.MoveRotation(Quaternion.Slerp(playerCharacterRigidbody.rotation, rotationCharacter, Time.deltaTime * interpolationSpeed));
            }
            else
            {
                //Rotate local.
                localRotation *= rotationPitch;
                //Clamp.
                localRotation = Clamp(localRotation);

                //Rotate character.
                playerCharacterRigidbody.MoveRotation(playerCharacterRigidbody.rotation * rotationYaw);
            }

            //Set.
            transform.localRotation = localRotation;

            //Crouch handling.
            HandleCrouch();
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Clamps the pitch of a quaternion according to our clamps.
        /// </summary>
        private Quaternion Clamp(Quaternion rotation)
        {
            rotation.x /= rotation.w;
            rotation.y /= rotation.w;
            rotation.z /= rotation.w;
            rotation.w = 1.0f;

            //Pitch.
            float pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);

            //Clamp.
            pitch = Mathf.Clamp(pitch, yClamp.x, yClamp.y);
            rotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);

            //Return.
            return rotation;
        }

        /// <summary>
        /// Handles crouching by adjusting camera height and collider.
        /// </summary>
        private void HandleCrouch()
        {
            //Check if Control key is held.
            bool isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            //Target values based on crouch state.
            float targetCameraHeight = isCrouching ? crouchCameraHeight : standCameraHeight;
            float targetColliderHeight = isCrouching ? crouchColliderHeight : defaultColliderHeight;
            Vector3 targetColliderCenter = isCrouching ? new Vector3(defaultColliderCenter.x, defaultColliderCenter.y - (defaultColliderHeight - crouchColliderHeight) / 2, defaultColliderCenter.z) : defaultColliderCenter;

            //Smoothly interpolate camera height.
            Vector3 currentCameraPos = transform.localPosition;
            currentCameraPos.y = Mathf.Lerp(currentCameraPos.y, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
            transform.localPosition = currentCameraPos;

            //Smoothly interpolate collider height and center.
            playerCollider.height = Mathf.Lerp(playerCollider.height, targetColliderHeight, Time.deltaTime * crouchTransitionSpeed);
            playerCollider.center = Vector3.Lerp(playerCollider.center, targetColliderCenter, Time.deltaTime * crouchTransitionSpeed);
        }

        #endregion
    }
}