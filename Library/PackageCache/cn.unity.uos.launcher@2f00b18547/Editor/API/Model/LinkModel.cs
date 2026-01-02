using System.Collections.Generic;
using System;

namespace Unity.UOS.Launcher
{
    [Serializable]
    public class LoginUserInfo
    {
        public string userId;
        public string displayName;
        public string name;
        public string primaryOrg;
        public string accessToken;
        public bool valid;
        public bool whitelisted;
        public string organizationForeignKeys;
    }

    [Serializable]
    public class UserInfo
    {
        public string name;
        public string foreign_key;
        public string primary_org;
        public List<Organization> orgs;
        public string email;
    }

    public enum OrganizationRole
    {
        owner = 0,
        manager = 1,
        user = 2
    }

    [Serializable]
    public class Organization
    {
        public string name;
        public string id;
        public string role;
    }

    [Serializable]
    public class CreateProjectReq
    {
        public string name;
        public bool active = true;
    }

    [Serializable]
    public class CreateProjectResp
    {
        public string id;
        public string name;
        public string guid;
        public string org_id;
        public string org_name;
    }
    
    [Serializable]
    public class CreateUosAppReq
    {
        public string projectId;
        public string userId;
        
        // public string accessToken;
        // public string projectName;
        // public string organizationId;
        // public string organizationName;
        // public string createUserId;
        // public string createUsername;
        // public string createUserPhone;
        // public string createUserEmail;
        // public string uosAppName;
    }

    [Serializable]
    public class CreateUosAppResponse
    {
        public string AppId;
        public string AppSecret;
        public string AppServiceSecret;
        public string OrganizationId;
        public string ProjectId;
        public string ProjectName;
    }
    
    [Serializable]
    public class GetProjectsReq
    {
        public string name;
        public bool active = true;
    }

    [Serializable]
    public class ProjectInfo
    {
        public string id;
        public string name;
        public string guid;
        public string org_id;
        public string created_at;
        public bool archived;
    }

    [Serializable]
    public class GetProjectsResp
    {
        public List<ProjectInfo> projects;
    }

}