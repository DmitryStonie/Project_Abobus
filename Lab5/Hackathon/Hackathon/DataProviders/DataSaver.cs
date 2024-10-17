using Hackathon.Database.SQLite;

namespace Hackathon.DataProviders;

public class DataSaver(ApplicationContext context) : IDataSavingInterface
{
    public void SaveData(List<Junior> juniors, List<TeamLead> teamLeads, List<Team> teams, Hackathon hackathon)
    {
        SaveHackathon(hackathon);
        SaveJuniors(juniors);
        SaveTeamLeads(teamLeads);
        context.SaveChanges();
        SaveTeams(teams, juniors, teamLeads, hackathon.Id);
        SaveWishLists(juniors, teamLeads, hackathon.Id);
        context.SaveChanges();
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
        foreach (var junior in juniors)
        {
            junior.Wishlist.InitWishes(junior.Id);
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