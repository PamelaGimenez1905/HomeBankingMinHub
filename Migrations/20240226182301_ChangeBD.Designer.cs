﻿// <auto-generated />
using System;
using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HomeBankingMindHub.Migrations
{
    [DbContext(typeof(HomeBankingContext))]
    [Migration("20240226182301_ChangeBD")]
    partial class ChangeBD
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HomeBankingMindHub.Models.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Card", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("CardHolder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<int>("Color")
                        .HasColumnType("int");

                    b.Property<int>("Cvv")
                        .HasColumnType("int");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ThruDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Client", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.ClientLoan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<long>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<long>("LoanId")
                        .HasColumnType("bigint");

                    b.Property<string>("Payments")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("LoanId");

                    b.ToTable("ClientLoans");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Loan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<double>("MaxAmount")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Payments")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Loans");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Transaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Account", b =>
                {
                    b.HasOne("HomeBankingMindHub.Models.Client", "Client")
                        .WithMany("Accounts")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Card", b =>
                {
                    b.HasOne("HomeBankingMindHub.Models.Client", "Client")
                        .WithMany("Cards")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.ClientLoan", b =>
                {
                    b.HasOne("HomeBankingMindHub.Models.Client", "Client")
                        .WithMany("ClientLoans")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeBankingMindHub.Models.Loan", "Loan")
                        .WithMany("ClientLoans")
                        .HasForeignKey("LoanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Loan");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Transaction", b =>
                {
                    b.HasOne("HomeBankingMindHub.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Account", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Client", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Cards");

                    b.Navigation("ClientLoans");
                });

            modelBuilder.Entity("HomeBankingMindHub.Models.Loan", b =>
                {
                    b.Navigation("ClientLoans");
                });
#pragma warning restore 612, 618
        }
    }
}
