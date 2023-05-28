namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_User
    {
        public int ID { get; set; }

        [StringLength(500)]
        public string FirstName { get; set; }

        [StringLength(500)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string EmailID { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(4000)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Username { get; set; }

        [StringLength(500)]
        public string Password { get; set; }

        public int? RoleID { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsAdmin { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }

        public bool? IsSuperAdmin { get; set; }

        [StringLength(100)]
        public string RoleName { get; set; }
        public string Remarks { get; set; }
        public bool? Admin { get; set; }
        public bool? VendorLogin { get; set; }

    }
}
