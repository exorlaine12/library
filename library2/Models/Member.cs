using System.ComponentModel.DataAnnotations;

namespace library2.Models;

// One Member object holds one row of lending.member.
public class Member
{
    public long MemberId { get; set; }               // member_id    BIGINT, the primary key

    // 1. Rule: a name of at most 80 characters.
    // 2. Rule: the name is written as Lastname, Firstname.
    public string FullName { get; set; } = "";       // full_name    VARCHAR(80) NOT NULL

    // 3. Rule: an email of at most 120 characters.
    // 4. Rule: an email such as ana@ubaguio.edu, when one is given.
    public string? Email { get; set; }               // email        VARCHAR(120), may be NULL, UNIQUE

    public string MemberType { get; set; } = "";     // member_type  VARCHAR(10) NOT NULL

    public long OpenLoans { get; set; }              // filled only by the member list; never saved
}
