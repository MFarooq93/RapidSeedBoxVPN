using Atom.SDK.Core.Enumerations;
using Newtonsoft.Json;
using PureVPN.Core.Models;
using PureVPN.Entity.Models;
using PureVPN.Entity.Models.DTO;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Helper
{
    public static class Extensions
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }

        public static void SendResponseToMixpanel(this IMixpanelService mixpanelService, string host, string localPath, double? response_time)
        {
            var props = new MixpanelProperties().MixPanelPropertiesDictionary;
            props.Add("host", host);
            props.Add("path", localPath);
            props.Add("response_time", response_time);
            mixpanelService.FireEvent(MixpanelEvents.app_api_response_time, AtomModel.HostingID, props);
        }
        public static bool IsStringNullorEmptyorWhitespace(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
                return true;
            else
                return false;
        }

        public static bool IsStringNotNullorEmptyorWhitespace(this string stringValue)
        {
            if (!string.IsNullOrEmpty(stringValue) && !string.IsNullOrWhiteSpace(stringValue))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets value for UTB event source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetUTBEventSource(this UTBEventSource source)
        {
            return source == UTBEventSource.InSession ? "during-session" : "session-start";
        }


        /// <summary>
        /// Throws an exception if the <paramref name="httpResponse"/>.StatusCode is not <see cref="HttpStatusCode.Success"/>
        /// </summary>
        /// <param name="httpResponse"></param>
        public static void EnsureSuccessStatusCode(this System.Net.Http.HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode == false)
            {
                SendAppApiFailureEvent(httpResponse);
            }
            httpResponse.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Throws an exception if the <paramref name="httpResponse"/>.StatusCode is not <see cref="HttpStatusCode.Success"/> or <see cref="HttpStatusCode.BadRequest"/>
        /// </summary>
        /// <param name="httpResponse"></param>
        public static void EnsureSupportedStatusCode(this System.Net.Http.HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode == false)
            {
                SendAppApiFailureEvent(httpResponse);
            }

            if (httpResponse.IsSuccessStatusCode || httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return;
            }
            httpResponse.EnsureSuccessStatusCode();
        }

        public static void SendAppApiFailureEvent(System.Net.Http.HttpResponseMessage httpResponse)
        {
            try
            {
                var props = new MixpanelProperties().MixPanelPropertiesDictionary;
                string content = httpResponse.Content.ReadAsStringAsync().Result;
                string authorization = httpResponse.RequestMessage?.Headers?.Authorization?.ToString();
                props.Add(MixpanelProperties.response, content);
                props.Add(MixpanelProperties.http_status_code, (int)httpResponse.StatusCode);
                props.Add(MixpanelProperties.host, httpResponse.RequestMessage?.RequestUri?.Host);
                props.Add(MixpanelProperties.path, httpResponse.RequestMessage?.RequestUri?.LocalPath);
                props.Add(MixpanelProperties.query, httpResponse.RequestMessage?.RequestUri?.Query);

                BaseNetwork responseObj = null;
                try { responseObj = JsonConvert.DeserializeObject<BaseNetwork>(content); } catch { }
                if (responseObj != null)
                {
                    props.Add(MixpanelProperties.code, responseObj?.header?.response_code);
                    props.Add(MixpanelProperties.reason, responseObj?.header?.message);
                }

                string userInfoJson;
                var userinfo = Utilities.ParseUserInfo(authorization, out userInfoJson);
                props.Add(MixpanelProperties.userInfoEmail, userinfo?.Email);
                props.Add(MixpanelProperties.userInfoAccountCode, userinfo?.Data?.AccountCode);
                props.Add(MixpanelProperties.userInfoJson, userInfoJson);

                Common.MixpanelService?.FireEvent(MixpanelEvents.app_api_failure, AtomModel.HostingID, props);
            }
            catch { }
        }

        /// <summary>
        /// Describes whether the <paramref name="HttpStatusCode"/> describes a Success status code or not
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool IsSuccessStatusCode(this System.Net.HttpStatusCode httpStatusCode)
        {
            int httpCode = (int)httpStatusCode;
            return httpCode > 199 && httpCode < 300;
        }


        /// <summary>
        /// Delete <paramref name="filename"/> if exists
        /// </summary>
        /// <param name="filename"></param>
        public static void DeleteFile(this string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        /// <summary>
        /// Creates <paramref name="directory"/> if does not exist already
        /// </summary>
        /// <param name="directory"></param>
        public static bool TryCreateDirectory(this string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch { }
            return Directory.Exists(directory);
        }

    }
}