using System.Runtime.Serialization;

namespace Hackathon;

[DataContract]
public class Wishlist
{
    [DataMember] private List<Employee> _wishlist;
    private Dictionary<Employee, int> _wishlistDictionary;
    private int _candidateIndex;

    public List<Wish> Wishes { get; private set; }
    public int Id { get; set; }
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
        var value = _wishlistDictionary.FirstOrDefault(e => e.Key.Id == employee.Id); 
        if (value.Key != null)
        {
            return _wishlistDictionary[value.Key];
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
        Wishes = new List<Wish>();
        foreach (var wish in _wishlistDictionary)
        {
            Wishes.Add(new Wish(wish.Value, Id, ownerId, wish.Key.Id));
        }
    }
    public void InitWishesById(int ownerId)
    {
        Wishes = new List<Wish>();
        foreach (var wish in _wishlistDictionary)
        {
            int partnerId = wish.Key.JuniorId == 0 ? wish.Key.TeamLeadId : wish.Key.JuniorId;
            Wishes.Add(new Wish(wish.Value, Id, ownerId, partnerId));
        }
    }

    public void InitWishlist()
    {
        _wishlistDictionary = ToDictionary();
        InitWishes(OwnerId);
    }
    public void InitWishlistById(int ownerId)
    {
        _wishlistDictionary = ToDictionary();
        InitWishesById(ownerId);
    }

    public List<Employee> GetEmployee()
    {
        return _wishlist;
    }
}