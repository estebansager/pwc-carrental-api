﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CarRental.DB.Entities;

namespace CarRental.DB.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            //Stores the Car Type Enum as a string and not an int.
            builder.Property(c => c.Type)
                   .HasConversion<string>()
                   .HasMaxLength(50);
        }
    }
}
