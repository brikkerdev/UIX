using System.IO;
using NUnit.Framework;
using UnityEngine;
using UIX.Editor.Pipeline;
using UIX.Templates;

namespace UIX.Tests.Editor
{
    public class UIXPrefabGeneratorTests
    {
        [Test]
        public void Generate_CreatesGameObject()
        {
            var template = ScriptableObject.CreateInstance<UIXTemplate>();
            template.SourcePath = "Assets/UI/Screens/Test/Test.xml";
            template.TemplateName = "Test";
            template.IsComponent = false;

            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            var testDir = Path.Combine(projectRoot, "Assets", "UI", "Screens", "Test");
            Directory.CreateDirectory(testDir);
            var xmlPath = Path.Combine(testDir, "Test.xml");
            var ussPath = Path.Combine(testDir, "Test.uss");

            File.WriteAllText(xmlPath, @"<screen name=""Test"" viewmodel=""TestVM"">
  <template><column><text text=""Hello"" /></column></template>
</screen>");
            File.WriteAllText(ussPath, ".screen { padding: 16; }");

            try
            {
                var go = UIXPrefabGenerator.Generate(template);
                Assert.IsNotNull(go);
                Assert.AreEqual("Test", go.name);
                Assert.IsNotNull(go.GetComponent<UIX.Core.UIXScreen>());
                Assert.Greater(go.transform.childCount, 0);
                Object.DestroyImmediate(go);
            }
            finally
            {
                if (File.Exists(xmlPath)) File.Delete(xmlPath);
                if (File.Exists(ussPath)) File.Delete(ussPath);
            }
        }

        [Test]
        public void SaveAsPrefab_CreatesPrefabFile()
        {
            var template = ScriptableObject.CreateInstance<UIXTemplate>();
            template.SourcePath = "Assets/UI/Screens/PrefabTest/PrefabTest.xml";
            template.TemplateName = "PrefabTest";
            template.IsComponent = false;

            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            var testDir = Path.Combine(projectRoot, "Assets", "UI", "Screens", "PrefabTest");
            Directory.CreateDirectory(testDir);
            var xmlPath = Path.Combine(testDir, "PrefabTest.xml");
            var ussPath = Path.Combine(testDir, "PrefabTest.uss");

            File.WriteAllText(xmlPath, @"<screen name=""PrefabTest"">
  <template><column><text text=""Test"" /></column></template>
</screen>");
            File.WriteAllText(ussPath, ".screen { padding: 8; }");

            try
            {
                var prefabPath = UIXPrefabGenerator.SaveAsPrefab(template);
                Assert.IsNotNull(prefabPath);
                Assert.IsTrue(File.Exists(Path.Combine(projectRoot, prefabPath)));
                Assert.IsTrue(prefabPath.Contains("_Generated"));
                Assert.IsTrue(prefabPath.EndsWith(".prefab"));
            }
            finally
            {
                if (File.Exists(xmlPath)) File.Delete(xmlPath);
                if (File.Exists(ussPath)) File.Delete(ussPath);
            }
        }
    }
}
