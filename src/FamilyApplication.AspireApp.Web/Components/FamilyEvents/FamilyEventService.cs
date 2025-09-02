using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Eiriklb.Utils;
using FamilyApplication.AspireApp.Web.CosmosDb.Family;
using FamilyApplication.AspireApp.Web.CosmosDb.User;
using FamilyApplication.AspireApp.Web.Databuffer;
using FamilyApplication.AspireApp.Web.Sessions;
namespace FamilyApplication.AspireApp.Web.Components.FamilyEvents;

public class FamilyEventService : IDisposable
{
    private readonly UserDto _userDto;
    private readonly FamilyDto? _familyDto;
    private readonly GlobalVm _globalVm;
    private readonly SessionManager _sessionManager;
    public event EventHandler? SomethingHasChanged;
    public event EventHandler? TreNesteHasChanged;
    public ObservableCollection<FamilyEvent> ObsColl = new();
    public ObservableCollection<FamilyEvent> ObsColl3Neste = new();

    public FamilyEventService(GlobalVm vm, SessionManager sessionManager)
    {
        _userDto = sessionManager.GetMyUserDto();
        _sessionManager = sessionManager;
        _globalVm = vm;
        _familyDto = _globalVm.FamilyDtos.FirstOrDefault(a => a.FamilyId == _userDto.FamilyId);

        if( _familyDto != null )
            _familyDto.FamilieEvents.CollectionChanged += FamilieEvents_CollectionChanged;

        Refresh();
    }

    private Guid delayGuidCollectionChanged;
  
    private async void FamilieEvents_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        var tmpGuid = Guid.NewGuid();
        delayGuidCollectionChanged = tmpGuid;
        await Task.Delay(50);

        if (delayGuidCollectionChanged != tmpGuid)
            return;
        Refresh();
    }

    private void Refresh()
    {

        if (_familyDto == null)
            return;

        RefreshBirthdays(_familyDto);
       

        var nye = from myrows in _familyDto.FamilieEvents
                  join finnes in ObsColl on myrows.Id equals finnes.Id
                  into joined
                  from j in joined.DefaultIfEmpty()
                  where j == null
                  select myrows;

        var fjernes = (from finnes in ObsColl
                      join ny in _familyDto.FamilieEvents on finnes.Id equals ny.Id
                      into joined
                      from j in joined.DefaultIfEmpty()
                      where j == null
                      select finnes).ToArray();

        var endret = false;

        foreach(var ny in nye)
        {
            endret = true;
            ObsColl.Add(ny);
            ny.PropertyChanged += familyEvent_PropertyChanged;
        }

        foreach(var fjernet in fjernes)
        {
            endret = true;
            ObsColl.Remove(fjernet);
            fjernet.PropertyChanged -= familyEvent_PropertyChanged;
        }

        if (endret)
        {
            var sortedEvents = ObsColl.OrderBy(e => e.Date).ThenBy(a=>a.Time).ToList();
            ObsColl.Clear();
            foreach (var evt in sortedEvents)
            {
                ObsColl.Add(evt);
            }
            SomethingHasChanged?.Invoke(this, new EventArgs());
        }

        Refresh3Neste();
    }

    private void Refresh3Neste()
    {

        bool trenesteChanged = false;
        var skalVære = (from myrows in ObsColl
                        where myrows.Date >= DateTime.Today.Date
                        select myrows).Take(3);

        var fjernes = (from myrows in ObsColl3Neste
                      join skal in skalVære on myrows.Id equals skal.Id
                      into joined
                      from j in joined.DefaultIfEmpty()
                      where j == null
                      select myrows).ToArray();

        var leggesTil = from myrows in skalVære
                        join finnes in ObsColl3Neste on myrows.Id equals finnes.Id
                        into joined
                        from j in joined.DefaultIfEmpty()
                        where j == null
                        select myrows;

        foreach (var rad in fjernes)
        {
            trenesteChanged = true;
            ObsColl3Neste.Remove(rad);
        }
        foreach (var rad in leggesTil)
        {
            trenesteChanged = true;
            ObsColl3Neste.SortedInsert(rad, a => a.Date, a => a.Time);
        }

        if(trenesteChanged)
            TreNesteHasChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshBirthdays(FamilyDto familtyDto)
    {
        var birthDays = from myrows in _globalVm.UserDtos
                        select new FamilyEvent()
                        {
                            Date = new DateTime(DateTime.Now.Year,myrows.BirthDate.Month,myrows.BirthDate.Day),
                            Description = "",
                            EndDate = new DateTime(DateTime.Now.Year, myrows.BirthDate.Month, myrows.BirthDate.Day),
                            Time = "",
                            Title = $" blir {DateTime.Now.Year - myrows.BirthDate.Year} år, hipp hipp hurra!",
                            Type = FamilieEventType.Bursdag,
                            Tag = myrows.Id
                        };

        var finnes = from birthDay in birthDays
                     join exists in familtyDto.FamilieEvents on new
                     {
                         birthDay.Date,
                         userId = birthDay.Tag
                     }
                     equals new
                     {
                         exists.Date,
                         userId = exists.Tag
                     }
                     into joined
                     from j in joined.DefaultIfEmpty()
                     where j == null
                     select birthDay;

        foreach (var rad in finnes)
            familtyDto.FamilieEvents.Add(rad);

    }

    private Guid delayGuidPropertyChanged;
    private async void familyEvent_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var tmpGuid = Guid.NewGuid();
        delayGuidCollectionChanged = tmpGuid;
        await Task.Delay(50);

        if (tmpGuid != delayGuidCollectionChanged)
            return;

        SomethingHasChanged?.Invoke(this, new EventArgs());


    }

    public List<FamilyEvent> GetMockEvents()
    {
        return new List<FamilyEvent>
        {
            // Birthday: A family member's birthday with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 3, 27),
                EndDate = new DateTime(2025, 3, 27),
                Title = "Grandma's Birthday",
                Time = "3:00 PM",
                Description = "We’re going to Grandma’s house at 123 Maple Street. We’ll meet Grandma, Grandpa, Uncle Tim, and Aunt Sarah. We’re having a chocolate cake with vanilla ice cream, and we’ll play board games after singing 'Happy Birthday'.",
                Type = FamilieEventType.Bursdag
            },

            // ExternalBirthday: A friend's birthday with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 4, 5),
                EndDate = new DateTime(2025, 4, 5),
                Title = "John’s Birthday Party",
                Time = "6:00 PM",
                Description = "We’re going to the park at 456 Oak Avenue for John’s birthday. We’ll meet John, his parents, and his sister Emily. They’re serving pizza, lemonade, and a vanilla cake with sprinkles. There will be a piñata and outdoor games.",
                Type = FamilieEventType.EksternBursdag
            },

            // Vacation: A family vacation with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 4, 18),
                EndDate = new DateTime(2025, 4, 20),
                Title = "Easter Trip",
                Time = "All Day",
                Description = "We’re going to the beach in Sunnyville, staying at the Seaside Hotel. We’ll meet our cousins, Lily and Max, and their parents. On the first day, we’ll have sandwiches and fruit for lunch, and grilled fish with salad for dinner. We’ll spend time swimming, building sandcastles, and walking along the shore.",
                Type = FamilieEventType.Ferie
            },

            // HomeEvent: An event at home with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 3, 25),
                EndDate = new DateTime(2025, 3, 25),
                Title = "Movie Night",
                Time = "6:00 PM",
                Description = "We’re staying at home in the living room. It’ll just be our family: you, me, Mom, and your brother Jake. We’re watching 'The Incredibles' on the big TV. We’ll have popcorn, apple slices, and juice to eat while we watch. After the movie, we’ll play a short game of Uno.",
                Type = FamilieEventType.HjemmeEvent
            },

            // OutsideEvent: An event outside the home with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 3, 29),
                EndDate = new DateTime(2025, 3, 29),
                Title = "Soccer Practice",
                Time = "4:00 PM",
                Description = "We’re going to the soccer field at Green Park, 789 Pine Road. You’ll meet your team, including your friends Emma and Noah, and Coach Mike. We’ll bring water and granola bars for a snack. Practice will include running drills and a short game, and we’ll be done by 5:30 PM.",
                Type = FamilieEventType.UteEvent
            },

            // SchoolEvent: A school-related event with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 5, 10),
                EndDate = new DateTime(2025, 5, 10),
                Title = "School Play",
                Time = "2:00 PM",
                Description = "We’re going to your school auditorium at Lincoln Elementary. You’ll be performing in the spring play with your classmates, including Sophie and Liam, and your teacher Ms. Carter will be there. We’ll bring a small snack of crackers and cheese for after the play. The play is about animals in the forest, and you’re playing a bunny.",
                Type = FamilieEventType.SkoleEvent
            },

            // HolidayCelebration: A major holiday event with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 12, 25),
                EndDate = new DateTime(2025, 12, 25),
                Title = "Christmas Day",
                Time = "All Day",
                Description = "We’re celebrating at home with a family gathering. Grandma, Grandpa, Uncle Tim, Aunt Sarah, and your cousins Lily and Max will come over. We’ll have a big lunch with roast turkey, mashed potatoes, green beans, and pumpkin pie for dessert. After lunch, we’ll open presents near the Christmas tree and sing carols.",
                Type = FamilieEventType.HelligDag
            },

            // MedicalAppointment: A medical or dental appointment with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 6, 15),
                EndDate = new DateTime(2025, 6, 15),
                Title = "Dentist Appointment",
                Time = "10:00 AM",
                Description = "We’re going to Dr. Smith’s dental office at 321 Elm Street. You’ll meet Dr. Smith and her assistant, Lisa. They’ll check your teeth, and you’ll get a new toothbrush. We’ll bring your favorite stuffed bunny to hold during the visit. After, we’ll stop for a treat—maybe a strawberry smoothie.",
                Type = FamilieEventType.LegeAvtale
            },

            // WorkEvent: A work-related event with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 7, 20),
                EndDate = new DateTime(2025, 7, 20),
                Title = "Company Family Picnic",
                Time = "12:00 PM",
                Description = "We’re going to the company picnic at Central Park. You’ll meet my coworkers, including my friend Anna and her son Ben, who’s your age. They’ll have burgers, hot dogs, chips, and lemonade for lunch, and there will be games like sack races and a treasure hunt for kids.",
                Type = FamilieEventType.ArbeidsEvent
            },

            // Anniversary: A family anniversary with detailed description
            new FamilyEvent
            {
                Date = new DateTime(2025, 8, 30),
                EndDate = new DateTime(2025, 8, 30),
                Title = "Mom and Dad’s Anniversary",
                Time = "6:30 PM",
                Description = "We’re going to La Bella Restaurant at 654 Cedar Lane to celebrate Mom and Dad’s anniversary. It’ll just be our family: you, me, Mom, and Jake. We’ll have pasta with marinara sauce, garlic bread, and chocolate cake for dessert. After dinner, we’ll take a family photo and go for a short walk in the nearby garden.",
                Type = FamilieEventType.Jubileum
            }
        };
    }

    public void Dispose()
    {
        foreach (var rad in ObsColl)
            rad.PropertyChanged -= familyEvent_PropertyChanged;
    }
}