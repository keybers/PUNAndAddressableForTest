#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

//构建时执行此操作的脚本，每次更换预制体的时候，不用担心游戏模式
public class ResourcesPrefabPathBuilder : IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        MasterManager.PopulateNetworkedPrefabs();
    }
}
#endif 
