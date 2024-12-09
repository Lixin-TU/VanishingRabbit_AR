using UnityEngine;

public class RabbitBehavior : MonoBehaviour
{
    public enum RabbitState { Idle, Shrinking, PlayingFireworks, FinalTransformation }
    public RabbitState state = RabbitState.Idle;

    public GameObject deerPrefab; // Assign your deer model
    public GameObject fireworksEffect; // Assign the fireworks GameObject in the scene
    public GameObject snowEffect; // Assign the snow particle system
    public GameObject christmasScene; // Assign the Christmas scene background

    private static int clickCount = 0;

    void Update()
    {
        // Detect touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Cast a ray to detect the object being touched
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform) // Check if this object is touched
                    {
                        HandleTouch();
                    }
                }
            }
        }
    }

    void HandleTouch()
    {
        clickCount++;

        if (clickCount == 1 && state == RabbitState.Idle) // First rabbit clicked
        {
            state = RabbitState.Shrinking;
            StartCoroutine(ShrinkAndDisappear());
        }
        else if (clickCount == 2 && state == RabbitState.Idle) // Second rabbit clicked
        {
            state = RabbitState.PlayingFireworks;
            ActivateFireworks();
        }
        else if (clickCount == 3 && state == RabbitState.Idle) // Last rabbit clicked
        {
            state = RabbitState.FinalTransformation;
            TransformToDeerAndChangeScene();
        }
    }

    System.Collections.IEnumerator ShrinkAndDisappear()
    {
        float shrinkSpeed = 0.01f;
        while (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);
            yield return null;
        }
        Destroy(gameObject);
    }

    void ActivateFireworks()
    {
        if (fireworksEffect != null && !fireworksEffect.activeSelf)
        {
            fireworksEffect.SetActive(true); // Activate the existing fireworks GameObject
            Debug.Log("Fireworks activated.");
        }
        Destroy(gameObject); // Remove the clicked rabbit
    }

    void TransformToDeerAndChangeScene()
    {
        // Instantiate the deer
        if (deerPrefab != null)
        {
            GameObject deer = Instantiate(deerPrefab, transform.position, Quaternion.identity);
            StartCoroutine(FadeInDeer(deer)); // Gradually make the deer appear
            Debug.Log("Deer instantiated at position: " + transform.position);
        }
        else
        {
            Debug.LogError("Deer prefab is not assigned!");
        }

        // Destroy the last rabbit
        Destroy(gameObject);

        // Activate Christmas scene and snow effect
        if (christmasScene != null)
        {
            christmasScene.SetActive(true);
            Debug.Log("Christmas scene activated.");
        }
        else
        {
            Debug.LogError("Christmas Scene is not assigned!");
        }

        if (snowEffect != null)
        {
            snowEffect.SetActive(true);
            Debug.Log("Snow effect activated.");
        }
        else
        {
            Debug.LogError("Snow Effect is not assigned!");
        }
    }

    System.Collections.IEnumerator FadeInDeer(GameObject deer)
    {
        Renderer[] renderers = deer.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("Deer does not have Renderer components!");
            yield break;
        }

        float fadeDuration = 2.0f; // Time to fully appear
        float elapsedTime = 0;

        // Prepare materials for fade-in
        Material[] materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material; // Instance of the material
            Color color = materials[i].color;
            color.a = 0; // Fully transparent initially
            materials[i].color = color;
        }

        // Gradually increase alpha for all materials
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            foreach (Material material in materials)
            {
                Color color = material.color;
                color.a = alpha; // Update alpha
                material.color = color; // Apply updated color
            }

            yield return null;
        }

        // Ensure fully visible at the end
        foreach (Material material in materials)
        {
            Color color = material.color;
            color.a = 1; // Fully opaque
            material.color = color;
        }
    }
}