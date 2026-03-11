using UnityEditor;
using System.IO;

namespace UIX.Editor.Tools
{
    public static class UIXScreenScaffolder
    {
        [MenuItem("UIX/Create Screen")]
        public static void CreateScreen()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create Screen", "NewScreen", "xml", "Save screen XML");
            if (string.IsNullOrEmpty(path)) return;

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);

            var xml = $@"<screen name=""{name}"" viewmodel=""{name}ViewModel"">
    <template>
        <column class=""screen"">
            <text class=""title"" text=""{name}"" />
        </column>
    </template>
</screen>";

            var uss = @".screen {
    padding: 24;
}
.title {
    font-size: 24;
}
";

            var vm = @"using UIX.Binding;

namespace UIX.Screens
{
    public class " + name + @"ViewModel : ViewModel
    {
        public override void OnShown() { }
    }
}
";

            File.WriteAllText(path, xml);
            File.WriteAllText(Path.Combine(dir, name + ".uss"), uss);
            File.WriteAllText(Path.Combine(dir, name + "ViewModel.cs"), vm);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("UIX", $"Screen {name} created.", "OK");
        }
    }
}
