using EF.Support.Entities.Implements;
using EF.Support.Entities.Interfaces;

namespace IdentityServer4.Domain.Entities;

public class ProfileEntities : Entity, IEntity
{
    private Guid id;
    private string name;
    private string description;
    private string image;
    private Guid userId;

    public ProfileEntities()
    {
    }

    public ProfileEntities(Guid id, string name, string description, string image, Guid userId)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.image = image;
        this.userId = userId;
    }

    public Guid Id
    {
        get => id;
        set => id = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public string Image
    {
        get => image;
        set => image = value;
    }

    public Guid UserId
    {
        get => userId;
        set => userId = value;
    }

    public virtual UserEntity User { get; set; }
}