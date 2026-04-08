using System.Text.Json.Serialization;
using System.Text.Json;

namespace HubCinemaAdmin.Models
{
    public class CustomSeatLayoutViewModel
    {
        public int IdRoom { get; set; }

        public string LayoutJson { get; set; }

        public List<List<string?>> ParsedLayout
        {
            get
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<List<string?>>>(LayoutJson ?? "[]")
                       ?? new List<List<string?>>();
            }
        }

        public int MaRap { get; set; }

        public List<SeatTypeDto> SeatTypeRows { get; set; } = new();
        public bool HasExistingLayout { get; set; } = false;
        public string? LayoutMatrix { get; set; }
    }

}
