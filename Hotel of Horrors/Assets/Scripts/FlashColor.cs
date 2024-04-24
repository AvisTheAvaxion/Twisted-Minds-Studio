using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashColor : MonoBehaviour
{
    [SerializeField] Material flashMaterial;
    [SerializeField] Material defaultMaterial;
    [SerializeField] SpriteRenderer[] spriteRenderers;

    private void Start()
    {
        if (spriteRenderers.Length == 0)
        {
            spriteRenderers = new SpriteRenderer[1];
            spriteRenderers[0] = GetComponent<SpriteRenderer>();
        }

        if (defaultMaterial == null)
            defaultMaterial = spriteRenderers[0].material;
    }

    Coroutine flashCoroutine;
    public void Flash(float length)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashSequence(length));
    }

    IEnumerator FlashSequence(float length)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = flashMaterial;
        }
        yield return new WaitForSeconds(length);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
