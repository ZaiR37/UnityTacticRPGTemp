using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    [SerializeField] private float cameraMovementSpeed = 11f;
    [SerializeField] private float cameraRotationSpeed = 100f;
    [SerializeField] private float cameraZoomSpeed = 4f;
    [SerializeField] private bool invertCameraZoom = false;
    
    [SerializeField] private const float MIN_FOLLOW_Y_OFFSET = 2f;
    [SerializeField] private const float MAX_FOLLOW_Y_OFFSET = 12f;

    private void Start() {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update() {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement(){
        Vector3 InputMoveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVector = transform.forward * InputMoveDir.z + transform.right * InputMoveDir.x;
        transform.position += moveVector * cameraMovementSpeed * Time.deltaTime;
    }

    private void HandleRotation(){
        Vector3 rotationVector = new Vector3(0, Input.GetKey(KeyCode.E) ? -1f : Input.GetKey(KeyCode.Q) ? 1f : 0f, 0);
        transform.eulerAngles += rotationVector * cameraRotationSpeed * Time.deltaTime;
    }

    private void HandleZoom(){
        int zoomDirection = invertCameraZoom ? -1 : 1;
        targetFollowOffset.y += Input.mouseScrollDelta.y * cameraZoomSpeed * zoomDirection;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
    }
}
