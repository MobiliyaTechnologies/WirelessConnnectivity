namespace EnergyManagement_IotHubReceiver_WebJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using EnergyManagement_IotHubReceiver_WebJob.DataContracts;
    using EnergyManagement_IotHubReceiver_WebJob.Utilities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class SensorService
    {
        private DataTable sensorDataTable;
        private static string CLIENT_SUB_ADDRESS = "/ethClient.asmx/";
        private static string GET_TAG_LIST = "GetTagList";

        public static string Token { get; set; }

        public Dictionary<string, int> GetSensors()
        {
            var sensors = new Dictionary<string, int>();

            var query = "Select Sensor_Id, Sensor_Name from SensorMaster";

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var sensorId = SqlTypeConverter.ToInt32(sqlDataReader["Sensor_Id"]);
                            var sensorName = SqlTypeConverter.ToString(sqlDataReader["Sensor_Name"]);

                            if (sensorId > 0 && !string.IsNullOrWhiteSpace(sensorName))
                            {
                                sensors.Add(sensorName, sensorId);
                            }
                        }

                        sqlDataReader.Close();
                    }
                }

                sqlConnection.Close();
            }

            return sensors;
        }

        public int AddSensor(string sensorName)
        {
            var query = @"INSERT INTO SensorMaster(Sensor_Name) OUTPUT INSERTED.Sensor_Id VALUES (@SensorName)";
            var sensorID = 0;

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("SensorName", sensorName));
                    sensorID = (int)sqlCommand.ExecuteScalar();

                    if (sensorID == 0)
                    {
                        throw new InvalidOperationException(string.Format("Failed to add sensor - {0}.", sensorName));
                    }
                }

                sqlConnection.Close();
            }

            this.AddNewAlert(sensorID, sensorName);

            return sensorID;
        }

        public void AddSensorData(List<SensorData> sensorDatas)
        {
            this.SetDataTableValues(sensorDatas);

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ApplicationConfiguration.SqlConnectionString, SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.BatchSize = ApplicationConstant.SqlBulkCopyBatchSize;
                bulkCopy.DestinationTableName = ApplicationConstant.SensorLiveDataTableName;

                bulkCopy.WriteToServer(this.sensorDataTable);

                bulkCopy.Close();
            }
        }

        public List<SensorData> GetUpdatedSensorInfo()
        {
            GenerateToken();
            string jsonResponse = Post(CLIENT_SUB_ADDRESS, GET_TAG_LIST);
            return ConvertSensorData(JsonConvert.DeserializeObject<SensorsDataModel>(jsonResponse).d);
        }

        public void GenerateToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var request1 = new HttpRequestMessage(HttpMethod.Post, ApplicationConfiguration.BaseAddress + ApplicationConfiguration.AuthApiSubAddess);
                request1.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                        { "client_id", ApplicationConfiguration.Client_Id},
                        { "client_secret", ApplicationConfiguration.Client_Secret },
                        { "code", ApplicationConfiguration.Client_Code }
                     });
                var response = client.SendAsync(request1).Result;
                response.EnsureSuccessStatusCode();

                var payload = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                Token = payload.Value<string>("access_token");
            }
        }

        private static string Post(string subAddress, string apiName, string requestData = "")
        {
            string response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApplicationConfiguration.BaseAddress + subAddress);
                    if (Token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    client.DefaultRequestHeaders
                         .Accept
                         .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiName);
                    request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");

                    var responseMsg = client.SendAsync(request).Result;
                    responseMsg.EnsureSuccessStatusCode();
                    response = responseMsg.Content.ReadAsStringAsync().Result;
                }

            }
            catch (Exception e)
            {

            }

            return response;
        }

        private static List<SensorData> ConvertSensorData(List<D> rawSensorData)
        {
            List<SensorData> sensorDataList = new List<SensorData>();
            foreach (var rawSensor in rawSensorData)
            {
                sensorDataList.Add(new SensorData { Brightness = rawSensor.lux, Humidity = rawSensor.cap, SensorName = rawSensor.name, Temperature = ConvertCelsiusToFahrenheit(rawSensor.temperature), Timestamp = DateTime.UtcNow });
            }

            return sensorDataList;
        }

        private static double ConvertCelsiusToFahrenheit(double c)
        {
            return ((9.0 / 5.0) * c) + 32;
        }

        public void SetDataTableValues(List<SensorData> sensorDatas)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SensorData));

            if (this.sensorDataTable == null)
            {
                this.sensorDataTable = new DataTable();

                foreach (PropertyDescriptor property in properties)
                {
                    this.sensorDataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
            }

            this.sensorDataTable.Rows.Clear();

            foreach (var sensorData in sensorDatas)
            {
                DataRow row = this.sensorDataTable.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(sensorData) ?? DBNull.Value;
                }

                this.sensorDataTable.Rows.Add(row);
            }
        }

        private void AddNewAlert(int sensor_Id, string sensorName)
        {
            var query = @"INSERT INTO Alerts(Sensor_Log_Id,Sensor_Id,Alert_Type,Description,Timestamp) VALUES (@Sensor_Log_Id,@Sensor_Id,@Alert_Type,@Description,@Timestamp)";
            var description = string.Format("New device found with name {0}", sensorName);

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@Sensor_Log_Id", (Int64)1));
                    sqlCommand.Parameters.Add(new SqlParameter("@Sensor_Id", sensor_Id));
                    sqlCommand.Parameters.Add(new SqlParameter("@Alert_Type", "Device Alert"));
                    sqlCommand.Parameters.Add(new SqlParameter("@Description", description));
                    sqlCommand.Parameters.Add(new SqlParameter("@Timestamp", DateTime.Now));

                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }
    }
}
