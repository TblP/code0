# Update

Сделал список вопросов/заданий ему по репо. 

---

## 1. В репо поправить код стайл, так чтобы он соответствовал стадартам Unity и C# (в принципе желательно это делать всегда, если планируешь показывать код кому либо да и для себя тоже, следование правилам код стайла первый пункт в любой проверке кода везде).

Как пример исправил для CameraToCoff.cs и загрузил версию CameraToCoffv2.cs

---

## 2. Предложить варианты решения без transform.parent.name.Contains (везде где это используется)

```csharp
// Вместо:
if (hit.collider.transform.parent.name.Contains("broken"))
// Можно:
if (hit.collider.transform.parent.CompareTag("BrokenObject"))
```

```csharp
// Можно использовать Enum для статусов объекта
public enum ObjectType
{
    Normal,
    Broken,
    Transport,
    Camera,
    Repairable,
    Fixed
}
```

---

## 3. Использовать предпочтительный способ работы с LayerMask

```csharp
[SerializeField] private LayerMask _raycastLayerMask = ~(1 << LayerMask.NameToLayer("Transport"));
if (Physics.Raycast(transform.position, forward, out hit, 35f, _raycastLayerMask))
```

---

## 4. Какое будет альтернативное решение без проверки по имени? hitRenderer.materials[i].name != Holo.name + " (Instance)"

```csharp
private bool CheckHoloMaterial_Alter(Renderer hitRenderer)
{
    bool addHolo = true;
    
    foreach (Material material in hitRenderer.materials)
    {
        // Сравниваем прямые ссылки на объекты Material
        if (material == Holo)
        {
            addHolo = false;
            break; 
        }
    }
    
    return addHolo;
}
```

---

## 5. Использовать Linq для вычисления addHolo

```csharp
private void LINQSolution(Renderer hitRenderer)
{
	addHolo = !hitRenderer.materials.Contains(Holo);
}
```

---

## 6. В AddHighlightMaterial использовать встроенные операции C# для уменьшения количества кода. Использовать hat operator.

```csharp
private void AddHighlightMaterial_HatOperator(Renderer renderer)
{
    Material[] materials = renderer.materials;
    Material[] newMaterials = new Material[materials.Length + 1];
    
    // Копируем существующие материалы
    Array.Copy(materials, newMaterials, materials.Length);
    
    newMaterials[^1] = highlightMaterial;
    
    renderer.materials = newMaterials;
}
```

---

## 7. Какое решение является предпочтительным для Unity, без добавления множества материалов на один объект?

- Единый материал и менять его состояние через параметры шарда (Shader Properties)

---

## 8. В bar_foam предложить решение без использования индексов child объектов, GameObject.Find("Camera") и transform.root.Find

- В целом можно просто назначить через инспектор, объекты не меняются 

---

## 10. То же для nav_npc

- В целом можно просто назначить через инспектор, объекты не меняются 

---

## 11. Обращаться к именам переменных в анимациях по индексу, вместо имени.

- Не очень понимаю, имеется ввиду что setbool принимает id помимо имени? 

---

## 12. Можно ли nav_npc.Update переделать на корутины?

- Можно переделать все в корутины и в целом убрать update. Просто ожидать orderDone (yield return new WaitUntil(() => orderDone);) 
- Разделить все действия на этапы через yield return new WaitUntil()
