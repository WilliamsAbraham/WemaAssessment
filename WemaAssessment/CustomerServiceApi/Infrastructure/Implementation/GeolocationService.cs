using CustomerServiceApi.Core.Application.Features.Geolocations;
using CustomerServiceApi.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NigerianStates;

namespace CustomerServiceApi.Infrastructure.Implementation
{
    public class GeolocationService : IGeolocation
    {
        private readonly ILogger<GeolocationService> _logger;
        public GeolocationService(ILogger<GeolocationService> logger)
        {
            _logger = logger;
        }
        public IEnumerable<LGADto> GetLGAs(int stateId)
        {
            try
            {
                var Lga = Nigeria.GetLgasByState(stateId);
                return Lga.Select(lga => new LGADto
                {
                    Name = lga.Name,
                    StateId = stateId
                });
            }

            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception("An internal error has occured");
            }

        }

        public IEnumerable<StateDto> GetStates()
        {
           try
            {
                var states = Nigeria.GetStates();

                return states.Select(state => new StateDto
                {
                    Id = state.Id,
                    Name = state.Name
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception("An internal error has occured");
            }
        }
    }
}
