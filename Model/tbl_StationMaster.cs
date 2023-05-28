namespace AWSAPI.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_StationMaster
    {
        public int ID { get; set; }

       
        public string Name { get; set; }

    
        public string Latitude { get; set; }

     
        public string Longitude { get; set; }

      
        public string City { get; set; }

     
        public string District { get; set; }

    
        public string State { get; set; }

     
        public string TehsilTaluk { get; set; }

    
        public string Block { get; set; }


        public string Village { get; set; }

        [Column(TypeName = "xml")]
        public string Image { get; set; }


        public string Address { get; set; }


        public string Bank { get; set; }


        public string BusStand { get; set; }


        public string RailwayStation { get; set; }

    
        public string Airport { get; set; }

   
        public string OtherInformation { get; set; }

        public int? StationCategoryID { get; set; }

  
        public string Profile { get; set; }

        public string Gain { get; set; }

        public string Offset { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }

      public DateTime? InstallationDate { get; set; }
        public string StationID { get; set; }
        public string SerialNumber { get; set; }
        public string ShowInGraph { get; set; }
        public string ShowInGrid { get; set; }

    }
}
