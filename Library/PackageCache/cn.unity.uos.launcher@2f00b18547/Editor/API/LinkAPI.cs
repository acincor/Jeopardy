using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Unity.UOS.Common;
using Unity.UOS.Networking;

namespace Unity.UOS.Launcher
{
    public class LinkAPI
    {
        public static async Task<UserInfo> GetUserInfo(string accessToken)
        {
            var client = new HttpClient(new HttpClientOptions()
            {
                BaseUrl = new Uri("https://core.cloud.unity.cn")
            });
            
            var bearerHeader = new Dictionary<string, string>()
            {
                { "AUTHORIZATION", "Bearer " + accessToken }
            };
            
            var resp = await client.Get<UserInfo>("/api/users/me",
                new Dictionary<string, object>()
                {
                    {"include", "orgs,projects"}
                }, 
                bearerHeader);

            var filteredOrgs = resp.orgs.Where(org => org.role == OrganizationRole.owner.ToString()).ToList();
            resp.orgs = filteredOrgs;
            return resp;
        }

        public static async Task<CreateProjectResp> CreateProject(string accessToken, string orgId, string projectName)
        {
            var url = $"/api/orgs/{orgId}/projects";
            var client = new HttpClient(new HttpClientOptions()
            {
                BaseUrl = new Uri("https://core.cloud.unity.cn")
            });
            
            var headers = new Dictionary<string, string>()
            {
                { "AUTHORIZATION", "Bearer " + accessToken },
                { "Content-Type", "application/json;charset=UTF-8"}
            };

            return await client.Post<CreateProjectResp>(url,
                JsonUtility.ToJson(new CreateProjectReq()
                {
                    name = projectName,
                }),
                null, 
                headers);
        }

        public static async Task<CreateUosAppResponse> GetUosApp(CreateUosAppReq req, string accessToken)
        {
            var url = "/service/app/bind";
            var client = new HttpClient(new HttpClientOptions()
            {
                BaseUrl = new Uri("https://uos.unity.cn")
            });
            
            var headers = new Dictionary<string, string>()
            {
                { "Authorization", "Bearer " + accessToken },
                { "Content-Type", "application/json;charset=UTF-8"}
            };

            return await client.Post<CreateUosAppResponse>(url,
                JsonUtility.ToJson(req),
                null, 
                headers);
        }
        
        public static async Task<GetProjectsResp> GetProjects(string accessToken, string orgId)
        {
            var url = $"/api/orgs/{orgId}/projects";
            var client = new HttpClient(new HttpClientOptions()
            {
                BaseUrl = new Uri("https://core.cloud.unity.cn")
            });
            
            var headers = new Dictionary<string, string>()
            {
                { "AUTHORIZATION", "Bearer " + accessToken },
                { "Content-Type", "application/json;charset=UTF-8"}
            };

            var resp = await client.Get<GetProjectsResp>(url,
                null,
                headers);

            resp.projects.Sort((project1, project2) =>
            {
                var time1 = DateTime.Parse(project1.created_at);
                var time2 = DateTime.Parse(project2.created_at);
                return time1 > time2 ? 0: 1;
            });
            var filteredProjects = resp.projects.Where(item => !item.archived).ToList();
            resp.projects = filteredProjects;

            return resp;
        }
        
    }
}