using EF.Support.Entities.Implements;
using EF.Support.Entities.Interfaces;

namespace IdentityServer4.Domain.Entities;

public class ProfileEntities:Entity,IEntity
{
    private Guid id;
    private string name;
    private string description;
    private string image;
    public ProfileEntities()
    {

    }

    public ProfileEntities(Guid id, string name, string description, string image)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.image = image;
    }

    public Guid Id { get => id; set => id = value; }

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public string Image { get => image; set => image = value; }

    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
}