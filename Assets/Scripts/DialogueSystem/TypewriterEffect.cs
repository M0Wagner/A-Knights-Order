using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float writingSpeed = 50f;

    public Coroutine Run(string text, TMP_Text textLabel)
    {
        return StartCoroutine(TypeText(text, textLabel));
    }

    private IEnumerator TypeText(string text, TMP_Text textLabel)
    {
        textLabel.text = string.Empty;

        // elapsed time since begun writing
        float t = 0;

        int charIndex = 0;

        while (charIndex < text.Length)
        {
            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);

            textLabel.text = text.Substring(0, charIndex);
            
            yield return null;
        }

        textLabel.text = text;
    }
}
