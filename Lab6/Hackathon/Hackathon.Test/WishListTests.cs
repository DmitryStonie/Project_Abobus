using Hackathon;

namespace Hackathon.Test;

public class WishListTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void WishlistSizeTest_WishlistSizeShouldBeAsExpected()
    {
        //Arrange
        var juniors = DataProvider.GetJuniors();
        var teamLeads = DataProvider.GetTeamLeads();
        var generator = new RandomWishlistGenerator();
        //Act
        juniors.ForEach(junior =>
            junior.Wishlist = new Wishlist(generator.CreateWishlist(teamLeads.Cast<Employee>().ToList())));
        teamLeads.ForEach(teamLead =>
            teamLead.Wishlist = new Wishlist(generator.CreateWishlist(juniors.Cast<Employee>().ToList())));
        //Assert
        foreach (var junior in juniors)
        {
            Assert.AreEqual(junior.Wishlist.GetSize(), teamLeads.Count, "The number of junior wishlist is incorrect");
        }

        foreach (var teamLead in teamLeads)
        {
            Assert.AreEqual(teamLead.Wishlist.GetSize(), juniors.Count, "The number of teamLead wishlist is incorrect");
        }
    }

    [Test]
    public void WishlistIntegrityTest_AllWishlistsShouldContainAllEmployees()
    {
        //Arrange
        var juniors = DataProvider.GetJuniors();
        var teamLeads = DataProvider.GetTeamLeads();
        var generator = new RandomWishlistGenerator();
        //Act
        juniors.ForEach(junior =>
            junior.Wishlist = new Wishlist(generator.CreateWishlist(teamLeads.Cast<Employee>().ToList())));
        teamLeads.ForEach(teamLead =>
            teamLead.Wishlist = new Wishlist(generator.CreateWishlist(juniors.Cast<Employee>().ToList())));
        //Assert
        foreach (var junior in juniors)
        {
            foreach (var teamLead in teamLeads)
            {
                if (junior.GetScore(teamLead) == 0)
                {
                    Assert.Fail($"TeamLead {teamLead} not found in wishlist.");
                }
            }
        }

        foreach (var teamLead in teamLeads)
        {
            foreach (var junior in juniors)
            {
                if (teamLead.GetScore(junior) == 0)
                {
                    Assert.Fail($"Junior {junior} not found in wishlist.");
                }
            }
        }

        Assert.Pass("All wishlists correct.");
    }
}