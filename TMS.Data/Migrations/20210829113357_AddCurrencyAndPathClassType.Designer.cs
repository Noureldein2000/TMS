﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TMS.Data;

namespace TMS.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210829113357_AddCurrencyAndPathClassType")]
    partial class AddCurrencyAndPathClassType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TMS.Data.Entities.BillPaymentMode", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("BillPaymentModes");
                });

            modelBuilder.Entity("TMS.Data.Entities.Currency", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18, 3)");

                    b.HasKey("ID");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("TMS.Data.Entities.Denomination", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("APIValue")
                        .HasColumnType("decimal(18, 3)");

                    b.Property<int>("BillPaymentModeID")
                        .HasColumnType("int");

                    b.Property<short>("ClassType")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurrencyID")
                        .HasColumnType("int");

                    b.Property<bool>("Inquirable")
                        .HasColumnType("bit");

                    b.Property<int>("Interval")
                        .HasColumnType("int");

                    b.Property<decimal>("MaxValue")
                        .HasColumnType("decimal(18, 3)");

                    b.Property<decimal>("MinValue")
                        .HasColumnType("decimal(18, 3)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OldDenominationID")
                        .HasColumnType("int");

                    b.Property<string>("PathClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PaymentModeID")
                        .HasColumnType("int");

                    b.Property<int>("ServiceCategoryID")
                        .HasColumnType("int");

                    b.Property<int>("ServiceID")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("BillPaymentModeID");

                    b.HasIndex("CurrencyID");

                    b.HasIndex("PaymentModeID");

                    b.HasIndex("ServiceCategoryID");

                    b.ToTable("Denominations");
                });

            modelBuilder.Entity("TMS.Data.Entities.DenominationServiceProvider", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DenominationID")
                        .HasColumnType("int");

                    b.Property<int?>("OldServiceID")
                        .HasColumnType("int");

                    b.Property<decimal>("ProviderAmount")
                        .HasColumnType("decimal(18,3)");

                    b.Property<string>("ProviderCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ProviderHasFees")
                        .HasColumnType("bit");

                    b.Property<int>("ServiceProviderID")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("DenominationID");

                    b.HasIndex("ServiceProviderID");

                    b.ToTable("DenominationServiceProviders");
                });

            modelBuilder.Entity("TMS.Data.Entities.Fee", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AmountFrom")
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("AmountTo")
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FeesTypeID")
                        .HasColumnType("int");

                    b.Property<int>("PaymentModeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UpdatedBy")
                        .HasColumnType("int");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,3)");

                    b.HasKey("ID");

                    b.HasIndex("FeesTypeID");

                    b.ToTable("Fees");
                });

            modelBuilder.Entity("TMS.Data.Entities.FeesType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("FeesTypes");
                });

            modelBuilder.Entity("TMS.Data.Entities.InquiryBill", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProviderServiceResponseID")
                        .HasColumnType("int");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("ProviderServiceResponseID");

                    b.ToTable("InquiryBills");
                });

            modelBuilder.Entity("TMS.Data.Entities.InquiryBillDetails", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("InquiryBillID")
                        .HasColumnType("int");

                    b.Property<int>("ParameterID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("InquiryBillID");

                    b.HasIndex("ParameterID");

                    b.ToTable("InquiryBillDetails");
                });

            modelBuilder.Entity("TMS.Data.Entities.Parameter", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Parameters");
                });

            modelBuilder.Entity("TMS.Data.Entities.PaymentMode", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("PaymentModes");
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceConfigeration", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DenominationServiceProviderID")
                        .HasColumnType("int");

                    b.Property<int>("ServiceConfigerationID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("DenominationServiceProviderID");

                    b.HasIndex("ServiceConfigerationID");

                    b.ToTable("ProviderServiceConfigerations");
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceRequest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BillingAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Brn")
                        .HasColumnType("int");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DenominationID")
                        .HasColumnType("int");

                    b.Property<int>("ProviderServiceRequestStatusID")
                        .HasColumnType("int");

                    b.Property<int>("RequestTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ProviderServiceRequestStatusID");

                    b.HasIndex("RequestTypeID");

                    b.ToTable("ProviderServiceRequests");
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceRequestParam", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ParameterID")
                        .HasColumnType("int");

                    b.Property<int>("ProviderServiceRequestID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ParameterID");

                    b.HasIndex("ProviderServiceRequestID");

                    b.ToTable("ProviderServiceRequestParams");
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceRequestStatus", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("ProviderServiceRequestStatus");
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceResponse", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProviderServiceRequestID")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("ProviderServiceRequestID");

                    b.ToTable("ProviderServiceResponses");
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceResponseParam", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ParameterID")
                        .HasColumnType("int");

                    b.Property<int>("ProviderServiceResponseID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ParameterID");

                    b.HasIndex("ProviderServiceResponseID");

                    b.ToTable("ProviderServiceResponseParams");
                });

            modelBuilder.Entity("TMS.Data.Entities.ReceiptBodyParam", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ParameterID")
                        .HasColumnType("int");

                    b.Property<int>("ProviderServiceRequestID")
                        .HasColumnType("int");

                    b.Property<int>("TransactionID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ParameterID");

                    b.HasIndex("ProviderServiceRequestID");

                    b.ToTable("ReceiptBodyParams");
                });

            modelBuilder.Entity("TMS.Data.Entities.RequestType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("RequestTypes");
                });

            modelBuilder.Entity("TMS.Data.Entities.Service", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ClassType")
                        .HasColumnType("smallint");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PathClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceCategoryID")
                        .HasColumnType("int");

                    b.Property<int>("ServiceEntityID")
                        .HasColumnType("int");

                    b.Property<int>("ServiceTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("ServiceCategoryID");

                    b.HasIndex("ServiceEntityID");

                    b.HasIndex("ServiceTypeID");

                    b.ToTable("Service");
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryNameAr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("ServiceCategories");
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceConfigParms", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceConfigerationID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ServiceConfigerationID");

                    b.ToTable("ServiceConfigParms");
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceConfigeration", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TimeOut")
                        .HasColumnType("int");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPassword")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ServiceConfigerations");
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceEntity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("ServiceEntity");
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceProvider", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("ServiceProviders");
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("ServiceType");
                });

            modelBuilder.Entity("TMS.Data.Entities.Denomination", b =>
                {
                    b.HasOne("TMS.Data.Entities.BillPaymentMode", "BillPaymentMode")
                        .WithMany("Denominations")
                        .HasForeignKey("BillPaymentModeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.Currency", "Currency")
                        .WithMany("Denominations")
                        .HasForeignKey("CurrencyID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.PaymentMode", "PaymentMode")
                        .WithMany("Denominations")
                        .HasForeignKey("PaymentModeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ServiceCategory", "ServiceCategory")
                        .WithMany("Denominations")
                        .HasForeignKey("ServiceCategoryID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.DenominationServiceProvider", b =>
                {
                    b.HasOne("TMS.Data.Entities.Denomination", "Denomination")
                        .WithMany("DenominationServiceProviders")
                        .HasForeignKey("DenominationID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ServiceProvider", "ServiceProvider")
                        .WithMany("DenominationServiceProviders")
                        .HasForeignKey("ServiceProviderID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.Fee", b =>
                {
                    b.HasOne("TMS.Data.Entities.FeesType", "FeesType")
                        .WithMany("Fees")
                        .HasForeignKey("FeesTypeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.InquiryBill", b =>
                {
                    b.HasOne("TMS.Data.Entities.ProviderServiceResponse", "ProviderServiceResponse")
                        .WithMany("InquiryBills")
                        .HasForeignKey("ProviderServiceResponseID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.InquiryBillDetails", b =>
                {
                    b.HasOne("TMS.Data.Entities.InquiryBill", "InquiryBill")
                        .WithMany("InquiryBillDetails")
                        .HasForeignKey("InquiryBillID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.Parameter", "Parameter")
                        .WithMany("InquiryBillDetails")
                        .HasForeignKey("ParameterID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceConfigeration", b =>
                {
                    b.HasOne("TMS.Data.Entities.DenominationServiceProvider", "DenominationServiceProvider")
                        .WithMany("ProviderServiceConfigerations")
                        .HasForeignKey("DenominationServiceProviderID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ServiceConfigeration", "ServiceConfigeration")
                        .WithMany("ProviderServiceConfigerations")
                        .HasForeignKey("ServiceConfigerationID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceRequest", b =>
                {
                    b.HasOne("TMS.Data.Entities.ProviderServiceRequestStatus", "ProviderServiceRequestStatus")
                        .WithMany("ProviderServiceRequests")
                        .HasForeignKey("ProviderServiceRequestStatusID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.RequestType", "RequestType")
                        .WithMany("ProviderServiceRequests")
                        .HasForeignKey("RequestTypeID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceRequestParam", b =>
                {
                    b.HasOne("TMS.Data.Entities.Parameter", "Parameter")
                        .WithMany("ProviderServiceRequestParams")
                        .HasForeignKey("ParameterID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ProviderServiceRequest", "ProviderServiceRequest")
                        .WithMany()
                        .HasForeignKey("ProviderServiceRequestID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceResponse", b =>
                {
                    b.HasOne("TMS.Data.Entities.ProviderServiceRequest", "ProviderServiceRequest")
                        .WithMany("ProviderServiceResponses")
                        .HasForeignKey("ProviderServiceRequestID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ProviderServiceResponseParam", b =>
                {
                    b.HasOne("TMS.Data.Entities.Parameter", "Parameter")
                        .WithMany("ProviderServiceResponseParams")
                        .HasForeignKey("ParameterID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ProviderServiceResponse", "ProviderServiceResponse")
                        .WithMany("ProviderServiceResponseParams")
                        .HasForeignKey("ProviderServiceResponseID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ReceiptBodyParam", b =>
                {
                    b.HasOne("TMS.Data.Entities.Parameter", "Parameter")
                        .WithMany("ReceiptBodyParams")
                        .HasForeignKey("ParameterID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ProviderServiceRequest", "ProviderServiceRequest")
                        .WithMany("ReceiptBodyParams")
                        .HasForeignKey("ProviderServiceRequestID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.Service", b =>
                {
                    b.HasOne("TMS.Data.Entities.ServiceCategory", "ServiceCategory")
                        .WithMany("Services")
                        .HasForeignKey("ServiceCategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ServiceEntity", "ServiceEntity")
                        .WithMany("Services")
                        .HasForeignKey("ServiceEntityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TMS.Data.Entities.ServiceType", "ServiceType")
                        .WithMany("Services")
                        .HasForeignKey("ServiceTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TMS.Data.Entities.ServiceConfigParms", b =>
                {
                    b.HasOne("TMS.Data.Entities.ServiceConfigeration", "ServiceConfigeration")
                        .WithMany("ServiceConfigParms")
                        .HasForeignKey("ServiceConfigerationID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
