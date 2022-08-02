using PureVPN.Entity.Models;
using System;
using System.Collections.Generic;

namespace PureVPN.Service.Helper
{
    public class MixpanelProperties
    {
        public Dictionary<string, object> MixPanelPropertiesDictionary { get; set; }


        public MixpanelProperties()
        {
            try
            {
                MixPanelPropertiesDictionary = new Dictionary<string, object>();

                MixPanelPropertiesDictionary.Add("$os", "Windows");
                MixPanelPropertiesDictionary.Add("$app_version_string", Service.Helper.Common.ProductFileVersion);
                MixPanelPropertiesDictionary.Add("$app_build_number", Service.Helper.Common.ProductBuildVersion);
                MixPanelPropertiesDictionary.Add("app_release_code", Service.Helper.Common.ReleaseTypeCode);
                MixPanelPropertiesDictionary.Add("$os_version", Service.Helper.Common.OSName);
                MixPanelPropertiesDictionary.Add("$user_id", string.IsNullOrEmpty(AtomModel.HostingID) ? "-1" : AtomModel.HostingID);
                MixPanelPropertiesDictionary.Add(username, string.IsNullOrEmpty(AtomModel.Username) ? "" : AtomModel.Username);
                MixPanelPropertiesDictionary.Add(locale, Service.Helper.Common.AppLanguageLocale);
            }
            catch (Exception ex)
            {
                //Service.Helper.Sentry.LoggingException(ex);
            }
        }

        /// <summary>
        /// Mixpanel property name to describe Username
        /// </summary>
        public const string username = "username";

        /// <summary>
        /// Mixpanel property name to describe Addon
        /// </summary>
        public const string addon = "addon";
        /// <summary>
        /// Mixpanel property name to describe how connection was requested
        /// </summary>
        public const string selected_interface = "selected_interface";
        /// <summary>
        /// Mixpanel property name to describe connection interface screen
        /// </summary>
        public const string selected_interface_screen = "selected_interface_screen";
        /// <summary>
        /// Mixpanel property name to describe previously selected user language
        /// </summary>
        public const string current_language = "current_language";
        /// <summary>
        /// Mixpanel property name to describe changed user language
        /// </summary>
        public const string selected_language = "selected_language";
        /// <summary>
        /// Mixpanel property name to describe user selected language
        /// </summary>
        public const string locale = "locale";

        /// <summary>
        /// Mixpanel property name to describe number of unread notifications
        /// </summary>
        public const string pending_notifications = "pending_notifications";

        /// <summary>
        /// Mixpanel property name to describe notification title
        /// </summary>
        public const string notification_title = "title";

        /// <summary>
        /// Mixpanel property name to describe notification type
        /// </summary>
        public const string notification_type = "type";

        /// <summary>
        /// Mixpanel property name to describe notification category
        /// </summary>
        public const string notification_category = "category";

        /// <summary>
        /// Mixpanel property name to describe action for clicked CTA on notification
        /// </summary>
        public const string notification_action = "action";

        /// <summary>
        /// Mixpanel property name to describe destination for clicked CTA on notification
        /// </summary>
        public const string notification_destination = "destination";

        /// <summary>
        /// Mixpanel property name to describe title for primary CTA on notitification
        /// </summary>
        public const string notification_cta_primary_title = "cta_primary_title";

        /// <summary>
        /// Mixpanel property name to describe action for primary CTA on notitification
        /// </summary>
        public const string notification_cta_primary_action = "cta_primary_action";

        /// <summary>
        /// Mixpanel property name to describe destination for primary CTA on notitification
        /// </summary>
        public const string notification_cta_primary_destination = "cta_primary_destination";

        /// <summary>
        /// Mixpanel property name to describe title for secondary CTA on notitification
        /// </summary>
        public const string notification_cta_secondary_title = "cta_secondary_title";

        /// <summary>
        /// Mixpanel property name to describe action for secondary CTA on notitification
        /// </summary>
        public const string notification_cta_secondary_action = "cta_secondary_action";

        /// <summary>
        /// Mixpanel property name to describe destination for secondary CTA on notitification
        /// </summary>
        public const string notification_cta_secondary_destination = "cta_secondary_destination";

        /// <summary>
        /// Mixpanel property name to describe notification description
        /// </summary>
        public const string notification_body = "body";

        /// <summary>
        /// Mixpanel property name to describe notification whether primary or secondary cta was clicked
        /// </summary>
        public const string notification_cta_clicked = "cta_clicked";

        /// <summary>
        /// Mixpanel property name to describe notification origin
        /// </summary>
        public const string notification_origin = "origin";

        /// <summary>
        /// Mixpanel property name to describe session time in seconds
        /// </summary>
        public const string time_since_connection = "time_since_connection";

        /// <summary>
        /// Mixpanel property name to describe UTB event source
        /// </summary>
        public const string triggered_at = "triggered_at";

        public const string is_popup_shown = "is_popup_shown";

        /// <summary>
        /// Mixpanel property name to describe login via
        /// </summary>
        public const string login_via = "login_via";

        /// <summary>
        /// Mixpanel property name to describe Authorization grant code generated by FA
        /// </summary>
        public const string grant_code = "grant_code";

        /// <summary>
        /// Mixpanel property name to describe action
        /// </summary>
        public const string action = "action";

        /// <summary>
        /// Mixpanel property name to describe method
        /// </summary>
        public const string method = "method";

        /// <summary>
        /// Mixpanel property name to describe source
        /// </summary>
        public const string source = "source";

        /// <summary>
        /// Mixpanel property name to describe reason for error
        /// </summary>
        public const string reason = "reason";

        /// <summary>
        /// Mixpanel property name to describe api response
        /// </summary>
        public const string response = "response";

        /// <summary>
        /// Mixpanel property name to describe http status code from api
        /// </summary>
        public const string http_status_code = "http_status_code";

        /// <summary>
        /// Mixpanel property name to describe host part of api url
        /// </summary>
        public const string host = "host";

        /// <summary>
        /// Mixpanel property name to describe path part of api url
        /// </summary>
        public const string path = "path";

        /// <summary>
        /// Mixpanel property name to describe path part of api url
        /// </summary>
        public const string query = "params";

        /// <summary>
        /// Mixpanel property name to describe code for error
        /// </summary>
        public const string code = "code";

        /// <summary>
        /// Mixpanel property name to describe user email
        /// </summary>
        public const string email = "email";

        /// <summary>
        /// Mixpanel property name to describe source of event
        /// </summary>
        public const string via = "via";

        /// <summary>
        /// Mixpanel property name to describe id found in user info from jwt
        /// </summary>
        public const string userInfoId = "user_info_id";

        /// <summary>
        /// Mixpanel property name to describe account code found in user info from jwt
        /// </summary>
        public const string userInfoAccountCode = "user_info_accountCode";

        /// <summary>
        /// Mixpanel property name to describe email found in user info from jwt
        /// </summary>
        public const string userInfoEmail = "user_info_email";

        /// <summary>
        /// Mixpanel property name to describe user info from jwt
        /// </summary>
        public const string userInfoJson = "user_info_json";

        /// <summary>
        /// Mixpanel property name to describe url from deep links.
        /// </summary>
        public const string url = "url";

        /// <summary>
        /// Mixpanel property name to describe source of event
        /// </summary>
        public const string setup_devicetype = "setup_devicetype";

        /// <summary>
        /// 
        /// </summary>
        public const string selected_filters_list = "selected_filters_list";

        /// <summary>
        /// Mixpanel property name to describe connected server ip
        /// </summary>
        public const string server_ip = "server_ip";

        /// <summary>
        /// Mixpanel property name to describe connected server dns
        /// </summary>
        public const string server_dns = "server_dns";

        /// <summary>
        /// Mixpanel property name to describe selected protocol name
        /// </summary>
        public const string selected_protocol_name = "selected_protocol_name";

        /// <summary>
        /// Mixpanel property name to describe selected location
        /// </summary>
        public const string selected_location = "selected_location";

        /// <summary>
        /// Mixpanel property name to describe dialed protocol name
        /// </summary>
        public const string dialed_protocol_name = "dialed_protocol_name";

        /// <summary>
        /// Mixpanel property name to describe selected interface state when connection initiated
        /// </summary>
        public const string selected_interface_state = "selected_interface_state";

        /// <summary>
        /// Mixpanel property name to describe connect via country, city, smart-connect, dedicated-IP
        /// </summary>
        public const string connect_via = "connect_via";

        /// <summary>
        /// Mixpanel property name to describe connection initiated by auto-reconnect or auto-connect-on-launch
        /// </summary>
        public const string connection_initiated_by = "connection_initiated_by";

        /// <summary>
        /// Mixpanel property name to describe the list of included NAS identifiers
        /// </summary>
        public const string included_nas_identifiers = "included_nas_identifiers";

        /// <summary>
        /// Mixpanel property name to describe the list of excluded NAS identifiers
        /// </summary>
        public const string excluded_nas_identifiers = "excluded_nas_identifiers";

        /// <summary>
        /// Mixpanel property name to describe the server is experimented or not
        /// </summary>
        public const string is_expiremented_server = "is_expiremented_server";

        /// <summary>
        /// Mixpanel property name to describe that the experimented server is requested or not
        /// </summary>
        public const string is_expirement_server_requested = "is_expirement_server_requested";

        /// <summary>
        /// Mixpanel property name to describe speed measurment time
        /// </summary>
        public const string speed_check_time = "speed_check_time";

        /// <summary>
        /// Mixpanel property name to describe the error message during speed measuremnet
        /// </summary>
        public const string error_message = "error_message";

        /// <summary>
        /// Mixpanel property name to describe the download speed of the user
        /// </summary>
        public const string download_speed = "download_speed";

        /// <summary>
        /// Mixpanel property name to describe the upload speed of the user
        /// </summary>
        public const string upload_speed = "upload_speed";

        /// <summary>
        /// Mixpanel property name to describe the dialed location of the current connection
        /// </summary>
        public const string dialed_location = "dialed_location";

        /// <summary>
        /// Mixpanel property name to describe the dialed location of the last connected connection
        /// </summary>
        public const string last_dialed_location = "last_dialed_location";
    }
}