using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_2021_3_OR_NEWER
//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
public class TabbedMenu : MonoBehaviour
{
    private TabbedMenuController controller;

    private void OnEnable()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;

        controller = new TabbedMenuController(root);

        controller.RegisterTabCallbacks();
    }
}
#endif