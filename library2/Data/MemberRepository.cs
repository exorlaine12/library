
using library2.Models;
using Npgsql;

namespace library2.Data;

// Every SQL statement about members lives in this one class.
public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            MemberType = reader.GetString(3)
        };
    }

    // READ: every member, sorted by full_name.
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql = "SELECT member_id, full_name, email, member_type FROM lending.member ORDER BY full_name;";

        var members = new List<Member>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            members.Add(ReadMember(reader));
        }

        return members;
    }
    // CREATE: insert a new member. The database chooses member_id and join_date.
    public async Task AddAsync(Member member)
    {
        const string sql = "INSERT INTO lending.member (full_name, email, member_type) " +
                           "VALUES (@full_name, @email, @member_type);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("full_name", member.FullName);
        command.Parameters.AddWithValue("email", (object?)member.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("member_type", member.MemberType);

        await command.ExecuteNonQueryAsync();
    }
}