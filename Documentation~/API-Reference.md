# UIX Framework — API Reference

Подробная справка по публичным API фреймворка.

---

## UIXEngine

Главная точка входа. Статический класс.

### Initialize

```csharp
public static void Initialize(UIXConfiguration config)
```

Инициализирует фреймворк. Вызывать один раз при старте приложения (например, в Awake/Start).

**Параметры:**
- `config` — конфигурация с путями, темой, Canvas

**Исключения:** может выбросить при некорректной конфигурации.

---

### Navigator

```csharp
public static UIXNavigator Navigator { get; }
```

Навигатор экранов. Доступен после Initialize.

---

### Themes

```csharp
public static UIXThemeManager Themes { get; }
```

Менеджер тем. Методы SetTheme, CurrentTheme, OnThemeChanged.

---

### Registry

```csharp
public static ComponentRegistry Registry { get; }
```

Реестр компонентов. Список всех зарегистрированных компонентов.

---

### Renderer

```csharp
public static UIXRenderer Renderer { get; }
```

Рендерер. Создаёт uGUI из UIXTemplate.

---

## UIXNavigator

Навигация между экранами.

### Push

```csharp
public void Push<T>() where T : ViewModel
public void Push<T>(object props) where T : ViewModel
```

Добавляет экран в стек. `props` передаётся в IViewModelWithProps.SetProps.

---

### Replace

```csharp
public void Replace<T>() where T : ViewModel
public void Replace<T>(object props) where T : ViewModel
```

Заменяет текущий экран.

---

### Pop

```csharp
public void Pop()
public void PopToRoot()
public void PopTo<T>() where T : ViewModel
```

Удаляет экран(ы) из стека.

---

### ShowModal / CloseModal

```csharp
public void ShowModal<T>() where T : ViewModel
public void ShowModal<T>(object props) where T : ViewModel
public void CloseModal()
public void CloseAllModals()
```

Модальные окна поверх стека.

---

### ShowOverlay / HideOverlay

```csharp
public void ShowOverlay<T>() where T : ViewModel
public void ShowOverlay<T>(object props) where T : ViewModel
public void HideOverlay<T>() where T : ViewModel
```

Overlay поверх всего (не в стеке).

---

### Свойства

```csharp
public ViewModel CurrentScreen { get; }
public IReadOnlyList<ViewModel> ScreenStack { get; }
public bool HasModal { get; }
```

---

### События

```csharp
public event Action<ViewModel> OnScreenPushed;
public event Action<ViewModel> OnScreenPopped;
public event Action<ViewModel> OnModalShown;
public event Action<ViewModel> OnModalClosed;
```

---

## UIXThemeManager

Управление темами.

### SetTheme

```csharp
public void SetTheme(string themeName)
public void SetTheme(UIXTheme theme)
```

Устанавливает активную тему. `themeName` — имя для Resources.Load($"Themes/{themeName}").

---

### CurrentTheme

```csharp
public UIXTheme CurrentTheme { get; }
```

Текущая тема.

---

### OnThemeChanged

```csharp
public event Action<UIXTheme, UIXTheme> OnThemeChanged;
```

Вызывается при смене темы. Параметры: (oldTheme, newTheme).

---

## UIXMaterialRegistry

Реестр материалов для UI. Статический класс.

### GetMaterial

```csharp
public static Material GetMaterial(MaterialType type)
```

Возвращает кэшированный материал. Типы: Solid, Rounded, Image, RoundedImage, Shadow.

---

### GetRoundedMaterial

```csharp
public static Material GetRoundedMaterial(float radius)
```

Создаёт новый Material с заданным radius (0–0.5, нормализовано).

---

### GetRoundedImageMaterial

```csharp
public static Material GetRoundedImageMaterial(float radius)
```

Аналог для текстуры с border-radius.

---

### GetCustomMaterial

```csharp
public static Material GetCustomMaterial(string path)
```

Загружает материал через Resources.Load<Material>(path).

---

### RegisterCustomMaterial

```csharp
public static void RegisterCustomMaterial(string path, Material material)
```

Регистрирует материал для пути (например, из темы).

---

### PixelRadiusToNormalized

```csharp
public static float PixelRadiusToNormalized(float pixelRadius, Vector2 rectSize)
```

Конвертирует радиус в пикселях в нормализованный (0–0.5) для шейдера.

---

### ClearCache

```csharp
public static void ClearCache()
```

Очищает кэш материалов. Вызывать при смене темы или domain reload.

---

## ReactiveProperty<T>

Реактивное свойство с событием OnChanged.

### Конструктор

```csharp
public ReactiveProperty(T initialValue = default)
```

---

### Value

```csharp
public T Value { get; set; }
```

Текущее значение.

---

### OnChanged

```csharp
public event Action<T> OnChanged;
```

Вызывается при изменении Value.

---

### implicit operator T

```csharp
public static implicit operator T(ReactiveProperty<T> prop)
```

Позволяет использовать prop как T в выражениях.

---

## ReactiveCollection<T>

Реактивная коллекция. Реализует IReadOnlyList<T>.

### Add, Remove, RemoveAt, Insert, Clear

```csharp
public void Add(T item)
public void Remove(T item)
public void RemoveAt(int index)
public void Insert(int index, T item)
public void Clear()
```

---

### События

```csharp
public event Action OnCollectionChanged;
public event Action<int, T> OnItemAdded;
public event Action<int, T> OnItemRemoved;
public event Action<int, T, T> OnItemReplaced;
```

---

### Индексатор

```csharp
public T this[int index] { get; set; }
```

---

## ViewModel

Базовый класс для экранов и overlay.

### Navigator

```csharp
protected UIXNavigator Navigator { get; }
```

Доступ к навигации.

---

### Lifecycle

```csharp
public virtual void OnCreated() { }
public virtual void OnShown() { }
public virtual void OnHidden() { }
public virtual void OnDestroyed() { }
```

Переопределять при необходимости.

---

## IViewModelWithProps

Интерфейс для ViewModel с входными параметрами.

```csharp
public interface IViewModelWithProps
{
    void SetProps(object props);
}
```

Реализовать для Push/Replace/ShowModal/ShowOverlay с props.

---

## UIXBindingExtensions

Расширения для поиска элементов.

### FindDeep

```csharp
public static T FindDeep<T>(this GameObject root, string id) where T : Component
```

Рекурсивный поиск дочернего GameObject по имени (id) и возврат компонента типа T.

---

## StyleApplicator

Применение стилей к uGUI. Статический класс.

### ApplyToElement

```csharp
public static void ApplyToElement(GameObject go, IReadOnlyDictionary<string, string> styles)
```

Применяет стили к GameObject (Image, RawImage, Text, CanvasGroup, RectTransform, box-shadow).

---

### ApplyToImage

```csharp
public static void ApplyToImage(Image image, IReadOnlyDictionary<string, string> styles)
```

Применяет стили к Image: background-color, background-image, background-size, border-radius, border, material.

---

### AddBackgroundIfNeeded

```csharp
public static void AddBackgroundIfNeeded(GameObject go, IReadOnlyDictionary<string, string> styles)
```

Добавляет дочерний Image как фон, если заданы background-color или background-image.

---

### ReapplyStyles

```csharp
public static void ReapplyStyles(GameObject go, IReadOnlyDictionary<string, string> styles)
```

Повторное применение стилей (для pseudo-classes).

---

## UIXLogger

Уровни логирования.

```csharp
public static UIXLogLevel MinLevel = UIXLogLevel.Warning;
```

Уровни: Verbose, Debug, Info, Warning, Error.
