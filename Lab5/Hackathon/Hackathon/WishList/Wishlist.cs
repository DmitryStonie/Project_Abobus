using System.Runtime.Serialization;

namespace Hackathon;

[DataContract]
public class Wishlist
{
    [DataMember] private List<Employee> _wishlist;
    private Dictionary<Employee, int> _wishlistDictionary;
    private int _candidateIndex;

    public List<Wish> Wishes { get; private set; }
    public int Id { get; init; }
    public int HackathonId { get; set; }
    public int OwnerId { get; set; }

    public Wishlist()
    {
    }

    public Wishlist(List<Employee> employees)
    {
        _wishlist = new List<Employee>(employees);
        _wishlistDictionary = ToDictionary();
        _candidateIndex = 0;
    }

    private Dictionary<Employee, int> ToDictionary()
    {
        var dictionary = new Dictionary<Employee, int>();
        for (int index = 0; index < _wishlist.Count; index++)
        {
            dictionary.Add(_wishlist[index], _wishlist.Count - index);
        }

        return dictionary;
    }

    public int GetScore(Employee employee)
    {
        if (_wishlistDictionary.ContainsKey(employee))
        {
            return _wishlistDictionary[employee];
        }

        throw new KeyNotFoundException();
    }

    public int GetSize()
    {
        return _wishlist.Count;
    }

    public Employee? GetNextCandidate()
    {
        if (_candidateIndex < _wishlist.Count)
        {
            return _wishlist[_candidateIndex++];
        }

        return null;
    }

    public void ResetCandidateSequence()
    {
        _candidateIndex = 0;
    }

    public void InitWishes(int ownerId)
    {
        Console.WriteLine($"wishlist of {ownerId}");
        Wishes = new List<Wish>();
        Console.WriteLine("Wishlists dictionary");
        foreach (var data in _wishlistDictionary)
        {
            Console.WriteLine($"key {data.Key} - value {data.Value}");
        }
        Console.WriteLine($"Init wishes for {ownerId}");
        foreach (var wish in _wishlistDictionary)
        {
            Console.WriteLine($"score {wish.Value} {wish.Key.Id}");
            Wishes.Add(new Wish(wish.Value, Id, ownerId, wish.Key.Id));
        }
    }

    public void InitWishlist()
    {
        _wishlistDictionary = ToDictionary();
        InitWishes(OwnerId);
    }

    public List<Employee> GetEmployee()
    {
        return _wishlist;
    }
}