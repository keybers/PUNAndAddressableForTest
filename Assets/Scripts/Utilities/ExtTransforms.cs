using UnityEngine;

public static class Transforms
{
    public static void DstroyChildren(this Transform t, bool destroyImmediately = false)
    {
        foreach(Transform child in t)
        {
            if (destroyImmediately)
            {
                MonoBehaviour.DestroyImmediate(child.gameObject);
            }
            else
            {
                MonoBehaviour.Destroy(child.gameObject);
            }
        }
    }
}
