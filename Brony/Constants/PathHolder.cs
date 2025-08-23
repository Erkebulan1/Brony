﻿namespace Brony.Constants;

public class PathHolder
{
    private static readonly string parentRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
    public static readonly string UsersFilePath = Path.Combine(parentRoot, "Data", "users.json");
    public static readonly string BookingsFilePath = Path.Combine(parentRoot, "Data", "bookings.json");
    public static readonly string StadiumsFilePath = Path.Combine(parentRoot, "Data", "stadiums.json");
    public const string ConnectionString = "Host=localhost;Port=5432;Database=testdb;Username=postgres;Password=root;";
}