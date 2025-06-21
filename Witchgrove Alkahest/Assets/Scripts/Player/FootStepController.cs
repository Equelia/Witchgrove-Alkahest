// FootStepController.cs
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootStepController : MonoBehaviour
{
    [Serializable]
    public class SurfaceFootstep
    {
        [Tooltip("Tag of the surface collider, e.g. 'Grass', 'Sand'.")]
        public string surfaceTag;
        [Tooltip("Names of footstep sounds registered in SoundManager for this surface.")]
        public string[] soundNames;
    }

    [Header("Footstep Settings")]
    [SerializeField] private SurfaceFootstep[] footstepSounds;
    [Tooltip("Time between footsteps when walking.")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [Tooltip("Time between footsteps when sprinting.")]
    [SerializeField] private float sprintStepInterval = 0.35f;
    [Tooltip("Distance for ground raycast to detect surface.")]
    [SerializeField] private float raycastDistance = 1.5f;

    private CharacterController controller;
    private FirstPersonController fps;
    private float stepTimer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        fps = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (fps != null && fps.InventoryOpen) // Modified: skip and reset if inventory open
        {
            stepTimer = 0f;
            return;
        }

        if (!controller.isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        Vector3 horizontalVel = controller.velocity;
        horizontalVel.y = 0f;
        float speed = horizontalVel.magnitude;

        if (speed > 0.1f)
        {
            float interval = Input.GetKey(KeyCode.LeftShift) ? sprintStepInterval : walkStepInterval;
            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            string tag = hit.collider.tag;
            foreach (var surface in footstepSounds)
            {
                if (surface.surfaceTag == tag && surface.soundNames.Length > 0)
                {
                    string randomName = surface.soundNames[UnityEngine.Random.Range(0, surface.soundNames.Length)];
                    SoundManager.Instance.PlaySound(randomName);
                    return;
                }
            }
        }
    }
}
