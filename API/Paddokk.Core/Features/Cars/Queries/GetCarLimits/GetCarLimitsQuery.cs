using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Queries.GetCarLimits;

public record GetCarLimitsQuery(SubscriptionTier SubscriptionTier) : IQuery<CarLimitDto>;
