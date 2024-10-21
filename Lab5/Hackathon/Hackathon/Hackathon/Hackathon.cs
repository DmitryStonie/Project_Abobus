using Hackathon.Database.SQLite;
using Hackathon.DataProviders;
using Microsoft.EntityFrameworkCore;

namespace Hackathon
{
    public class Hackathon
    {
        readonly HrDirector _hrDirector;
        readonly HrManager _hrManager;
        readonly List<TeamLead> _teamLeads;
        readonly List<Junior> _juniors;
        readonly IWishListGenerator _wishListGenerator;
        private readonly IDataLoadingInterface _dataLoader;
        private readonly IDataSavingInterface? _dataSaver;

        public List<Team> Teams { get; private set; }
        public int Id { get; init; }
        public double HarmonicMean { get; set; }

        public Hackathon()
        {
            Id = 0;
            HarmonicMean = 0.0;
        }
        public Hackathon(double harmonicMean, int id)
        {
            HarmonicMean = harmonicMean;
            Id = id;
        }

        public Hackathon(List<Team> teams, IDataSavingInterface dataSaver)
        {
            HarmonicMean = 0;
            Id = 0;
            Teams = teams;
            _juniors = new List<Junior>();
            _teamLeads = new List<TeamLead>();
            _hrDirector = new HrDirector();
            foreach (var team in teams)
            {
                team.Junior.Wishlist.InitWishlist();
                team.TeamLead.Wishlist.InitWishlist();
            }

            foreach (var team in teams)
            {
                _juniors.Add(team.Junior);
                _teamLeads.Add(team.TeamLead);
            }

            _dataSaver = dataSaver;
        }

        public Hackathon(int employeesCount, ITeamBuildingStrategy teamBuildingStrategy,
            IWishListGenerator wishListGenerator,
            IDataLoadingInterface dataLoader, IDataSavingInterface? dataSaver = null)
        {
            _dataLoader = dataLoader;
            _dataSaver = dataSaver;
            _juniors = _dataLoader.LoadJuniors();
            _teamLeads = _dataLoader.LoadTeamLeads();
            if (_juniors.Count != employeesCount || _teamLeads.Count != employeesCount)
            {
                throw new IncorrectEmployeesDataException("Invalid number of employees");
            }

            _hrDirector = new HrDirector();
            _hrManager = new HrManager(_juniors, _teamLeads, teamBuildingStrategy);
            _wishListGenerator = wishListGenerator;
            Teams = new List<Team>();
        }

        private void GeneratePreferences()
        {
            foreach (var junior in _juniors)
            {
                junior.Wishlist = new Wishlist(_wishListGenerator.CreateWishlist<TeamLead>(_teamLeads));
            }

            foreach (var teamLead in _teamLeads)
            {
                teamLead.Wishlist = new Wishlist(_wishListGenerator.CreateWishlist<Junior>(_juniors));
            }
        }

        public void Run()
        {
            GeneratePreferences();
            Teams = _hrManager.DistributeParticipants();
            HarmonicMean = _hrDirector.CountHarmonicMean(Teams);
            _dataSaver?.SaveData(_juniors, _teamLeads, Teams, this);
        }

        public void Complete()
        {
            HarmonicMean = _hrDirector.CountHarmonicMean(Teams);
            var newTeams = new List<Team>();
            foreach (var team in Teams)
            {
                team.Id = 0;
                team.TeamLead.Id = 0;
                team.Junior.Id = 0;
                team.Junior.Wishlist.Id = 0;
                team.TeamLead.Wishlist.Id = 0;
                foreach (var wish in team.Junior.Wishlist.Wishes)
                {
                    wish.Id = 0;
                }
                foreach (var wish in team.TeamLead.Wishlist.Wishes)
                {
                    wish.Id = 0;
                }
            }
            _dataSaver?.SaveData(_juniors, _teamLeads, Teams, this);
        }
    }
}