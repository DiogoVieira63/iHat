using iHat.Model.Obras;
namespace iHat.Model.Users;
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Obra> Obras { get; set; }
}