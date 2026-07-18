using Microsoft.EntityFrameworkCore;
using Domain.Entities.CourseAggregate;
using Domain.ValueObjects;
using Infrastructure.Persistence.WriteContext.Context;
using Infrastructure.Outbox;

namespace Infrastructure.Persistence.Seeding;

internal static class CourseSeeder
{
    public static async Task SeedAsync(
        WriteDbContext context,
        OutboxChannel outboxChannel,
        CancellationToken cancellationToken = default)
    {
        if (await context.Courses.AnyAsync(cancellationToken))
            return;

        var seedData = GetSeedData();
        var courses = new List<Course>();

        foreach (var data in seedData)
        {
            Money? price = null;

            if (data.PriceAmount is not null)
            {
                var moneyResult = Money.Create(data.PriceAmount.Value, data.Currency ?? "USD");
                if (moneyResult.Failed)
                    throw new InvalidOperationException(
                        $"Failed to create Money for course '{data.Title}': {moneyResult.Error}");

                price = moneyResult.Value;
            }

            var courseResult = Course.Create(
                data.Title,
                data.Description,
                data.DurationInMinutes,
                data.Type,
                price);

            if (courseResult.Failed)
                throw new InvalidOperationException(
                    $"Failed to create course '{data.Title}': {courseResult.Error}");

            courses.Add(courseResult.Value);
        }

        outboxChannel.Notify(); // Notify the OutboxBackgroundService that there are new messages to process

        await context.Courses.AddRangeAsync(courses, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<CourseSeedDto> GetSeedData() =>
    [
        // ---- Programming ----
        new("ASP.NET Core Web API Masterclass", "Build production-grade REST APIs with ASP.NET Core, EF Core, and Clean Architecture.", 720, CourseType.Programming, 49.99m, "USD"),
        new("C# Fundamentals for Beginners", "Learn C# syntax, OOP principles, and .NET basics from scratch.", 480, CourseType.Programming, null, null),
        new("Advanced React with TypeScript", "Deep dive into hooks, context, performance optimization, and type-safe components.", 600, CourseType.Programming, 39.99m, "USD"),
        new("Database Design with PostgreSQL", "Master relational modeling, indexing, and query optimization in PostgreSQL.", 540, CourseType.Programming, 34.99m, "USD"),
        new("Introduction to MongoDB", "Learn document modeling, aggregation pipelines, and read-model design with MongoDB.", 360, CourseType.Programming, 24.99m, "USD"),
        new("Microservices with Docker & Kubernetes", "Design, containerize, and orchestrate microservices at scale.", 780, CourseType.Programming, 59.99m, "USD"),
        new("Python for Data Engineers", "Build ETL pipelines and automate workflows using Python.", 600, CourseType.Programming, 29.99m, "USD"),
        new("Clean Architecture in .NET", "Structure maintainable .NET solutions using Clean Architecture and CQRS.", 450, CourseType.Programming, 44.99m, "USD"),
        new("JavaScript Algorithms and Data Structures", "Strengthen your problem-solving skills with core CS fundamentals in JS.", 660, CourseType.Programming, null, null),
        new("Building Payment Systems with Stripe", "Integrate PaymentIntents, webhooks, and idempotent payment flows securely.", 300, CourseType.Programming, 19.99m, "USD"),

        // ---- Design ----
        new("UI/UX Design Foundations", "Learn design thinking, wireframing, and prototyping with Figma.", 420, CourseType.Design, 29.99m, "USD"),
        new("Design Systems from Scratch", "Build scalable, reusable design systems for product teams.", 360, CourseType.Design, 34.99m, "USD"),
        new("Typography for Digital Products", "Understand type hierarchy, pairing, and readability in interfaces.", 240, CourseType.Design, null, null),
        new("Mobile App Design with Figma", "Design polished, accessible mobile interfaces end to end.", 390, CourseType.Design, 27.99m, "USD"),
        new("Color Theory for UI Designers", "Apply color psychology and accessible contrast in real projects.", 180, CourseType.Design, null, null),
        new("Motion Design for Interfaces", "Add purposeful animation and micro-interactions to your UI.", 300, CourseType.Design, 32.99m, "USD"),

        // ---- Marketing ----
        new("Digital Marketing Essentials", "Cover SEO, paid ads, email, and content marketing fundamentals.", 420, CourseType.Marketing, 24.99m, "USD"),
        new("Social Media Growth Strategy", "Grow and monetize an audience across Instagram, TikTok, and X.", 300, CourseType.Marketing, 19.99m, "USD"),
        new("SEO for E-commerce", "Rank product pages and drive organic traffic to online stores.", 360, CourseType.Marketing, 29.99m, "USD"),
        new("Copywriting That Converts", "Write high-converting landing pages, ads, and email sequences.", 270, CourseType.Marketing, null, null),
        new("Google Ads Masterclass", "Plan, launch, and optimize profitable Google Ads campaigns.", 330, CourseType.Marketing, 34.99m, "USD"),
        new("Content Marketing Strategy", "Build a content engine that compounds traffic and leads over time.", 300, CourseType.Marketing, 22.99m, "USD"),

        // ---- Business ----
        new("Startup Fundamentals", "Validate ideas, build MVPs, and find product-market fit.", 480, CourseType.Business, 29.99m, "USD"),
        new("Business Model Innovation", "Explore frameworks like the Business Model Canvas and Lean Startup.", 360, CourseType.Business, null, null),
        new("Financial Modeling for Founders", "Build investor-ready financial models and forecasts from scratch.", 420, CourseType.Business, 39.99m, "USD"),
        new("Negotiation Skills for Entrepreneurs", "Master negotiation tactics for deals, hiring, and partnerships.", 240, CourseType.Business, 19.99m, "USD"),
        new("Raising Your First Round", "Understand pitch decks, term sheets, and investor conversations.", 300, CourseType.Business, 34.99m, "USD"),
        new("Operations Research for Managers", "Apply decision trees, EOQ, and payoff tables to real business decisions.", 360, CourseType.Business, null, null),

        // ---- Personal Development ----
        new("Productivity Systems That Work", "Design a personal workflow using time-blocking and task triage.", 240, CourseType.PersonalDevelopment, null, null),
        new("Public Speaking with Confidence", "Structure talks and manage nerves for high-stakes presentations.", 300, CourseType.PersonalDevelopment, 19.99m, "USD"),
        new("Habit Building Fundamentals", "Use behavioral science to build habits that actually stick.", 210, CourseType.PersonalDevelopment, null, null),
        new("Critical Thinking and Decision Making", "Sharpen reasoning and avoid common cognitive biases.", 270, CourseType.PersonalDevelopment, 17.99m, "USD"),
        new("Career Growth for Junior Developers", "Navigate your first few years in tech deliberately and strategically.", 300, CourseType.PersonalDevelopment, null, null),

        // ---- Health and Fitness ----
        new("Strength Training for Beginners", "Learn safe form and progressive overload for major lifts.", 360, CourseType.HealthAndFitness, 24.99m, "USD"),
        new("Nutrition Fundamentals", "Understand macros, meal planning, and sustainable eating habits.", 300, CourseType.HealthAndFitness, null, null),
        new("Mobility and Flexibility Training", "Improve joint health and range of motion with daily routines.", 240, CourseType.HealthAndFitness, 14.99m, "USD"),
        new("Beginner's Guide to Running", "Build endurance safely from a 5K to your first 10K.", 270, CourseType.HealthAndFitness, null, null),
        new("Yoga for Stress Relief", "Practical yoga flows for relaxation and better sleep.", 210, CourseType.HealthAndFitness, 12.99m, "USD"),

        // ---- Music ----
        new("Guitar for Absolute Beginners", "Learn chords, strumming patterns, and your first songs.", 360, CourseType.Music, 19.99m, "USD"),
        new("Music Theory Essentials", "Understand scales, chords, and harmony for any instrument.", 300, CourseType.Music, null, null),
        new("Piano Fundamentals", "Build finger technique and read sheet music from scratch.", 360, CourseType.Music, 22.99m, "USD"),
        new("Music Production with Ableton", "Produce, mix, and master electronic tracks from start to finish.", 480, CourseType.Music, 39.99m, "USD"),
        new("Songwriting Basics", "Craft melodies, lyrics, and song structure that resonate.", 240, CourseType.Music, null, null),

        // ---- Photography ----
        new("Photography Fundamentals", "Master exposure, composition, and lighting with any camera.", 300, CourseType.Photography, 19.99m, "USD"),
        new("Portrait Photography Masterclass", "Direct subjects and light portraits like a professional.", 360, CourseType.Photography, 29.99m, "USD"),
        new("Photo Editing with Lightroom", "Develop a consistent editing workflow and personal style.", 270, CourseType.Photography, 17.99m, "USD"),
        new("Travel Photography Essentials", "Capture compelling travel stories on location, anywhere in the world.", 300, CourseType.Photography, null, null),

        // ---- Language Learning ----
        new("English for Professionals", "Improve business English writing, email, and meeting fluency.", 420, CourseType.LanguageLearning, 24.99m, "USD"),
        new("Conversational Spanish", "Build practical speaking skills for everyday conversations.", 360, CourseType.LanguageLearning, null, null),
        new("German for Beginners", "Learn grammar basics and everyday vocabulary from zero.", 390, CourseType.LanguageLearning, 22.99m, "USD"),

        // ---- Other ----
        new("Introduction to Chess Strategy", "Learn openings, tactics, and endgame principles.", 240, CourseType.Other, null, null),
        new("Creative Writing Workshop", "Develop voice, structure, and editing skills for short fiction.", 300, CourseType.Other, 17.99m, "USD"),
    ];

    private sealed record CourseSeedDto(
        string Title,
        string Description,
        int DurationInMinutes,
        CourseType Type,
        decimal? PriceAmount,
        string? Currency);
}