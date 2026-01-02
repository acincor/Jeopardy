using UnityEngine.UIElements;
using UOSLauncher.Editor.Environment;

namespace Unity.UOS.Launcher.UI
{
    public class EnvironmentUI
    {
        private static DropdownField _environmentDropdownField;
        private static Button _configureButton;

        public static void Init(VisualElement root)
        {
            _environmentDropdownField = root.Q<DropdownField>("EnvironmentDropdownField");
            _environmentDropdownField.choices = EnvironmentManager.Options;
            _environmentDropdownField.UnregisterValueChangedCallback(SwitchEnvironment);
            _environmentDropdownField.RegisterValueChangedCallback(SwitchEnvironment);

            _configureButton = root.Q<Button>("ConfigureButton");
            _configureButton.clicked += EnvironmentManager.Configure;
        }

        public static void SetActiveEnvironment()
        {
            EnvironmentManager.CorrectSelectedAppID();
            _environmentDropdownField.SetValueWithoutNotify(EnvironmentManager.SelectedEnvironment?.environmentName);
        }

        private static async void SwitchEnvironment(ChangeEvent<string> e)
        {
            var activeEnvironment = EnvironmentManager.SwitchSelectedAppID(e.newValue);
            await LinkAppInMainUI.LinkApp(activeEnvironment.appID, activeEnvironment.appSecret, activeEnvironment.appServiceSecret);
        }

        public static void UpdateOptions()
        {
            _environmentDropdownField.choices = EnvironmentManager.Options;
        }
    }
}