namespace AWSAPI.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using AWSAPI.Model;


    public partial class AWSDatabaseContext : DbContext
    {
        public AWSDatabaseContext()
            : base("name=AWSDatabaseContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
      
        public virtual DbSet<tbl_GUJSiteVistAWS> tbl_GUJSiteVistAWS { get; set; }

        public virtual DbSet<tbl_StieVisit2_AWLRGeneralInformation> tbl_StieVisit2_AWLRGeneralInformation { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWSGeneralInformation> tbl_SiteVisit2_AWSGeneralInformation { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWSSystemParameter> tbl_SiteVisit2_AWSSystemParameter { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWSStationPhotographs> tbl_SiteVisit2_AWSStationPhotographs { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWSOtherInformation> tbl_SiteVisit2_AWSOtherInformation { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWSObservation> tbl_SiteVisit2_AWSObservation { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWLRSystemParameter> tbl_SiteVisit2_AWLRSystemParameter { get; set; }
        public virtual DbSet<tbl_InstallationFormLevel5> tbl_InstallationFormLevel5 { get; set; }
        public virtual DbSet<tbl_InstallationFormLevel6> tbl_InstallationFormLevel6 { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWLRStationPhotographs> tbl_SiteVisit2_AWLRStationPhotographs { get; set; }
        public virtual DbSet<tbl_InstallationFormLevel3> tbl_InstallationFormLevel3 { get; set; }
        public virtual DbSet<tbl_InstallationFormLevel4> tbl_InstallationFormLevel4 { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWLRObservation> tbl_SiteVisit2_AWLRObservation { get; set; }
        public virtual DbSet<tbl_SiteVisit2_AWLROtherInformation> tbl_SiteVisit2_AWLROtherInformation { get; set; }
        public virtual DbSet<tbl_InstallationFormLevel2> tbl_InstallationFormLevel2 { get; set; }
        public virtual DbSet<tbl_InstallationFormLevel1> tbl_InstallationFormLevel1 { get; set; }
        public virtual DbSet<tbl_SMSConfiguration> tbl_SMSConfiguration { get; set; }
        public virtual DbSet<tbl_SitePhotographs> tbl_SitePhotographs { get; set; }
        public virtual DbSet<SiteVisitAWS> SiteVisitAWS { get; set; }
        public virtual DbSet<SiteSurvey> SiteSurveys { get; set; }
        public virtual DbSet<SiteVisitAWLR> SiteVisitAWLRs { get; set; }
        public virtual DbSet<SiteSurveyAWLRSiteSelection> SiteSurveyAWLRSiteSelections { get; set; }
        public virtual DbSet<SiteSurveyAWSSiteSelection> SiteSurveyAWSSiteSelections { get; set; }
        public virtual DbSet<SiteSurveyGeneralInformation> SiteSurveyGeneralInformations { get; set; }
        public virtual DbSet<tbl_FTPDetails> tbl_FTPDetails { get; set; }
        public virtual DbSet<tbl_FTPRawData> tbl_FTPRawData { get; set; }
        public virtual DbSet<tbl_ParameterMaster> tbl_ParameterMaster { get; set; }
        public virtual DbSet<tbl_SensorParameterTemp> tbl_SensorParameterTemp { get; set; }
        public virtual DbSet<tbl_StationRangeValidation> tbl_StationRangeValidation { get; set; }
        public virtual DbSet<tbl_ProfileMaster> tbl_ProfileMaster { get; set; }
        public virtual DbSet<tbl_Role> tbl_Role { get; set; }
        public virtual DbSet<tbl_SensorMaster> tbl_SensorMaster { get; set; }
        public virtual DbSet<tbl_TypeMaster> tbl_TypeMaster { get; set; }
        public virtual DbSet<tbl_StationCategoryMaster> tbl_StationCategoryMaster { get; set; }
        public virtual DbSet<tbl_StationMaster> tbl_StationMaster { get; set; }
        public virtual DbSet<tbl_APPermission> tbl_APPermission { get; set; }

        public virtual DbSet<tbl_ShowInGraph> tbl_ShowInGraph { get; set; }
        public virtual DbSet<tbl_User> tbl_User { get; set; }
        public virtual DbSet<tbl_Permission> tbl_Permission { get; set; }
        public virtual DbSet<tbl_ProfileTemp> tbl_ProfileTemp { get; set; }
        public virtual DbSet<tbl_SiteSurvey> tbl_SiteSurvey { get; set; }
        public virtual DbSet<tbl_AuditLog> tbl_AuditLog { get; set; }
        public virtual DbSet<tbl_ParameterInSensor> tbl_ParameterInSensor { get; set; }
        public virtual DbSet<tbl_parametertemp> tbl_parametertemp { get; set; }
        public virtual DbSet<tbl_StationConfigTemp> tbl_StationConfigTemp { get; set; }
        public virtual DbSet<tbl_MissingDataPermission> tbl_MissingDataPermission { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_FTPDetails>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_FTPDetails>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_FTPDetails>()
                .Property(e => e.Duration)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_FTPRawData>()
                .Property(e => e.FileName)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_ParameterMaster>()
                .Property(e => e.Name)
                .IsUnicode(false);
            modelBuilder.Entity<tbl_ProfileMaster>()
                .Property(e => e.Name)
                .IsUnicode(false);
            modelBuilder.Entity<tbl_ProfileMaster>()
                .Property(e => e.SensorID)
                .IsUnicode(false);
            modelBuilder.Entity<tbl_ProfileMaster>()
                .Property(e => e.Delimiter)
                .IsUnicode(false);
            modelBuilder.Entity<tbl_ProfileMaster>()
                .Property(e => e.DateFormat)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Role>()
                .Property(e => e.RoleName)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Role>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_SensorMaster>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_SensorMaster>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_SensorMaster>()
                .Property(e => e.Make)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_SensorMaster>()
                .Property(e => e.Model)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_SensorMaster>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationCategoryMaster>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationCategoryMaster>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Latitude)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Longitude)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.District)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.TehsilTaluk)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Block)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Village)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Bank)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.BusStand)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.RailwayStation)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Airport)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.OtherInformation)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Profile)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Gain)
               .IsUnicode(false);

            modelBuilder.Entity<tbl_StationMaster>()
                .Property(e => e.Offset)
                .IsUnicode(false);



            modelBuilder.Entity<tbl_User>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.EmailID)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_User>()
                .Property(e => e.RoleName)
                .IsUnicode(false);



            modelBuilder.Entity<tbl_ProfileTemp>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_ProfileTemp>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_ProfileTemp>()
                .Property(e => e.Make)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_ProfileTemp>()
                .Property(e => e.Model)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_ProfileTemp>()
                .Property(e => e.Description)
                .IsUnicode(false);
        }
    }
}
