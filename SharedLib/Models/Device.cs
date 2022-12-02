using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Models
{
    public class Device
    {
        [Required]
        public string SerialNumber { get; set; }

        [Required]
        public string DeviceReferenceId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
