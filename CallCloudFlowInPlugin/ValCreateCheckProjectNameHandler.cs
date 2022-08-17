using Microsoft.Xrm.Sdk;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace CallCloudFlowInPlugin
{
    internal class ValCreateCheckProjectNameHandler
    {
        IOrganizationService service;
        public ValCreateCheckProjectNameHandler(IOrganizationService _service)
        {
            service = _service;
        }

        public void Handle(Entity entity)
        {
            if (!entity.Contains("hz_name")) return;

            //Check(entity);
            CheckWithFlow(entity);
        }

        public void Check(Entity entity)
        {
            if (entity.GetAttributeValue<string>("hz_name") == "xyz")
                throw new InvalidPluginExecutionException($"Invalid project name ({entity.GetAttributeValue<string>("hz_name")})");
        }

        public void CheckWithFlow(Entity entity)
        {
            var projectName = entity.GetAttributeValue<string>("hz_name");

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://prod-49.westeurope.logic.azure.com/workflows/55dfb05649164970.............");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            var data = Serializer.Serialize<PostRequest>(new PostRequest { ProjectName = projectName });
            var dataContent = Encoding.UTF8.GetBytes(data);
            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(dataContent, 0, dataContent.Length);
            }

            Stream response = ((HttpWebResponse)webRequest.GetResponse()).GetResponseStream();
            string result;
            using (var sr = new StreamReader(response))
            {
                result = sr.ReadToEnd();
            }

            var post = Serializer.Deserialize<PostResponse>(result);
            if (post.IsValid == "invalid")
                throw new InvalidPluginExecutionException($"(Flow) Invalid project name ({projectName})");

        }
    }

    [DataContract]
    public class PostRequest
    {
        [DataMember(Name = "projectName")]
        public string ProjectName { get; set; }
    }

    [DataContract]
    public class PostResponse
    {
        [DataMember(Name = "isValid")]
        public string IsValid { get; set; }
    }
}
