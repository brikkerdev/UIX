# UIX Framework — Documentation

Документация пакета com.uix.framework.

---

## Содержание

- **[index.md](index.md)** — Этот файл, обзор и ссылки
- **[TechnicalSpecification.md](TechnicalSpecification.md)** — Полное техническое задание (XML, USS, темы, биндинг, pipeline, навигация, UACF)
- **[API-Reference.md](API-Reference.md)** — Справка по API: UIXEngine, UIXNavigator, UIXMaterialRegistry, ReactiveProperty и др.

---

## Быстрые ссылки

| Тема | Раздел |
|------|--------|
| Общее описание | TechnicalSpecification §1 |
| Структура пакета | TechnicalSpecification §2 |
| XML-разметка | TechnicalSpecification §3 |
| Система стилей (USS) | TechnicalSpecification §4 |
| Система тем | TechnicalSpecification §5 |
| Биндинг данных | TechnicalSpecification §6 |
| Материалы и шейдеры | TechnicalSpecification §14 |
| Устранение неполадок | TechnicalSpecification §21 |

---

## Обзор

**UIX** — фреймворк для декларативной разработки UI в Unity на основе XML-разметки, CSS-подобной стилизации (USS) и компонентной архитектуры. Компилирует декларативное описание в нативные uGUI-объекты (UnityEngine.UI + TextMeshPro).

**Ключевые особенности:**

- **Декларативность:** интерфейс в XML и USS
- **Компонентность:** переиспользуемые компоненты с props и slots
- **Реактивность:** биндинги к ReactiveProperty и ReactiveCollection
- **Темизация:** CSS-переменные, переключение тем в runtime
- **UACF:** AI-агенты могут создавать и изменять UI через HTTP API

**Платформа:** Unity 2021.3+, Built-in Render Pipeline, uGUI, TextMeshPro 3.0+
