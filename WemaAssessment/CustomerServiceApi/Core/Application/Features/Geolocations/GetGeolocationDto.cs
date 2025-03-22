namespace CustomerServiceApi.Core.Application.Features.Geolocations
{
    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LGADto
    {
        public int StateId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
