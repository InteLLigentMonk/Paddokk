using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PaddokkDbContext context, ILogger logger)
    {
        if (await context.Users.CountAsync() >= 5)
        {
            logger.LogInformation("Database already has seed data — skipping seed");
            return;
        }

        Randomizer.Seed = new Random(42);

        var makes = await context.CarMakes.Include(m => m.Models).ThenInclude(m => m.Generations).ToListAsync();

        var users = SeedUsers();
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} users", users.Count);

        var cars = SeedCars(users, makes);
        await context.UserCars.AddRangeAsync(cars);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} user cars", cars.Count);

        var journeys = SeedJourneys(users, cars);
        await context.Journeys.AddRangeAsync(journeys);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} journeys", journeys.Count);

        var posts = SeedPosts(journeys);
        await context.JourneyPosts.AddRangeAsync(posts);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} journey posts", posts.Count);

        var comments = SeedComments(posts, users);
        await context.PostComments.AddRangeAsync(comments);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} comments", comments.Count);

        var (journeyLikes, journeySubscriptions) = SeedJourneyEngagement(journeys, users);
        await context.JourneyLikes.AddRangeAsync(journeyLikes);
        await context.JourneySubscriptions.AddRangeAsync(journeySubscriptions);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Likes} journey likes and {Subs} journey subscriptions",
            journeyLikes.Count, journeySubscriptions.Count);

        var (carLikes, carSubscriptions) = SeedCarEngagement(cars, users);
        await context.UserCarLikes.AddRangeAsync(carLikes);
        await context.UserCarSubscriptions.AddRangeAsync(carSubscriptions);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Likes} car likes and {Subs} car subscriptions",
            carLikes.Count, carSubscriptions.Count);

        logger.LogInformation("Database seeding complete");
    }

    private static List<ApplicationUser> SeedUsers()
    {
        var faker = new Faker<ApplicationUser>("en")
            .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.DisplayName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.DisplayName))
            .RuleFor(u => u.Bio, f => f.PickRandom(
                "JDM enthusiast. Track days on weekends.",
                "Building my R33 one part at a time.",
                "Daily driver turned weekend warrior.",
                "Ex-dealership tech, now wrenching for fun.",
                "Drift missile pilot in training.",
                "Restoration junkie — mostly Toyotas.",
                "Photographer and car meet organizer.",
                "Budget builds only. Making it work.",
                "Turbo everything.",
                "Chasing power-to-weight ratio perfection."
            ))
            .RuleFor(u => u.AvatarUrl, f => $"https://picsum.photos/seed/{f.Random.AlphaNumeric(6)}/200/200")
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt)
            .RuleFor(u => u.SubscriptionTier, f => f.PickRandom(
                SubscriptionTier.Free, SubscriptionTier.Free, SubscriptionTier.Free,
                SubscriptionTier.Silver, SubscriptionTier.Gold))
            .RuleFor(u => u.Role, _ => Role.User);

        return faker.Generate(10);
    }

    private static List<UserCar> SeedCars(List<ApplicationUser> users, List<CarMake> makes)
    {
        var faker = new Faker("en");
        var cars = new List<UserCar>();

        var colors = new[] { "Pearl White", "Midnight Black", "Gunmetal Grey", "Sunrise Red", "Deep Blue", "Forest Green", "Orange Peel", "Silver", "Titanium", "Matte Black" };
        var customBuilds = new[]
        {
            ("Frankenbike", "Full tube-frame custom. Nothing stock remains."),
            ("Project Midnight", "1JZ-swapped AE86 shell. Still figuring out the gearbox."),
            ("Track Rat", "Stripped and caged daily driver turned circuit car."),
            ("The Tank", "Off-road prepped Hilux. Lockers front and rear."),
            ("Widebody Project", "S14 with custom carbon wide-body kit.")
        };

        foreach (var user in users)
        {
            var carCount = faker.Random.Int(2, 4);
            var isFirst = true;

            for (var i = 0; i < carCount; i++)
            {
                var useCustomBuild = faker.Random.Bool(0.2f);

                UserCar car;
                if (useCustomBuild)
                {
                    var build = faker.PickRandom(customBuilds);
                    car = new UserCar
                    {
                        PrincipalId = user.Id,
                        IsCustomBuild = true,
                        CustomBuildName = build.Item1,
                        Description = build.Item2,
                        Nickname = faker.PickRandom("The Beast", "Project X", "Daily Hack", "The Sleeper", null),
                        Color = faker.PickRandom(colors),
                        PrimaryImageUrl = $"https://picsum.photos/seed/{faker.Random.AlphaNumeric(8)}/800/600",
                        IsPrimary = isFirst,
                        IsActive = true,
                        CreatedAt = faker.Date.Past(2).ToUniversalTime(),
                        UpdatedAt = faker.Date.Recent(90).ToUniversalTime()
                    };
                }
                else
                {
                    var make = faker.PickRandom(makes);
                    var model = make.Models.Any() ? faker.PickRandom(make.Models) : null;
                    var generation = model?.Generations.Any() == true
                        ? faker.PickRandom(model.Generations)
                        : null;

                    car = new UserCar
                    {
                        PrincipalId = user.Id,
                        CarMakeId = make.Id,
                        CarModelId = model?.Id,
                        CarGenerationId = generation?.Id,
                        Year = generation != null
                            ? faker.Random.Int(generation.StartYear, generation.EndYear ?? 2024)
                            : faker.Random.Int(1990, 2023),
                        IsCustomBuild = false,
                        Nickname = faker.PickRandom("DD", "Track Toy", "Project Car", "The Daily", "Beater", null),
                        Color = faker.PickRandom(colors),
                        Description = faker.PickRandom(
                            "Stage 2 tune, full exhaust, coilovers.",
                            "Mostly stock, keeping it clean.",
                            "Long-term build project. Slowly adding mods.",
                            "Just bought it, assessing condition.",
                            "Fully built motor. All OEM+ everything else.",
                            "Weekend car only. Pampered and babied."
                        ),
                        PrimaryImageUrl = $"https://picsum.photos/seed/{faker.Random.AlphaNumeric(8)}/800/600",
                        IsPrimary = isFirst,
                        IsActive = true,
                        CreatedAt = faker.Date.Past(2).ToUniversalTime(),
                        UpdatedAt = faker.Date.Recent(90).ToUniversalTime()
                    };
                }

                cars.Add(car);
                isFirst = false;
            }
        }

        return cars;
    }

    private static List<Journey> SeedJourneys(List<ApplicationUser> users, List<UserCar> cars)
    {
        var faker = new Faker("en");
        var journeys = new List<Journey>();

        var titles = new[]
        {
            "Full Engine Rebuild", "Suspension Overhaul", "Track Prep Season 2",
            "The Restoration Diaries", "Budget Boost Build", "Winter Sleeper Project",
            "Daily Mod Log", "Drift Build Chronicles", "Road Trip Prep", "Club Meet Season",
            "From Rust to Road", "Turbo Install", "Interior Refresh", "Brake Upgrade Project",
            "Widebody Dreams", "The Reliability Build", "Season Opener Prep", "Endurance Build",
            "First Car Journey", "The Collection"
        };

        var descriptions = new[]
        {
            "Documenting every step of this build. Nothing hidden, including the mistakes.",
            "Following along as I figure this out. Advice welcome.",
            "Budget-conscious build. Trying to do it right without breaking the bank.",
            "Long-term project. No deadline, just doing it when time and money allow.",
            "Race-spec build for the upcoming track season. All receipts included.",
            "Bringing a barn find back to life. It's worse than expected.",
            "Pushing the platform as far as I can on a realistic budget.",
            "Mostly maintenance and small upgrades. Keeping it honest."
        };

        var statuses = new[]
        {
            JourneyStatus.Active, JourneyStatus.Active, JourneyStatus.Active,
            JourneyStatus.Completed, JourneyStatus.Active
        };

        foreach (var user in users)
        {
            var userCars = cars.Where(c => c.PrincipalId == user.Id).ToList();
            if (!userCars.Any()) continue;

            var journeyCount = faker.Random.Int(1, 3);
            for (var i = 0; i < journeyCount; i++)
            {
                var car = faker.PickRandom(userCars);
                var status = faker.PickRandom(statuses);
                var createdAt = faker.Date.Past(1).ToUniversalTime();

                journeys.Add(new Journey
                {
                    Title = faker.PickRandom(titles),
                    Description = faker.PickRandom(descriptions),
                    Category = faker.PickRandom(
                        JourneyCategory.BuildAndMods, JourneyCategory.Restoration,
                        JourneyCategory.Racing, JourneyCategory.Adventures, JourneyCategory.Ownership),
                    Status = status,
                    PrincipalId = user.Id,
                    UserCarId = car.Id,
                    CreatedAt = createdAt,
                    UpdatedAt = faker.Date.Between(createdAt, DateTime.UtcNow).ToUniversalTime(),
                    CompletedAt = status == JourneyStatus.Completed
                        ? faker.Date.Recent(60).ToUniversalTime()
                        : null
                });
            }
        }

        return journeys;
    }

    private static List<JourneyPost> SeedPosts(List<Journey> journeys)
    {
        var faker = new Faker("en");
        var posts = new List<JourneyPost>();

        var postContents = new[]
        {
            "Ordered the parts. Should arrive by end of week. Already regretting the shipping cost.",
            "Got everything laid out. Here's the plan before I start tearing things apart.",
            "Day one. Pulled the engine. It's in worse shape than expected but nothing catastrophic.",
            "Cleaned up the bay while everything is out. Found some surprises behind the firewall.",
            "New parts arrived. The quality actually looks solid this time.",
            "Ran into an issue with the fitment. Spent three hours getting it sorted. Finally works.",
            "First startup after the rebuild. A few leaks to chase down but it runs.",
            "Test drive done. Feels completely different. Still dialing it in.",
            "Took it to the track for the first shakedown. No issues in three sessions.",
            "Small update — addressed the oil weep from the valve cover. New gasket, done.",
            "Corner balance appointment tomorrow. Excited to see where the numbers land.",
            "Alignment done. Car tracks straight for the first time in years.",
            "Ordered the last few bits to finish this chapter. Should be done next month.",
            "Had to pull it back apart. Missed something obvious. Lesson learned.",
            "Wrapped up the exterior work. Paint correction made a bigger difference than expected.",
            "Installed the new coilovers. Initial impression is very positive.",
            "Dyno session today. Numbers came in above target. Pleased with the result.",
            "Finished the wiring for the new gauges. Clean install took way longer than expected.",
            "Brake bedding complete. Pedal feel is night and day compared to before.",
            "Took it to the Sunday meet. Got great feedback from people who know this platform."
        };

        foreach (var journey in journeys)
        {
            var postCount = faker.Random.Int(3, 8);
            var postDate = journey.CreatedAt.AddDays(faker.Random.Int(1, 5));

            for (var i = 0; i < postCount; i++)
            {
                posts.Add(new JourneyPost
                {
                    JourneyId = journey.Id,
                    AuthorId = journey.PrincipalId,
                    TextContent = faker.PickRandom(postContents),
                    CreatedAt = postDate.ToUniversalTime(),
                    UpdatedAt = postDate.ToUniversalTime(),
                    IsEdited = false
                });

                postDate = postDate.AddDays(faker.Random.Int(3, 21));
            }
        }

        return posts;
    }

    private static List<PostComment> SeedComments(List<JourneyPost> posts, List<ApplicationUser> users)
    {
        var faker = new Faker("en");
        var comments = new List<PostComment>();

        var commentContents = new[]
        {
            "Solid progress. Following this closely.",
            "Same issue here on mine. Fixed it by shimming the bracket.",
            "What fluid are you running in the diff?",
            "That fitment issue is common on this generation. Good fix.",
            "Numbers look good. What's the power goal?",
            "I did mine differently but your approach makes sense too.",
            "Subscribed. This is exactly the kind of build I want to do.",
            "How are the temperatures under hard driving?",
            "Did you notice any change in NVH after that?",
            "Great documentation. Most people skip the detail you're including.",
            "That part is notorious for going at this mileage. Smart to address it now.",
            "Which shop did the alignment? Always looking for recommendations.",
            "Impressive numbers for the platform. What's the supporting mods list?",
            "Been watching this since the start. Coming along nicely.",
            "That's a clean engine bay. Did you detail it or is it just tidy from maintenance?",
            "How's the daily-ability after all these changes?",
            "I had the same part fail on mine. Different experience with the replacement.",
            "Good call going overkill on that. Better than coming back to it later.",
            "Following for the track updates.",
            "What tyres are you running currently?"
        };

        foreach (var post in posts)
        {
            var commentCount = faker.Random.Int(0, 5);
            var otherUsers = users.Where(u => u.Id != post.AuthorId).ToList();
            if (!otherUsers.Any()) continue;

            for (var i = 0; i < commentCount; i++)
            {
                var commenter = faker.PickRandom(otherUsers);
                comments.Add(new PostComment
                {
                    JourneyPostId = post.Id,
                    AuthorId = commenter.Id,
                    Content = faker.PickRandom(commentContents),
                    CreatedAt = post.CreatedAt.AddHours(faker.Random.Int(1, 72)).ToUniversalTime(),
                    UpdatedAt = post.CreatedAt.AddHours(faker.Random.Int(1, 72)).ToUniversalTime(),
                    IsEdited = false
                });
            }
        }

        return comments;
    }

    private static (List<JourneyLike> likes, List<JourneySubscription> subscriptions) SeedJourneyEngagement(
        List<Journey> journeys, List<ApplicationUser> users)
    {
        var faker = new Faker("en");
        var likes = new List<JourneyLike>();
        var subscriptions = new List<JourneySubscription>();

        foreach (var journey in journeys)
        {
            var otherUsers = users.Where(u => u.Id != journey.PrincipalId).ToList();
            var likerCount = faker.Random.Int(0, otherUsers.Count);
            var subCount = faker.Random.Int(0, otherUsers.Count);

            var likers = faker.Random.ListItems(otherUsers, likerCount);
            foreach (var user in likers)
            {
                likes.Add(new JourneyLike
                {
                    UserId = user.Id,
                    JourneyId = journey.Id,
                    CreatedAt = faker.Date.Between(journey.CreatedAt, DateTime.UtcNow).ToUniversalTime()
                });
            }

            var subscribers = faker.Random.ListItems(otherUsers, subCount);
            foreach (var user in subscribers)
            {
                if (subscriptions.Any(s => s.UserId == user.Id && s.JourneyId == journey.Id))
                    continue;

                subscriptions.Add(new JourneySubscription
                {
                    UserId = user.Id,
                    JourneyId = journey.Id,
                    CreatedAt = faker.Date.Between(journey.CreatedAt, DateTime.UtcNow).ToUniversalTime(),
                    IsActive = true
                });
            }
        }

        return (likes, subscriptions);
    }

    private static (List<UserCarLike> likes, List<UserCarSubscription> subscriptions) SeedCarEngagement(
        List<UserCar> cars, List<ApplicationUser> users)
    {
        var faker = new Faker("en");
        var likes = new List<UserCarLike>();
        var subscriptions = new List<UserCarSubscription>();

        foreach (var car in cars)
        {
            var otherUsers = users.Where(u => u.Id != car.PrincipalId).ToList();
            var likerCount = faker.Random.Int(0, otherUsers.Count);
            var subCount = faker.Random.Int(0, Math.Min(3, otherUsers.Count));

            var likers = faker.Random.ListItems(otherUsers, likerCount);
            foreach (var user in likers)
            {
                if (likes.Any(l => l.UserId == user.Id && l.UserCarId == car.Id))
                    continue;

                likes.Add(new UserCarLike
                {
                    UserId = user.Id,
                    UserCarId = car.Id,
                    CreatedAt = faker.Date.Recent(180).ToUniversalTime()
                });
            }

            var subscribers = faker.Random.ListItems(otherUsers, subCount);
            foreach (var user in subscribers)
            {
                if (subscriptions.Any(s => s.UserId == user.Id && s.UserCarId == car.Id))
                    continue;

                subscriptions.Add(new UserCarSubscription
                {
                    UserId = user.Id,
                    UserCarId = car.Id,
                    CreatedAt = faker.Date.Recent(180).ToUniversalTime(),
                    IsActive = true
                });
            }
        }

        return (likes, subscriptions);
    }
}
