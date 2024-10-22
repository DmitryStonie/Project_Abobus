using Hackathon;

namespace Hackathon.Test;

public static class DataProvider
{
    public static List<Junior> GetJuniors()
    {
        var juniors = new List<Junior>()
        {
            new Junior(1, "Юдин Адам"),
            new Junior(2, "Яшина Яна"),
            new Junior(3, "Никитина Вероника"),
            new Junior(4, "Рябинин Александр"),
            new Junior(5, "Ильин Тимофей"),
            new Junior(6, "Кулагина Виктория"),
            new Junior(7, "Лапшина Варвара"),
            new Junior(8, "Лопатина Камилла"),
            new Junior(9, "Кузьмина Елизавета"),
            new Junior(10, "Галкина Есения"),
            new Junior(11, "Панина Таисия"),
            new Junior(12, "Семина Арина"),
            new Junior(13, "Бабушкин Иван"),
            new Junior(14, "Кузьмин Глеб"),
            new Junior(15, "Добрынин Степан"),
            new Junior(16, "Фомин Никита"),
            new Junior(17, "Маркина Кристина"),
            new Junior(18, "Зеленина Кира"),
            new Junior(19, "Капустина Елизавета"),
            new Junior(20, "Костин Александр"),
        };
        return juniors;
    }

    public static List<TeamLead> GetTeamLeads()
    {
        var teamLeads = new List<TeamLead>()
        {
            new TeamLead(1, "Филиппова Ульяна"),
            new TeamLead(2, "Николаев Григорий"),
            new TeamLead(3, "Андреева Вероника"),
            new TeamLead(4, "Коротков Михаил"),
            new TeamLead(5, "Кузнецов Александр"),
            new TeamLead(6, "Максимов Иван"),
            new TeamLead(7, "Павлова Мария"),
            new TeamLead(8, "Артемов Матвей"),
            new TeamLead(9, "Денисов Дмитрий"),
            new TeamLead(10, "Астафьев Андрей"),
            new TeamLead(11, "Демидов Дмитрий"),
            new TeamLead(12, "Климов Михаил"),
            new TeamLead(13, "Терехов Демид"),
            new TeamLead(14, "Полякова Мария"),
            new TeamLead(15, "Волков Мирослав"),
            new TeamLead(16, "Волкова Ольга"),
            new TeamLead(17, "Киреева Виктория"),
            new TeamLead(18, "Волков Артём"),
            new TeamLead(19, "Панов Максим"),
            new TeamLead(20, "Комаров Макар"),
        };
        return teamLeads;
    }

    public static List<Team> GetTeams()
    {
        var teams = new List<Team>()
        {
            new Team(new Junior(1, "Юдин Адам"), new TeamLead(1, "Филиппова Ульяна")),
            new Team(new Junior(2, "Яшина Яна"), new TeamLead(2, "Николаев Григорий")),
            new Team(new Junior(3, "Никитина Вероника"), new TeamLead(3, "Андреева Вероника")),
            new Team(new Junior(4, "Рябинин Александр"), new TeamLead(4, "Коротков Михаил")),
            new Team(new Junior(5, "Ильин Тимофей"), new TeamLead(5, "Кузнецов Александр")),
        };
        return teams;
    }

    public static List<Team> GetTeamsFull()
    {
        var juniors = new List<Junior>
        {
            new Junior(1, "Юдин Адам"), new Junior(2, "Яшина Яна"), new Junior(3, "Никитина Вероника"),
            new Junior(4, "Рябинин Александр"), new Junior(5, "Ильин Тимофей")
        };
        var teamLeads = new List<TeamLead>
        {
            new TeamLead(1, "Филиппова Ульяна"), new TeamLead(2, "Николаев Григорий"),
            new TeamLead(3, "Андреева Вероника"), new TeamLead(4, "Коротков Михаил"),
            new TeamLead(5, "Кузнецов Александр")
        };
        var juniorsWishlist = new Wishlist(teamLeads.Cast<Employee>().ToList());
        var teamLeadsWishlist = new Wishlist(juniors.Cast<Employee>().ToList());
        juniors.ForEach(j => j.Wishlist = juniorsWishlist);
        teamLeads.ForEach(t => t.Wishlist = teamLeadsWishlist);
        var teams = new List<Team>();
        for (int i = 0; i < juniors.Count; i++)
        {
            teams.Add(new Team(juniors[i], teamLeads[i]));
        }

        return teams;
    }
}