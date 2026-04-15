using System;
using TMPro;
using UnityEditor.Advertisements;
using UnityEngine;


public class DamageTextSc : MonoBehaviour
{
    private float floatSpeed = 100f;
    private float lifetime = 2f;
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private Color textColor;
    private float timer;
    private Vector3 startScale;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        
        startScale = transform.localScale;
        transform.localScale = startScale * 1.25f;
        textColor = textMesh.color;
        

    }

    private void Update()
    {

        timer += Time.deltaTime;
        float t = timer / lifetime;
        t = 1f - MathF.Pow(1f - t, 2f);
        
        // move up
        rectTransform.anchoredPosition += Vector2.up * (floatSpeed * Time.deltaTime);
        
        // fade out
        textColor.a = Mathf.Lerp(1f, 0f, t);
        textMesh.color = textColor;
        
        // scale down
        transform.localScale = Vector3.Lerp(startScale, startScale * 0.25f, t);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
  
    }
    
}
