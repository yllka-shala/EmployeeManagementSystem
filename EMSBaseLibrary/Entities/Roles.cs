

namespace EMSBaseLibrary.Entities
{
    public class Roles : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}
