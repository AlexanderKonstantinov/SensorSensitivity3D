using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SensorSensitivity3D.Domain.Entities;

namespace SensorSensitivity3D.Services
{
    public class GCSDbReadService
    {
        public IEnumerable<Geophone> GetGeophonesFromGCSDb(string connectionString, out string errorMessage)
        {
            List<Geophone> geophones = null;
            errorMessage = String.Empty;

            try
            {
                using (var cnn = new SqlConnection(connectionString))
                {
                    cnn.Open();

                    using (var command = cnn.CreateCommand())
                    {
                        var sensorHoles = new Dictionary<int, int>();

                        command.CommandText = @"
SELECT Sensors.HWID, Holes.Name
  FROM SensorHole

LEFT OUTER JOIN Sensors
ON SensorHole.SensorID = Sensors.ID

LEFT OUTER JOIN Holes
ON SensorHole.HoleID = Holes.ID

WHERE SensorHole.BeginTime IN (
  SELECT MAX(BeginTime) FROM SensorHole Where BeginTime IN (
  SELECT MAX(BeginTime) FROM SensorHole GROUP BY HoleID) GROUP BY SensorID)";

                        geophones = new List<Geophone>(sensorHoles.Count);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                geophones.Add(new Geophone
                                {
                                    Name = $"HWID {reader.GetInt32(0)}", 
                                    HoleNumber = reader.GetInt32(1)
                                });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return geophones;
            }


            return geophones;
        }
    }
}
