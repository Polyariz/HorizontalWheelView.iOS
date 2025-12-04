# HorizontalWheelView для iOS

Полный порт Android компонента [HorizontalWheelView](https://github.com/shchurov/HorizontalWheelView) на Xamarin iOS с полным соответствием оригинальной функциональности.

## Описание

HorizontalWheelView - это кастомный iOS view компонент, который отображает горизонтальное колесо для выбора значений с 3D эффектом. Компонент поддерживает жесты прокрутки, инерционное движение (fling), привязку к меткам (snap to marks) и множество настроек.

## Возможности

- ✅ Полное соответствие оригинальному Java коду
- ✅ Поддержка жестов прокрутки с инерцией
- ✅ 3D эффект меток (перспективное отображение)
- ✅ Настраиваемые цвета (нормальный и активный)
- ✅ Привязка к меткам (snap to marks)
- ✅ Блокировка концов диапазона (end lock)
- ✅ Только положительные значения (only positive values)
- ✅ Отображение активного диапазона
- ✅ Настраиваемое количество меток
- ✅ События изменения угла и состояния скролла
- ✅ Плавная анимация с замедлением

## Структура проекта

```
HorizontalWheelViewiOS/
├── HorizontalWheelView.cs       # Основной класс view
├── Drawer.cs                     # Отрисовка меток и курсора
├── TouchHandler.cs               # Обработка жестов и анимаций
├── Utils.cs                      # Вспомогательные утилиты
├── SampleViewController.cs       # Пример использования
└── SceneDelegate.cs             # Настройка приложения
```

## Использование

### Базовый пример

```csharp
// Создание HorizontalWheelView
var wheelView = new HorizontalWheelView
{
    Frame = new CGRect(0, 0, 200, 64)
};

// Настройка цветов
wheelView.SetNormalColor(UIColor.White);
wheelView.SetActiveColor(UIColor.Yellow);

// Настройка параметров
wheelView.SetMarksCount(40);
wheelView.SetShowActiveRange(true);
wheelView.SetSnapToMarks(false);

// Добавление обработчика событий
wheelView.SetListener(new MyWheelViewListener());

// Добавление на экран
View.AddSubview(wheelView);
```

### Обработка событий

```csharp
public class MyWheelViewListener : HorizontalWheelViewListener
{
    public override void OnRotationChanged(double radians)
    {
        // Обработка изменения угла
        double degrees = radians * 180 / Math.PI;
        Console.WriteLine($"Угол: {degrees:F0}°");
    }

    public override void OnScrollStateChanged(int state)
    {
        // Обработка изменения состояния скролла
        // state: SCROLL_STATE_IDLE, SCROLL_STATE_DRAGGING, SCROLL_STATE_SETTLING
        switch (state)
        {
            case HorizontalWheelView.SCROLL_STATE_IDLE:
                Console.WriteLine("Idle");
                break;
            case HorizontalWheelView.SCROLL_STATE_DRAGGING:
                Console.WriteLine("Dragging");
                break;
            case HorizontalWheelView.SCROLL_STATE_SETTLING:
                Console.WriteLine("Settling");
                break;
        }
    }
}
```

### Установка угла

```csharp
// Установка угла в радианах
wheelView.SetRadiansAngle(Math.PI / 2);  // 90 градусов

// Установка угла в градусах
wheelView.SetDegreesAngle(45);  // 45 градусов

// Установка через долю полного оборота
wheelView.SetCompleteTurnFraction(0.25);  // 1/4 оборота (90 градусов)
```

### Получение угла

```csharp
// Получение угла в радианах
double radians = wheelView.GetRadiansAngle();

// Получение угла в градусах
double degrees = wheelView.GetDegreesAngle();

// Получение доли полного оборота
double fraction = wheelView.GetCompleteTurnFraction();
```

## API

### Свойства и методы

| Метод | Описание |
|-------|----------|
| `SetMarksCount(int count)` | Устанавливает количество меток на колесе |
| `SetNormalColor(UIColor color)` | Устанавливает цвет обычных меток |
| `SetActiveColor(UIColor color)` | Устанавливает цвет активных меток и курсора |
| `SetShowActiveRange(bool show)` | Включает/выключает отображение активного диапазона |
| `SetSnapToMarks(bool snap)` | Включает/выключает привязку к меткам |
| `SetEndLock(bool lock)` | Включает/выключает блокировку концов диапазона |
| `SetOnlyPositiveValues(bool only)` | Ограничивает значения только положительными |
| `SetRadiansAngle(double radians)` | Устанавливает угол в радианах |
| `SetDegreesAngle(double degrees)` | Устанавливает угол в градусах |
| `SetCompleteTurnFraction(double fraction)` | Устанавливает угол через долю оборота |
| `GetRadiansAngle()` | Возвращает угол в радианах |
| `GetDegreesAngle()` | Возвращает угол в градусах |
| `GetCompleteTurnFraction()` | Возвращает долю полного оборота |
| `SetListener(HorizontalWheelViewListener listener)` | Устанавливает обработчик событий |

### Константы состояния скролла

| Константа | Значение | Описание |
|-----------|----------|----------|
| `SCROLL_STATE_IDLE` | 0 | Колесо в покое |
| `SCROLL_STATE_DRAGGING` | 1 | Пользователь перетаскивает колесо |
| `SCROLL_STATE_SETTLING` | 2 | Колесо замедляется после fling |

### Значения по умолчанию

- Количество меток: **40**
- Цвет обычных меток: **Белый (#ffffff)**
- Цвет активных меток: **Синий (#54acf0)**
- Показывать активный диапазон: **true**
- Привязка к меткам: **false**
- Блокировка концов: **false**
- Только положительные значения: **false**

## Технические детали

### Отличия от Android версии

1. **Платформенные API**: Использует iOS UIKit вместо Android View
2. **Жесты**: Использует `UIPanGestureRecognizer` вместо `GestureDetector`
3. **Анимация**: Использует `CADisplayLink` вместо `ValueAnimator`
4. **Рисование**: Использует `CoreGraphics` (CGContext) вместо Android Canvas
5. **Padding**: Использует `ContentEdgeInsets` вместо Android padding
6. **Сохранение состояния**: iOS автоматически управляет состоянием view

### Соответствие оригиналу

Все классы точно повторяют структуру и логику оригинального Java кода:

- **HorizontalWheelView.cs** ≈ HorizontalWheelView.java (217 строк → 244 строки)
- **Drawer.cs** ≈ Drawer.java (237 строк → 292 строки)
- **TouchHandler.cs** ≈ TouchHandler.java (125 строк → 205 строк)
- **Utils.cs** ≈ Utils.java (14 строк → 16 строк)

## Требования

- **.NET**: 10.0 или выше
- **iOS**: 15.0 или выше
- **Xamarin.iOS** или **.NET iOS**

## Примеры приложений

Проект включает два примера использования:

### 1. SampleViewController.cs (Базовый пример)

Демонстрирует основное использование:
- Создание и настройку `HorizontalWheelView`
- Обработку событий изменения угла
- Синхронизацию вращения изображения с углом колеса
- Отображение текущего угла в градусах

### 2. AdvancedSampleViewController.cs (Расширенный пример)

Демонстрирует все возможности API:
- ✅ Все методы установки и получения угла (радианы, градусы, доля оборота)
- ✅ Динамическое изменение количества меток (слайдер 10-100)
- ✅ Переключение Snap to Marks
- ✅ Переключение End Lock
- ✅ Переключение Only Positive Values
- ✅ Переключение Show Active Range
- ✅ Отображение состояния скролла (IDLE/DRAGGING/SETTLING)
- ✅ Установка цветов через int (0xffffffff, 0xff54acf0)

**Для использования расширенного примера**, замените в `SceneDelegate.cs`:
```csharp
var vc = new SampleViewController();
```
на:
```csharp
var vc = new AdvancedSampleViewController();
```

Для запуска примера просто откройте проект в Visual Studio и запустите на iOS симуляторе или устройстве.

## Лицензия

Этот порт следует лицензии оригинального проекта. Оригинальный Android компонент создан [shchurov](https://github.com/shchurov).

## Автор порта

Портировано на Xamarin iOS с полным соответствием оригинальной функциональности.

---

**Примечание**: В примере используется системная иконка самолета вместо оригинальной ракеты из Android проекта. Для использования собственного изображения замените создание `rocketImageView` в `SampleViewController.cs`.
