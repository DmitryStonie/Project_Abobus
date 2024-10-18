using Hackathon.Database.SQLite;

namespace Hackathon.DataProviders;

public class DataSaver(ApplicationContext context) : IDataSavingInterface
{
    public void SaveData(List<Junior> juniors, List<TeamLead> teamLeads, List<Team> teams, Hackathon hackathon)
    {
        SaveHackathon(hackathon);
        SaveJuniors(juniors);
        SaveTeamLeads(teamLeads);
        Console.WriteLine("Employees before saving");
        juniors.ForEach(j => Console.WriteLine(j.ToString()));
        teamLeads.ForEach(j => Console.WriteLine(j.ToString()));
        context.SaveChanges();
        Console.WriteLine("Employees after saving");
        juniors.ForEach(j => Console.WriteLine(j.ToString()));
        teamLeads.ForEach(j => Console.WriteLine(j.ToString()));
        SaveTeams(teams, juniors, teamLeads, hackathon.Id);
        SaveWishLists(juniors, teamLeads, hackathon.Id);
        teams.ForEach(j => Console.WriteLine($"teamlead {j.TeamLeadId} junior {j.JuniorId} teamid {j.Id} hackathonid{j.HackathonId}"));
        context.SaveChanges();
        teams.ForEach(j => Console.WriteLine($"teamlead {j.TeamLeadId} junior {j.JuniorId} teamid {j.Id} hackathonid{j.HackathonId}"));
        SaveWishes(juniors, teamLeads);
        context.SaveChanges();
    }

    private void SaveJuniors(List<Junior> juniors)
    {
        foreach (var junior in juniors)
        {
            if (context.Juniors.Any(j => j.Id == junior.Id))
            {
                context.Juniors.Attach(junior);
            }
            else
            {
                context.Juniors.Add(junior);
            }
        }
    }

    private void SaveTeamLeads(List<TeamLead> teamLeads)
    {
        foreach (var teamLead in teamLeads)
        {
            if (context.Teamleads.Any(t => t.Id == teamLead.Id))
            {
                context.Teamleads.Attach(teamLead);
            }
            else
            {
                context.Teamleads.Add(teamLead);
            }
        }
    }

    private void SaveHackathon(Hackathon hackathon)
    {
        if (context.Hackathons.Any(t => t.Id == hackathon.Id))
        {
            context.Hackathons.Attach(hackathon);
        }
        else
        {
            context.Hackathons.Add(hackathon);
        }
    }

    private void SaveTeams(List<Team> teams, List<Junior> juniors, List<TeamLead> teamLeads, int hackathonId)
    {
        var juniorDict = new Dictionary<int, Junior>();
        var teamLeadDict = new Dictionary<int, TeamLead>();
        juniors.ForEach(j => juniorDict.Add(j.JuniorId, j));
        teamLeads.ForEach(t => teamLeadDict.Add(t.TeamLeadId, t));
        foreach (var team in teams)
        {
            team.HackathonId = hackathonId;
            team.JuniorId = juniorDict[team.Junior.JuniorId].Id;
            team.TeamLeadId = teamLeadDict[team.TeamLead.TeamLeadId].Id;
        }

        foreach (var team in teams)
        {
            if (context.Teams.Any(t => t.Id == team.Id))
            {
                context.Teams.Attach(team);
            }
            else
            {
                context.Teams.Add(team);
            }
        }
    }

    private void SaveWishLists(List<Junior> juniors, List<TeamLead> teamLeads, int hackathonId)
    {
        var wishlists = new List<Wishlist>();
        foreach (var junior in juniors)
        {
            junior.Wishlist.HackathonId = hackathonId;
            junior.Wishlist.OwnerId = junior.Id;
            wishlists.Add(junior.Wishlist);
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.Wishlist.HackathonId = hackathonId;
            teamLead.Wishlist.OwnerId = teamLead.Id;
            wishlists.Add(teamLead.Wishlist);
        }
        Console.WriteLine("Wishlists before saving");
        wishlists.ForEach(w => Console.WriteLine($"owner {w.OwnerId} hackathon {w.HackathonId} wishlist id {w.Id}"));
        foreach (var wishlist in wishlists)
        {
            if (context.WishLists.Any(w => w.Id == wishlist.Id))
            {
                context.WishLists.Attach(wishlist);
            }
            else
            {
                context.WishLists.Add(wishlist);
            }
        }
    }

    private void SaveWishes(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        Console.WriteLine("Wishes before saving");
        foreach (var junior in juniors)
        {
            junior.Wishlist.InitWishes(junior.Id);
            junior.Wishlist.Wishes.ForEach(w => Console.WriteLine($"owner {w.OwnerId} partner {w.PartnerId} wishlist id {w.Id}"));
            
            SaveWishesList(junior.Wishlist.Wishes);
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.Wishlist.InitWishes(teamLead.Id);
            SaveWishesList(teamLead.Wishlist.Wishes);
        }
    }

    private void SaveWishesList(List<Wish> wishes)
    {
        foreach (var wish in wishes)
        {
            Console.WriteLine($"{wish.Id}  {wish.OwnerId}  {wish.PartnerId}  {wish.Score}  {wish.WishlistId}");
            if (context.Wishes.Any(w => w.Id == wish.Id))
            {
                context.Wishes.Attach(wish);
            }
            else
            {
                context.Wishes.Add(wish);
            }
        }
    }
}