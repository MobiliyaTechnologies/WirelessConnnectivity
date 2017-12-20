namespace EnergyManagement_IotHubReceiver_WebJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using EnergyManagement_IotHubReceiver_WebJob.DataContracts;
    using EnergyManagement_IotHubReceiver_WebJob.Utilities;

    public sealed class PowerScoutService
    {
        public Dictionary<string, int> GetBuildings()
        {
            var buildings = new Dictionary<string, int>();

            var buildingQuery = @"SELECT  BuildingID ,BuildingName  FROM [dbo].[Building] 
                                where IsActive = 1 and IsDeleted = 0";

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(buildingQuery, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var buildingId = SqlTypeConverter.ToInt32(sqlDataReader["BuildingID"]);
                            var buildingName = SqlTypeConverter.ToString(sqlDataReader["BuildingName"]);

                            if (buildingId > 0 && !string.IsNullOrWhiteSpace(buildingName))
                            {
                                buildings.Add(buildingName, buildingId);
                            }
                        }

                        sqlDataReader.Close();
                    }
                }

                sqlConnection.Close();
            }

            return buildings;
        }

        public int AddBuilding(string buildingName)
        {
            var buildingQuery = @"INSERT INTO [Building](BuildingName) OUTPUT INSERTED.BuildingID VALUES (@BuildingName)";
            var buildingID = 0;

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(buildingQuery, sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("BuildingName", buildingName));
                    buildingID = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    if (buildingID == 0)
                    {
                        throw new InvalidOperationException(string.Format("Failed to add building - {0}.", buildingName));
                    }
                }

                sqlConnection.Close();
            }

            return buildingID;
        }

        public List<string> GetPowerScout()
        {
            var powerScouts = new List<string>();

            var buildingQuery = @"SELECT PowerScout FROM [dbo].[MeterDetails]";

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(buildingQuery, sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var powerScout = SqlTypeConverter.ToString(sqlDataReader["PowerScout"]);

                            if (string.IsNullOrWhiteSpace(powerScout))
                            {
                                powerScouts.Add(powerScout);
                            }
                        }

                        sqlDataReader.Close();
                    }
                }
                sqlConnection.Close();
            }

            return powerScouts;
        }

        public void AddPowerScout(MeterInfo meterInfo)
        {
            var powerScoutQuery = @"INSERT INTO MeterDetails(PowerScout,Breaker_details,BuildingId,UTCConversionTime) VALUES (@PowerScout,@Breaker_details,@BuildingId,@UTCConversionTime)";

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(powerScoutQuery, sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@PowerScout", meterInfo.PowerScout));
                    sqlCommand.Parameters.Add(new SqlParameter("@Breaker_details", meterInfo.Breaker_details));
                    sqlCommand.Parameters.Add(new SqlParameter("@BuildingId", meterInfo.BuildingId));
                    sqlCommand.Parameters.Add(new SqlParameter("@UTCConversionTime", meterInfo.UTCConversionTime));

                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }

        public void AddPowerScoutData(PowerScoutData powerScoutData)
        {
            var powerScoutDataQuery = @"INSERT INTO LiveData(AMPS_SYSTEM_AVG,Breaker_details,Building,Daily_electric_cost,Daily_KWH_System,Monthly_electric_cost,Monthly_KWH_System,PowerScout,Temperature,Timestamp,Visibility,kW_System,days,time_period,PiServerName, ClassOccupiedValue, Relative_humidity, ClassOccupanyRemaining, TotalClassCapacity, isClassOccupied, Pressure)VALUES (@AMPS_SYSTEM_AVG,@Breaker_details,@Building,@Daily_electric_cost,@Daily_KWH_System,@Monthly_electric_cost,@Monthly_KWH_System,@PowerScout,@Temperature,@Timestamp,@Visibility,@kW_System,@days,@time_period,@PiServerName, @ClassOccupiedValue, @Relative_humidity, @ClassOccupanyRemaining, @TotalClassCapacity, @isClassOccupied, @Pressure)";

            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(powerScoutDataQuery, sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("AMPS_SYSTEM_AVG", powerScoutData.AMPS_SYSTEM_AVG));
                    sqlCommand.Parameters.Add(new SqlParameter("Breaker_details", powerScoutData.Breaker_details));
                    sqlCommand.Parameters.Add(new SqlParameter("Building", powerScoutData.Building));
                    sqlCommand.Parameters.Add(new SqlParameter("Daily_electric_cost", powerScoutData.Daily_electric_cost));
                    sqlCommand.Parameters.Add(new SqlParameter("Daily_KWH_System", powerScoutData.Daily_KWH_System));
                    sqlCommand.Parameters.Add(new SqlParameter("Monthly_electric_cost", powerScoutData.Monthly_electric_cost));
                    sqlCommand.Parameters.Add(new SqlParameter("Monthly_KWH_System", powerScoutData.Monthly_KWH_System));
                    sqlCommand.Parameters.Add(new SqlParameter("PowerScout", powerScoutData.PowerScout));
                    sqlCommand.Parameters.Add(new SqlParameter("Temperature", powerScoutData.Temperature));
                    sqlCommand.Parameters.Add(new SqlParameter("Timestamp", powerScoutData.Timestamp.ToString(ApplicationConstant.DateTimeFormat)));
                    sqlCommand.Parameters.Add(new SqlParameter("Visibility", powerScoutData.Visibility));
                    sqlCommand.Parameters.Add(new SqlParameter("kW_System", powerScoutData.KwSystem));
                    sqlCommand.Parameters.Add(new SqlParameter("days", powerScoutData.Timestamp.ToString("dddd")));
                    sqlCommand.Parameters.Add(new SqlParameter("time_period", this.GetTimePriodFromTimestamp(powerScoutData.Timestamp)));
                    sqlCommand.Parameters.Add(new SqlParameter("PiServerName", powerScoutData.PiServerName));
                    sqlCommand.Parameters.Add(new SqlParameter("ClassOccupiedValue", powerScoutData.ClassOccupiedValue));
                    sqlCommand.Parameters.Add(new SqlParameter("Relative_humidity", powerScoutData.Relative_humidity));
                    sqlCommand.Parameters.Add(new SqlParameter("ClassOccupanyRemaining", powerScoutData.ClassOccupanyRemaining));
                    sqlCommand.Parameters.Add(new SqlParameter("TotalClassCapacity", powerScoutData.TotalClassCapacity));
                    sqlCommand.Parameters.Add(new SqlParameter("isClassOccupied", powerScoutData.IsClassOccupied));
                    sqlCommand.Parameters.Add(new SqlParameter("Pressure", powerScoutData.Pressure));

                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }

        private string GetTimePriodFromTimestamp(DateTime timeStamp)
        {
            string timeperiod = string.Empty;
            if (timeStamp.Hour >= 0 && timeStamp.Hour <= 3)
            {
                timeperiod = "Mid Night";
            }
            else if (timeStamp.Hour >= 4 && timeStamp.Hour <= 7)
            {
                timeperiod = "Early Morning";
            }
            else if (timeStamp.Hour >= 8 && timeStamp.Hour <= 11)
            {
                timeperiod = "Morning";
            }
            else if (timeStamp.Hour >= 12 && timeStamp.Hour <= 15)
            {
                timeperiod = "Afternoon";
            }
            else if (timeStamp.Hour >= 16 && timeStamp.Hour <= 19)
            {
                timeperiod = "Evening";
            }
            else if (timeStamp.Hour >= 20 && timeStamp.Hour <= 23)
            {
                timeperiod = "Night";
            }
            return timeperiod;
        }
    }
}
