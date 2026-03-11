# UIX Framework

Declarative UI framework for Unity based on XML markup, CSS-like styling (USS), and component architecture. Compiles declarative UI descriptions into native uGUI objects. Integrates with [UACF](https://github.com/brikkerdev/UACF) for AI-agent automation.

## Requirements

- Unity 2021.3+
- uGUI (UnityEngine.UI)
- TextMeshPro
- UACF (for AI integration)

## Installation

Add to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.uix.framework": "https://github.com/brikkerdev/UIX.git#main",
    "com.uacf.editor": "https://github.com/brikkerdev/UACF.git?path=Packages/com.uacf.editor#main"
  }
}
```

## Quick Start

1. Create a component in `Assets/UI/Components/MyButton/`:
   - `MyButton.xml` - XML template
   - `MyButton.uss` - Styles

2. Create a screen in `Assets/UI/Screens/MainMenu/`:
   - `MainMenu.xml` - Screen template
   - `MainMenu.uss` - Styles
   - `MainMenuViewModel.cs` - ViewModel

3. Configure paths in `Edit → Project Settings → UIX`

4. Initialize at runtime: `UIXEngine.Initialize(config)`

## UACF Integration

When UACF is running, UIX tools are available via HTTP API:

```bash
curl -X POST http://localhost:7890/uacf \
  -H "Content-Type: application/json" \
  -d '{"action":"uix.get_registry","params":{}}'
```

Available actions: `uix.create_component`, `uix.create_screen`, `uix.list_components`, `uix.list_screens`, `uix.get_component`, `uix.get_screen`, and more.

## License

MIT
