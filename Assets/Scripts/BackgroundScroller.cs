using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    Material material;
    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        GameManager.Instance.updateBackground = (x) => material.SetFloat("_XPosition", x);
    }
}
