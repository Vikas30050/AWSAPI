namespace AWSAPI.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<SiteSurveyAWLRSiteSelection> SiteSurveyAWLRSiteSelections { get; set; }
        public virtual DbSet<SiteSurveyAWSSiteSelection> SiteSurveyAWSSiteSelections { get; set; }
        public virtual DbSet<SiteSurveyGeneralInformation> SiteSurveyGeneralInformations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SiteSurveyAWLRSiteSelection>()
                .Property(e => e.SiteSize)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyAWLRSiteSelection>()
                .Property(e => e.LandType)
                .IsUnicode(false);

         
            modelBuilder.Entity<SiteSurveyAWLRSiteSelection>()
                .Property(e => e.DistanceDataLoggerWaterLevel)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyAWLRSiteSelection>()
                .Property(e => e.FencingSpace)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyAWSSiteSelection>()
                .Property(e => e.SiteSize)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyAWSSiteSelection>()
                .Property(e => e.LandType)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.StationName)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Latitude)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Longitude)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.CustomerName)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.CustomerAddress)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.CustomerEmailID)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.InChargeName)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.InChargeContact)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.InChargeAddress)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.InChargeEmailID)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.SiteAccessibilityTime)
                .IsUnicode(false);

           

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.LodgingBoardingFacility)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.NearbyATM)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.NearbyCityWithDistance)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.LocalLanguage)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Photos1)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Photos2)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Photos3)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Photos4)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.LaborAvailability)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.CivilMaterialAvailability)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.MountingLocation)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.GSMNetwork)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Distance230VACPoint)
                .IsUnicode(false);

            modelBuilder.Entity<SiteSurveyGeneralInformation>()
                .Property(e => e.Notes)
                .IsUnicode(false);
        }
    }
}
