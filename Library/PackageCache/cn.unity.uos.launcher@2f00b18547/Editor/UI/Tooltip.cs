using System;
using UnityEngine.UIElements;
using UnityEngine;

namespace Unity.UOS.Launcher.UI
{
    public class Tooltip
    {
        public static void Listen(VisualElement element, string tooltip, Vector2 size)
        {
            if (!string.IsNullOrEmpty(tooltip))
            {
                element.RegisterCallback<TooltipEvent>((evt) =>
                {
                    var pos = element.worldBound.position;
                    evt.rect = new Rect(new Vector2(pos.x, pos.y), size);
                    evt.tooltip = tooltip;
                });
            }
        }
    }
}