namespace Sortec.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}