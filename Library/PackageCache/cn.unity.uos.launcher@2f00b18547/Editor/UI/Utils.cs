using UnityEngine.UIElements;

namespace Unity.UOS.Launcher.UI
{
    public class Utils
    {
        public static void Hide(VisualElement element, bool hide = true)
        {
            if (element == null) return;
            element.style.display = hide ? DisplayStyle.None: DisplayStyle.Flex;
            element.style.visibility = hide? Visibility.Hidden: Visibility.Visible;
        }
    }
}