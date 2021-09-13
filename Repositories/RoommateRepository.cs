﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;


namespace Roommates.Repositories
{
    ///  Class responsible for interacting with Roommate data and inherits from the BaseRepository class so that it can use
    ///  the BaseRepository's Connection property:
    public class RoommateRepository : BaseRepository
    {
        ///  When new RoommateRepository is instantiated, pass the connection string along to the BaseRepository
        public RoommateRepository(string connectionString) : base(connectionString) { }

        ///  Returns a single roommate with the given id:
        public Roommate GetRoommateById(int id)
        {
            using (SqlConnection roommateConn = Connection)
            {
                roommateConn.Open();
                using (SqlCommand cmd = roommateConn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.FirstName, rm.RentPortion, rm.RoomId, r.Name
                                        FROM Roommate rm
                                        INNER JOIN Room r
                                        ON rm.RoomId = r.Id
                                        WHERE rm.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null; // creating a local roommate variable with initial value of null
                    // If we only expect a single row back from the database, we don't need a while loop:
                    if (reader.Read())
                    {
                        // Create a new roommate object via object initializer using the data from the database:
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            Room = new Room
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }

        // Retrieve list of all roommates:
        public List<Roommate> GetAllRoommates()
        {
            using (SqlConnection roommateConn = Connection)
            {
                roommateConn.Open();
                using (SqlCommand cmd = roommateConn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.Id [rmId], rm.FirstName, rm.LastName, rm.RentPortion, rm.MoveInDate, rm.RoomId, r.Id [rId], r.Name, r.MaxOccupancy
                                      FROM Roommate rm
                                      INNER JOIN Room r
                                      ON rm.RoomId = r.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int rmIdValue = reader.GetInt32(reader.GetOrdinal("rmId")); // Roommate.Id primary key on Roommate table
                        string rmFirstNameValue = reader.GetString(reader.GetOrdinal("FirstName"));
                        string rmLastNameValue = reader.GetString(reader.GetOrdinal("LastName"));
                        int rmRentPortionValue = reader.GetInt32(reader.GetOrdinal("RentPortion"));
                        DateTime rmMoveInDateValue = reader.GetDateTime(reader.GetOrdinal("MoveInDate"));
                        int roomIdValue = reader.GetInt32(reader.GetOrdinal("RoomId")); // RoomId foreign key on Roommate table
                        int rIdValue = reader.GetInt32(reader.GetOrdinal("rId")); // Room.Id primary key on Room table
                        string rNameValue = reader.GetString(reader.GetOrdinal("Name"));
                        int rMaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"));

                        Roommate roommate = new Roommate
                        {
                            Id = rmIdValue,
                            FirstName = rmFirstNameValue,
                            LastName = rmLastNameValue,
                            RentPortion = rmRentPortionValue,
                            MovedInDate = rmMoveInDateValue,
                            Room = new Room
                            {
                                Id = roomIdValue,
                                Name = rNameValue,
                                MaxOccupancy = rMaxOccupancy
                            }
                        };

                        roommates.Add(roommate);
                    }

                    reader.Close();

                    return roommates;
                }
            }
        }

    }
}


