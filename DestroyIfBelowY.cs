using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfBelowY : MonoBehaviour
{
    public float killY = -50f;
    // 옵션: 삭제하지 않을 태그들 (Player 등)
    public string[] ignoreTags;

    void Update()
    {
        if (transform.position.y <= killY)
        {
            // 태그 예외 처리
            if (ignoreTags != null && ignoreTags.Length > 0)
            {
                foreach (var t in ignoreTags)
                {
                    if (!string.IsNullOrEmpty(t) && gameObject.CompareTag(t))
                        return;
                }
            }
            Destroy(gameObject);
        }
    }
}