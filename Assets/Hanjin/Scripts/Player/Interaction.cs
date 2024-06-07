using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [Header("Detect Property")]
    public float radius = 2.0f;

    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public Collider[] detectedObject;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    [Header("Interaction Property")]
    public GameObject promptObj;
    public TMP_Text promptText;

    public GameObject pickupIcon;
    public GameObject useIcon;
    public GameObject talkIcon;

    private void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            DetectObject();
        }
    }

    private void DetectObject()
    {
        detectedObject = Physics.OverlapSphere(transform.position, radius, layerMask);
        
        if(detectedObject.Length > 0)
        {
            curInteractGameObject = DetectedNearObject(detectedObject).gameObject;
            if (curInteractGameObject.TryGetComponent(out IInteractable interactable))
            {
                curInteractable = interactable;
                SetPromptText(true);
            }
        }
        else
        {
            curInteractGameObject = null;
            curInteractable = null;
            SetPromptText(false);
        }
    }

    private Collider DetectedNearObject(Collider[] targets)
    {
        Collider nearTarget = null;
        float distance = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            if(nearTarget == null)
            {
                distance = Vector3.Distance(targets[i].transform.position, transform.position);
                nearTarget = targets[i];
            }
            else
            {
                float dist = Vector3.Distance(targets[i].transform.position, transform.position);
                if (dist < distance)
                {
                    distance = dist;
                    nearTarget = targets[i];
                }
            }
        }
        
        return nearTarget;
    }

    private void SetPromptText(bool active)
    {
        if(active)
        {
            promptObj.SetActive(true);
            
            InteractableType type = curInteractable.GetInteractableType();
            pickupIcon.SetActive(type == InteractableType.Pickup);
            useIcon.SetActive(type == InteractableType.Use);
            talkIcon.SetActive(type == InteractableType.Talk);

            promptText.text = curInteractable.GetInteractPrompt();
        }
        else
        {
            promptObj.SetActive(false);
            promptText.text = "";
        }
    }

    // E ��ư Ŭ�� �� ����
    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            // 
            curInteractable.OnInteract();
            
            // �ʱ�ȭ
            curInteractGameObject = null;
            curInteractable = null;
            promptObj.SetActive(false);
        }
    }
}
