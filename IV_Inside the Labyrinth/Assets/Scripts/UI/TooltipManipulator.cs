using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManipulator : Manipulator
{
    private VisualElement element;
    readonly private VisualElement root;

    public TooltipManipulator(VisualElement root)
    {
        this.root = root;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(MouseIn);
        target.RegisterCallback<MouseOutEvent>(MouseOut);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(MouseIn);
        target.UnregisterCallback<MouseOutEvent>(MouseOut);
    }

    private void MouseIn(MouseEnterEvent e)
    {
        if (element == null)
        {
            element = new VisualElement();
            element.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0.8f));
            element.style.position = Position.Absolute;
            element.style.left = this.target.worldBound.center.x;
            element.style.top = this.target.worldBound.yMax;
            element.style.borderBottomLeftRadius = 10;
            element.style.borderBottomRightRadius = 10;
            element.style.borderTopLeftRadius = 10;
            element.style.borderTopRightRadius = 10;

            Label label = new Label(this.target.tooltip);
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.color = Color.black;
            label.style.marginTop = 5;
            label.style.marginBottom = 5;
            label.style.marginLeft = 5;
            label.style.marginRight = 5;

            element.Add(label);
            root.Add(element);
        }
        element.style.visibility = Visibility.Visible;
        element.BringToFront();
    }

    private void MouseOut(MouseOutEvent e)
    {
        element.style.visibility = Visibility.Hidden;
    }
}
