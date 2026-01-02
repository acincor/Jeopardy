using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UOS.Launcher.UI
{
    public class ServiceButton
    {
        private VisualElement _loadingIcon;
        private Button _button;
        private VisualElement _buttonIcon;
        private const int RotateSpeed = 6;

        public ServiceButton(Button button)
        {
            _loadingIcon = button.Q<VisualElement>("LoadingIcon");
            _buttonIcon = button.Q<VisualElement>("LButtonIcon");
            _button = button;
        }

        public void Loading(bool loading)
        {
            _button.SetEnabled(!loading);
            Utils.Hide(_loadingIcon, !loading);
            Utils.Hide(_buttonIcon, loading);
#if UNITY_2021_2_OR_NEWER
            if (_loadingIcon == null) return;
            if (loading)
            {
                LauncherUI.OnFixedUpdate.AddListener(FixedUpdate);
            }
            else
            {
                LauncherUI.OnFixedUpdate.RemoveListener(FixedUpdate);
                _loadingIcon.style.rotate = new Rotate(0);

            }
#endif
        }

#if UNITY_2021_2_OR_NEWER
        private void FixedUpdate()
        {
            if (_loadingIcon == null)
            {
                return;
            }
            var currentAngle = _loadingIcon.style.rotate.value.angle.value;
            _loadingIcon.style.rotate = new Rotate(RotateSpeed + currentAngle);
        }
#endif
    }
}
