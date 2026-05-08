using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Queries.GetJourneyBySlug;

public record GetJourneyBySlugQuery(string Username, string Slug) : IQuery<Result<JourneyDto>>;
