namespace MTCG.Models;

public interface Card
{
    string name {
        get;
    }
    int damage{
        get;
    }
    string elementType{
        get;
    }
}