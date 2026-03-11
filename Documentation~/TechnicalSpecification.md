# Техническое задание: UIX Framework

## Core Framework + UACF Integration

---

## Содержание

1. [Общее описание](#1-общее-описание)
2. [Структура пакета](#2-структура-пакета)
3. [XML-разметка](#3-xml-разметка)
4. [Система стилей (USS)](#4-система-стилей-uss)
5. [Система тем](#5-система-тем)
6. [Система биндинга данных](#6-система-биндинга-данных)
7. [Asset Pipeline](#7-asset-pipeline)
8. [Навигация](#8-навигация)
9. [Component Registry](#9-component-registry)
10. [Runtime API](#10-runtime-api)
11. [Editor Tools](#11-editor-tools)
12. [UACF Integration](#12-uacf-integration)
13. [Встроенные компоненты](#13-встроенные-компоненты)
14. [Материалы и шейдеры](#14-материалы-и-шейдеры)
15. [Виртуализация списков](#15-виртуализация-списков)
16. [Обработка ошибок](#16-обработка-ошибок)
17. [Настройки проекта](#17-настройки-проекта)
18. [Ограничения](#18-ограничения-и-границы-скоупа)
19. [Зависимости](#19-зависимости)
20. [Gitignore](#20-gitignore)
21. [Устранение неполадок](#21-устранение-неполадок)

---

## 1. Общее описание

### 1.1 Назначение

**UIX** — фреймворк для декларативной разработки UI в Unity, основанный на XML-разметке, CSS-подобной стилизации (USS) и компонентной архитектуре. Фреймворк компилирует декларативное описание интерфейса в нативные uGUI-объекты (UnityEngine.UI + TextMeshPro) в режиме редактора.

### 1.2 Ключевые особенности

- **Декларативность:** интерфейс описывается в XML и USS, без ручного создания UI в коде
- **Компонентность:** переиспользуемые компоненты с props и slots
- **Реактивность:** биндинги к ReactiveProperty и ReactiveCollection
- **Темизация:** переменные CSS, переключение тем в runtime
- **UACF:** AI-агенты могут создавать и изменять UI через HTTP API

### 1.3 Целевая платформа

- **Unity:** 2021.3+, Built-in Render Pipeline
- **uGUI:** UnityEngine.UI, TextMeshPro 3.0+
- **Форма поставки:** UPM-пакет (Unity Package Manager)

### 1.4 Жизненный цикл разработки

```
[XML/USS исходники] → [Asset Pipeline] → [UIXTemplate, UIXStyleSheet, Prefab]
                                                    ↓
[Runtime] → UIXEngine.Initialize → UIXNavigator.Push → UIXRenderer.Render → uGUI
```

---

## 2. Структура пакета

```
com.uix.framework/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE
│
├── Runtime/
│   ├── Core/
│   │   ├── UIXEngine.cs                    # Точка входа, главный API
│   │   ├── UIXScreen.cs                    # MonoBehaviour для экранов
│   │   └── UIXComponent.cs                 # MonoBehaviour для компонентов
│   │
│   ├── Parsing/
│   │   ├── XMLParser.cs                    # Парсер XML-разметки
│   │   ├── USSParser.cs                    # Парсер стилей
│   │   ├── Nodes/                          # AST-ноды
│   │   │   ├── UIXNode.cs
│   │   │   ├── ElementNode.cs
│   │   │   ├── TextNode.cs
│   │   │   ├── ComponentNode.cs
│   │   │   ├── SlotNode.cs
│   │   │   └── ConditionalNode.cs
│   │   └── Tokens/
│   │       ├── StyleToken.cs
│   │       ├── StyleRule.cs
│   │       └── StyleSelector.cs
│   │
│   ├── Templates/
│   │   ├── UIXTemplate.cs                  # ScriptableObject — скомпилированный шаблон
│   │   ├── UIXStyleSheet.cs                # ScriptableObject — скомпилированные стили
│   │   ├── UIXComponentDefinition.cs       # ScriptableObject — определение компонента
│   │   ├── PropDefinition.cs               # Описание входного параметра
│   │   └── SlotDefinition.cs               # Описание слота
│   │
│   ├── Rendering/
│   │   ├── UIXRenderer.cs                  # Создание uGUI-объектов из шаблона
│   │   ├── UIXMaterialRegistry.cs          # Реестр материалов (Solid, Rounded, Image, Shadow)
│   │   ├── ElementRenderers/               # Рендереры для каждого типа элемента
│   │   │   ├── IElementRenderer.cs
│   │   │   ├── ColumnRenderer.cs
│   │   │   ├── RowRenderer.cs
│   │   │   ├── TextRenderer.cs
│   │   │   ├── ImageRenderer.cs
│   │   │   ├── ButtonRenderer.cs
│   │   │   ├── ScrollRenderer.cs
│   │   │   ├── InputRenderer.cs
│   │   │   ├── ToggleRenderer.cs
│   │   │   ├── SliderRenderer.cs
│   │   │   ├── DropdownRenderer.cs
│   │   │   └── GridRenderer.cs
│   │   ├── StyleApplicator.cs              # Применение стилей (включая материалы, box-shadow)
│   │   └── LayoutMapper.cs                 # Маппинг CSS-layout → uGUI LayoutGroups
│   │
│   ├── Shaders/                            # HLSL-шейдеры (Built-in Unlit)
│   │   ├── UIX-Solid.shader                # Сплошной цвет
│   │   ├── UIX-RoundedRect.shader          # border-radius, border
│   │   ├── UIX-Image.shader                # Текстура + tint
│   │   ├── UIX-RoundedImage.shader        # Текстура + border-radius
│   │   └── UIX-Shadow.shader               # box-shadow
│   │
│   ├── Styling/
│   │   ├── UIXTheme.cs                     # ScriptableObject — тема
│   │   ├── UIXThemeManager.cs              # Управление темами, hot-swap
│   │   ├── StyleResolver.cs               # Каскад стилей, специфичность
│   │   ├── StyleScopeManager.cs           # Scoping стилей компонентов
│   │   ├── CSSProperties.cs               # Реестр поддерживаемых CSS-свойств
│   │   └── VariableResolver.cs            # Резолв var(--name) переменных
│   │
│   ├── Binding/
│   │   ├── ReactiveProperty.cs            # Observable-значение
│   │   ├── ReactiveCollection.cs          # Observable-коллекция
│   │   ├── BindingEngine.cs               # Движок биндинга выражений
│   │   ├── BindingExpression.cs           # Разбор {expression}
│   │   ├── ExpressionEvaluator.cs         # Вычисление выражений
│   │   └── ViewModel.cs                   # Базовый класс для ViewModels
│   │
│   ├── Navigation/
│   │   ├── UIXNavigator.cs                # Навигация между экранами
│   │   ├── ScreenStack.cs                 # Стек экранов
│   │   ├── ModalManager.cs                # Модальные окна
│   │   └── TransitionAnimator.cs          # Анимации переходов
│   │
│   ├── Components/
│   │   ├── ComponentRegistry.cs           # Реестр всех зарегистрированных компонентов
│   │   └── ComponentResolver.cs            # Поиск и инстанциирование компонентов
│   │
│   └── Utilities/
│       ├── ColorParser.cs                 # Парсинг цветов (#hex, rgb(), именованные)
│       ├── UnitConverter.cs              # Конвертация единиц (px, %, auto)
│       └── StringInterpolator.cs          # Интерполяция строк с биндингами
│
├── Editor/
│   ├── Pipeline/
│   │   ├── UIXAssetProcessor.cs           # AssetPostprocessor для .xml и .uss
│   │   ├── UIXCompiler.cs                 # XML+USS → ScriptableObject шаблоны
│   │   ├── UIXPrefabGenerator.cs          # Шаблон → Prefab
│   │   ├── UIXBindingCodeGen.cs            # Генерация C#-биндинг кода
│   │   ├── UIXValidation.cs                # Валидация XML/USS
│   │   └── UIXBuildPreprocessor.cs        # Подготовка к билду
│   │
│   ├── Preview/
│   │   ├── UIXPreviewWindow.cs            # EditorWindow — превью экранов
│   │   ├── UIXSceneOverlay.cs             # Overlay в Scene View
│   │   └── UIXScreenshotCapture.cs        # Захват скриншотов для сравнения
│   │
│   ├── Inspector/
│   │   ├── UIXTemplateInspector.cs        # Кастомный инспектор для шаблонов
│   │   ├── UIXThemeInspector.cs           # Инспектор тем
│   │   └── UIXScreenInspector.cs          # Инспектор экранов
│   │
│   ├── Tools/
│   │   ├── UIXComponentScaffolder.cs      # Создание нового компонента по шаблону
│   │   ├── UIXScreenScaffolder.cs         # Создание нового экрана по шаблону
│   │   └── UIXProjectSetup.cs             # Первоначальная настройка проекта
│   │
│   └── UACF/
│       ├── UIXToolProvider.cs             # Регистрация UACF tool-ов
│       └── Tools/
│           ├── CreateComponentTool.cs
│           ├── CreateScreenTool.cs
│           ├── UpdateComponentTool.cs
│           ├── UpdateScreenTool.cs
│           ├── UpdateThemeTool.cs
│           ├── ListComponentsTool.cs
│           ├── GetComponentTool.cs
│           ├── GetScreenTool.cs
│           ├── BuildUITool.cs
│           ├── ValidateTool.cs
│           ├── ScreenshotTool.cs
│           ├── SetThemeTool.cs
│           ├── ListScreensTool.cs
│           ├── DeleteComponentTool.cs
│           ├── DeleteScreenTool.cs
│           ├── GetThemeTool.cs
│           ├── ListThemesTool.cs
│           ├── AddPropTool.cs
│           ├── RemovePropTool.cs
│           └── GetRegistryTool.cs
│
├── Samples~/
│   ├── BasicSetup/
│   │   ├── Themes/
│   │   │   └── DefaultTheme/
│   │   │       └── theme.uss
│   │   ├── Components/
│   │   │   ├── Button/
│   │   │   ├── Text/
│   │   │   └── Card/
│   │   └── Screens/
│   │       └── SampleScreen/
│   │
│   └── FullDemo/
│       ├── Themes/
│       │   ├── LightTheme/
│       │   └── DarkTheme/
│       ├── Components/
│       └── Screens/
│
└── Tests/
    ├── Runtime/
    │   ├── ParsingTests.cs
    │   ├── StyleResolverTests.cs
    │   ├── BindingTests.cs
    │   ├── RenderingTests.cs
    │   └── NavigationTests.cs
    └── Editor/
        ├── CompilerTests.cs
        ├── PrefabGeneratorTests.cs
        ├── ValidationTests.cs
        └── UACFToolsTests.cs
```

---

## 3. XML-разметка

### 3.1. Базовые элементы

Фреймворк поддерживает следующие встроенные XML-элементы, каждый из которых маппится на соответствующие uGUI-компоненты:

| XML-элемент | uGUI-маппинг | Описание |
|---|---|---|
| `<column>` | `GameObject` + `VerticalLayoutGroup` | Вертикальный контейнер |
| `<row>` | `GameObject` + `HorizontalLayoutGroup` | Горизонтальный контейнер |
| `<stack>` | `GameObject` (дети накладываются друг на друга) | Наложение элементов |
| `<grid>` | `GameObject` + `GridLayoutGroup` | Сетка |
| `<scroll>` | `ScrollRect` + `Viewport` + `Content` | Прокручиваемый контейнер |
| `<text>` | `TextMeshProUGUI` | Текстовый элемент |
| `<image>` | `Image` | Изображение |
| `<button>` | `Button` + `Image` | Кнопка |
| `<toggle>` | `Toggle` + `Image` | Переключатель |
| `<slider>` | `Slider` + дочерние `Image` | Ползунок |
| `<input>` | `TMP_InputField` | Поле ввода |
| `<dropdown>` | `TMP_Dropdown` | Выпадающий список |
| `<mask>` | `Mask` или `RectMask2D` | Маска |
| `<canvas-group>` | `CanvasGroup` | Группа с общим alpha/interactable |
| `<raw-image>` | `RawImage` | Для рендер-текстур |
| `<container>` | `GameObject` + `RectTransform` | Пустой контейнер |

### 3.2. Атрибуты элементов

Каждый элемент поддерживает общие атрибуты:

```xml
<element
    class="css-class1 css-class2"       <!-- CSS-классы -->
    id="unique-id"                       <!-- Уникальный идентификатор -->
    visible="{expression}"               <!-- Видимость -->
    enabled="{expression}"               <!-- Интерактивность -->
    if="{condition}"                     <!-- Условный рендеринг (элемент не создаётся) -->
/>
```

Специфичные атрибуты по типам элементов:

```xml
<!-- text -->
<text text="{expression}" />
<text>Статический текст</text>

<!-- image -->
<image sprite="path/to/sprite" />
<image sprite="{expression}" tint="#FF0000" />

<!-- button -->
<button onClick="{MethodName}" interactable="{expression}">
    <!-- дочерние элементы -->
</button>

<!-- toggle -->
<toggle isOn="{expression}" onToggled="{MethodName}" />

<!-- slider -->
<slider value="{expression}" min="0" max="100" wholeNumbers="true"
        onValueChanged="{MethodName}" />

<!-- input -->
<input text="{expression}" placeholder="Enter text..."
       contentType="standard|integer|decimal|alphanumeric|email|password"
       characterLimit="50"
       onValueChanged="{MethodName}"
       onEndEdit="{MethodName}" />

<!-- dropdown -->
<dropdown options="{expression}" selected="{expression}"
          onSelected="{MethodName}" />

<!-- scroll -->
<scroll direction="vertical|horizontal|both"
        elasticity="0.1" inertia="true">
    <!-- контент -->
</scroll>

<!-- grid -->
<grid columns="4" cellSize="100,100" spacing="8,8">
    <!-- элементы сетки -->
</grid>

<!-- canvas-group -->
<canvas-group alpha="{expression}" interactable="{expression}"
              blocksRaycasts="{expression}">
    <!-- дочерние элементы -->
</canvas-group>
```

### 3.3. Описание компонента

```xml
<!-- Components/ComponentName/ComponentName.xml -->

<component name="ComponentName">

    <!-- Определение входных параметров -->
    <props>
        <prop name="title"
              type="string"
              default="Default Title" />

        <prop name="count"
              type="int"
              default="0" />

        <prop name="icon"
              type="sprite"
              optional="true" />

        <prop name="variant"
              type="enum(primary,secondary,danger)"
              default="primary" />

        <prop name="items"
              type="list<string>" />

        <prop name="onClick"
              type="event" />

        <prop name="onValueChanged"
              type="event<float>" />
    </props>

    <!-- Шаблон разметки -->
    <template>
        <!-- Разметка компонента -->
        <!-- Все CSS-классы автоматически скоупятся -->
    </template>

</component>
```

**Поддерживаемые типы props:**

| Тип | Описание | C# маппинг |
|---|---|---|
| `string` | Строка | `string` |
| `int` | Целое число | `int` |
| `float` | Дробное число | `float` |
| `bool` | Логическое значение | `bool` |
| `sprite` | Ссылка на спрайт | `Sprite` |
| `texture` | Ссылка на текстуру | `Texture` |
| `color` | Цвет | `Color` |
| `font` | Ссылка на шрифт | `TMP_FontAsset` |
| `enum(a,b,c)` | Перечисление | `string` (с валидацией) |
| `list<T>` | Список | `IReadOnlyList<T>` |
| `event` | Событие без параметров | `Action` |
| `event<T>` | Событие с параметром | `Action<T>` |

### 3.4. Описание экрана

```xml
<!-- Screens/ScreenName/ScreenName.xml -->

<screen name="ScreenName" viewmodel="ScreenNameViewModel">

    <template>
        <!-- Разметка экрана -->
        <!-- Может использовать кастомные компоненты: <Button />, <PlayerCard /> -->
        <!-- Может использовать биндинги к ViewModel: {PropertyName} -->
    </template>

</screen>
```

### 3.5. Слоты (Slots)

Компоненты могут определять слоты для вставки дочернего контента:

```xml
<!-- Определение компонента с дефолтным слотом -->
<component name="Card">
    <props>
        <prop name="title" type="string" />
    </props>
    <template>
        <column class="card">
            <text class="card-title" text="{title}" />
            <column class="card-body">
                <slot />    <!-- дефолтный слот -->
            </column>
        </column>
    </template>
</component>

<!-- Определение компонента с именованными слотами -->
<component name="Layout">
    <template>
        <column class="layout">
            <row class="layout-header">
                <slot name="header" />
            </row>
            <column class="layout-body">
                <slot />                     <!-- дефолтный слот -->
            </column>
            <row class="layout-footer">
                <slot name="footer" />
            </row>
        </column>
    </template>
</component>

<!-- Использование -->
<Layout>
    <slot-content name="header">
        <text text="Title" />
    </slot-content>

    <!-- Без указания name → попадает в дефолтный слот -->
    <text text="Body content" />

    <slot-content name="footer">
        <Button text="OK" />
    </slot-content>
</Layout>
```

### 3.6. Условный рендеринг и циклы

```xml
<!-- Условный рендеринг — элемент не создаётся если false -->
<text if="{HasError}" class="error" text="{ErrorMessage}" />

<!-- Показать/скрыть — элемент создаётся, но меняется видимость -->
<image visible="{IsLoading}" sprite="spinner" />

<!-- Цикл по коллекции -->
<column>
    <foreach items="{PlayerList}" var="player">
        <PlayerCard
            name="{player.Name}"
            level="{player.Level}"
            avatar="{player.Avatar}"
            onClick="{() => SelectPlayer(player)}" />
    </foreach>
</column>

<!-- Цикл с индексом -->
<grid columns="4">
    <foreach items="{InventorySlots}" var="slot" index="i">
        <ItemSlot
            item="{slot.Item}"
            isEmpty="{slot.Item == null}"
            slotIndex="{i}" />
    </foreach>
</grid>
```

### 3.7. Ссылки на ресурсы

```xml
<!-- Спрайты — путь относительно Resources или Addressables -->
<image sprite="Sprites/Icons/sword" />
<image sprite="{DynamicSpritePath}" />

<!-- Шрифты -->
<text font="Fonts/Inter-Bold" text="Bold text" />

<!-- Ресурсы из темы -->
<image sprite="theme:icon-close" />
```

---

## 4. Система стилей (USS)

### 4.1. Синтаксис

Стили описываются в `.uss`-файлах с CSS-подобным синтаксисом. Расширение `.uss` (UI Style Sheet) выбрано для отличия от стандартных CSS-файлов и совместимости с Unity-конвенциями.

### 4.2. Селекторы

```css
/* По классу */
.button { }

/* По ID */
#main-title { }

/* По типу элемента */
text { }
button { }
image { }

/* Комбинации */
.card .title { }            /* потомок */
.card > .title { }           /* прямой потомок */
.btn.primary { }             /* оба класса */

/* Псевдоклассы */
button:hover { }
button:pressed { }
button:disabled { }
toggle:checked { }
input:focused { }

/* Псевдоклассы для компонентов с вариантами */
.btn-primary { }             /* через динамический класс btn-{variant} */
```

**Поддерживаемые псевдоклассы:**

| Псевдокласс | Описание | uGUI-маппинг |
|---|---|---|
| `:hover` | Курсор наведён | `IPointerEnterHandler` / `IPointerExitHandler` |
| `:pressed` | Нажат | `IPointerDownHandler` / `IPointerUpHandler` |
| `:disabled` | Неактивен | `interactable = false` |
| `:checked` | Включён (toggle) | `Toggle.isOn = true` |
| `:focused` | В фокусе (input) | `TMP_InputField.isFocused` |
| `:first-child` | Первый дочерний | По индексу в иерархии |
| `:last-child` | Последний дочерний | По индексу в иерархии |

### 4.3. Поддерживаемые CSS-свойства

Полный список свойств в `CSSProperties.SupportedProperties`. Дополнительно: `material`, `box-shadow`.

#### Layout

| CSS-свойство | uGUI-маппинг | Допустимые значения |
|---|---|---|
| `width` | `RectTransform.sizeDelta.x` или `LayoutElement.preferredWidth` | `число`, `%`, `auto` |
| `height` | `RectTransform.sizeDelta.y` или `LayoutElement.preferredHeight` | `число`, `%`, `auto` |
| `min-width` | `LayoutElement.minWidth` | `число` |
| `min-height` | `LayoutElement.minHeight` | `число` |
| `max-width` | `LayoutElement.preferredWidth` + constraint | `число` |
| `max-height` | `LayoutElement.preferredHeight` + constraint | `число` |
| `flex` | `LayoutElement.flexibleWidth/Height` | `число` |
| `padding` | `LayoutGroup.padding` | `число` или `top right bottom left` |
| `padding-left/right/top/bottom` | Соответствующие поля padding | `число` |
| `gap` | `LayoutGroup.spacing` | `число` |
| `align-items` | `LayoutGroup.childAlignment` + `childForceExpand` | `flex-start`, `center`, `flex-end`, `stretch` |
| `justify-content` | `LayoutGroup.childAlignment` | `flex-start`, `center`, `flex-end`, `space-between` |
| `flex-direction` | Тип LayoutGroup | `row`, `column` |
| `overflow` | `Mask` или `RectMask2D` | `visible`, `hidden`, `scroll` |

#### Визуальные

| CSS-свойство | uGUI-маппинг | Допустимые значения |
|---|---|---|
| `background-color` | `Image.color` | цвет |
| `background-image` | `Image.sprite` | путь к спрайту |
| `background-size` | `Image.type` | `cover` → `Sliced`, `contain` → `PreserveAspect` |
| `opacity` | `CanvasGroup.alpha` (создаётся автоматически) | `0` — `1` |
| `border-radius` | `Image` с 9-slice или процедурная генерация | `число` или `tl tr br bl` |
| `border` | `Outline` компонент или доп. `Image` | `width style color` |
| `border-color` | Цвет бордера | цвет |
| `border-width` | Ширина бордера | `число` |
| `tint` | `Image.color` / `Graphic.color` | цвет |
| `scale` | `RectTransform.localScale` | `число` |
| `rotation` | `RectTransform.localEulerAngles.z` | `число` (градусы) |
| `material` | `Image.material` | путь к Material (Resources) |
| `box-shadow` | Дочерний GameObject с UIX-Shadow | `offsetX offsetY blur color` или `color` |

#### Текст

| CSS-свойство | uGUI-маппинг | Допустимые значения |
|---|---|---|
| `font-family` | `TextMeshProUGUI.font` | имя шрифтового ассета |
| `font-size` | `TextMeshProUGUI.fontSize` | `число` |
| `font-weight` | `TextMeshProUGUI.fontStyle` | `normal`, `bold` |
| `font-style` | `TextMeshProUGUI.fontStyle` | `normal`, `italic` |
| `color` | `TextMeshProUGUI.color` | цвет |
| `text-align` | `TextMeshProUGUI.alignment` | `left`, `center`, `right` |
| `text-overflow` | `TextMeshProUGUI.overflowMode` | `clip`, `ellipsis`, `truncate` |
| `line-height` | `TextMeshProUGUI.lineSpacing` | `число` или `%` |
| `letter-spacing` | `TextMeshProUGUI.characterSpacing` | `число` |
| `white-space` | `TextMeshProUGUI.enableWordWrapping` | `normal`, `nowrap` |

#### Анимации/Переходы

| CSS-свойство | uGUI-маппинг | Допустимые значения |
|---|---|---|
| `transition` | Runtime-интерполяция значений | `property duration [easing]` |
| `transition-property` | Какое свойство анимировать | имя CSS-свойства |
| `transition-duration` | Длительность | `число` (секунды) |
| `transition-easing` | Кривая | `ease`, `linear`, `ease-in`, `ease-out`, `ease-in-out` |

### 4.4. Форматы цветов

Поддерживаемые форматы (ColorParser):

| Формат | Пример |
|--------|--------|
| Hex 3 | `#F00` |
| Hex 6 | `#FF0000` |
| Hex 8 | `#FF000080` |
| rgb() | `rgb(255, 0, 0)` |
| rgba() | `rgba(255, 0, 0, 0.5)` |
| Именованные | `black`, `white`, `red`, `transparent` |

### 4.5. Переменные (CSS Custom Properties)

```css
/* Определение */
:root {
    --color-primary: #3A7BF2;
    --spacing-md: 16;
    --font-main: "Inter";
}

/* Использование */
.button {
    background-color: var(--color-primary);
    padding: var(--spacing-md);
    font-family: var(--font-main);
}

/* Fallback-значение */
.button {
    background-color: var(--color-accent, #FF0000);
}
```

### 4.6. Scoping стилей

Стили компонентов автоматически скоупятся. Селекторы из `Button.uss` не влияют на элементы вне компонента `Button`.

Механизм: при компиляции класс `.btn` внутри `Button.uss` трансформируется в `.Button__btn`. В итоговом uGUI-объекте используется трансформированный класс.

Стили экрана (`ScreenName.uss`) скоупятся на уровень экрана, но не проникают внутрь компонентов. Для стилизации компонента извне используются props, передающие класс:

```xml
<Button text="OK" class="large-button" />
```

Компонент может принимать внешний класс через специальный `class`-атрибут. Этот класс применяется к корневому элементу шаблона компонента. Стили этого класса разрешаются в контексте экрана, а не компонента.

### 4.7. Каскад и специфичность

Порядок приоритета (от низкого к высокому):

1. Стили по типу элемента (`text { }`)
2. Стили по классу (`.title { }`)
3. Стили по ID (`#main-title { }`)
4. Inline-стили (через атрибут `style=""` — поддерживается, но не рекомендуется)
5. Стили из темы (переменные переопределяют значения)

При равной специфичности — последний объявленный побеждает.

---

## 5. Система тем

### 5.1. Структура темы

Тема — это `.uss`-файл, содержащий только определение переменных в `:root`:

```css
/* Themes/DarkTheme/theme.uss */

:root {
    /* Colors */
    --color-primary: #BB86FC;
    --color-primary-variant: #3700B3;
    --color-secondary: #03DAC6;
    --color-background: #121212;
    --color-surface: #1E1E1E;
    --color-error: #CF6679;
    --color-on-primary: #000000;
    --color-on-background: #FFFFFF;
    --color-on-surface: #FFFFFF;
    --color-text: #FFFFFF;
    --color-text-muted: #8A8A8A;

    /* Typography */
    --font-main: "Inter";
    --font-size-xs: 12;
    --font-size-sm: 14;
    --font-size-md: 16;
    --font-size-lg: 20;
    --font-size-xl: 28;
    --font-size-xxl: 36;

    /* Spacing */
    --spacing-xs: 4;
    --spacing-sm: 8;
    --spacing-md: 16;
    --spacing-lg: 24;
    --spacing-xl: 32;
    --spacing-xxl: 48;

    /* Borders */
    --radius-sm: 4;
    --radius-md: 8;
    --radius-lg: 16;
    --radius-full: 9999;

    /* Shadows / Effects */
    --transition-fast: 0.15;
    --transition-normal: 0.3;
    --transition-slow: 0.5;
}
```

Тема также может содержать ссылки на ресурсы:

```css
:root {
    --icon-close: "Themes/DarkTheme/Icons/close";
    --icon-back: "Themes/DarkTheme/Icons/back";
    --background-pattern: "Themes/DarkTheme/Textures/pattern";
}
```

### 5.2. UIXTheme ScriptableObject

При компиляции `.uss`-файла темы создаётся `UIXTheme` ScriptableObject:

```csharp
[CreateAssetMenu(menuName = "UIX/Theme")]
public class UIXTheme : ScriptableObject
{
    public string ThemeName;
    public List<ThemeVariable> Variables;
    public List<ThemeResource> Resources;
}
```

### 5.3. Переключение тем

```csharp
// Runtime API
UIXThemeManager.SetTheme("DarkTheme");
UIXThemeManager.SetTheme(darkThemeSO);

// Получить текущую тему
var current = UIXThemeManager.CurrentTheme;

// Подписка на смену темы
UIXThemeManager.OnThemeChanged += (oldTheme, newTheme) => { };
```

При переключении темы:
1. Все `var()` переменные пересчитываются
2. `StyleApplicator` проходит по всем активным UI-элементам
3. Значения свойств обновляются без пересоздания GameObjects
4. Анимации transition применяются к изменениям (если заданы)

---

## 6. Система биндинга данных

### 6.1. ReactiveProperty

```csharp
public class ReactiveProperty<T>
{
    public T Value { get; set; }
    public event Action<T> OnChanged;

    public ReactiveProperty(T initialValue = default);

    // Implicit conversion
    public static implicit operator T(ReactiveProperty<T> prop);
}
```

### 6.2. ReactiveCollection

```csharp
public class ReactiveCollection<T> : IReadOnlyList<T>
{
    public event Action OnCollectionChanged;
    public event Action<int, T> OnItemAdded;
    public event Action<int, T> OnItemRemoved;
    public event Action<int, T, T> OnItemReplaced;

    public void Add(T item);
    public void Remove(T item);
    public void RemoveAt(int index);
    public void Insert(int index, T item);
    public void Clear();
    public T this[int index] { get; set; }
}
```

### 6.3. ViewModel

```csharp
public abstract class ViewModel
{
    // Доступ к навигации
    protected UIXNavigator Navigator { get; }

    // Lifecycle
    public virtual void OnCreated() { }
    public virtual void OnShown() { }
    public virtual void OnHidden() { }
    public virtual void OnDestroyed() { }
}
```

### 6.4. Синтаксис биндингов в XML

```xml
<!-- Простой биндинг к свойству -->
<text text="{PlayerName}" />

<!-- Биндинг к вложенному свойству -->
<text text="{CurrentPlayer.Stats.Level}" />

<!-- Форматированный биндинг -->
<text text="Level: {Level}" />
<text text="{Health}/{MaxHealth} HP" />

<!-- Биндинг с форматированием числа -->
<text text="{Score:N0}" />           <!-- 1,234,567 -->
<text text="{Progress:P0}" />         <!-- 75% -->
<text text="{Price:F2}" />            <!-- 19.99 -->

<!-- Биндинг к методу (event) -->
<button onClick="{OnPlayClicked}" />

<!-- Биндинг с lambda-выражением (для foreach) -->
<button onClick="{() => SelectItem(item)}" />

<!-- Условный биндинг -->
<text visible="{IsLoggedIn}" />
<button enabled="{CanSubmit}" />

<!-- Отрицание -->
<text visible="{!IsLoading}" text="Ready" />

<!-- Тернарный оператор -->
<text text="{IsOnline ? 'Online' : 'Offline'}" />

<!-- Сравнение -->
<image visible="{Health < 20}" sprite="warning" />

<!-- Двусторонний биндинг (для input, slider, toggle) -->
<input text="{=Username}" />
<slider value="{=Volume}" />
<toggle isOn="{=IsMuted}" />
```

Двусторонний биндинг обозначается префиксом `=` и автоматически обновляет `ReactiveProperty` при изменении UI-элемента пользователем.

---

## 7. Asset Pipeline

### 7.1. Процесс компиляции

При сохранении или импорте `.xml` / `.uss` файлов запускается `UIXAssetProcessor`:

```
[Файл сохранён]
      │
      ▼
[UIXAssetProcessor.OnPostprocessAllAssets]
      │
      ├── .xml файл изменён?
      │     │
      │     ▼
      │   [XMLParser.Parse] → UIXNode дерево
      │     │
      │     ▼
      │   [UIXValidation.Validate] → ошибки/предупреждения → Console
      │     │
      │     ▼
      │   [UIXCompiler.CompileTemplate] → UIXTemplate.asset
      │     │
      │     ▼
      │   [UIXPrefabGenerator.Generate] → Prefab в _Generated/
      │     │
      │     ▼
      │   [UIXBindingCodeGen.Generate] → *_Bindings.generated.cs (если screen)
      │
      ├── .uss файл изменён?
      │     │
      │     ▼
      │   [USSParser.Parse] → список StyleRule
      │     │
      │     ▼
      │   [UIXCompiler.CompileStyleSheet] → UIXStyleSheet.asset
      │     │
      │     ▼
      │   [Перегенерировать затронутые Prefab'ы]
      │
      └── theme.uss файл изменён?
            │
            ▼
          [USSParser.Parse] → переменные
            │
            ▼
          [UIXCompiler.CompileTheme] → UIXTheme.asset
            │
            ▼
          [Перегенерировать все Prefab'ы, использующие эту тему]
```

### 7.2. Валидация

`UIXValidation` проверяет:

- **XML:**
  - Корректность синтаксиса XML
  - Все используемые компоненты зарегистрированы в `ComponentRegistry`
  - Все обязательные props переданы
  - Типы props соответствуют определению
  - Биндинг-выражения корректны синтаксически
  - ViewModel-класс существует и содержит указанные свойства/методы (если screen)
  - Слоты используются корректно

- **USS:**
  - Корректность синтаксиса CSS
  - Все `var()` ссылаются на существующие переменные (или имеют fallback)
  - Все свойства из поддерживаемого списка
  - Значения свойств валидны для их типов

Результаты валидации выводятся в Unity Console с указанием файла и строки. Формат ошибки: `path(line): message` для кликабельных ссылок.

### 7.3. Генерация prefab

`UIXPrefabGenerator` создаёт uGUI-иерархию:

1. Создаёт корневой `GameObject` с `RectTransform` и `UIXScreen`/`UIXComponent`
2. Рекурсивно обходит дерево `UIXNode`
3. Для каждого элемента вызывает соответствующий `IElementRenderer`
4. `IElementRenderer` создаёт `GameObject` с нужными uGUI-компонентами
5. `StyleApplicator` применяет resolved стили к компонентам
6. `LayoutMapper` настраивает `LayoutGroup`-ы и `LayoutElement`-ы
7. Для компонентных нод (`ComponentNode`) — вставляет prefab компонента
8. Сохраняет как `.prefab` в `_Generated/`

### 7.4. Генерация binding-кода

Для экранов генерируется C#-файл с биндингами:

```csharp
// _Generated/Screens/Settings_Bindings.generated.cs

// AUTO-GENERATED FILE. DO NOT EDIT.

public partial class SettingsScreen_Bindings : UIXBindingBase
{
    private TextMeshProUGUI _headerTitle;
    private Slider _musicSlider;
    // ...

    public override void Bind(ViewModel viewModel, GameObject root)
    {
        var vm = (SettingsViewModel)viewModel;

        _headerTitle = root.FindDeep<TextMeshProUGUI>("HeaderTitle");
        _musicSlider = root.FindDeep<Slider>("MusicSlider");

        // Reactive bindings
        vm.MusicVolume.OnChanged += v => _musicSlider.value = v;
        _musicSlider.onValueChanged.AddListener(v => vm.OnMusicChanged(v));

        // Initial values
        _musicSlider.value = vm.MusicVolume.Value;
        // ...
    }

    public override void Unbind() { /* cleanup */ }
}
```

### 7.5. Инкрементальная компиляция

Pipeline отслеживает зависимости:

- Компонент `Button` изменён → перегенерировать все экраны, использующие `<Button />`
- Тема изменена → перегенерировать все prefab'ы
- USS компонента изменён → перегенерировать prefab компонента и все его потребители

Граф зависимостей хранится в `UIXDependencyGraph.asset` и обновляется при каждой компиляции.

---

## 8. Навигация

### 8.1. UIXNavigator API

```csharp
public class UIXNavigator
{
    // Показать экран (добавить в стек)
    public void Push<T>() where T : ViewModel;
    public void Push<T>(object props) where T : ViewModel;

    // Показать экран (заменить текущий)
    public void Replace<T>() where T : ViewModel;
    public void Replace<T>(object props) where T : ViewModel;

    // Вернуться назад
    public void Pop();
    public void PopToRoot();
    public void PopTo<T>() where T : ViewModel;

    // Модальные окна
    public void ShowModal<T>() where T : ViewModel;
    public void ShowModal<T>(object props) where T : ViewModel;
    public void CloseModal();
    public void CloseAllModals();

    // Overlay (поверх всего, не в стеке)
    public void ShowOverlay<T>() where T : ViewModel;
    public void HideOverlay<T>() where T : ViewModel;

    // Состояние
    public ViewModel CurrentScreen { get; }
    public IReadOnlyList<ViewModel> ScreenStack { get; }
    public bool HasModal { get; }

    // Events
    public event Action<ViewModel> OnScreenPushed;
    public event Action<ViewModel> OnScreenPopped;
    public event Action<ViewModel> OnModalShown;
    public event Action<ViewModel> OnModalClosed;
}
```

### 8.2. Анимации переходов

```csharp
public enum TransitionType
{
    None,
    Fade,
    SlideLeft,
    SlideRight,
    SlideUp,
    SlideDown,
    Scale,
    Custom
}
```

Настройка в XML:

```xml
<screen name="Settings"
        viewmodel="SettingsViewModel"
        transition-in="SlideRight"
        transition-out="SlideLeft"
        transition-duration="0.3">
```

---

## 9. Component Registry

### 9.1. Автоматическая регистрация

Все компоненты из папки `Components/` автоматически регистрируются при компиляции. `ComponentRegistry` ScriptableObject хранит: `ComponentEntry` с `Name`, `SourceXMLPath`, `Definition`, `Template`, `Prefab`, `Props`, `Slots`, `UsedByScreens`, `UsedByComponents`.

### 9.2. Ручная регистрация программных компонентов

```csharp
[UIXComponent("CustomChart")]
public class CustomChartComponent : UIXComponentBase
{
    [UIXProp] public ReactiveProperty<float[]> Data { get; set; }
    [UIXProp] public ReactiveProperty<Color> ChartColor { get; set; }

    protected override void OnRender(RectTransform container)
    {
        // Программное создание UI
    }
}
```

---

## 10. Runtime API

### 10.1. UIXEngine — главная точка входа

```csharp
public static class UIXEngine
{
    public static void Initialize(UIXConfiguration config);
    public static UIXNavigator Navigator { get; }
    public static UIXThemeManager Themes { get; }
    public static ComponentRegistry Registry { get; }
    public static UIXRenderer Renderer { get; }
}
```

### 10.2. UIXConfiguration

```csharp
[CreateAssetMenu(menuName = "UIX/Configuration")]
public class UIXConfiguration : ScriptableObject
{
    public UIXTheme DefaultTheme;
    public UIXTheme[] AvailableThemes;
    public Canvas TargetCanvas;
    public TransitionType DefaultTransition;
    public float DefaultTransitionDuration;
    public bool EnableHotReload;
    public string GeneratedAssetsPath;
    public string ComponentsSearchPath;
    public string ScreensSearchPath;
    public string ThemesSearchPath;
}
```

---

## 11. Editor Tools

- **UIXPreviewWindow** — превью экранов/компонентов, mock-данные, темы
- **UIXComponentScaffolder** — создание компонента
- **UIXScreenScaffolder** — создание экрана
- **Theme Editor** — визуальное редактирование переменных
- **Inspector расширения** — UIXTemplate, UIXTheme, UIXScreen
- **UIXScreenshotCapture** — CaptureScreen, CaptureToFile

---

## 12. UACF Integration

Фреймворк предоставляет UACF-tool'ы: `uix_create_component`, `uix_create_screen`, `uix_update_component`, `uix_update_screen`, `uix_get_component`, `uix_get_screen`, `uix_list_components`, `uix_list_screens`, `uix_delete_component`, `uix_delete_screen`, `uix_update_theme`, `uix_get_theme`, `uix_list_themes`, `uix_set_theme`, `uix_add_prop`, `uix_remove_prop`, `uix_build_ui`, `uix_validate`, `uix_screenshot`, `uix_get_registry`.

Подробности в `.cursor/rules/uacf-mandatory.mdc` и UACF HTTP API.

---

## 13. Встроенные компоненты

| Компонент | Описание |
|---|---|
| UIXButton | Кнопка с вариантами, размерами, иконкой |
| UIXText | Текст с вариантами (h1-h6, body, caption) |
| UIXInput | Поле ввода с label, placeholder |
| UIXToggle | Переключатель с label |
| UIXSlider | Ползунок с label |
| UIXDropdown | Выпадающий список |
| UIXCard | Контейнер-карточка |
| UIXModal | Модальное окно |
| UIXDivider | Разделитель |
| UIXSpacer | Пустое пространство |
| UIXBadge | Бейдж |
| UIXProgressBar | Прогресс-бар |
| UIXTabs | Табы |
| UIXTooltip | Подсказка |
| UIXList | Виртуализированный список |

---

## 14. Материалы и шейдеры

### 14.1 Обзор

UIX использует кастомные HLSL-шейдеры (Built-in Unlit) для `border-radius`, `box-shadow`, `border`, `background-image`.

### 14.2 Шейдеры

| Шейдер | Назначение |
|--------|------------|
| UIX/Solid | Сплошной цвет |
| UIX/RoundedRect | border-radius, border |
| UIX/Image | Текстура + tint |
| UIX/RoundedImage | Текстура + border-radius |
| UIX/Shadow | box-shadow |

### 14.3 UIXMaterialRegistry

```csharp
var mat = UIXMaterialRegistry.GetMaterial(UIXMaterialRegistry.MaterialType.Solid);
var roundedMat = UIXMaterialRegistry.GetRoundedMaterial(0.1f);
var customMat = UIXMaterialRegistry.GetCustomMaterial("UIX/Materials/GameCard");
UIXMaterialRegistry.RegisterCustomMaterial("theme:card", myMaterial);
```

### 14.4 Маппинг USS → материалы

| USS-свойство | Действие |
|--------------|----------|
| `background-color` | Image.color + UIX-Solid |
| `background-image` | Resources.Load<Sprite> |
| `border-radius` | UIX-RoundedRect / UIX-RoundedImage |
| `box-shadow` | Дочерний GameObject с UIX-Shadow |
| `material` | Resources.Load<Material> |

### 14.5 Формат box-shadow

```css
box-shadow: 4 4 8 rgba(0,0,0,0.3);
box-shadow: #00000080;
```

---

## 15. Виртуализация списков

`UIXList` использует object pooling для больших коллекций. Рендерятся только видимые элементы + буфер.

---

## 16. Обработка ошибок

- **Compile-time:** ошибки в Console с кликабельной ссылкой
- **Runtime:** логирование, placeholder при отсутствии компонента/спрайта
- **UIXLogger.MinLevel:** Verbose, Debug, Info, Warning, Error

---

## 17. Настройки проекта

`Edit → Project Settings → UIX`: Components Path, Screens Path, Themes Path, Generated Path, Default Theme, Auto Compile, Auto Generate Prefabs, Log Level.

---

## 18. Ограничения и границы скоупа

Не включает: анимации спрайтов, particle effects, blur/glow шейдеры, 3D в UI, интеграцию с БД, accessibility, Figma.

---

## 19. Зависимости

Unity 2021.3+, TextMeshPro 3.0+, UACF latest.

---

## 20. Gitignore

```gitignore
Assets/UI/_Generated/
*_Bindings.generated.cs
```

---

## 21. Устранение неполадок

### 21.1 Компиляция не запускается

- Auto Compile = true
- Файлы в путях Components/Screens/Themes
- Console на ошибки UIXValidation

### 21.2 Компонент не найден в Registry

- Компонент в папке Components
- XML корректен
- `uix.build_ui` с targets `["all"]`

### 21.3 Стили не применяются

- UIXStyleSheet.asset создан
- Селектор совпадает
- VariableResolver: переменные темы загружены

### 21.4 border-radius / box-shadow не отображаются

- Шейдеры UIX/* в пакете
- UIXMaterialRegistry.GetMaterial не null
- box-shadow: `offsetX offsetY blur color`

### 21.5 Биндинги не работают

- ViewModel содержит свойства/методы
- *_Bindings.generated.cs сгенерирован
- EvaluateBinding в RenderContext
