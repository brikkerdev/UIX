using UnityEditor;
using UnityEngine;
using System.IO;

namespace UIX.Editor.Tools
{
    public static class UIXComponentScaffolder
    {
        [MenuItem("UIX/Create Component")]
        public static void CreateComponent()
        {
            var path = EditorUtility.SaveFilePanelInProject("Create Component", "NewComponent", "xml", "Save component XML");
            if (string.IsNullOrEmpty(path)) return;

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);

            var xml = $@"<component name=""{name}"">
    <props>
        <prop name=""text"" type=""string"" default="""" />
    </props>
    <template>
        <button class=""btn"">
            <text text=""{{text}}"" />
        </button>
    </template>
</component>";

            var uss = @".btn {
    padding: 16;
    background-color: #3A7BF2;
}
";

            File.WriteAllText(path, xml);
            File.WriteAllText(Path.Combine(dir, name + ".uss"), uss);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("UIX", $"Component {name} created.", "OK");
        }
    }
}
