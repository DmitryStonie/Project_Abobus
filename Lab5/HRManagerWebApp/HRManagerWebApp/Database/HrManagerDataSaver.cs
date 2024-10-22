﻿using Hackathon;
using Hackathon.DataProviders;

namespace HRManagerWebApp.Database;

public class HrManagerDataSaver(HrManagerApplicationContext context) : IDataSavingInterface
{
    
    public void SaveData(List<Junior> juniors, List<TeamLead> teamLeads, List<Team> teams, Hackathon.Hackathon hackathon)
    {
        SaveHackathon(hackathon);
        SaveJuniors(juniors);
        SaveTeamLeads(teamLeads);
        context.SaveChangesAsync().Wait();
        SaveTeams(teams, juniors, teamLeads, hackathon.Id);
        SaveWishLists(juniors, teamLeads, hackathon.Id);
        context.SaveChangesAsync().Wait();
        SaveWishes(juniors, teamLeads);
        context.SaveChangesAsync().Wait();
    }
    public void SaveEmployees(List<Junior> juniors, List<TeamLead> teamLeads, Hackathon.Hackathon hackathon)
    {
        SaveHackathon(hackathon);
        SaveJuniors(juniors);
        SaveTeamLeads(teamLeads);
        context.SaveChangesAsync().Wait();
        SaveWishLists(juniors, teamLeads, hackathon.Id);
        context.SaveChangesAsync().Wait();
        SaveWishes(juniors, teamLeads);
        context.SaveChangesAsync().Wait();
    }

    private void SaveJuniors(List<Junior> juniors)
    {
        for (int i = 0; i < juniors.Count; i++)
        {
            if (context.Juniors.Any(j => j.JuniorId == juniors[i].JuniorId))
            {
                var id = context.Juniors.FirstOrDefault(j => j.JuniorId == juniors[i].JuniorId)!.Id;
                juniors[i] = new Junior(juniors[i].JuniorId, juniors[i].Name, id, juniors[i].Wishlist);
            }
            else
            {
                context.Juniors.Add(juniors[i]);
            }
        }
    }

    private void SaveTeamLeads(List<TeamLead> teamLeads)
    {
        for (int i = 0; i < teamLeads.Count; i++)
        {
            if (context.Teamleads.Any(t => t.TeamLeadId == teamLeads[i].TeamLeadId))
            {
                var id = context.Teamleads.FirstOrDefault(t => t.TeamLeadId == teamLeads[i].TeamLeadId)!.Id;
                teamLeads[i] = new TeamLead(teamLeads[i].TeamLeadId, teamLeads[i].Name, id, teamLeads[i].Wishlist);
            }
            else
            {
                context.Teamleads.Add(teamLeads[i]);
            }
        }
    }

    private void SaveHackathon(Hackathon.Hackathon hackathon)
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
            team.JuniorId = juniorDict[team.Junior.JuniorId].JuniorId;
            team.TeamLeadId = teamLeadDict[team.TeamLead.TeamLeadId].JuniorId;
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
            junior.Wishlist.OwnerId = junior.JuniorId;
            wishlists.Add(junior.Wishlist);
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.Wishlist.HackathonId = hackathonId;
            teamLead.Wishlist.OwnerId = teamLead.TeamLeadId;
            wishlists.Add(teamLead.Wishlist);
        }

        for (int i = 0; i < wishlists.Count; i++)
        {
            if (context.WishLists.Any(w => w.Id == wishlists[i].Id))
            {
                wishlists[i] = context.WishLists.FirstOrDefault(w => w.Id == wishlists[i].Id)!;
            }
            else
            {
                context.WishLists.Add(wishlists[i]);
            }
        }
    }

    private void SaveWishes(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        var wishesList = new List<Wish>();
        foreach (var junior in juniors)
        {
            junior.Wishlist.InitWishlistById(junior.JuniorId);
            var wishlist = junior.Wishlist.GetEmployee();
            foreach (var wish in junior.Wishlist.Wishes)
            {
                    wishesList.Add(wish);
            }
        }

        foreach (var teamLead in teamLeads)
        {
            teamLead.Wishlist.InitWishlistById(teamLead.TeamLeadId);
            var wishlist = teamLead.Wishlist.GetEmployee();
            foreach (var employee in wishlist)
            {
                foreach (var wish in teamLead.Wishlist.Wishes)
                {
                    wishesList.Add(wish);
                }
            }
        }

        SaveWishesList(wishesList);
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