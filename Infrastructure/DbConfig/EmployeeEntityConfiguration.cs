using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig;

public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees", SchemaNames.HR)
            .HasIndex(x => x.FirstName)
            .HasDatabaseName("IX_Employees_FirstName");

        builder
            .HasIndex(x => x.LastName)
            .HasDatabaseName("IX_Employees_LastName");
    }
}