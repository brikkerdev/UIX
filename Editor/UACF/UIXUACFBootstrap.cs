using UnityEditor;
using UACF.Core;

namespace UIX.Editor.UACF
{
    [InitializeOnLoad]
    public static class UIXUACFBootstrap
    {
        static UIXUACFBootstrap()
        {
            EditorApplication.delayCall += RegisterTools;
        }

        private static void RegisterTools()
        {
            var dispatcher = UACFBootstrap.GetDispatcher();
            if (dispatcher != null)
                UIXToolProvider.Register(dispatcher);
        }
    }
}
