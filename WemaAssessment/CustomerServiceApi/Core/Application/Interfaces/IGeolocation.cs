using CustomerServiceApi.Core.Application.Features.Geolocations;

namespace CustomerServiceApi.Core.Application.Interfaces
{
    public interface IGeolocation
    {
        IEnumerable<StateDto> GetStates();
        IEnumerable<LGADto> GetLGAs(int stateId);
    }
}
