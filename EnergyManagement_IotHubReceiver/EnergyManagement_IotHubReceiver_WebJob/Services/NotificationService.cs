namespace EnergyManagement_IotHubReceiver_WebJob.Services
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Text;
    using EnergyManagement_IotHubReceiver_WebJob.DataContracts;
    using EnergyManagement_IotHubReceiver_WebJob.Utilities;
    using Newtonsoft.Json;

    public class NotificationService
    {
        public void SendNotification(string body)
        {
            var notificationConfiguration = this.GetNotificationConfiguration();

            if (!(string.IsNullOrWhiteSpace(notificationConfiguration.NotificationAuthorizationKey) || string.IsNullOrWhiteSpace(notificationConfiguration.NotificationSender) || string.IsNullOrWhiteSpace(notificationConfiguration.NotificationReceiver) || string.IsNullOrWhiteSpace(notificationConfiguration.NotificationURL) || string.IsNullOrWhiteSpace(notificationConfiguration.NotificationClickAction)))
            {
                WebRequest webRequest = WebRequest.Create(notificationConfiguration.NotificationURL);
                webRequest.Method = "post";
                webRequest.ContentType = "application/json";

                var data = new
                {
                    notification = new
                    {
                        body = body,
                        title = notificationConfiguration.NotificationTitle,
                        click_action = notificationConfiguration.NotificationClickAction,
                        sound = "default"
                    },
                    to = notificationConfiguration.NotificationReceiver
                };

                var json = JsonConvert.SerializeObject(data);

                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                webRequest.Headers.Add(string.Format("Authorization: key={0}", notificationConfiguration.NotificationAuthorizationKey));
                webRequest.Headers.Add(string.Format("Sender: id={0}", notificationConfiguration.NotificationSender));
                webRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    var response = webRequest.GetResponse();

                    if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                    {
                        using (Stream dataStreamResponse = response.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string responseFromServer = tReader.ReadToEnd();
                                Console.WriteLine("Notification request failed, server response - {0}", responseFromServer);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Notification configuration missing.");
            }
        }

        private NotificatioConfiguration GetNotificationConfiguration()
        {
            var notificationConfiguration = new NotificatioConfiguration();

            var query = "Select ConfigurationKey, ConfigurationValue from ApplicationConfigurationEntry where ApplicationConfigurationId = (select id from applicationconfiguration where ConfigurationType = @Firebase)";

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Firebase", ApplicationConstant.FirebaseApplicationConfiguration);

                    using (SqlDataReader result = sqlCommand.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            var configValue = SqlTypeConverter.ToString(result["ConfigurationValue"]);

                            switch (SqlTypeConverter.ToString(result["ConfigurationKey"]))
                            {
                                case ApplicationConstant.NotificationAuthorizationEntryKey:
                                    notificationConfiguration.NotificationAuthorizationKey = configValue;
                                    break;

                                case ApplicationConstant.NotificationSenderEntryKey:
                                    notificationConfiguration.NotificationSender = configValue;
                                    break;

                                case ApplicationConstant.NotificationReceiverEntryKey:
                                    notificationConfiguration.NotificationReceiver = configValue;
                                    break;
                            }
                        }

                        result.Close();
                    }
                }

                sqlConnection.Close();
            }

            return notificationConfiguration;
        }
    }
}
