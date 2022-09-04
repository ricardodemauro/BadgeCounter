using BadgeCounters.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadgeCounters.Db.Configurations
{
    public class GithubProfileConfig : IEntityTypeConfiguration<GithubProfile>
    {
        public void Configure(EntityTypeBuilder<GithubProfile> builder)
        {
            builder.ToTable("GIT_LOG");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(100);

            builder.Property(x => x.Active);

            builder.Property(x => x.Count);

            builder.Property(x => x.Created);

            builder.HasIndex(nameof(GithubProfile.Name), nameof(GithubProfile.Active));
        }
    }
}
