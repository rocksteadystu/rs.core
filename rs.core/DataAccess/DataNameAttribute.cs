namespace rs.core.DataAccess;

public class DataNameAttribute : Attribute
{
    public string Name {get;}

    public DataNameAttribute(string name)
    {
        Name = name;
    }
}
