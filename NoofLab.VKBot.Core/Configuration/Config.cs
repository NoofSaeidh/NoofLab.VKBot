using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace NoofLab.VKBot.Core.Configuration
{
    public class Config
    {
        [Required]
        [DebuggerHidden]
        public string AccessToken { get; init; }
        [Required]
        public ulong GroupId { get; init; }
    }
}
