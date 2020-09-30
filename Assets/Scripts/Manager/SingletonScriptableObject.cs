using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                T[] result = Resources.FindObjectsOfTypeAll<T>();
                if(result.Length == 0)//如果没有
                {
                    Debug.LogError("Err: SingletonScriptableObject -> Instance -> results length is 0 for type " + typeof(T).ToString() + ".");
                    return null;
                }
                if(result.Length > 1)//如果长度大于1，则超过
                {
                    Debug.LogError("Err: SingletonScriptableObject -> Instance -> results length is greather than 1 for type" + typeof(T).ToString() + ".");
                    return null;
                }
                _instance = result[0];

            }
            return _instance;//返回本身
        }
    }
}
