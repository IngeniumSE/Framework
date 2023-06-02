using Ingenium.Platform.Data;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// Provides extensions for the <see cref="ModelBuilder"/> type.
	/// </summary>
	public static class ModelBuilderExtensions
	{
		/// <summary>
		/// Configures an entity using an entity type configuration.
		/// </summary>
		/// <typeparam name="TEntity">The entity type.</typeparam>
		/// <typeparam name="TEntityTypeConfiguration">The entity type configuration.</typeparam>
		/// <param name="modelBuilder">The model builder.</param>
		/// <returns>The model builder.</returns>
		public static ModelBuilder ApplyConfiguration<TEntity, TEntityTypeConfiguration>(
			this ModelBuilder modelBuilder)
			where TEntity : Entity
			where TEntityTypeConfiguration : IEntityTypeConfiguration<TEntity>, new()
		{
			Ensure.IsNotNull(modelBuilder, nameof(modelBuilder));

			modelBuilder.ApplyConfiguration(new TEntityTypeConfiguration());

			return modelBuilder;
		}
	}
}
