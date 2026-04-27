using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BaseCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace BaseCore.Repository.Authen
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(string id);
        Task<List<User>> GetAllAsync();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
        Task<(List<User> Users, int TotalCount)> SearchAsync(string keyword, int page, int pageSize);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public async Task<User> GetByUsernameAsync(string username)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(
                "SELECT * FROM Users WHERE UserName = @username AND IsActive = 1", conn);
            cmd.Parameters.AddWithValue("@username", username);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapUser(reader);
            return null;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT * FROM Users WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapUser(reader);
            return null;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            using var conn = GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT * FROM Users WHERE IsActive = 1", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                users.Add(MapUser(reader));
            return users;
        }

       public async Task CreateAsync(User user)
{
    try
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        var cmd = new SqlCommand(@"
            INSERT INTO Users 
            (Id, Name, UserName, Password, Salt, Contact, Email, Phone, Position, Image, IsActive, UserType, Created)
            VALUES 
            (@id, @name, @username, @password, @salt, @contact, @email, @phone, @position, @image, @isActive, @userType, @created)", conn);

        cmd.Parameters.AddWithValue("@id", user.Id ?? Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@name", user.Name ?? "");
        cmd.Parameters.AddWithValue("@username", user.UserName);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@salt", user.Salt);
        cmd.Parameters.AddWithValue("@contact", user.Contact ?? "");
        cmd.Parameters.AddWithValue("@email", user.Email ?? "");
        cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");
        cmd.Parameters.AddWithValue("@position", user.Position ?? "");
        cmd.Parameters.AddWithValue("@image", user.Image ?? "");
        cmd.Parameters.AddWithValue("@isActive", true);
        cmd.Parameters.AddWithValue("@userType", user.UserType);
        cmd.Parameters.AddWithValue("@created", DateTime.Now);

        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("✅ User inserted successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ CreateAsync error: {ex.Message}");
        throw;
    }
}
        public async Task UpdateAsync(User user)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE Users SET 
                Name = @name, Email = @email, Phone = @phone,
                Position = @position, Image = @image, IsActive = @isActive
                WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@name", user.Name ?? "");
            cmd.Parameters.AddWithValue("@email", user.Email ?? "");
            cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");
            cmd.Parameters.AddWithValue("@position", user.Position ?? "");
            cmd.Parameters.AddWithValue("@image", user.Image ?? "");
            cmd.Parameters.AddWithValue("@isActive", user.IsActive);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(string id)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(
                "UPDATE Users SET IsActive = 0 WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<(List<User> Users, int TotalCount)> SearchAsync(string keyword, int page, int pageSize)
        {
            var users = new List<User>();
            using var conn = GetConnection();
            await conn.OpenAsync();

            var where = "WHERE IsActive = 1";
            if (!string.IsNullOrEmpty(keyword))
                where += " AND (UserName LIKE @kw OR Name LIKE @kw OR Email LIKE @kw OR Phone LIKE @kw)";

            var countCmd = new SqlCommand($"SELECT COUNT(*) FROM Users {where}", conn);
            if (!string.IsNullOrEmpty(keyword))
                countCmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
            var total = (int)await countCmd.ExecuteScalarAsync();

            var dataCmd = new SqlCommand($@"
                SELECT * FROM Users {where}
                ORDER BY Created DESC
                OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", conn);
            if (!string.IsNullOrEmpty(keyword))
                dataCmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
            dataCmd.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
            dataCmd.Parameters.AddWithValue("@take", pageSize);

            using var reader = await dataCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                users.Add(MapUser(reader));

            return (users, total);
        }

        // Helper: Map SQL row → User object
        private User MapUser(SqlDataReader reader) => new User
        {
            Id = reader["Id"].ToString(),
            Name = reader["Name"].ToString(),
            UserName = reader["UserName"].ToString(),
            Password = reader["Password"].ToString(),
             Salt = reader["Salt"] as byte[],  
            Email = reader["Email"].ToString(),
            Phone = reader["Phone"].ToString(),
            Position = reader["Position"].ToString(),
            Image = reader["Image"].ToString(),
            IsActive = (bool)reader["IsActive"],
            UserType = (int)reader["UserType"],
            Created = (DateTime)reader["Created"]
        };

        // Helper: Tạo Salt
        private byte[] GenerateSalt()
        {
            var salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        // Helper: Hash Password với Salt
        private string HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA512);
            return Convert.ToBase64String(pbkdf2.GetBytes(64));
        }
    }
}