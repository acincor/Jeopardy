using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;
#if UNITY_2021_3_OR_NEWER
namespace Unity.UOS.Launcher.UI
{
    public class LinkAppByUnityProjectTab
    {
        private enum Type
        {
            SelectProject = 0,
            CreateProject = 1
        }

        private static VisualElement _selector;
        private static VisualElement _root;
        
        // UI 元素
        private static DropdownField _organizationDropdown;
        private static RadioButtonGroup _createOrSelectProject;
        private static VisualElement _selectProjectPanel;
        private static VisualElement _createProjectPanel;
        private static DropdownField _projectListDropdown;
        private static TextField _createProjectNameTextField;
        private static Label _errorMessageLabel;
        
        // 数据
        private static List<Organization> _organizations = new List<Organization>();
        private static UserInfo _userInfo;
        private static List<ProjectInfo> _currentProjectList;

        private const string LoginHint = "未登录，请登录后使用 Link App by Unity Project 功能。";
        private static bool _loggedIn;
        
        public static async void Init(VisualElement root)
        {
            InitElements(root);
            _loggedIn = await AppLinker.IsLoggedIn();

            // 注册回调
            _createOrSelectProject.RegisterValueChangedCallback(HandleCreateOrSelectProject);
            _organizationDropdown.RegisterValueChangedCallback((e) =>
            {
                // 显示项目列表
                ResetProjectListDropdown(e.newValue);
            });
            
            // 设置新建项目的默认名称
            _createProjectNameTextField.value = GenProjectName();

            SetLoginStatus(_loggedIn);
        }

        private static string GenProjectName()
        {
            var dateTime = DateTime.Now.ToString();
            return $"{Application.productName} {dateTime.Replace("/", "-")}";
        }

        private static void ResetDropdowns()
        {
            _organizationDropdown.choices = new List<string>();
            _organizationDropdown.SetValueWithoutNotify("");
            _projectListDropdown.choices = new List<string>();
            _projectListDropdown.SetValueWithoutNotify("");
        }

        private static async void GetUserInfo()
        {
            ResetDropdowns();
            _userInfo = await AppLinker.GetUserInfo();
            if (_userInfo == null)
            {
                Debug.LogError(LoginHint);
                return;
            }
            
            _organizations = _userInfo.orgs;
            var orgOptions = _organizations.Select(org => org.name).ToList();
            _organizationDropdown.choices = orgOptions;
            _projectListDropdown.value = "";
            // 默认选中第一个
            _organizationDropdown.value = orgOptions[0];
        }

        public static async void CheckLoginStatus()
        {
            var loggedIn = await AppLinker.IsLoggedIn();
            var loginStatusChanged = _loggedIn != loggedIn;
            
            // 登录状态未发生变化
            if (!loginStatusChanged) return;
            
            _loggedIn = loggedIn;
            SetLoginStatus(_loggedIn);
        }

        private static void SetLoginStatus(bool loggedIn)
        {
            if (loggedIn)
            {
                ClearErrorMessage();
                GetUserInfo();
            }
            else
            {
                ResetDropdowns();
                ShowErrorMessage("Please log in.");
                Debug.LogError(LoginHint);
            };
        }
        
        private static void ShowErrorMessage(string message)
        {
            _errorMessageLabel.text = message;
        }

        private static void InitElements(VisualElement root)
        {
            _organizationDropdown = root.Q<DropdownField>("Organizations");
            _createOrSelectProject = root.Q<RadioButtonGroup>("CreateOrSelectProject");
            _selectProjectPanel = root.Q<VisualElement>("SelectProjectPanel");
            _createProjectPanel = root.Q<VisualElement>("CreateProjectPanel");
            _projectListDropdown = root.Q<DropdownField>("ProjectList");
            _createProjectNameTextField = root.Q<TextField>("CreateProjectName");
            _errorMessageLabel = root.Q<Label>("ErrorMessage");
        }
        
        /// <summary>
        /// 重置项目列表
        /// </summary>
        private static async void ResetProjectListDropdown(string selectedOrgName)
        {
            if (string.IsNullOrEmpty(selectedOrgName))
            {
                _projectListDropdown.value = "";
                _projectListDropdown.choices = new List<string>();
                return;
            }
            var accessToken = AppLinker.GetAccessToken();
            var selectedOrgId = GetOrgIdByName(selectedOrgName);
            var resp = await LinkAPI.GetProjects(accessToken, selectedOrgId);
            var projectOptions = resp.projects.Select(project => @project.name).ToList();
            _projectListDropdown.choices = projectOptions;
            _projectListDropdown.value = projectOptions.Any() ? projectOptions[0]: "";
            _currentProjectList = resp.projects;
        }

        /// <summary>
        /// 处理选择或新建项目
        /// </summary>
        /// <param name="e"></param>
        private static void HandleCreateOrSelectProject(ChangeEvent<int> e)
        {
            if (e.newValue == (int)Type.SelectProject)
            {
                // 选择
                Utils.Hide(_createProjectPanel);
                Utils.Hide(_selectProjectPanel, false);
                
                // 检查是否已经选择组织
                // 未选择
                var selectedOrgName = _organizationDropdown.value;
                if (selectedOrgName == "")
                {
                    _projectListDropdown.choices = new List<string>();
                    _projectListDropdown.value = "";
                }
                // 已选择
                ClearErrorMessage();
                ResetProjectListDropdown(selectedOrgName);
            }
            else
            {
                Utils.Hide(_createProjectPanel, false);
                Utils.Hide(_selectProjectPanel);
            }
        }

        private static void ClearErrorMessage()
        {
            ShowErrorMessage("");
        }

        internal static async void ConfirmLink()
        {
            var selectedProjectGuid = "";
            var accessToken = AppLinker.GetAccessToken();

            // 检查组织是否已选择
            if (string.IsNullOrEmpty(_organizationDropdown.value))
            {
                ShowErrorMessage("Please select organization.");
                return;
            }
            
            if (_createOrSelectProject.value == (int)Type.CreateProject)
            {
                // 检查名称
                var projectName = _createProjectNameTextField.value;
                if (string.IsNullOrEmpty(projectName))
                {
                    ShowErrorMessage("Please input project name.");
                    return;
                }

                var orgId = GetSelectedOrgId();
                // 创建项目
                try
                {
                    var newProject = await LinkAPI.CreateProject(
                        accessToken,
                        orgId,
                        projectName);
                    selectedProjectGuid = newProject.guid;
                }
                catch (Exception e)
                {
                    ShowErrorMessage(e.Message);
                    return;
                }
            }
            else
            {
                // 检查
                if (string.IsNullOrEmpty(_projectListDropdown.value))
                {
                    ShowErrorMessage("Please select project.");
                    return;
                }
                
                selectedProjectGuid = GetSelectedProjectGuid();
            }

            try
            {
                AppLinker.LinkProject(accessToken, selectedProjectGuid, _userInfo.foreign_key);
                // 清空
                LinkAppWindow.CloseWindow();
                ClearErrorMessage();
            }
            catch (Exception e)
            {
                ShowErrorMessage(e.ToString());
            }

        }

        private static string GetSelectedOrgId()
        {
            return GetOrgIdByName(_organizationDropdown.value);
        }

        private static string GetSelectedProjectGuid()
        {
            return _currentProjectList.Find(project => project.name == _projectListDropdown.value).guid;
        }
        private static string GetOrgIdByName(string orgName)
        {
            return _organizations.Find(org => org.name == orgName).id;
        }
    }
}
#endif