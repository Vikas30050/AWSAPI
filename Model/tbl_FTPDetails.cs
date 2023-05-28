namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_FTPDetails
    {
        public int ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Username { get; set; }

        [Required]
        [StringLength(1000)]
        public string Password { get; set; }

        [Required]
        public string FTPUrl { get; set; }

        [Required]
        public string ActualPath { get; set; }

        [Required]
        public string BackupPath { get; set; }

        public DateTime ScheduleAt { get; set; }

        [StringLength(400)]
        public string Duration { get; set; }

        public bool? IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
