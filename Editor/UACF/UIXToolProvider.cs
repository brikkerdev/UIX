using System.Threading.Tasks;
using UACF.Core;

namespace UIX.Editor.UACF
{
    public static class UIXToolProvider
    {
        public static void Register(ActionDispatcher dispatcher)
        {
            dispatcher.Register("uix.create_component", Tools.CreateComponentTool.Handle);
            dispatcher.Register("uix.create_screen", Tools.CreateScreenTool.Handle);
            dispatcher.Register("uix.update_component", Tools.UpdateComponentTool.Handle);
            dispatcher.Register("uix.update_screen", Tools.UpdateScreenTool.Handle);
            dispatcher.Register("uix.get_component", Tools.GetComponentTool.Handle);
            dispatcher.Register("uix.get_screen", Tools.GetScreenTool.Handle);
            dispatcher.Register("uix.list_components", Tools.ListComponentsTool.Handle);
            dispatcher.Register("uix.list_screens", Tools.ListScreensTool.Handle);
            dispatcher.Register("uix.delete_component", Tools.DeleteComponentTool.Handle);
            dispatcher.Register("uix.delete_screen", Tools.DeleteScreenTool.Handle);
            dispatcher.Register("uix.update_theme", Tools.UpdateThemeTool.Handle);
            dispatcher.Register("uix.get_theme", Tools.GetThemeTool.Handle);
            dispatcher.Register("uix.list_themes", Tools.ListThemesTool.Handle);
            dispatcher.Register("uix.set_theme", Tools.SetThemeTool.Handle);
            dispatcher.Register("uix.add_prop", Tools.AddPropTool.Handle);
            dispatcher.Register("uix.remove_prop", Tools.RemovePropTool.Handle);
            dispatcher.Register("uix.build_ui", Tools.BuildUITool.Handle);
            dispatcher.Register("uix.validate", Tools.ValidateTool.Handle);
            dispatcher.Register("uix.screenshot", Tools.ScreenshotTool.Handle);
            dispatcher.Register("uix.get_registry", Tools.GetRegistryTool.Handle);
        }
    }
}
