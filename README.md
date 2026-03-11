# UIX Framework

Declarative UI framework for Unity based on XML markup, CSS-like styling (USS), and component architecture. Compiles declarative UI descriptions into native uGUI objects. Integrates with [UACF](https://github.com/brikkerdev/UACF) for AI-agent automation.

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Architecture](#architecture)
- [Components](#components)
- [Screens](#screens)
- [Styling (USS)](#styling-uss)
- [Themes](#themes)
- [Binding](#binding)
- [Navigation](#navigation)
- [Materials and Shaders](#materials-and-shaders)
- [UACF Integration](#uacf-integration)
- [Project Settings](#project-settings)
- [Samples](#samples)
- [API Reference](#api-reference)
- [Troubleshooting](#troubleshooting)
- [License](#license)

---

## Requirements

| Dependency | Version | Purpose |
|------------|---------|---------|
| Unity | 2021.3+ | Minimum version |
| uGUI | 1.0+ | UnityEngine.UI |
| TextMeshPro | 3.0+ | Text rendering |
| UACF | latest | AI integration (optional) |

---

## Installation

### Via manifest.json

Add to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.uix.framework": "https://github.com/brikkerdev/UIX.git#main",
    "com.uacf.editor": "https://github.com/brikkerdev/UACF.git?path=Packages/com.uacf.editor#main"
  }
}
```

### Via Package Manager

1. Window → Package Manager
2. Add package from git URL
3. Enter the UIX repository URL

### Post-installation

1. Import the package (automatic)
2. Add the FullDemo or BasicSetup sample if needed
3. Configure paths in `Edit → Project Settings → UIX`

---

## Quick Start

### 1. Create a component

Create folder `Assets/UI/Components/MyButton/`:

**MyButton.xml**
```xml
<component name="MyButton">
    <props>
        <prop name="text" type="string" default="Click" />
        <prop name="onClick" type="event" />
    </props>
    <template>
        <button class="my-btn" onClick="{onClick}">
            <text text="{text}" />
        </button>
    </template>
</component>
```

**MyButton.uss**
```css
.my-btn {
    padding: 12;
    min-height: 36;
    background-color: #3A7BF2;
}
.my-btn .uix-text {
    color: #FFFFFF;
    font-size: 16;
}
```

### 2. Create a screen

Create folder `Assets/UI/Screens/MainMenu/`:

**MainMenu.xml**
```xml
<screen name="MainMenu" viewmodel="MainMenuViewModel">
    <template>
        <column class="screen">
            <text id="title" text="{Title}" />
            <MyButton text="Play" onClick="{OnPlayClicked}" />
        </column>
    </template>
</screen>
```

**MainMenu.uss**
```css
.screen {
    padding: 24;
    background-color: var(--color-background);
}
#title {
    font-size: 32;
    color: var(--color-text);
}
```

**MainMenuViewModel.cs**
```csharp
using UIX.Binding;
using UIX.Navigation;

public class MainMenuViewModel : ViewModel
{
    public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>("Main Menu");

    public void OnPlayClicked()
    {
        Navigator.Push<GameViewModel>();
    }
}
```

### 3. Initialize at runtime

```csharp
using UIX.Core;
using UnityEngine;

void Start()
{
    var config = UIXConfiguration.CreateDefault();
    UIXEngine.Initialize(config);
    UIXEngine.Navigator.Push<MainMenuViewModel>();
}
```

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     UIX Framework                            │
├─────────────────────────────────────────────────────────────┤
│  XML (.xml)     USS (.uss)     Theme (theme.uss)            │
│       │              │                  │                    │
│       ▼              ▼                  ▼                    │
│  ┌─────────────────────────────────────────────┐            │
│  │         UIXAssetProcessor (Editor)           │            │
│  │  Parse → Validate → Compile → Prefab         │            │
│  └─────────────────────────────────────────────┘            │
│       │              │                  │                    │
│       ▼              ▼                  ▼                    │
│  UIXTemplate   UIXStyleSheet      UIXTheme                   │
│       │              │                  │                    │
│       └──────────────┼──────────────────┘                    │
│                      ▼                                       │
│  ┌─────────────────────────────────────────────┐            │
│  │           UIXRenderer (Runtime)              │            │
│  │  UIXNode tree → uGUI GameObjects             │            │
│  │  StyleResolver + StyleApplicator            │            │
│  └─────────────────────────────────────────────┘            │
│                      │                                       │
│                      ▼                                       │
│  Canvas, Image, Text, Button, LayoutGroup, ...               │
└─────────────────────────────────────────────────────────────┘
```

### Key types

- **UIXTemplate**: Compiled XML tree (ScriptableObject)
- **UIXStyleSheet**: Compiled USS rules (ScriptableObject)
- **UIXTheme**: Theme variables (ScriptableObject)
- **UIXRenderer**: Creates uGUI from template
- **StyleResolver**: Resolves which styles apply to each element
- **StyleApplicator**: Applies styles to Image, Text, etc.

---

## Components

Components are reusable UI building blocks with props and slots.

### Structure

```
Components/ComponentName/
├── ComponentName.xml   # Template + props definition
├── ComponentName.uss   # Styles (scoped to component)
└── ComponentName.cs   # Optional logic
```

### Props types

| Type | C# mapping | Example |
|------|------------|---------|
| string | string | `"Hello"` |
| int, float, bool | int, float, bool | `42`, `0.5`, `true` |
| sprite, texture | Sprite, Texture | Path to asset |
| color | Color | `#FF0000` |
| font | TMP_FontAsset | Path to font |
| enum(a,b,c) | string | `"primary"` |
| list<T> | IReadOnlyList<T> | ReactiveCollection |
| event | Action | Method name |
| event<T> | Action<T> | Method with parameter |

### Slots

```xml
<component name="Card">
    <template>
        <column>
            <slot name="header" />
            <slot />  <!-- default slot -->
            <slot name="footer" />
        </column>
    </template>
</component>

<!-- Usage -->
<Card>
    <slot-content name="header"><text text="Title" /></slot-content>
    <text text="Body" />  <!-- goes to default -->
    <slot-content name="footer"><MyButton text="OK" /></slot-content>
</Card>
```

---

## Screens

Screens are full-page views with a ViewModel.

### Structure

```
Screens/ScreenName/
├── ScreenName.xml       # Template
├── ScreenName.uss       # Styles
└── ScreenNameViewModel.cs
```

### ViewModel lifecycle

```csharp
public class MyViewModel : ViewModel
{
    public override void OnCreated() { }   // After instantiation
    public override void OnShown() { }     // When screen becomes visible
    public override void OnHidden() { }    // When another screen covers this
    public override void OnDestroyed() { }  // When popped from stack
}
```

---

## Styling (USS)

USS uses CSS-like syntax. Supported selectors:

| Selector | Example |
|----------|---------|
| Class | `.button` |
| ID | `#main-title` |
| Element type | `text`, `button`, `image` |
| Descendant | `.card .title` |
| Direct child | `.card > .title` |
| Combined | `.btn.primary` |
| Pseudo-classes | `button:hover`, `toggle:checked`, `input:focused` |

### Layout properties

`width`, `height`, `min-width`, `min-height`, `max-width`, `max-height`, `flex`, `padding`, `gap`, `align-items`, `justify-content`, `flex-direction`, `overflow`

### Visual properties

`background-color`, `background-image`, `background-size`, `opacity`, `border-radius`, `border`, `border-color`, `border-width`, `tint`, `scale`, `rotation`, `material`, `box-shadow`

### Text properties

`font-family`, `font-size`, `font-weight`, `font-style`, `color`, `text-align`, `text-overflow`, `line-height`, `letter-spacing`, `white-space`

---

## Themes

Themes define CSS variables in `:root`:

```css
:root {
    --color-primary: #3A7BF2;
    --color-background: #121212;
    --spacing-md: 16;
    --radius-md: 8;
}
```

Usage: `background-color: var(--color-primary);`

Runtime switch: `UIXEngine.Themes.SetTheme("DarkTheme");`

---

## Binding

### One-way

```xml
<text text="{PlayerName}" />
<image visible="{IsLoading}" sprite="spinner" />
```

### Two-way (prefix `=`)

```xml
<input text="{=Username}" />
<slider value="{=Volume}" />
<toggle isOn="{=IsMuted}" />
```

### Events

```xml
<button onClick="{OnClick}" />
<button onClick="{() => SelectItem(item)}" />
```

### Formatting

```xml
<text text="{Score:N0}" />   <!-- 1,234,567 -->
<text text="{Progress:P0}" /> <!-- 75% -->
```

---

## Navigation

```csharp
// Stack
Navigator.Push<SettingsViewModel>();
Navigator.Push<SettingsViewModel>(new { userId = 123 });
Navigator.Replace<GameViewModel>();
Navigator.Pop();
Navigator.PopToRoot();

// Modals
Navigator.ShowModal<ConfirmViewModel>();
Navigator.ShowModal<ConfirmViewModel>(new { message = "Delete?" });
Navigator.CloseModal();

// Overlays
Navigator.ShowOverlay<ToastViewModel>();
Navigator.HideOverlay<ToastViewModel>();
```

---

## Materials and Shaders

UIX provides HLSL shaders for advanced styling:

| Shader | Use case |
|--------|----------|
| UIX/Solid | Solid color (no texture) |
| UIX/RoundedRect | border-radius, border |
| UIX/Image | Texture + tint |
| UIX/RoundedImage | Texture + border-radius |
| UIX/Shadow | box-shadow |

### Custom materials

```css
.card {
    material: var(--material-card);
    border-radius: var(--radius-md);
    box-shadow: 4 4 8 rgba(0,0,0,0.3);
}
```

Place materials in `Resources/` for `Resources.Load<Material>(path)`.

---

## UACF Integration

When UACF server is running (localhost:7890):

```bash
# List components
curl -X POST http://localhost:7890/uacf \
  -H "Content-Type: application/json" \
  -d '{"action":"uix.list_components","params":{}}'

# Create component
curl -X POST http://localhost:7890/uacf \
  -H "Content-Type: application/json" \
  -d '{"action":"uix.create_component","params":{"name":"MyButton","template_xml":"...","styles_uss":"..."}}'
```

Available actions: `uix.create_component`, `uix.create_screen`, `uix.update_component`, `uix.update_screen`, `uix.get_component`, `uix.get_screen`, `uix.list_components`, `uix.list_screens`, `uix.delete_component`, `uix.delete_screen`, `uix.update_theme`, `uix.get_theme`, `uix.list_themes`, `uix.set_theme`, `uix.add_prop`, `uix.remove_prop`, `uix.build_ui`, `uix.validate`, `uix.screenshot`, `uix.get_registry`.

---

## Project Settings

`Edit → Project Settings → UIX`

| Setting | Default | Description |
|---------|---------|-------------|
| Components Path | Assets/UI/Components | Where to find components |
| Screens Path | Assets/UI/Screens | Where to find screens |
| Themes Path | Assets/UI/Themes | Where to find themes |
| Generated Path | Assets/UI/_Generated | Output for prefabs |
| Auto Compile | true | Compile on save |
| Auto Generate Prefabs | true | Generate prefabs on compile |
| Log Level | Warning | Verbose, Debug, Info, Warning, Error |

---

## Samples

- **BasicSetup**: Minimal Button, Text, Card, SampleScreen
- **FullDemo**: LightTheme, DarkTheme, ThemeDemo, ComponentsDemo, NavigationDemo

Add via Package Manager → UIX Framework → Samples → Import.

---

## API Reference

### UIXEngine

```csharp
UIXEngine.Initialize(UIXConfiguration config);
UIXEngine.Navigator   // UIXNavigator
UIXEngine.Themes      // UIXThemeManager
UIXEngine.Registry    // ComponentRegistry
UIXEngine.Renderer    // UIXRenderer
```

### UIXMaterialRegistry

```csharp
UIXMaterialRegistry.GetMaterial(MaterialType type);
UIXMaterialRegistry.GetRoundedMaterial(float radius);
UIXMaterialRegistry.GetRoundedImageMaterial(float radius);
UIXMaterialRegistry.GetCustomMaterial(string path);
UIXMaterialRegistry.RegisterCustomMaterial(string path, Material mat);
UIXMaterialRegistry.PixelRadiusToNormalized(float px, Vector2 rectSize);
UIXMaterialRegistry.ClearCache();
```

### ReactiveProperty

```csharp
var prop = new ReactiveProperty<int>(0);
prop.Value = 42;
prop.OnChanged += v => Debug.Log(v);
```

### ReactiveCollection

```csharp
var list = new ReactiveCollection<Item>();
list.Add(item);
list.OnCollectionChanged += () => { };
list.OnItemAdded += (i, item) => { };
```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| No compilation | Check Auto Compile, verify file paths |
| Component not found | Ensure in Components path, check XML |
| Styles not applied | Verify USS compiled, check selector match |
| border-radius not visible | Shaders must be in package, check UIXMaterialRegistry |
| Bindings not working | Verify ViewModel members, check generated _Bindings.cs |

**Documentation:**
- `Documentation~/TechnicalSpecification.md` — полное техническое задание
- `Documentation~/API-Reference.md` — справка по API

---

## License

MIT
