using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UIX.Editor.Pipeline
{
    public class UIXBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
        }
    }
}
